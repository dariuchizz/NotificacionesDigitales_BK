using System;

namespace Common.Model.Dto
{
    public class NotificacionesDigitalesDetalleDto
    {
        public long IdTipoComunicacion { get; set; }
        public long IdEnvio { get; set; }
        public DateTime FechaPago { get; set; }
        public long CantidadDias { get; set; }
        public long CantidadFacturas { get; set; }
        public decimal TotalRecaudado { get; set; }
        public decimal Saldo { get; set; }
        public decimal Totales { get; set; }
        public decimal PorcentajeCobrado { get; set; }
        public decimal PorcentajeRecaudado { get; set; }

    }
}
