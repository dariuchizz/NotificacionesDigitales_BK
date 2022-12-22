using Common.IServices;
using Microsoft.Extensions.Logging;
using Processor.ProcessModule;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Processor.Services;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Caching.Memory;

namespace Processor.HangfireProcess
{
    public class ProcessManager : IProcessManager
    {
        private readonly Dictionary<string, ProcessDto> _processes;
        private readonly ILogger<ProcessManager> _logger;

        public ProcessManager(ILogger<ProcessManager> logger,
            ITipoComunicacionServices tipoComunicacionServices,
            IStoreServices storeServices,
            IEventoEmailServices eventoEmailServices,
            IEventoSMSServices eventoSMSServices,
            IComunicacionServices comunicacionServices,
            IConfiguration configuration,
            IProcesoEventoServices procesoEventoServices,
            IEnvioServices envioServices,
            ICampaniaServices campaniaServices,
            ISmStartPlus startPlus,
            IMailgun mailgun,
            ICompressUrl compressUrl,
            TelemetryClient telemetryClient,
            IHuemul huemul,
            IMemoryCache memoryCache,
            INotificacionHuemulServices comunicacionHuemulServices)
        {
            _logger = logger;
            _processes = new Dictionary<string, ProcessDto>
            {
                ["enviar-comunicaciones"] = new ProcessDto
                {
                    Name = "enviar-comunicaciones",
                    ProcessFactory = () => new TipoComunicacionProcess(_logger, telemetryClient, tipoComunicacionServices, storeServices, comunicacionServices, envioServices)
                },
                ["enviar-campania"] = new ProcessDto
                {
                    Name = "enviar-campania",
                    ProcessFactory = () => new CampaniaProcess(_logger, telemetryClient, storeServices, comunicacionServices, envioServices, campaniaServices)
                },
                ["reenviar-comunicaciones"] = new ProcessDto
                {
                    Name = "reenviar-comunicaciones",
                    ProcessFactory = () => new EventReactionProcess(_logger, telemetryClient, storeServices, this, configuration)
                },
                ["generar-comunicaciones"] = new ProcessDto
                {
                    Name = "generar-comunicaciones",
                    ProcessFactory = () => new StoreProcess(_logger, telemetryClient, storeServices, tipoComunicacionServices)
                },
                ["generar-campania"] = new ProcessDto
                {
                    Name = "generar-campania",
                    ProcessFactory = () => new StoreCampanaProcess(_logger, telemetryClient, storeServices, campaniaServices)
                },
                ["generador-eventos"] = new ProcessDto
                {
                    Name = "generador-eventos",
                    ProcessFactory = () => new EventProcesGeneratorProcess(_logger, telemetryClient, procesoEventoServices, this)
                },
                ["email-eventos"] = new ProcessDto
                {
                    Name = "email-eventos",
                    ProcessFactory = () => new EventMailProcess(_logger, telemetryClient, procesoEventoServices, eventoEmailServices, configuration, mailgun)
                },
                ["sms-eventos"] = new ProcessDto
                {
                    Name = "sms-eventos",
                    ProcessFactory = () => new EventSMSProcess(_logger, telemetryClient, procesoEventoServices, eventoSMSServices, configuration, startPlus)
                },
                ["test-mailgun"] = new ProcessDto
                {
                    Name = "test-mailgun",
                    ProcessFactory = () => new TestSendMailgun(_logger, telemetryClient, configuration, tipoComunicacionServices, campaniaServices, mailgun)
                },
                ["process-huemul"] = new ProcessDto
                {
                    Name = "process-huemul",
                    ProcessFactory = () => new IndexedReceiptProcess(_logger, telemetryClient, configuration, huemul, memoryCache, comunicacionHuemulServices)
                },
                ["test-startPlus"] = new ProcessDto
                {
                    Name = "test-startPlus",
                    ProcessFactory = () => new TestSendStartPlus(_logger, telemetryClient, configuration, tipoComunicacionServices, campaniaServices, startPlus, compressUrl)
                },
            };
        }

        public IEnumerable<ProcessDto> GetAvaiableProcesses()
        {
            return _processes.Select(x => x.Value);
        }

        public bool TryGetProcess(string name, out ProcessDto process)
        {
            return _processes.TryGetValue(name, out process);
        }
    }
}
