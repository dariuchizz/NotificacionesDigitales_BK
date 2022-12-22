using System;

namespace Common.Model.NotificacionesDigitales
{
    public class ProcesoEventoDto
    {
        public long IdProcesoEvento { get; set; }

        public int Tipo { get; set; }

        public bool? Aviso { get; set; }

        public DateTime Fecha { get; set; }

        public int Hora { get; set; }

        public int Estado { get; set; }

        public int Cantidad { get; set; }

        public DateTime FechaUltimaModificacion { get; set; }
    }
}