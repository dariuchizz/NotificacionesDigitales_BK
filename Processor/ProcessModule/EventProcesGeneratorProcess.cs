using Common.IServices;
using Hangfire;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;
using System.Threading.Tasks;
using NotificacionesDigitalesApi.Model;
using System;
using System.Globalization;
using Common.Model.Enum;
using Common.Model.NotificacionesDigitales;
using System.Diagnostics;
using System.Threading;
using Polly;
using Microsoft.Data.SqlClient;
using Polly.Retry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Collections.Generic;

namespace Processor.ProcessModule
{
    public class EventProcesGeneratorProcess : IProcess
    {
        const string EVENT_GENERATOR_PROCESS = "EVENT_GENERATOR_PROCESS";

        private readonly ILogger<ProcessManager> _logger;
        private readonly IProcesoEventoServices _procesoEventoServices;
        private readonly IProcessManager _processManager;
        private readonly TelemetryClient _telemetryClient;
        private int cantHourProcess = 8;
        protected AsyncRetryPolicy RetryPolicySQL { get; }

        public EventProcesGeneratorProcess(ILogger<ProcessManager> logger, TelemetryClient telemetryClient,
                IProcesoEventoServices procesoEventoServices,
                IProcessManager processManager
            )
        {
            _logger = logger;
            _procesoEventoServices = procesoEventoServices;
            _processManager = processManager;
            _telemetryClient = telemetryClient;
            RetryPolicySQL = Policy
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

        //"{\"proceso\":\"email-eventos\",\"fecha-a-procesar\":\"17-07-2020\"}"
        [Queue("aalpha")]
        public async Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            using (_telemetryClient.StartOperation<RequestTelemetry>(EVENT_GENERATOR_PROCESS))
            {
                _telemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "parameters", parameters } });
                _logger.LogInformation($"EventGeneratorStart: {parameters}");
                try
                {
                    EventProcessGeneratorDto eventProcessGeneratorDto = await ValidateParameters(parameters);
                    if (eventProcessGeneratorDto == null)
                    {
                        return;
                    }
                    else
                    {
                        DateTime fechaAProcesar = DateTime.ParseExact(eventProcessGeneratorDto.FechaAProcesar, "dd-MM-yyyy HH", null);
                        if (cantHourProcess == 6)
                            fechaAProcesar.AddHours(-4);

                        //if (getEnumProcess(eventProcessGeneratorDto.Proceso) == TipoProcesoEvento.sms)
                        //Solamente recolectamos una hora.
                        cantHourProcess = 1;
                        for (var h = 0; h < cantHourProcess; h++)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            var eventoProcessDto = new EventProcessDto
                            {
                                FechaAProcesar = fechaAProcesar.Date.ToString("dd-MM-yyyy"),
                                HoraAProcesar = fechaAProcesar.Hour,
                                Aviso = eventProcessGeneratorDto.Aviso,
                            };
                            ProcesoEventoDto procesoEventoDto = await RetryPolicySQL.ExecuteAsync(
                                async () => await _procesoEventoServices.GetAsync(fechaAProcesar, getEnumProcess(eventProcessGeneratorDto.Proceso), eventProcessGeneratorDto.Aviso));
                            if (procesoEventoDto is null)
                            {
                                procesoEventoDto = new ProcesoEventoDto
                                {
                                    Tipo = (int)getEnumProcess(eventProcessGeneratorDto.Proceso),
                                    Aviso = eventProcessGeneratorDto.Aviso,
                                    Fecha = fechaAProcesar.Date,
                                    Hora = fechaAProcesar.Hour,
                                    Cantidad = 0,
                                    Estado = (int)PlanificacionProcesoEvento.planificado,
                                    FechaUltimaModificacion = DateTime.Now
                                };
                                procesoEventoDto.IdProcesoEvento = await RetryPolicySQL.ExecuteAsync(
                                    async () => await _procesoEventoServices.AddEventProcessByStoreAsync(procesoEventoDto));

                                eventoProcessDto.IdRegistro = procesoEventoDto.IdProcesoEvento.ToString();
                                _logger.LogInformation($"EventProcesGeneratorProcess: {eventoProcessDto}");

                                //Después de esto llamo al proceso de recolecto de eventos
                                if (_processManager.TryGetProcess(eventProcessGeneratorDto.Proceso, out var process))
                                {
                                    var executor = process.ProcessFactory();
                                    BackgroundJob.Enqueue(() =>
                                       executor.ExecuteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(eventoProcessDto), CancellationToken.None)
                                    );
                                }
                            }
                            else
                            {
                                if (procesoEventoDto.Estado == (int)PlanificacionProcesoEvento.error)
                                {
                                    procesoEventoDto = new ProcesoEventoDto
                                    {
                                        Tipo = (int)getEnumProcess(eventProcessGeneratorDto.Proceso),
                                        Aviso = eventProcessGeneratorDto.Aviso,
                                        Fecha = fechaAProcesar.Date,
                                        Hora = fechaAProcesar.Hour,
                                        Estado = (int)PlanificacionProcesoEvento.planificado,
                                        Cantidad = 0,
                                        FechaUltimaModificacion = DateTime.Now
                                    };
                                    procesoEventoDto.IdProcesoEvento = await RetryPolicySQL.ExecuteAsync(
                                        async () => await _procesoEventoServices.AddEventProcessByStoreAsync(procesoEventoDto));

                                    eventoProcessDto.IdRegistro = procesoEventoDto.IdProcesoEvento.ToString();
                                    _logger.LogInformation($"EventProcesGeneratorProcess: {eventoProcessDto}");

                                    //Después de esto llamo al proceso de recolecto de eventos
                                    if (_processManager.TryGetProcess(eventProcessGeneratorDto.Proceso, out var process))
                                    {
                                        var executor = process.ProcessFactory();
                                        BackgroundJob.Enqueue(() =>
                                           executor.ExecuteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(eventoProcessDto), CancellationToken.None)
                                        );
                                    }
                                }
                            }
                            //Le saco una hora para el proximo procesamineto
                            fechaAProcesar = fechaAProcesar.AddHours(-1);
                        }
                    }
                }
                catch (Exception e)
                {
                    _telemetryClient.TrackException(e);
                    _logger.LogError($"EventGeneratorError: {e}");
                }
                finally
                {
                    time.Stop();
                    _logger.LogInformation($"EventGeneratorEnd: {time.ElapsedMilliseconds}");
                    _telemetryClient.TrackEvent("End", properties: new Dictionary<string, string> { { "parameters", parameters } }, metrics: new Dictionary<string, double> { { "time", time.ElapsedMilliseconds } });
                }
            }
        }

        private TipoProcesoEvento getEnumProcess(string proceesEvent)
        {
            if (proceesEvent == "sms-eventos")
                return TipoProcesoEvento.sms;
            else
                return TipoProcesoEvento.Email;
        }

        private Task<EventProcessGeneratorDto> ValidateParameters(string parameteres)
        {
            EventProcessGeneratorDto eventProcessGeneratorDto;
            try
            {
                eventProcessGeneratorDto = Newtonsoft.Json.JsonConvert.DeserializeObject<EventProcessGeneratorDto>(parameteres);
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e);
                _logger.LogError($"EventProcessGenerator: error en los parametros que se recibieron");
                return Task.FromResult<EventProcessGeneratorDto>(null);
            }

            if (string.IsNullOrEmpty(eventProcessGeneratorDto.Proceso))
            {
                _logger.LogError($"EventProcessGenerator: no existe el parametro proceso");
                return Task.FromResult<EventProcessGeneratorDto>(null);
            }
            else
            {
                if (eventProcessGeneratorDto.Proceso != "email-eventos" && eventProcessGeneratorDto.Proceso != "sms-eventos")
                {
                    _logger.LogError($"EventProcessGenerator: el parametro proceso es inexistente");
                    return Task.FromResult<EventProcessGeneratorDto>(null);
                }
                else
                {
                    if (eventProcessGeneratorDto.Proceso == "email-eventos")
                    {
                        if (eventProcessGeneratorDto.Aviso is null)
                        {
                            _logger.LogError($"EventProcessGenerator: el parametro aviso es inexistente");
                            return Task.FromResult<EventProcessGeneratorDto>(null);
                        }
                    }
                }
            }

            DateTime date;
            if (string.IsNullOrEmpty(eventProcessGeneratorDto.FechaAProcesar))
            {
                _logger.LogInformation($"EventProcessGenerator: no existe el parametro fecha");
                eventProcessGeneratorDto.FechaAProcesar = DateTime.Now.AddHours(-4).ToString("dd-MM-yyyy HH");
                cantHourProcess = 6;
            }
            else
            {
                if (!DateTime.TryParseExact(eventProcessGeneratorDto.FechaAProcesar, "dd-MM-yyyy HH",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None,
                                           out date))
                {
                    _logger.LogError($"EventProcessGenerator: error en el formato del parametro fecha, formato correcto dd-MM-yyyy HH");
                    return Task.FromResult<EventProcessGeneratorDto>(null);
                }
                else
                {
                    if (date >= DateTime.Today && date.Hour > DateTime.Now.Hour)
                    {
                        _logger.LogError($"EventProcessGenerator: la fecha/hora tiene que se menor a la actual");
                        return Task.FromResult<EventProcessGeneratorDto>(null);
                    }
                    else
                    {
                        if (date > DateTime.Now.AddHours(-4))
                            cantHourProcess = 6;
                        else
                            cantHourProcess = 8;
                    }
                }
            }
            return Task.FromResult(eventProcessGeneratorDto);
        }
    }
}