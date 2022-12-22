namespace Common.Model.Dto
{
    public class NotificacionDto: ComunicacionDto
    {
        public string Numero { get; set; }
        public string Email { get; set; }
        public string EstadoMail { get; set; }
        public string EstadoSms { get; set; }
    }
}
