using Common.IServices;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;
using System.Threading.Tasks;
using System;
using NotificacionesDigitalesApi.Model;
using System.Globalization;
using Common.Model.NotificacionesDigitales;
using Common.Model.Enum;
using Polly;
using System.Net.Http;
using Polly.Retry;
using Microsoft.Data.SqlClient;
using Microsoft.ApplicationInsights;

namespace Processor.ProcessModule
{
    public class BaseEvent
    {
        protected ILogger<ProcessManager> Logger { get; }

        protected TelemetryClient TelemetryClient { get; }
        protected IProcesoEventoServices ProcesoEventoServices { get; }

        protected AsyncRetryPolicy RetryPolicyComunication { get; }
        protected AsyncRetryPolicy RetryPolicySQL { get; }

        public BaseEvent(
            ILogger<ProcessManager> logger,
            TelemetryClient _telemetryClient,
            IProcesoEventoServices procesoEventoServices)
        {
            Logger = logger;
            TelemetryClient = _telemetryClient;
            ProcesoEventoServices = procesoEventoServices;
            RetryPolicyComunication = Policy
                         .Handle<ComunicationException>()
                         .Or<HttpRequestException>().WaitAndRetryAsync(new[]
                            {
                                TimeSpan.FromMinutes(10),
                                TimeSpan.FromMinutes(15),
                                TimeSpan.FromMinutes(22),
                                TimeSpan.FromMinutes(30)
                            }, (exception, timeSpan) => {
                                TelemetryClient.TrackException(exception);
                                Logger.LogError($"Error accediendo al servicio HTTP {exception.StackTrace}");
                            });
            RetryPolicySQL = Policy
                        .Handle<SqlException>()
                        .WaitAndRetryAsync(new[]
                        {
                            TimeSpan.FromSeconds(20),
                            TimeSpan.FromSeconds(60),
                            TimeSpan.FromSeconds(120)
                        }, (exception, timeSpan) => {
                            TelemetryClient.TrackException(exception);
                            Logger.LogError($"Error accediendo a la base de datos {exception.StackTrace}");
                        });
        }

        protected async Task<(EventProcessDto, ProcesoEventoDto)> ValidateParametersAsyc(string parameteres, TipoProcesoEvento tipoEvento)
        {
            ProcesoEventoDto procesoEventoDto = null;
            EventProcessDto eventProcessDto;
            try
            {
                eventProcessDto = Newtonsoft.Json.JsonConvert.DeserializeObject<EventProcessDto>(parameteres);
            }
            catch (Exception e)
            {
                TelemetryClient.TrackException(e);
                Logger.LogError($"Event: error en los parametros que se recibieron");
                return (null, procesoEventoDto);
            }

            DateTime date;
            if (string.IsNullOrEmpty(eventProcessDto.FechaAProcesar))
            {
                Logger.LogInformation($"Event: no existe el parametro fecha");
                return (null, procesoEventoDto);
            }
            else
            {
                if (!DateTime.TryParseExact(eventProcessDto.FechaAProcesar, "dd-MM-yyyy",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None,
                                           out date))
                {
                    Logger.LogError($"Event: error en el formato del parametro fecha, formato correcto dd-MM-yyyy");
                    return (null, procesoEventoDto);
                }
            }

            //Valido Hora
            if (!(eventProcessDto.HoraAProcesar >= 0 && eventProcessDto.HoraAProcesar <= 23))
            {
                Logger.LogInformation($"Event: error en el parametro hora, el valor tiene que estar entre 0 y 23");
                return (null, procesoEventoDto);
            }

            //Valido IdRegistro
            if (string.IsNullOrEmpty(eventProcessDto.IdRegistro))
            {
                //Creo o actualizo con el estado procesando
                procesoEventoDto = new ProcesoEventoDto
                {
                    Tipo = (int)tipoEvento,
                    Aviso = eventProcessDto.Aviso,
                    Fecha = DateTime.ParseExact(eventProcessDto.FechaAProcesar, "dd-MM-yyyy", null),
                    Hora = eventProcessDto.HoraAProcesar,
                    Estado = (int)PlanificacionProcesoEvento.procesando,
                    FechaUltimaModificacion = DateTime.Now
                };
                procesoEventoDto.IdProcesoEvento = await RetryPolicySQL.ExecuteAsync(async () => await ProcesoEventoServices.AddEventProcessByStoreAsync(procesoEventoDto));
            }
            else
            {
                if (!long.TryParse(eventProcessDto.IdRegistro, out long j))
                {
                    Logger.LogInformation($"Event: error en el formato del parametro IdRegistro");
                    return (null, procesoEventoDto);
                }
                else
                {
                    if (!DateTime.TryParseExact(eventProcessDto.FechaAProcesar, "dd-MM-yyyy",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out date))
                    {
                        Logger.LogInformation($"Event: error en el formato del parametro FechaAProcesar");
                        return (null, procesoEventoDto);
                    }

                    procesoEventoDto = await RetryPolicySQL.ExecuteAsync(async () => await ProcesoEventoServices.GetAsync(Convert.ToInt64(eventProcessDto.IdRegistro)));

                    if (procesoEventoDto == null)
                    {
                        Logger.LogInformation($"Event: no existe el IdRegistro");
                        return (null, procesoEventoDto);
                    }
                    else
                    {
                        if (procesoEventoDto.Hora != eventProcessDto.HoraAProcesar ||
                            procesoEventoDto.Fecha.ToString("dd-MM-yyyy") != eventProcessDto.FechaAProcesar)
                        {
                            Logger.LogInformation($"Event: la hora y el dia no concuerdan con el IdRegistro");
                            return (null, procesoEventoDto);
                        }
                        else
                        {
                            if (procesoEventoDto.Estado != (int)PlanificacionProcesoEvento.planificado ||
                                procesoEventoDto.Estado != (int)PlanificacionProcesoEvento.error)
                            {
                                procesoEventoDto.Estado = (int)PlanificacionProcesoEvento.procesando;
                                procesoEventoDto.FechaUltimaModificacion = DateTime.Now;
                                procesoEventoDto.Cantidad = 0;
                            }
                            else
                            {
                                await RetryPolicySQL.ExecuteAsync(async () => await ProcesoEventoServices.UpdateProcesoEventoAsync(procesoEventoDto));
                                Logger.LogInformation($"Event: el estado del proceso es incorrecto");
                                return (null, procesoEventoDto);
                            }
                        }
                    }
                }
            }

            return (eventProcessDto, procesoEventoDto);
        }

        protected static DateTime UnixTimestampToDateTime(double unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, DateTimeKind.Utc);
        }
        
        protected bool Log(System.Exception ex)
        {
            TelemetryClient.TrackException(ex);
            return false;
        }
    }
}