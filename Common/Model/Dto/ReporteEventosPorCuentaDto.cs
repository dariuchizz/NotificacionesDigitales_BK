using System;
using System.Collections.Generic;

namespace Common.Model.Dto
{
    public class ReporteEventosPorCuentaDto
    {
        public long IdComunicacion { get; set; }
        public long IdEnvio { get; set; }
        public long IdTipoComunicacion { get; set; }
        public string TipoComunicacion { get; set; }
        public string Descripcion { get; set; }
        public long IdCanal { get; set; }
        public string Canal { get; set; }
        public long IdContacto { get; set; }
        public long IdRelacionado { get; set; }
        public string Relacionado { get; set; }
        public string Contacto { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public IEnumerable<EventoComunicacionDto> Eventos { get; set; }

    }
}
