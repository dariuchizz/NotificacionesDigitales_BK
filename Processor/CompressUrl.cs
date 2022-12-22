using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Processor.Dto;
using Processor.ProcessModule;
using RestSharp;

namespace Processor
{
    public class CompressUrl : ICompressUrl
    {
        public async Task<string> CompressAsync(IConfiguration configuration, string url)
        {
            var request = new RestRequest();
            request.AddParameter("short", url);
            request.AddParameter("key", configuration.GetValue<string>("UrlCompressApiKey"));
            request.AddParameter("noTitle", 1);
            request.AddParameter("userDomain", "1");
            var response = await GetRestClient(configuration).ExecuteAsync(request, Method.GET);
            if (response.IsSuccessful)
            {
                var conten = JsonConvert.DeserializeObject<CuttlyContent>(response.Content.ToString());
                if (conten.Url.Status == 7)
                    return conten.Url.ShortLink;
                else
                    throw new ComunicationException("Error al comprimir la URL, Status del servicio cuttly" + conten.Url.Status);
            }
            else
                throw new ComunicationException("Error al comprimir la URL " + response.ErrorMessage);
        }

        private RestClient GetRestClient(IConfiguration configuration)
        {
            return new RestClient
            {
                BaseUrl = new Uri(configuration.GetValue<string>("UrlCompress"))
            };
        }
    }
}
