using System;
using Common.IServices;
using Common.Model.Dto;
using Hangfire;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Common;
using Common.Enums;
using Common.Functions;
using Common.Model.Enum;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using Processor.Dto;
using Processor.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Processor.ProcessModule
{
    public class CampaniaProcess : IProcess
    {
        const string CAMPANA = "CAMPANA_PROCESS";
        const string CAMPANA_SEND = "CAMPANA_SEND";
        private readonly ILogger<ProcessManager> _logger;
        private readonly IStoreServices _storeServices;
        private readonly IComunicacionServices _comunicacionServices;
        private readonly IEnvioServices _envioServices;
        private readonly ICampaniaServices _campaniaServices;
        private readonly AsyncRetryPolicy _retryPolicySQL;
        private readonly TelemetryClient _telemetryClient;
        public CampaniaProcess(ILogger<ProcessManager> logger, TelemetryClient telemetryClient,
                IStoreServices storeServices, IComunicacionServices comunicacionServices,
                IEnvioServices envioServices, ICampaniaServices campaniaServices)
        {
            _logger = logger;
            _storeServices = storeServices;
            _comunicacionServices = comunicacionServices;
            _envioServices = envioServices;
            _campaniaServices = campaniaServices;
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

        [Queue("beta")]
        public async virtual Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            using (_telemetryClient.StartOperation<RequestTelemetry>(CAMPANA))
            {
                try
                {
                    _telemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "parameters", parameters } });
                    if (!int.TryParse(parameters, out var id))
                    {
                        return;
                    }

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
                    //campaniaDto.IdEstadoCampania = (long)EstadoProcesoType.EnEjecucion;
                    //await _campaniaServices.UpdateAsync(campaniaDto, 1);

                    var tope = campaniaDto.TopeLectura != 0 ? campaniaDto.TopeLectura : 10;

                    dynamic response;
                    if (!string.IsNullOrEmpty(campaniaDto.TipoCampania.StoreObtener))
                    {
                        List<dynamic> lstResponseLote;
                        var index = 0;
                        do
                        {
                            response = await _retryPolicySQL.ExecuteAsync(async () => await _storeServices.GetStoreCampaniaAsync(campaniaDto.TipoCampania.StoreObtener, campaniaDto.IdCampania, tope, 240)); //obtengo los registros del Store
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
                                        IdTipoComunicacion = campaniaDto.IdCampania,
                                        IdCanal = campaniaDto.IdCanalCampania,
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

                                _logger.LogInformation($"Campania-Enqueue-SendigMail: {index}");
                                var sendMessage = new SendMessageDto
                                {
                                    TipoEnvio = TipoEnvioType.Campania,
                                    Template = campaniaDto.TemplateName,
                                    Tag = campaniaDto.Tag,
                                    Asunto = campaniaDto.Asunto
                                };
                                BackgroundJob.Enqueue(() => SendMessageAsync(index, request, sendMessage, campaniaDto, CancellationToken.None)); //Pongo en cola el proceso para mandar los mensajes
                            }
                        } while (lstResponseLote.Count > 0);
                    }
                }
                catch (Exception ex) when (Log(ex))
                {
                }
                finally
                {
                    time.Stop();
                    _telemetryClient.TrackEvent("End", properties: new Dictionary<string, string> { { "parameters", parameters } }, metrics: new Dictionary<string, double> { { "time", time.ElapsedMilliseconds } });
                }
            }
        }

        [Queue("beta")]
        public async virtual Task SendMessageAsync(int index, IList<DynamicDto> items, SendMessageDto sendMessage, CampaniaDto campaniaDto, CancellationToken cancellationToken)
        {
            //Se verfica que la campaña es por SMS y se esta ejectando en el ranto 11 a 19.
            //De lo contrario se programa para el dia siguiente a las 11 de la mañana.
            if ((campaniaDto.IdCanalCampania == 2) && (DateTime.Now.Hour < 11 || DateTime.Now.Hour > 19))
            {
                BackgroundJob.Schedule(() =>
                    SendMessageAsync(index, items, sendMessage, campaniaDto, cancellationToken), new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day,11,0,0));
            }
            else
            {
                var time = Stopwatch.StartNew();
                using (_telemetryClient.StartOperation<RequestTelemetry>(CAMPANA_SEND))
                {
                    try
                    {
                        _telemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "IdCampania", campaniaDto.IdCampania.ToString() } });
                        await _envioServices.SendMessageAsync(index, items, sendMessage, cancellationToken);
                    }
                    catch (Exception ex) when (Log(ex))
                    {
                    }
                    finally
                    {
                        time.Stop();
                        _telemetryClient.TrackEvent("End", properties: new Dictionary<string, string> { { "IdCampania", campaniaDto.IdCampania.ToString() } }, metrics: new Dictionary<string, double> { { "time", time.ElapsedMilliseconds } });
                    }
                }
            }
        }

        private bool Log(System.Exception ex)
        {
            _telemetryClient.TrackException(ex);
            return false;
        }

        public void Dispose()
        {
        }
    }
}
