using Common.IServices;
using Hangfire;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using NotificacionesDigitalesApi.Model;
using Processor.Dto;
using System.Diagnostics;
using System.Collections.Generic;
using Common.Model.Dto;
using Common.Functions;
using Newtonsoft.Json;
using Common.Model.NotificacionesDigitales;
using Common.Model.Enum;
using System.Threading;
using System.Text.RegularExpressions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Processor.ProcessModule
{
    public class EventMailProcess : BaseEvent, IProcess
    {
        const string EMAIL_PROCESS = "EMAIL_PROCESS";
        static private readonly Regex BOUNCE = new Regex("(^[0-9]{1}[.][0-9][.][0-9]{1,2})");

        private readonly IEventoEmailServices _eventoEmailServices;
        private readonly IConfiguration _configuration;
        private readonly IMailgun _mailgun;

        public EventMailProcess(ILogger<ProcessManager> logger, TelemetryClient telemetryClient,
                IProcesoEventoServices procesoEventoServices,
                IEventoEmailServices eventoEmailServices,
                IConfiguration configuration,
                IMailgun mailgun
            ) : base(logger, telemetryClient, procesoEventoServices)
        {
            _eventoEmailServices = eventoEmailServices;
            _configuration = configuration;
            _mailgun = mailgun;
        }

        public void Dispose()
        {
        }

        //"{\"fecha-a-procesar\":\"15-07-2020\",\"Hora-a-procesar\":\"16\",\"id-registro\":\"2\",\"tipo-envio\":\"2\"}" 
        //tipo-envio = 2 -> Negocio. Si es negocio se debe informar si es Aviso o no.
        //tipo-envio = 1 -> Campania
        [Queue("delta")]
        public async Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            using (TelemetryClient.StartOperation<RequestTelemetry>(EMAIL_PROCESS))
            {
                try
                {
                    TelemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "parameters", parameters } });
                    (EventProcessDto eventProcessDto, ProcesoEventoDto procesoEventoDto) = await ValidateParametersAsyc(parameters, TipoProcesoEvento.Email);
                    if (eventProcessDto == null)
                        return;
                    else
                    {
                        //Actualizo el registro de la tabla ProcesoEvento con procesando
                        await RetryPolicySQL.ExecuteAsync(async () => await ProcesoEventoServices.UpdateProcesoEventoAsync(procesoEventoDto));

                        Logger.LogInformation($"EventMailStart: {parameters}");
                        try
                        {
                            string[] arrFechaAProcesar = eventProcessDto.FechaAProcesar.Split("-");
                            var dMailgun = new DateTime(
                                Convert.ToInt32(arrFechaAProcesar[2]),
                                Convert.ToInt32(arrFechaAProcesar[1]),
                                Convert.ToInt32(arrFechaAProcesar[0]),
                                eventProcessDto.HoraAProcesar, 0, 0);

                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            //Todo:Pendiente Dominio

                            TipoEnvioType tipoEnvioType = eventProcessDto.TipoEnvio == 1 ? TipoEnvioType.Campania :
                                eventProcessDto.Aviso.HasValue ?
                                    eventProcessDto.Aviso.Value ? TipoEnvioType.Negocio : TipoEnvioType.Campania
                                : TipoEnvioType.Negocio;
                            var page = await RetryPolicyComunication.ExecuteAsync(async () => await _mailgun.ReadFirstQuery((bool)eventProcessDto.Aviso, dMailgun, _configuration, Logger));
                            MailgunContentLog mailgunContentLog = new MailgunContentLog();

                            List<EventoEmailDto> lstEvent;
                            while (!(page.Items is null) && page.Items.Length > 0)
                            {
                                int cLote = 0;
                                lstEvent = new List<EventoEmailDto>();
                                foreach (var item in page.Items)
                                {
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        return;
                                    }

                                    //Verifico por codigo el end de la 
                                    if (TimeZoneInfo.ConvertTimeFromUtc(UnixTimestampToDateTime(item.Timestamp), TimeZoneInfo.Local) >
                                        dMailgun.AddHours(1))
                                    {
                                        page = null;
                                        break;
                                    }
                                    else
                                    {
                                        //Agrego un Evento para procesar el lote
                                        var evento = new EventoEmailDto
                                        {
                                            Fecha = TimeZoneInfo.ConvertTimeFromUtc(UnixTimestampToDateTime(item.Timestamp), TimeZoneInfo.Local),
                                            DEvento = item.Event,
                                            IdExterno = FunctionsText.CutText(item.Message?.Headers?.MessageId, 100),
                                            IdEvento = FunctionsText.CutText(item.Id, 100),
                                            Message = FunctionsText.CutText(JsonConvert.SerializeObject(item), 2000),
                                            Reason = String.IsNullOrEmpty(item.Reason) ? "" : FunctionsText.CutText(item.Reason, 50),
                                            Code = item.DeliveryStatus?.Code,
                                            BounceCode = String.IsNullOrEmpty(item.DeliveryStatus?.BounceCode) ? GetBounceCode(item.DeliveryStatus?.Message) : FunctionsText.CutText(item.DeliveryStatus?.BounceCode, 10),
                                            Severity = String.IsNullOrEmpty(item.Severity) ? "" : FunctionsText.CutText(item.Severity, 10),
                                            MessageError = String.IsNullOrEmpty(item.DeliveryStatus?.Message) ? "" : FunctionsText.CutText(item.DeliveryStatus?.Message, 2000)
                                        };

                                        lstEvent.Add(evento);
                                        cLote += 1;
                                        //Suma uno a la cantidad para dejar registrado en la tabla procesoEvento
                                        procesoEventoDto.Cantidad += 1;

                                        if (cLote == 20)
                                        {
                                            //Ingreso lotes de a 20 (limite de parametros)
                                            var resultAE = await RetryPolicySQL.ExecuteAsync(async () => await _eventoEmailServices.AddEventsEmailsByStoreAsync(lstEvent));
                                            lstEvent.Clear();
                                            cLote = 0;
                                        }
                                    }
                                }
                                if (lstEvent.Count > 0)
                                {
                                    var resultAE = await RetryPolicySQL.ExecuteAsync(async () => await _eventoEmailServices.AddEventsEmailsByStoreAsync(lstEvent));
                                }

                                if (!(page is null))
                                {
                                    //leer siguiente pagina
                                    //Pendiente dominio
                                    page = await RetryPolicyComunication.ExecuteAsync(async () => await _mailgun.ReadNextQuery((bool)eventProcessDto.Aviso, page, _configuration, Logger));
                                }
                                else
                                    break;
                            }
                            //Actualizo el registro de la tabla ProcesoEvento con finalizado
                            procesoEventoDto.Estado = (int)PlanificacionProcesoEvento.finalizado;
                            procesoEventoDto.FechaUltimaModificacion = DateTime.Now;
                            await RetryPolicySQL.ExecuteAsync(async () => await ProcesoEventoServices.UpdateProcesoEventoAsync(procesoEventoDto));
                        }
                        catch (Exception e)
                        {
                            Logger.LogError($"EventMailError: {e}");

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
                            Logger.LogInformation($"EventMailEnd: {time.ElapsedMilliseconds}");
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

        private string GetBounceCode(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "";
            else
            {
                var match = BOUNCE.Match(message);
                return match.Value;
            }
        }
    }

}

