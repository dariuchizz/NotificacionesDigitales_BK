using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Model.Dto;
using Processor.Dto;

namespace Processor.Services
{
    public interface IEnvioServices
    {
        Task SendMessageAsync(int index, IList<DynamicDto> items, SendMessageDto tipo,
            CancellationToken cancellationToken);
    }
}
