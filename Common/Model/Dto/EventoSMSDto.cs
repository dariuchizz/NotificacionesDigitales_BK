using System;

namespace Common.Model.Dto
{
    public class EventoSMSDto
    {
        public long IdEventoSMS { get; set; }

        public long? IdComunicacion { get; set; }

        public string IdEvento { get; set; }

        public DateTime? Fecha { get; set; }

        public string DEvento { get; set; }

        public string IdExterno { get; set; }

        public string Message { get; set; }

        public string Telefonica { get; set; }
    }

}
