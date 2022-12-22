using System;
using System.Threading;
using System.Threading.Tasks;

namespace Processor.HangfireProcess
{
    public interface IProcess : IDisposable
    {
        Task ExecuteAsync(string parameters, CancellationToken cancellationToken);
    }
}
