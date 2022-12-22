using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Enums;
using Common.IServices;
using Hangfire;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;

namespace Processor.ProcessModule
{
    public class StoreCampanaProcess : IProcess
    {
        const string STORE_CAMPANA_PROCESS = "STORE_GENERAR_COMUNICACIONES_CAMPANA";
        private readonly ILogger<ProcessManager> _logger;
        private readonly IStoreServices _storeServices;
        private readonly ICampaniaServices _campaniaServices;
        private readonly TelemetryClient _telemetryClient;

        public StoreCampanaProcess(ILogger<ProcessManager> logger, TelemetryClient telemetryClient, IStoreServices storeServices, ICampaniaServices campaniaServices)
        {
            _logger = logger;
            _storeServices = storeServices;
            _campaniaServices = campaniaServices;
            _telemetryClient = telemetryClient;
        }
        public void Dispose()
        {
        }

        [Queue("aalpha")]
        public async Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = System.Diagnostics.Stopwatch.StartNew();
            using (_telemetryClient.StartOperation<Microsoft.ApplicationInsights.DataContracts.RequestTelemetry>(STORE_CAMPANA_PROCESS))
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

                        var campaniaDto = await _campaniaServices.GetAsync(id);
                        if (campaniaDto.IdEstadoCampania == (long)EstadoProcesoType.Finalizada)
                        {
                            _logger.LogError("La campañia ya se encuentra finalizada");
                            return;
                        }
                        campaniaDto.IdEstadoCampania = (long)EstadoProcesoType.EnEjecucion;
                        await _campaniaServices.UpdateAsync(campaniaDto, 1);

                        var tipo = await _campaniaServices.GetAsync(id);
                        if (tipo != null)
                        {
                            await _storeServices.ExecuteAsync(tipo.TipoCampania.StoreGenerar, tipo.IdCampania, 300);
                        }
                    }
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
    }
}
