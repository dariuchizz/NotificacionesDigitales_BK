using System;

namespace Common.Model.Dto
{
    public class EventoEmailDto
    {
        public long IdEventoEmail { get; set; }

        public long IdComunicacion { get; set; }

        public string IdEvento { get; set; }

        public DateTime? Fecha { get; set; }

        public string DEvento { get; set; }

        public string IdExterno { get; set; }

        public string Message { get; set; }

        public string Reason { get; set; }

        public int? Code{ get; set; }

        public string BounceCode { get; set; }

        public string Severity { get; set; }

        public string MessageError { get; set; }
    }

}
