using Common.IServices;
using Hangfire;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NotificacionesDigitalesApi.Model;
using Common.Model.NotificacionesDigitales;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Common.Model.Dto;
using Common.Model.Enum;
using Common.Functions;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.ApplicationInsights;

namespace Processor.ProcessModule
{
    public class EventSMSProcess : BaseEvent, IProcess
    {
        const string EVENT_SMS_PROCESS = "EVENT_SMS_PROCESS";
        private readonly IEventoSMSServices _eventoSMSServices;
        private readonly IConfiguration _configuration;
        private readonly ISmStartPlus _smStartPlus;

        public EventSMSProcess(ILogger<ProcessManager> logger, TelemetryClient telemetryClient,
                IProcesoEventoServices procesoEventoServices,
                IEventoSMSServices eventoSMSServices,
                IConfiguration configuration,
                ISmStartPlus smStartPlus
            ) : base(logger, telemetryClient, procesoEventoServices)
        {
            _eventoSMSServices = eventoSMSServices;
            _configuration = configuration;
            _smStartPlus = smStartPlus;
        }

        public void Dispose()
        {
        }

        //"{\"fecha-a-procesar\":\"15-07-2020\":\"Hora-a-procesar\":\"16\":\"id-registro\":\"2\",\"tipo-envio\":\"2\"} 
        //tipo-envio = 2 -> Negocio. Si es negocio se debe informar si es Aviso o no.
        //tipo-envio = 1 -> Campania
        [Queue("aalpha")]
        public async Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            using (TelemetryClient.StartOperation<Microsoft.ApplicationInsights.DataContracts.RequestTelemetry>(EVENT_SMS_PROCESS))
            {
                try
                {
                    TelemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "parameters", parameters } });
                    (EventProcessDto eventProcessDto, ProcesoEventoDto procesoEventoDto) = await ValidateParametersAsyc(parameters, TipoProcesoEvento.sms);
                    if (eventProcessDto == null)
                    {
                        return;
                    }
                    else
                    {
                        //Actualizo el registro de la tabla ProcesoEvento con procesando
                        await RetryPolicySQL.ExecuteAsync(async () => await ProcesoEventoServices.UpdateProcesoEventoAsync(procesoEventoDto));

                        Logger.LogInformation($"EventSMSStart: {parameters}");
                        try
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            var eventsSMS = await RetryPolicyComunication.ExecuteAsync(async () => await _smStartPlus.ReadEvents(eventProcessDto.FechaAProcesar, eventProcessDto.HoraAProcesar, _configuration, Logger));

                            List<EventoSMSDto> lstEvent = new List<EventoSMSDto>();
                            if (!(eventsSMS is null))
                            {
                                int cLote = 0;
                                foreach (var item in eventsSMS)
                                {
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        return;
                                    }

                                    //Agrego un Evento para procesar el lote
                                    var evento = new EventoSMSDto
                                    {
                                        IdComunicacion = getIdComunicacion(item.Dato),
                                        Fecha = item.Hora.DateTime,
                                        DEvento = item.Res, 
                                        IdExterno = string.IsNullOrEmpty(item.MessageId) ? "" : FunctionsText.CutText(item.MessageId.ToString(), 100),
                                        IdEvento = string.IsNullOrEmpty(item.MessageId) ? "" : FunctionsText.CutText(item.MessageId.ToString(), 100),
                                        Message = FunctionsText.CutText(JsonConvert.SerializeObject(item), 2000),
                                        Telefonica = item.Telefonica
                                    };
                                    lstEvent.Add(evento);
                                    cLote += 1;

                                    if (cLote == 20)
                                    {
                                        //Ingreso lotes de a 20
                                        var resultLote = await RetryPolicySQL.ExecuteAsync(async () => await _eventoSMSServices.AddEventsSMSByStoreAsync(lstEvent));
                                        lstEvent.Clear();
                                        cLote = 0;
                                    }
                                }
                                if (lstEvent.Count > 0)
                                {
                                    var resultFinal = await RetryPolicySQL.ExecuteAsync(async () => await _eventoSMSServices.AddEventsSMSByStoreAsync(lstEvent));
                                }
                            }
                            //Actualizo el registro de la tabla ProcesoEvento con finalizado
                            procesoEventoDto.Cantidad = (eventsSMS is null) ? 0 : eventsSMS.Count;
                            procesoEventoDto.Estado = (eventsSMS is null) ? (int)PlanificacionProcesoEvento.error : (int)PlanificacionProcesoEvento.finalizado;
                            procesoEventoDto.FechaUltimaModificacion = DateTime.Now;
                            await RetryPolicySQL.ExecuteAsync(async () => await ProcesoEventoServices.UpdateProcesoEventoAsync(procesoEventoDto));
                        }
                        catch (Exception e)
                        {
                            TelemetryClient.TrackException(e);
                            Logger.LogError($"EventSMSError: {e}");

                            //Actualizo el registro de la tabla ProcesoEvento con error
                            procesoEventoDto.Estado = (int)PlanificacionProcesoEvento.error;
                            procesoEventoDto.FechaUltimaModificacion = DateTime.Now;
                            procesoEventoDto.Cantidad = 0;
                            await RetryPolicySQL.ExecuteAsync(async () => await ProcesoEventoServices.UpdateProcesoEventoAsync(procesoEventoDto));
                            throw;
                        }
                        finally
                        {
                            time.Stop();
                            Logger.LogInformation($"EventSMSEnd: {time.ElapsedMilliseconds}");
                        }
                    }
                }
                catch (System.Exception ex) when (Log(ex))
                {
                }
                finally
                {
                    TelemetryClient.TrackEvent("End", properties: new Dictionary<string, string> { { "parameters", parameters } }, metrics: new Dictionary<string, double> { { "time", time.ElapsedMilliseconds } });
                }
            }
        }


        private long? getIdComunicacion(string strIdComunicacion)
        {
            long lngIdComunicacion;
            long.TryParse(strIdComunicacion, out lngIdComunicacion);
            if (lngIdComunicacion == 0)
                return null;
            else
                return lngIdComunicacion;
        }
    }
}

