using System;

namespace Processor.ProcessModule
{
    public class ComunicationException : Exception
    {
        public ComunicationException(String message) : base(message) { }
    }
}
