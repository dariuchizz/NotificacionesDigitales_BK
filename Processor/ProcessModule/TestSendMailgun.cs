using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Enum;
using Hangfire;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Processor.Dto;
using Processor.HangfireProcess;

namespace Processor.ProcessModule
{
    public class TestSendMailgun : IProcess
    {
        const string TEST_SEND_MAILGUN = "TEST_SEND_MAILGUN";
        private readonly ILogger<ProcessManager> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITipoComunicacionServices _tipoComunicacionServices;
        private readonly ICampaniaServices _campaniaServices;
        private readonly IMailgun _mailgun;
        private readonly TelemetryClient _telemetryClient;

        public TestSendMailgun(ILogger<ProcessManager> logger, TelemetryClient telemetryClient, IConfiguration configuration, ITipoComunicacionServices tipoComunicacionServices, ICampaniaServices campaniaServices, IMailgun mailgun)
        {
            _logger = logger;
            _configuration = configuration;
            _tipoComunicacionServices = tipoComunicacionServices;
            _telemetryClient = telemetryClient;
            _campaniaServices = campaniaServices;
            _mailgun = mailgun;
        }

        public void Dispose()
        {
        }

        // "{\"tipoEnvio\":\"1\",\"email\":\"rhuberman@xsidesolutions.com\",\"tipoComunicacion\":\"7\"}"
        // EJEMPLO DE NEGOCIO DE VENCIMIENTO DE FACTURAS CON PARAMETROS VARIABLES PARA EL HTML
        // "{\"tipoEnvio\":\"2\",\"email\":\"damian.lococo@camuzzigas.com.ar\",\"tipoComunicacion\":\"1\",\"nombreApellido\": \"test_nombreApellido\",\"nombre\": \"test_nombre\",\"fechaVencimiento\": \"test_fechaVencimiento\",\"periodo\": \"test_periodo\",\"cuentaUnificadaFormat\": \"test_cuentaUnificadaFormat\",\"domicilioSuministro\": \"test_domicilioSuministro\",\"cuentaUnificada\": \"test_cuentaUnificada\",\"guid\": \"test_guid\",\"IdComunicacion\": \"IdComunicacion\"}"
        //
        [Queue("mailgun-test")]
        public async Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = System.Diagnostics.Stopwatch.StartNew();
            using (_telemetryClient.StartOperation<Microsoft.ApplicationInsights.DataContracts.RequestTelemetry>(TEST_SEND_MAILGUN))
            {
                _telemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "parameters", parameters } });
                try
                {
                    if (string.IsNullOrEmpty(parameters))
                    {
                        return;
                    }

                    var jsonDto = JsonConvert.DeserializeObject<TestMailgunDto>(parameters);
                    jsonDto.JsonData = parameters;

                    _logger.LogInformation($"TipoComunicacionStart: {jsonDto.TipoComunicacion}");
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    DynamicDto dinamico;
                    SendMessageDto sendMessage;
                    if (jsonDto.TipoEnvio == 1)
                    {
                        var tipo = await _campaniaServices.GetWithRelationshipsAsync(jsonDto.TipoComunicacion);
                        dinamico = new DynamicDto
                        {
                            Message = "",
                            Email = jsonDto.Email,
                            idComunicacion = tipo.IdCampania,
                            IdCanal = Canal.Mailgun,
                        };
                        sendMessage = new SendMessageDto
                        {
                            Asunto = tipo.Asunto,
                            Template = tipo.TemplateName,
                            Tag = tipo.Tag,
                            TipoEnvio = TipoEnvioType.Campania
                        };
                    }
                    else
                    {
                        var tipo = await _tipoComunicacionServices.GetAsync(jsonDto.TipoComunicacion);
                        dinamico = new DynamicDto
                        {
                            Message = "",
                            Email = jsonDto.Email,
                            idComunicacion = tipo.IdTipoComunicacion,
                            IdCanal = Canal.Mailgun,
                        };
                        sendMessage = new SendMessageDto
                        {
                            Asunto = tipo.Asunto,
                            Template = tipo.Template,
                            Tag = tipo.Tag,
                            TipoEnvio = TipoEnvioType.Negocio
                        };
                    }

                    dinamico.JsonData = jsonDto.JsonData;
                    await SendMailgunAsync(sendMessage, dinamico);
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

        public async Task SendMailgunAsync(SendMessageDto tipo, DynamicDto item)
        {
            var sended = new MailgunResponse();
            try
            {
                var request = new MailgunRequest
                {
                    To = item.Email,
                    Subject = tipo.Asunto,
                    Tag = tipo.Tag,
                    Template = tipo.Template,
                    Variables = item.JsonData
                };

                await _mailgun.SendMessageAsync(tipo.TipoEnvio, request, _configuration);
            }
            catch (Exception e)
            {
                _logger.LogError($"SendMessageError: Email: '{item.Email}' | {e}");
                _telemetryClient.TrackException(e);
            }
        }
        private bool Log(System.Exception ex)
        {
            _telemetryClient.TrackException(ex);
            return false;
        }
    }

    public class TestMailgunDto
    {
        public int TipoEnvio { get; set; }
        public string Email { get; set; }
        public int TipoComunicacion { get; set; }
        public string JsonData { get; set; }
    }

}
