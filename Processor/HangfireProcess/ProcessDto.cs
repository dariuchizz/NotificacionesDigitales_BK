using System;

namespace Processor.HangfireProcess
{
    public class ProcessDto
    {
        public string Name { get; set; }

        public Func<IProcess> ProcessFactory { get; set; }
    }
}
