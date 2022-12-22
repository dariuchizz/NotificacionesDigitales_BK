using Common.Model.Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Processor.Dto;
using Processor.HangfireProcess;
using System;
using System.Threading.Tasks;

namespace Processor
{
    public interface IMailgun
    {
        Task<MailgunTemplateResponse> AddTemplateAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration, ILogger<ProcessManager> _logger);
        Task<MailgunTemplateResponse> DeleteTemplateAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration, ILogger<ProcessManager> _logger);
        Task<MailgunTemplateResponse> GetTemplateAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration, ILogger<ProcessManager> _logger);
        Task<MailgunContentLog> ReadFirstQuery(bool esAviso, DateTime dMailgun, IConfiguration configuration, ILogger<ProcessManager> _logger);
        Task<MailgunContentLog> ReadFirstQuery(TipoEnvioType tipoEnvio, DateTime dMailgun, IConfiguration configuration, ILogger<ProcessManager> _logger);
        Task<MailgunContentLog> ReadNextQuery(bool esAviso, MailgunContentLog mailgunContentLog, IConfiguration configuration, ILogger<ProcessManager> _logger);
        Task<MailgunContentLog> ReadNextQuery(TipoEnvioType tipoEnvio, MailgunContentLog mailgunContentLog, IConfiguration configuration, ILogger<ProcessManager> _logger);
        Task<MailgunResponse> SendMessageAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration);
        Task<MailgunTemplateResponse> UpdateTemplateAsync(TipoEnvioType tipoEnvio, MailgunRequest requestDto, IConfiguration configuration, ILogger<ProcessManager> _logger);
    }
}