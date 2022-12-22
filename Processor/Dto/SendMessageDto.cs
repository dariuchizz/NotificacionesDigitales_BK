using Common.Model.Enum;

namespace Processor.Dto
{
    public class SendMessageDto
    {
        public string Asunto { get; set; }
        public string Tag { get; set; }
        public string Template { get; set; }
        public TipoEnvioType TipoEnvio { get; set; }
    }
}
