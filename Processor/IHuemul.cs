using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Processor.HangfireProcess;

namespace Processor
{
    public interface IHuemul
    {
        Task<QueueClient> GenerateQueueClientAsync(IConfiguration configuration);

        Task<string> GetLinkHuemulAsync(string cuenta, string tipoComprobante, string comprobante, IConfiguration configuration, ILogger<ProcessManager> _logger, IMemoryCache memoryCache);
    }
}