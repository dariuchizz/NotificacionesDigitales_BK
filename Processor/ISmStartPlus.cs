using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Processor.Dto;
using Processor.HangfireProcess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor
{
    public interface ISmStartPlus
    {
        Task<List<SMStartContentLog>> ReadEvents(string dSMStart, int hora, IConfiguration configuration, ILogger<ProcessManager> _logger);
        Task<SmsResponse> SendAsync(IConfiguration configuration, SmsRequest requestDto, ILogger<ProcessManager> _logger);
    }
}