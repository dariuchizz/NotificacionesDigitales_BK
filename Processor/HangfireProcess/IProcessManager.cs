using System.Collections.Generic;

namespace Processor.HangfireProcess
{
    public interface IProcessManager
    {
        IEnumerable<ProcessDto> GetAvaiableProcesses();
        bool TryGetProcess(string name, out ProcessDto process);
    }
}
