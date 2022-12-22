using System;
using System.Collections.Generic;

namespace Common.Model.Dto
{
    public class NotificacionesDigitalesHaedDto
    {
        public long IdTipoComunicacion { get; set; }
        public long IdEnvio { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public long CantidadNotificaciones { get; set; }
        public long CantidadFacturas { get; set; }
        public decimal TotalNotificado { get; set; }
        public IEnumerable<NotificacionesDigitalesDetalleDto> Detalle { get; set; }
    }
}
