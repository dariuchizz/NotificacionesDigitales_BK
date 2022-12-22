using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Functions;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Enum;
using Microsoft.ApplicationInsights;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Processor.Dto;
using Processor.HangfireProcess;
using Processor.ProcessModule;

namespace Processor.Services
{
    public class EnvioServices : IEnvioServices
    {
        private readonly ILogger<ProcessManager> _logger;
        private readonly IComunicacionServices _comunicacionServices;
        private readonly ICambioContactoServices _cambioContactoServices;
        private readonly IConfiguration _configuration;
        private readonly AsyncRetryPolicy _retryPolicySQL;
        private readonly AsyncRetryPolicy _retryComunication;
        private readonly TelemetryClient _telemetryClient;
        private readonly IMailgun _mailgun;
        private readonly ISmStartPlus _smStartPlus;
        private readonly ICompressUrl _compressUrl;

        public EnvioServices(ILogger<ProcessManager> logger, IConfiguration configuration, IComunicacionServices comunicacionServices, ICambioContactoServices cambioContactoServices, TelemetryClient telemetryClient, IMailgun mailgun, ISmStartPlus smStartPlus, ICompressUrl compressUrl)
        {
            _logger = logger;
            _comunicacionServices = comunicacionServices;
            _cambioContactoServices = cambioContactoServices;
            _configuration = configuration;
            _telemetryClient = telemetryClient;
            _mailgun = mailgun;
            _smStartPlus = smStartPlus;
            _compressUrl = compressUrl;
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
                .Or<Exception>(x => x.Message != null && x.Message.Contains("timed out", StringComparison.InvariantCultureIgnoreCase))
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

        public async Task SendMessageAsync(int index, IList<DynamicDto> items, SendMessageDto tipo, CancellationToken cancellationToken)
        {
            var time = Stopwatch.StartNew();
            _logger.LogInformation($"SendMessageStart: {index}");
            foreach (DynamicDto item in items)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var comunicacion = await _comunicacionServices.GetAsync(item.idComunicacion);
                if (comunicacion.Enviado == 0)
                {
                    switch (item.IdCanal)
                    {
                        case Canal.Mailgun:
                            await SendMailgunAsync(tipo, item, time);
                            break;
                        case Canal.SmsStartPlus:
                            await SendSmsAsync(tipo, item, time);
                            break;
                        default: throw new Exception("No se encuentra el canal seleccionado");
                    }
                }

            }
            _logger.LogInformation($"SendMessageEnd: {index}, Time: {time.ElapsedMilliseconds}");
        }

        private async Task SendMailgunAsync(SendMessageDto tipo, DynamicDto item, Stopwatch time)
        {
            var sended = new MailgunResponse();
            try
            {
                var request = new MailgunRequest
                {
                    To = item.Email,
                    Subject = (string.IsNullOrEmpty(tipo.Asunto)) ? null : string.Format(tipo.Asunto, item.cuentaUnificadaFormat, item.fechaVencimiento),
                    Tag = tipo.Tag,
                    Template = tipo.Template,
                    Variables = item.JsonData
                };

                sended = await _retryComunication.ExecuteAsync(async () => await _mailgun.SendMessageAsync(tipo.TipoEnvio, request, _configuration));
                if (string.IsNullOrEmpty(sended.Id))
                {
                    if (string.IsNullOrEmpty(sended.Message))
                    {
                        sended.Message = "El correo informado es incorrecto o no existe";
                    }

                    throw new Exception(sended.Message);
                }

                await _retryPolicySQL.ExecuteAsync(async () => await UpdateComunicacionesSuccess(item, sended, EstadoComunicacion.Enviado));
                time.Stop();
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e);
                _logger.LogError($"SendMessageError: Email: '{item.Email}' | {e}");
                await _retryPolicySQL.ExecuteAsync(async () => await UpdateComunicacionesSuccess(item, sended, EstadoComunicacion.Error));
                if (sended.Message.IndexOf("parameter is not a valid address. please check documentatio") > 0)
                {
                    var cambioContacto = new CambioContactoDto
                    {
                        IdContacto = item.IdContacto,
                        IdCanal = 1,
                        Spam = false,
                        Procesado = "N",
                        FechaCreacion = DateTime.Now
                    };
                    await _retryPolicySQL.ExecuteAsync(async () => await _cambioContactoServices.AddContactChangeAsync(cambioContacto));
                }
                time.Stop();
            }
        }

        private async Task SendSmsAsync(SendMessageDto tipo, DynamicDto item, Stopwatch time)
        {
            var response = new SmsResponse();
            try
            {
                var link = (string.IsNullOrEmpty(tipo.Asunto)) ? null : string.Format(tipo.Asunto, item.CuentaUnificada, item.IdExterno, item.numeroFactura);
                var shortLink = (string.IsNullOrEmpty(tipo.Asunto)) ? null : await _retryComunication.ExecuteAsync(async () => await _compressUrl.CompressAsync(_configuration, link));
                var request = new SmsRequest
                {
                    IdComunicacion = item.idComunicacion,
                    Numero = item.Celular,
                    Message = $"{item.Message} {shortLink}",
                    Url = shortLink
                };
                response = await _smStartPlus.SendAsync(_configuration, request, _logger);
                if (response.Message.Equals("\r\ntransfer_data_ok"))
                    await _retryPolicySQL.ExecuteAsync(async () => await UpdateComunicacionesSuccess(item, response, EstadoComunicacion.Enviado));
                else
                {
                    _logger.LogError($"SendMessageError: SMS: ' {item.idComunicacion}' |  {item.Celular}' |  '{item.Message}' |  '{shortLink}' | {response.Message}");
                    await _retryPolicySQL.ExecuteAsync(async () => await UpdateComunicacionesSuccess(item, response, EstadoComunicacion.Error));
                }

                //await Task.Delay(1000);
            }
            catch (Exception e)
            {
                _telemetryClient.TrackException(e);
                _logger.LogError($"SendMessageError: SMS: ' {item.idComunicacion}' |  {item.Celular}' |  '{item.Message}' | Exception: {e}");
                await _retryPolicySQL.ExecuteAsync(async () => await UpdateComunicacionesSuccess(item, response, EstadoComunicacion.Error));
                time.Stop();
            }
        }

        private async Task UpdateComunicacionesSuccess(DynamicDto item, MailgunResponse sended, EstadoComunicacion estado)
        {
            var comunicacion = await _comunicacionServices.GetAsync(item.idComunicacion);
            comunicacion.Enviado = (int)estado;
            comunicacion.FechaProceso = DateTime.Now;
            comunicacion.IdExterno = FunctionsText.CutText(sended.Id?.Replace("<", "").Replace(">", ""), 100);
            comunicacion.Message = FunctionsText.CutText(sended.Message, 256);
            comunicacion.AutorModificacion = 1;
            comunicacion.FechaModificacion = DateTime.Now;
            await _retryPolicySQL.ExecuteAsync(async () => await _comunicacionServices.UpdateComunicacionAsync(comunicacion));
        }

        private async Task UpdateComunicacionesSuccess(DynamicDto item, SmsResponse sended, EstadoComunicacion estado)
        {
            var comunicacion = await _comunicacionServices.GetAsync(item.idComunicacion);
            comunicacion.Enviado = (int)estado;
            comunicacion.FechaProceso = DateTime.Now;
            comunicacion.IdExterno = "";
            comunicacion.Message = FunctionsText.CutText(sended.Message, 256);
            comunicacion.AutorModificacion = 1;
            comunicacion.FechaModificacion = DateTime.Now;
            await _retryPolicySQL.ExecuteAsync(async () => await _comunicacionServices.UpdateComunicacionAsync(comunicacion));
        }

    }
}
