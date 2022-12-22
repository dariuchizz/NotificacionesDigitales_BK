namespace Processor.Dto
{
    public class SmsRequest
    {
        public string Numero { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public long IdComunicacion { get; set; }

    }
}
