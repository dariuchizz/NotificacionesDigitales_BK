using System;
using System.Collections.Generic;
using System.Net.Http;
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
using Polly;
using Polly.Retry;
using Processor.Dto;
using Processor.HangfireProcess;

namespace Processor.ProcessModule
{
    public class TestSendStartPlus : IProcess
    {
        const string TEST_SEND_STARTPLUS = "TEST_SEND_STARTPLUS";
        private readonly ILogger<ProcessManager> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITipoComunicacionServices _tipoComunicacionServices;
        private readonly ICampaniaServices _campaniaServices;
        private readonly AsyncRetryPolicy _retryComunication;
        private readonly ISmStartPlus _startPlus;
        private readonly TelemetryClient _telemetryClient;
        private readonly ICompressUrl _compressUrl;

    public TestSendStartPlus(ILogger<ProcessManager> logger, TelemetryClient telemetryClient, IConfiguration configuration, ITipoComunicacionServices tipoComunicacionServices, ICampaniaServices campaniaServices, ISmStartPlus startPlus, ICompressUrl compressUrl)
    {
        _logger = logger;
        _configuration = configuration;
        _tipoComunicacionServices = tipoComunicacionServices;
        _telemetryClient = telemetryClient;
        _campaniaServices = campaniaServices;
        _startPlus = startPlus;
        _compressUrl = compressUrl;
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

    public void Dispose()
    {
    }

    [Queue("mailgun-test")]
    public async Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
    {
        var time = System.Diagnostics.Stopwatch.StartNew();
        using (_telemetryClient.StartOperation<Microsoft.ApplicationInsights.DataContracts.RequestTelemetry>(TEST_SEND_STARTPLUS))
        {
            _telemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "parameters", parameters } });
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    return;
                }

                var jsonDto = JsonConvert.DeserializeObject<TestSMSDto>(parameters);
                //jsonDto.JsonData = parameters;

                _logger.LogInformation($"TipoComunicacionStart: {jsonDto.TipoComunicacion}");
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                DynamicDto dinamico;
                SendMessageDto sendMessage;
                //if (jsonDto.TipoEnvio == 1)
                //{
                    var tipo = await _campaniaServices.GetWithRelationshipsAsync(jsonDto.TipoComunicacion);
                    dinamico = new DynamicDto
                    {
                        Message = tipo.TextoSMS.Replace("{{Variable1}}", jsonDto.NombreValor),
                        Celular = jsonDto.Celular,
                        idComunicacion = tipo.IdCampania,
                        IdCanal = Canal.SmsStartPlus,
                    };
                    sendMessage = new SendMessageDto
                    {
                        Asunto = tipo.Asunto,
                        Template = tipo.TemplateName,
                        Tag = tipo.Tag,
                        TipoEnvio = TipoEnvioType.Campania
                    };
                //}
                //else
                //{
                //    var tipo = await _tipoComunicacionServices.GetAsync(jsonDto.TipoComunicacion);
                //    dinamico = new DynamicDto
                //    {
                //        Message = tipo.Asunto,
                //        Celular = jsonDto.Celular,
                //        idComunicacion = tipo.IdTipoComunicacion,
                //        IdCanal = Canal.SmsStartPlus,
                //    };
                //    sendMessage = new SendMessageDto
                //    {
                //        Asunto = tipo.Asunto,
                //        Template = tipo.Template,
                //        Tag = tipo.Tag,
                //        TipoEnvio = TipoEnvioType.Negocio
                //    };
                //}

                //dinamico.JsonData = jsonDto.JsonData;
                await SendSmsAsync(sendMessage, dinamico);
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

    public async Task SendSmsAsync(SendMessageDto tipo, DynamicDto item)
    {
        var sended = new SmsResponse();
        var link = (string.IsNullOrEmpty(tipo.Asunto)) ? null : string.Format(tipo.Asunto, item.CuentaUnificada, item.IdExterno, item.numeroFactura);
        var shortLink = (string.IsNullOrEmpty(tipo.Asunto)) ? null : await _retryComunication.ExecuteAsync(async () => await _compressUrl.CompressAsync(_configuration, link));
        try
        {
            var request = new SmsRequest
            {
                IdComunicacion = item.idComunicacion,
                Numero = item.Celular,
                Message = $"{item.Message} {shortLink}",
                Url = shortLink
            };
            await _startPlus.SendAsync(_configuration, request, _logger);
        }
        catch (Exception e)
        {
            _logger.LogError($"SendMessageError: Celular: '{item.Email}' | {e}");
            _telemetryClient.TrackException(e);
        }
    }

    private bool Log(System.Exception ex)
    {
        _telemetryClient.TrackException(ex);
        return false;
    }
    }

    public class TestStartPlusDto
    {
        public int TipoEnvio { get; set; }
        
        public string Celular { get; set; }

        public string NombreValor { get; set; }

        public int TipoComunicacion { get; set; }
    }
}
