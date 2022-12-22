using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("RptNotificaciones")]

    public class RptNotificaciones
    {
        public RptNotificaciones()
        {
            this.RptNotificacionesDetalles = new HashSet<RptNotificacionesDetalle>();
        }

        [Column("IdTipoComunicacion", TypeName = "bigint", Order = 1)]
        [Required]
        public long IdTipoComunicacion { get; set; }
        [Column("IdEnvio", TypeName = "bigint", Order = 2)]
        [Required]
        public long IdEnvio { get; set; }
        [Column("FechaEnvio", TypeName = "datetime")]
        [Required]
        public DateTime FechaEnvio { get; set; }
        [Column("FechaVencimiento", TypeName = "datetime")]
        [Required]
        public DateTime FechaVencimiento { get; set; }
        [Column("CantidadNotificaciones", TypeName = "bigint")]
        [Required]
        public long CantidadNotificaciones { get; set; }
        [Column("CantidadFacturas", TypeName = "bigint")]
        [Required]
        public long CantidadFacturas { get; set; }
        [Column("TotalNotificado", TypeName = "decimal")]
        [Required]
        public decimal TotalNotificado { get; set; }

        public ICollection<RptNotificacionesDetalle> RptNotificacionesDetalles { get; set; }
    }
}
