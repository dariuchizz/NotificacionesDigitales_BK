using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Queues;
using Common;
using Common.Model.Response;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;
using Processor.ProcessModule;
using Microsoft.Extensions.Caching.Memory;

namespace Processor
{
    public class Huemul : IHuemul
    {
        public async Task<QueueClient> GenerateQueueClientAsync(IConfiguration configuration)
        {
            var connectionString = default(string);
            if (configuration[Utils.STORAGE_ACCOUNT_NAME].Equals(Utils.STORAGE_ACCOUNT_EMULATOR))
            {
                connectionString = Utils.STORAGE_EMULATOR_ACCOUNT_CONNECTION_STRING;
            }
            else
            {
                connectionString = string.Format(Utils.STORAGE_CONNECTION_STRING_TEMPLATE, configuration[Utils.STORAGE_ACCOUNT_NAME], configuration[Utils.STORAGE_ACCOUNT_KEY]);
            }
            var comprobanteIndexadoEventoQueue = new QueueClient(connectionString, Utils.INDEXED_EVENTS_QUEUE);
            return comprobanteIndexadoEventoQueue;
        }

        public async Task<string> GetLinkHuemulAsync(string cuenta, string tipoComprobante, string comprobante, IConfiguration configuration, ILogger<ProcessManager> _logger, IMemoryCache _memoryCache)
        {
            var section = configuration.GetSection("Huemul");
            var tokendeHueml = await GetTokenAuthAsync(section, _logger, _memoryCache);

            var request = new RestRequest("​Comprobante/cuentas​/{cuenta}​/tiposcomprobantes​/{tipoComprobante}​/comprobantes​/{comprobante}​/link".Replace("\u200B", ""), Method.GET);
            request.AddUrlSegment("cuenta", cuenta);
            request.AddUrlSegment("tipoComprobante", tipoComprobante);
            request.AddUrlSegment("comprobante", comprobante);

            request.AddHeader("Authorization"
                    , $"Bearer {tokendeHueml}");

            var client = GetLinkHuemulClient(section);
            var response = await client.ExecuteAsync(request, Method.GET);
            if (!response.IsSuccessful)
            {
                var msg = $"Huemul GetLinkHuemul(): {response.ErrorMessage}. Content {response?.Content?.ToString()}";
                _logger.LogError(msg);
                throw new ComunicationException(msg);
            }
            return JsonConvert.DeserializeObject<string>(response.Content);
        }

        private async Task<string> GetTokenAuthAsync(IConfigurationSection section, ILogger<ProcessManager> _logger, IMemoryCache _memoryCache)
        {
            TokenResponseWeb tokendeNotificaciones;
            var cache = _memoryCache.Get("TokenHuemul") as TokenResponseWeb;

            if (cache is TokenResponseWeb)
            {
                var currentTokenRespone = cache as TokenResponseWeb;
                if (currentTokenRespone.Expiration > DateTime.UtcNow.AddMinutes(2))
                    return currentTokenRespone.Token;
            }

            var clientId = section.GetValue<string>("ClientIdHuemul");
            var secret = section.GetValue<string>("SecretHuemul");
            var basic64String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{secret}"));

            var request = new RestRequest();
            request.AddHeader("Authorization"
                    , $"Basic {basic64String}");
            var client = GetAuthTokenClient(section);
            var response = await client.ExecuteAsync(request, Method.GET);
            if (!response.IsSuccessful)
            {
                var msg = $"Huemul GetTokenAuthAsync(): {response.ErrorMessage}. Content {response?.Content?.ToString()}";
                _logger.LogError(msg);
                throw new ComunicationException(msg);
            }

            var jsonString = response.Content.ToString();
            TokenResponseWeb AuthResponse = JsonConvert.DeserializeObject<TokenResponseWeb>(jsonString);

            int time;
            int.TryParse(section.GetValue<string>("CacheTime_TokenHuemul"), out time);
            _memoryCache.Set("TokenHuemul", AuthResponse, TimeSpan.FromMinutes(time));

            return AuthResponse.Token;
        }

        private RestClient GetAuthTokenClient(IConfigurationSection section)
        {
            return new RestClient
            {
                BaseUrl = new Uri(section.GetValue<string>("APITokenHuemul"))
            };
        }

        private RestClient GetLinkHuemulClient(IConfigurationSection section)
        {
            return new RestClient
            {
                BaseUrl = new Uri(section.GetValue<string>("APILinkHuemul"))
            };
        }

    }
}