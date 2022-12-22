using Common;
using Common.IServices;
using Common.Model.Dto;
using Hangfire;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Polly.Retry;
using Polly;
using System.Net.Http;
using Microsoft.ApplicationInsights;

namespace Processor.ProcessModule
{
    public class EventReactionProcess : IProcess
    {
        const string EVENT_REACTION_PROCESS = "EVENT_REACTION_PROCESS";
        private readonly ILogger<ProcessManager> _logger;
        private readonly IStoreServices _storeServices;
        private readonly IProcessManager _processManager;
        private readonly IConfiguration _configuration;
        private readonly AsyncRetryPolicy _retryPolicySQL;
        private readonly AsyncRetryPolicy _retryComunication;
        private readonly TelemetryClient _telemetryClient;

        public EventReactionProcess(ILogger<ProcessManager> logger, TelemetryClient telemetryClient, IStoreServices storeServices,
            IProcessManager processManager, IConfiguration configuration)
        {
            _logger = logger;
            _storeServices = storeServices;
            _processManager = processManager;
            _configuration = configuration;
            _telemetryClient = telemetryClient;
            _retryPolicySQL = Policy
                        .Handle<SqlException>()
                        .WaitAndRetryAsync(new[]
                        {
                                        TimeSpan.FromSeconds(20),
                                        TimeSpan.FromSeconds(60),
                                        TimeSpan.FromSeconds(120)
                        }, (exception, timeSpan) =>
                        {
                            _telemetryClient.TrackException(exception);
                            _logger.LogError($"Error accediendo a la base de datos {exception.StackTrace}");
                        });
            _retryComunication = Policy
                         .Handle<ComunicationException>()
                         .Or<HttpRequestException>()
                         .WaitAndRetryAsync(new[]
                         {
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(10),
                            TimeSpan.FromSeconds(20)
                         }, (exception, timeSpan) =>
                         {
                             _telemetryClient.TrackException(exception);
                             _logger.LogError($"Error accediendo al servicio HTTP {exception.StackTrace}");
                         });
        }

        public void Dispose()
        {
        }

        [Queue("alpha")]
        public async virtual Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            using (_telemetryClient.StartOperation<Microsoft.ApplicationInsights.DataContracts.RequestTelemetry>(EVENT_REACTION_PROCESS))
            {
                try
                {
                    _telemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "parameters", parameters } });
                    await this.ExecuteInternalAsync(parameters, cancellationToken);
                }
                catch (System.Exception ex) when (Log(ex))
                {
                }
                finally
                {
                    time.Stop();
                    _telemetryClient.TrackEvent("End", properties: new Dictionary<string, string> { { "parameters", parameters } }, metrics: new Dictionary<string, double> { { "time", time.ElapsedMilliseconds } });
                }
            }
        }

        private bool Log(System.Exception ex)
        {
            _telemetryClient.TrackException(ex);
            return false;
        }

        private async Task ExecuteInternalAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            _logger.LogInformation($"EventReactionProcessStart: {parameters}");

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            dynamic response;
            List<TipoComunicacionDto> lstResponseTiposComunicaciones;
            response = await _retryPolicySQL.ExecuteAsync(async () => await _storeServices.GetStoreAsync("ObtenerTipoComunicacionesAReenviar", 120)); //obtengo los registros del Store
            lstResponseTiposComunicaciones = Utils.ConvertToObject<List<TipoComunicacionDto>>(response);

            foreach (var item in lstResponseTiposComunicaciones)
            {
                if (_processManager.TryGetProcess("enviar-comunicaciones", out var process))
                {
                    var executor = process.ProcessFactory();
                    BackgroundJob.Enqueue(() =>
                       executor.ExecuteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(item.IdTipoComunicacion), CancellationToken.None)
                    );
                }
            }
        }
    }
}