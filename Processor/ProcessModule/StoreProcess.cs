using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.IServices;
using Hangfire;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;
using Microsoft.ApplicationInsights.DataContracts;

namespace Processor.ProcessModule
{
    public class StoreProcess : IProcess
    {
        const string STORE_PROCESS = "STORE_PROCESS_GENERAR_COMUNICACIONES_NEGOCIO";
        private readonly ILogger<ProcessManager> _logger;
        private readonly IStoreServices _storeServices;
        private readonly ITipoComunicacionServices _tipoComunicacionServices;
        private readonly TelemetryClient _telemetryClient;

        public StoreProcess(ILogger<ProcessManager> logger, TelemetryClient telemetryClient, IStoreServices storeServices, ITipoComunicacionServices tipoComunicacionServices)
        {
            _logger = logger;
            _storeServices = storeServices;
            _tipoComunicacionServices = tipoComunicacionServices;
            _telemetryClient = telemetryClient;
        }
        public void Dispose()
        {
        }

        [Queue("aalpha")]
        public async Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = System.Diagnostics.Stopwatch.StartNew();
            using (_telemetryClient.StartOperation<RequestTelemetry>(STORE_PROCESS))
            {
                try
                {
                    _telemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "parameters", parameters } });
                    if (int.TryParse(parameters, out var id))
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        var tipo = await _tipoComunicacionServices.GetAsync(id);
                        if (tipo != null)
                        {
                            await _storeServices.ExecuteAsync(tipo.StoreGenerarComunicaciones, 300);
                        }
                    }
                }
                catch (System.Exception ex) when (Log(ex)) {                   
                }
                finally
                {
                    time.Stop();
                    _telemetryClient.TrackEvent("End", properties: new Dictionary<string, string> { { "parameters", parameters } }, metrics: new Dictionary<string, double> { { "time", time.ElapsedMilliseconds } });
                }
            }
        }

        private bool Log(System.Exception ex) {
            _telemetryClient.TrackException(ex);
            return false;
        }
    }
}
