using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using Microsoft.ApplicationInsights;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Processor.HangfireProcess;
using Hangfire;
using Common.Model.Dto;
using Common.IServices;

namespace Processor.ProcessModule
{
    public class IndexedReceiptProcess : IProcess
    {
        const string INDEXED_RECEIPT_PROCESS = "INDEXED_RECEIPT_PROCESS";

        private readonly INotificacionHuemulServices _comunicacionHuemulServices;
        private readonly ILogger<ProcessManager> _logger;
        private readonly IConfiguration _configuration;
        private readonly TelemetryClient _telemetryClient;
        private readonly IHuemul _huemul;
        private readonly IMemoryCache _memoryCache;
        private readonly AsyncRetryPolicy _retryPolicySQL;
        private readonly AsyncRetryPolicy _retryComunication;

        public IndexedReceiptProcess(
            ILogger<ProcessManager> logger,
            TelemetryClient telemetryClient,
            IConfiguration configuration,
            IHuemul huemul,
            IMemoryCache memoryCache,
            INotificacionHuemulServices comunicacionHuemulServices)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _configuration = configuration;
            _huemul = huemul;
            _memoryCache = memoryCache;
            _comunicacionHuemulServices = comunicacionHuemulServices;
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

        public void Dispose()
        {
        }

        [Queue("mailgun-test")]
        public async Task ExecuteAsync(string parameters, CancellationToken cancellationToken)
        {
            var time = System.Diagnostics.Stopwatch.StartNew();
            using (_telemetryClient.StartOperation<Microsoft.ApplicationInsights.DataContracts.RequestTelemetry>(INDEXED_RECEIPT_PROCESS))
            {
                _telemetryClient.TrackEvent("Start", properties: new Dictionary<string, string> { { "parameters", parameters } });
                try
                {
                    var comprobanteIndexadoEventoQueue = await _huemul.GenerateQueueClientAsync(_configuration);
                    if (comprobanteIndexadoEventoQueue.Exists())
                    {
                        //Obtengo los mensaje de la cola, maximo 32
                        QueueMessage[] receivedMessages = await comprobanteIndexadoEventoQueue.ReceiveMessagesAsync(32);

                        int cLote = 0;
                        List<NotificacionHuemulDto> lstcomunicacionHuemul = new List<NotificacionHuemulDto>();
                        foreach (QueueMessage message in receivedMessages)
                        {
                            string[] dataMessage = message.Body.ToString().Split(";");

                            //Obtener el link de hueml del comprobante para despues agregarlo en el correo/sms
                            var link = await _retryComunication.ExecuteAsync(async () => await _huemul.GetLinkHuemulAsync(dataMessage[0], dataMessage[1], dataMessage[2], _configuration, _logger, _memoryCache));

                            //Agrego un ComunicacionHuemul para procesar el lote
                            var huemulMessage = new NotificacionHuemulDto
                            {
                                CuentaUnificada = long.Parse(dataMessage[0]),
                                TipoComprobante = dataMessage[1],
                                NroComprobante = dataMessage[2],
                                Canal = dataMessage[3],
                                LinkHuemul = link,
                                messageHuemul = message
                            };
                            lstcomunicacionHuemul.Add(huemulMessage);
                            cLote += 1;

                            if (cLote == 20)
                            {
                                //Ingreso lotes de a 20
                                var resultAE = await _retryPolicySQL.ExecuteAsync(async () => await _comunicacionHuemulServices.AddNotificacionesHuemulByStoreAsync(lstcomunicacionHuemul));
                                //Elimino el mensaje de la cola que recien pude ingresar
                                foreach (NotificacionHuemulDto comunicaionHuemul in lstcomunicacionHuemul)
                                {
                                    var del = await comprobanteIndexadoEventoQueue.DeleteMessageAsync(comunicaionHuemul.messageHuemul.MessageId, comunicaionHuemul.messageHuemul.PopReceipt);
                                }
                                lstcomunicacionHuemul.Clear();
                                cLote = 0;
                            }
                        }
                        if (lstcomunicacionHuemul.Count > 0)
                        {
                            //Insert en la DB la notificacion de Huemul en lote
                            var resultAE = await _retryPolicySQL.ExecuteAsync(async () => await _comunicacionHuemulServices.AddNotificacionesHuemulByStoreAsync(lstcomunicacionHuemul));
                            //Elimino el mensaje de la cola que recien pude ingresar
                            foreach (NotificacionHuemulDto comunicaionHuemul in lstcomunicacionHuemul)
                            {
                                var del = await comprobanteIndexadoEventoQueue.DeleteMessageAsync(comunicaionHuemul.messageHuemul.MessageId, comunicaionHuemul.messageHuemul.PopReceipt);
                            }
                        }
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

        private bool Log(System.Exception ex)
        {
            _telemetryClient.TrackException(ex);
            return false;
        }
    }
}