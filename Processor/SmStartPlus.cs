using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Processor.Dto;
using Processor.HangfireProcess;
using Processor.ProcessModule;
using RestSharp;

namespace Processor
{
    public class SmStartPlus : ISmStartPlus
    {

        //http://www.smstartplus.com/delivery_report_sc.php?usuario=Camuzzi_sc&clave=InnovaCrm2018&dia=2020-07-07&hora=16
        public async Task<List<SMStartContentLog>> ReadEvents(string dSMStart, int hora, IConfiguration configuration, ILogger<ProcessManager> _logger)
        {
            string[] arrFechaAProcesar = dSMStart.Split("-");

            var section = configuration.GetSection("SmStartPlus");
            RestRequest request = new RestRequest();
            request.AddQueryParameter("usuario", section.GetValue<string>("User"));
            request.AddQueryParameter("clave", section.GetValue<string>("Password"));
            request.AddQueryParameter("dia", arrFechaAProcesar[2] + "-" + arrFechaAProcesar[1] + "-" + arrFechaAProcesar[0]);
            request.AddQueryParameter("hora", hora.ToString("D2"));
            var result = await GetEventRestClient(section).ExecuteAsync(request);
            if (!result.IsSuccessful)
            {
                var msg = $"EventosError ReadEvents(): {result.ErrorMessage}";
                _logger.LogError(msg);
                throw new ComunicationException(msg);
            }
            else
            {
                if (result.Content.Contains("\r\ntoo_much_requests(try later)"))
                {
                    var msg = $"EventosError ReadEvents(): too_much_requests(try later)";
                    _logger.LogError(msg);
                    throw new ComunicationException(msg);
                }
            }

            return JsonConvert.DeserializeObject<List<SMStartContentLog>>(result.Content.ToString());
        }

        public async Task<SmsResponse> SendAsync(IConfiguration configuration, SmsRequest requestDto, ILogger<ProcessManager> _logger)
        {
            var section = configuration.GetSection("SmStartPlus");
            var request = new RestRequest();
            request.AddQueryParameter("usuario", section.GetValue<string>("User"));
            request.AddQueryParameter("clave", section.GetValue<string>("Password"));
            request.AddQueryParameter("celular", requestDto.Numero);
            request.AddQueryParameter("mensaje", requestDto.Message);
            request.AddQueryParameter("dato", requestDto.IdComunicacion.ToString());
            var client = GetSendRestClient(section);
            _logger.LogInformation(client.BuildUri(request).AbsoluteUri);
            var response = await client.ExecuteAsync(request, Method.POST);
            _logger.LogInformation(response.Content);
            if (response.IsSuccessful)
            {
                return new SmsResponse
                {
                    Message = response.Content
                };
            }
            return new SmsResponse
            {
                Message = response.ErrorMessage
            };
        }

        private RestClient GetSendRestClient(IConfigurationSection section)
        {
            return new RestClient
            {
                BaseUrl = new Uri(section.GetValue<string>("UrlSend"))
            };
        }

        private RestClient GetEventRestClient(IConfigurationSection section)
        {
            return new RestClient
            {
                BaseUrl = new Uri(section.GetValue<string>("UrlEvent"))
            };
        }
    }
}
