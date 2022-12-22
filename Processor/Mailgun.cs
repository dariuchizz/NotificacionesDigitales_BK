using Newtonsoft.Json;
using Processor.Dto;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading.Tasks;
using Common.Model.Enum;
using Microsoft.Extensions.Configuration;
using Processor.HangfireProcess;
using Microsoft.Extensions.Logging;
using Processor.ProcessModule;

namespace Processor
{
    public class Mailgun : IMailgun
    {
        public async Task<MailgunContentLog> ReadFirstQuery(TipoEnvioType tipoEnvio, DateTime dMailgun, IConfiguration configuration, ILogger<ProcessManager> _logger)
        {
            IConfigurationSection section;
            switch (tipoEnvio)
            {
                case TipoEnvioType.Campania:
                    section = configuration.GetSection("MailgunNotificaciones");
                    break;
                case TipoEnvioType.Negocio:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
                default:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
            }
            RestRequest request = new RestRequest();
            request.AddParameter("domain", section.GetValue<string>("Domain"), ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("begin", string.Format(new System.Globalization.CultureInfo("en-GB"), "{0:dddd, dd MMMM yyyy HH}", dMailgun.ToUniversalTime()) + ":00:00 -0000");
            request.AddParameter("end", string.Format(new System.Globalization.CultureInfo("en-GB"), "{0:dddd, dd MMMM yyyy HH}", dMailgun.ToUniversalTime()) + ":59:59 -9999");
            request.AddParameter("ascending", "yes");
            request.AddParameter("limit", section.GetValue<int>("CountRecolectorEvent"));
            request.AddParameter("pretty", "yes");
            var result = await GetRestClient(section, tipoEnvio).ExecuteAsync(request);
            if (!result.IsSuccessful)
            {
                var msg = $"EventosError ReadFirstQuery(): {result.ErrorMessage}. Content {result?.Content?.ToString()}";
                _logger.LogError(msg);
                throw new ComunicationException(msg);
            }

            return JsonConvert.DeserializeObject<MailgunContentLog>(result.Content.ToString());
        }

        public async Task<MailgunContentLog> ReadNextQuery(TipoEnvioType tipoEnvio, MailgunContentLog mailgunContentLog, IConfiguration configuration, ILogger<ProcessManager> _logger)
        {
            IConfigurationSection section;
            switch (tipoEnvio)
            {
                case TipoEnvioType.Campania:
                    section = configuration.GetSection("MailgunNotificaciones");
                    break;
                case TipoEnvioType.Negocio:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
                default:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
            }
            RestRequest request = new RestRequest();
            request.AddParameter("domain", section.GetValue<string>("Domain"), ParameterType.UrlSegment);
            var uri = mailgunContentLog.Paging.Next;
            var value = uri.PathAndQuery;
            var parts = value.Split('/');
            request.Resource = "{domain}/events/" + parts[4];
            var result = await GetRestClient(section, tipoEnvio).ExecuteAsync(request);
            if (!result.IsSuccessful)
            {
                var msg = $"EventosError ReadNextQuery(): {result.ErrorMessage}. Content { result?.Content?.ToString()} ";
                _logger.LogError(msg);
                throw new ComunicationException(msg);
            }

            return JsonConvert.DeserializeObject<MailgunContentLog>(result.Content.ToString());
        }

        public async Task<MailgunContentLog> ReadFirstQuery(bool esAviso, DateTime dMailgun, IConfiguration configuration, ILogger<ProcessManager> _logger)
        {
            IConfigurationSection section;
            if (esAviso)
                section = configuration.GetSection("MailgunAvisos");
            else
                section = configuration.GetSection("MailgunNotificaciones");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", section.GetValue<string>("Domain"), ParameterType.UrlSegment);
            request.Resource = "{domain}/events";
            request.AddParameter("begin", string.Format(new System.Globalization.CultureInfo("en-GB"), "{0:dddd, dd MMMM yyyy HH}", dMailgun.ToUniversalTime()) + ":00:00 -0000");
            request.AddParameter("end", string.Format(new System.Globalization.CultureInfo("en-GB"), "{0:dddd, dd MMMM yyyy HH}", dMailgun.ToUniversalTime()) + ":59:59 -9999");
            request.AddParameter("ascending", "yes");
            request.AddParameter("limit", section.GetValue<int>("CountRecolectorEvent"));
            request.AddParameter("pretty", "yes");
            var result = await GetRestClient(section, esAviso).ExecuteAsync(request);
            if (!result.IsSuccessful)
            {
                var msg = $"EventosError ReadFirstQuery(): {result.ErrorMessage}. Content {result?.Content?.ToString()}";
                _logger.LogError(msg);
                throw new ComunicationException(msg);
            }

            return JsonConvert.DeserializeObject<MailgunContentLog>(result.Content.ToString());
        }

        public async Task<MailgunContentLog> ReadNextQuery(Boolean esAviso, MailgunContentLog mailgunContentLog, IConfiguration configuration, ILogger<ProcessManager> _logger)
        {
            IConfigurationSection section;
            if (esAviso)
                section = configuration.GetSection("MailgunAvisos");
            else
                section = configuration.GetSection("MailgunNotificaciones");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", section.GetValue<string>("Domain"), ParameterType.UrlSegment);
            var uri = mailgunContentLog.Paging.Next;
            var value = uri.PathAndQuery;
            var parts = value.Split('/');
            request.Resource = "{domain}/events/" + parts[4];
            var result = await GetRestClient(section, esAviso).ExecuteAsync(request);
            if (!result.IsSuccessful)
            {
                var msg = $"EventosError ReadNextQuery(): {result.ErrorMessage}. Content { result?.Content?.ToString()} ";
                _logger.LogError(msg);
                throw new ComunicationException(msg);
            }

            return JsonConvert.DeserializeObject<MailgunContentLog>(result.Content.ToString());
        }


        public async Task<MailgunResponse> SendMessageAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration)
        {
            IConfigurationSection section;
            switch (tipoEnvio)
            {
                case TipoEnvioType.Campania:
                    section = configuration.GetSection("MailgunNotificaciones");
                    break;
                case TipoEnvioType.Negocio:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
                default:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
            }

            RestRequest request = new RestRequest();
            request.AddParameter("domain", section.GetValue<string>("Domain"), ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", section.GetValue<string>("From"));
            request.AddParameter("to", string.Join(";", requestDto.To.Trim()));
            request.AddParameter("subject", requestDto.Subject);
            request.AddParameter("o:tracking", requestDto.Tracking);
            request.AddParameter("o:tracking-clicks", requestDto.TrackingClicks);
            request.AddParameter("o:tracking-opens", requestDto.TrackingOpens);
            request.AddParameter("o:tag", requestDto.Tag);
            request.AddParameter("template", requestDto.Template);
            if (section.GetValue<bool>("TestMode"))
            {
                request.AddParameter("o:testmode", "true");
            }
            if (!string.IsNullOrEmpty(requestDto.Variables))
            {
                request.AddParameter("h:X-Mailgun-Variables", requestDto.Variables);
            }
            request.Method = Method.POST;
            var mailgunResponse = await GetRestClient(section, tipoEnvio).ExecuteAsync(request);
            if (mailgunResponse.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<MailgunResponse>(mailgunResponse.Content);
            }
            return new MailgunResponse
            {
                Message = (mailgunResponse.ErrorMessage ?? mailgunResponse.Content?.ToString()) ?? mailgunResponse.StatusCode.ToString()
            };
        }
        public async Task<MailgunTemplateResponse> AddTemplateAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration, ILogger<ProcessManager> _logger)
        {
            IConfigurationSection section;
            switch (tipoEnvio)
            {
                case TipoEnvioType.Campania:
                    section = configuration.GetSection("MailgunNotificaciones");
                    break;
                case TipoEnvioType.Negocio:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
                default:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
            }

            var request = new RestRequest();
            request.Resource = "{domain}/templates";
            request.AddParameter("domain", section.GetValue<string>("Domain"), ParameterType.UrlSegment);
            request.AddParameter("name", requestDto.Template);
            request.AddParameter("description", requestDto.Template);
            request.AddParameter("tag", "inital");
            request.AddParameter("template", requestDto.TemplateHtml);
            request.Method = Method.POST;

            var result = await GetRestClient(section, tipoEnvio).ExecuteAsync(request);
            if (result.IsSuccessful) return JsonConvert.DeserializeObject<MailgunTemplateResponse>(result.Content);
            var msg = $"GetTemplateError GetTemplateAsync(): {result.ErrorMessage}. Content { result.Content} ";
            _logger.LogError(msg);
            throw new ComunicationException(msg);

        }

        public async Task<MailgunTemplateResponse> GetTemplateAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration, ILogger<ProcessManager> _logger)
        {
            IConfigurationSection section;
            switch (tipoEnvio)
            {
                case TipoEnvioType.Campania:
                    section = configuration.GetSection("MailgunNotificaciones");
                    break;
                case TipoEnvioType.Negocio:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
                default:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
            }

            var request = new RestRequest { Resource = "/{domain}/templates/{name}" };
            request.AddUrlSegment("domain", section.GetValue<string>("Domain"));
            request.AddUrlSegment("name", requestDto.Template);
            request.AddParameter("active", "yes");
            var result = await GetRestClient(section, tipoEnvio).ExecuteAsync(request);
            if (string.IsNullOrEmpty(result.ErrorMessage)) return JsonConvert.DeserializeObject<MailgunTemplateResponse>(result.Content);
            var msg = $"GetTemplateError GetTemplateAsync(): {result.ErrorMessage}. Content { result.Content} ";
            _logger.LogError(msg);
            throw new ComunicationException(msg);

        }

        public async Task<MailgunTemplateResponse> UpdateTemplateAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration, ILogger<ProcessManager> _logger)
        {
            IConfigurationSection section;
            switch (tipoEnvio)
            {
                case TipoEnvioType.Campania:
                    section = configuration.GetSection("MailgunNotificaciones");
                    break;
                case TipoEnvioType.Negocio:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
                default:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
            }

            var domain = section.GetValue<string>("Domain");
            //var request = new RestRequest { Resource = "{domain}/templates/{name}/versions/{tag}" };
            //request.AddParameter("domain", domain, ParameterType.UrlSegment);
            //request.AddParameter("name", requestDto.Template, ParameterType.UrlSegment);
            //request.AddParameter("tag", requestDto.Tag, ParameterType.UrlSegment);
            //request.AddParameter("template", requestDto.TemplateHtml);
            //request.AddParameter("comment", requestDto.Description);
            //request.AddParameter("active", "yes");
            var request = new RestRequest { Resource = $"/{domain}/template/{requestDto.Template}" };
            request.AddParameter("description", requestDto.Description);
            request.AddParameter("template", requestDto.TemplateHtml);
            request.Method = Method.PUT;
            var result = await GetRestClient(section, tipoEnvio).ExecuteAsync(request);
            if (result.IsSuccessful) return new MailgunTemplateResponse { Message = result.Content };
            //if (result.IsSuccessful) return JsonConvert.DeserializeObject<MailgunTemplateResponse>(result.Content);
            var msg = $"UpdateTemplateError UpdateTemplateAsync(): {result.ErrorMessage}. Content { result.Content} ";
            _logger.LogError(msg);
            throw new ComunicationException(msg);
        }

        public async Task<MailgunTemplateResponse> DeleteTemplateAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration, ILogger<ProcessManager> _logger)
        {
            IConfigurationSection section;
            switch (tipoEnvio)
            {
                case TipoEnvioType.Campania:
                    section = configuration.GetSection("MailgunNotificaciones");
                    break;
                case TipoEnvioType.Negocio:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
                default:
                    section = configuration.GetSection("MailgunAvisos");
                    break;
            }

            var domain = section.GetValue<string>("Domain");
            var request = new RestRequest();
            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/templates/{name}";
            request.AddUrlSegment("name", requestDto.Template);
            request.Method = Method.DELETE;
            var result = await GetRestClient(section, tipoEnvio).ExecuteAsync(request);
            if (result.IsSuccessful) return JsonConvert.DeserializeObject<MailgunTemplateResponse>(result.Content);
            var msg = $"UpdateTemplateError UpdateTemplateAsync(): {result.ErrorMessage}. Content { result.Content} ";
            _logger.LogError(msg);
            throw new ComunicationException(msg);
        }

        private RestClient GetRestClient(IConfigurationSection section, bool esAviso)
        {
            if (esAviso)
            {
                return new RestClient
                {
                    BaseUrl = new Uri(section.GetValue<string>("Url")),
                    Authenticator = new HttpBasicAuthenticator("api",
                        section.GetValue<string>("Key"))
                };
            }
            else
            {
                return new RestClient
                {
                    BaseUrl = new Uri(section.GetValue<string>("Url")),
                    Authenticator = new HttpBasicAuthenticator("api",
                        section.GetValue<string>("Key"))
                };
            }
        }
        private RestClient GetRestClient(IConfigurationSection section, TipoEnvioType tipoEnvio)
        {
            switch (tipoEnvio)
            {
                case TipoEnvioType.Campania:
                    return new RestClient
                    {
                        BaseUrl = new Uri(section.GetValue<string>("Url")),
                        Authenticator = new HttpBasicAuthenticator("api",
                            section.GetValue<string>("Key"))
                    };
                case TipoEnvioType.Negocio:
                    return new RestClient
                    {
                        BaseUrl = new Uri(section.GetValue<string>("Url")),
                        Authenticator = new HttpBasicAuthenticator("api",
                            section.GetValue<string>("Key"))
                    };
                default:
                    return new RestClient
                    {
                        BaseUrl = new Uri(section.GetValue<string>("Url")),
                        Authenticator = new HttpBasicAuthenticator("api",
                            section.GetValue<string>("Key"))
                    };
            }
        }

    }
}