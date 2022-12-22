using Common;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Enum;
using Hangfire;
using Microsoft.Extensions.Logging;
using Processor.Dto;
using Processor.HangfireProcess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Common.Functions;
using System.Threading;
using Polly.Retry;
using Polly;
using Processor.Services;
using Microsoft.ApplicationInsights;
using System.Linq;

namespace Processor.ProcessModule
{
    public class TipoComunicacionProcess : IProcess
    {
        const string TIPO_COMUNICACION_PROCESS = "TIPO_COMUNICACION_PROCESS";
        const string TIPO_COMUNICACION_PROCESS_SEND = "TIPO_COMUNICACION_PROCESS_SEND";
        private readonly ILogger<ProcessManager> _logger;
        private readonly ITipoComunicacionServices _tipoComunicacionServices;
        private readonly IStoreServices _storeServices;
        private readonly IComunicacionServices _comunicacionServices;
        private readonly IEnvioServices _envioServices;
        private readonly AsyncRetryPolicy _retryPolicySQL;
        private readonly TelemetryClient _telemetryClient;


        public TipoComunicacionProcess(ILogger<ProcessManager> logger, TelemetryClient telemetryClient, ITipoComunicacionServices tipoComunicacionServices,
            IStoreServices storeServices, IComunicacionServices comunicacionServices,
            IEnvioServices envioServices)
        {
            _logger = logger;
            _tipoComunicacionServices = tipoComunicacionServices;
            _storeServices = storeServices;
            _comunicacionServices = comunicacionServices;
            _envioServices = envioServices;
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
        }

        public void Dispose()
        {
        }

        [Queue("alpha")]
        public async virtual Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            using (_telemetryClient.StartOperation<Microsoft.ApplicationInsights.DataContracts.RequestTelemetry>(TIPO_COMUNICACION_PROCESS))
            {
                try
                {
                    if (!int.TryParse(parameters, out var id))
                    {
                        return;
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var tipo = new TipoComunicacionDto();
                    tipo = await _tipoComunicacionServices.GetAsync(id);

                    var tope = tipo.TopeLectura != 0 ? tipo.TopeLectura : 10;

                    dynamic response;
                    if (!string.IsNullOrEmpty(tipo.StoreObtenerLote))
                    {
                        List<dynamic> lstResponseLote;
                        var index = 0;
                        do
                        {
                            response = await _retryPolicySQL.ExecuteAsync(async () => await _storeServices.GetStoreAsync(tipo.StoreObtenerLote, tope, 240)); //obtengo los registros del Store
                            lstResponseLote = Utils.ConvertToObject<List<dynamic>>(response);
                            IList<DynamicDto> request = new List<DynamicDto>();
                            foreach (var item in lstResponseLote)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    return;
                                }

                                try
                                {
                                    var dyn = Utils.ConvertToObject<DynamicDto>(item);
                                    dyn.JsonData = item.ToString();
                                    request.Add(dyn);
                                }
                                catch (Exception e)
                                {
                                    _telemetryClient.TrackException(e);
                                    _logger.LogError($"SendMessageError: {e}");
                                    var comunicacion = new ComunicacionDto
                                    {
                                        IdTipoComunicacion = tipo.IdTipoComunicacion,
                                        IdCanal = 1,
                                        IdRelacionado = item.IdRelacionado,
                                        IdContacto = item.IdContacto,
                                        Enviado = (int)EstadoComunicacion.Error,
                                        Message = FunctionsText.CutText(e.Message, 256)
                                    };
                                    await _retryPolicySQL.ExecuteAsync(async () => await _comunicacionServices.AddComunicacionAsync(comunicacion)); //Insert de la comunicación y las pongo en estado pendienteenvio
                                }
                            }
                            if (request.Count > 0)
                            {
                                index++;
                                _logger.LogInformation($"TipoComunicacion-Enqueue-SendigMail: {index}");
                                var sendMessage = new SendMessageDto
                                {
                                    Template = tipo.Template,
                                    Tag = tipo.Tag,
                                    Asunto = tipo.Asunto,
                                    TipoEnvio = tipo.Aviso ? TipoEnvioType.Negocio : TipoEnvioType.Campania
                                };
                                BackgroundJob.Enqueue(() => SendMessageAsync(index, request, sendMessage, CancellationToken.None)); //Pongo en cola el proceso para mandar los mensajes
                            }
                        } while (lstResponseLote.Count > 0);
                    }
                }
                catch (System.Exception ex) when (Log(ex))
                {
                }
                finally
                {
                    time.Stop();
                    _logger.LogInformation($"TipoComunicacionEnd: {time.ElapsedMilliseconds}");
                    _telemetryClient.TrackEvent("End", properties: new Dictionary<string, string> { { "parameters", parameters } }, metrics: new Dictionary<string, double> { { "time", time.ElapsedMilliseconds } });
                }
            }
        }

        [Queue("alpha")]
        [AutomaticRetry(Attempts = 0)]
        public async virtual Task SendMessageAsync(int index, IList<DynamicDto> items, SendMessageDto tipo, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            using (_telemetryClient.StartOperation<Microsoft.ApplicationInsights.DataContracts.RequestTelemetry>(TIPO_COMUNICACION_PROCESS_SEND))
            {
                try
                {
                    var itemCanal = items.Count > 0 ? items.First().IdCanal.ToString() : "undefined";
                    _telemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "TipoEnvio", tipo.TipoEnvio.ToString() }, { "index", index.ToString() }, { "canal", itemCanal } });
                    await _envioServices.SendMessageAsync(index, items, tipo, cancellationToken);
                }
                catch (System.Exception ex) when (Log(ex))
                {
                }
                finally
                {
                    time.Stop();
                    _telemetryClient.TrackEvent("End", properties: new Dictionary<string, string> { { "TipoEnvio", tipo.TipoEnvio.ToString() }, { "index", index.ToString() } }, metrics: new Dictionary<string, double> { { "time", time.ElapsedMilliseconds } });
                }
            }

        }

        private bool Log(System.Exception ex)
        {
            _telemetryClient.TrackException(ex);
            return false;
        }
    }
}
