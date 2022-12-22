using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("RptNotificacionesDetalle")]
    public class RptNotificacionesDetalle
    {
        [Column("IdTipoComunicacion", TypeName = "bigint", Order = 1)]
        [Required]
        public long IdTipoComunicacion { get; set; }
        [Column("IdEnvio", TypeName = "bigint", Order = 2)]
        [Required]
        public long IdEnvio { get; set; }
        [Column("FechaPago", TypeName = "datetime")]
        [Required]
        public DateTime FechaPago { get; set; }
        [Column("CantidadDias", TypeName = "bigint", Order = 3)]
        [Required]
        public long CantidadDias { get; set; }
        [Column("CantidadFacturas", TypeName = "bigint")]
        [Required]
        public long CantidadFacturas { get; set; }
        [Column("TotalRecaudado", TypeName = "decimal")]
        [Required]
        public decimal TotalRecaudado { get; set; }
        [Column("Saldo", TypeName = "decimal")]
        [Required]
        public decimal Saldo { get; set; }
        [Column("Totales", TypeName = "decimal")]
        [Required]
        public decimal Totales { get; set; }
        [Column("PorcentajeCobrado", TypeName = "decimal")]
        [Required]
        public decimal PorcentajeCobrado { get; set; }
        [Column("PorcentajeRecaudado", TypeName = "decimal")]
        [Required]
        public decimal PorcentajeRecaudado { get; set; }
        [Column("FechaEnvio", TypeName = "datetime")]
        [Required]
        public DateTime FechaEnvio { get; set; }
        [Column("FechaVencimiento", TypeName = "datetime")]
        [Required]
        public DateTime FechaVencimiento { get; set; }

        public RptNotificaciones RptNotificaciones { get; set; }
    }
}
