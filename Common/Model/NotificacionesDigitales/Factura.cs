using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Facturas")]
    public class Factura
    {
        public Factura()
        {
            AvisosDeudasDetalles = new HashSet<AvisosDeudasDetalle>();
        }

        [Column("IdFactura", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdFactura { get; set; }

        [Column("IdSuministro", TypeName = "bigint")]
        [Required]
        public long IdSuministro { get; set; }

        [Column("NroFactura", TypeName = "varchar(16)")]
        [MaxLength(16)]
        [StringLength(16)]
        [Required]
        public string NroFactura { get; set; }

        [Column("Total", TypeName = "decimal")]
        [Required]
        public decimal Total { get; set; }

        [Column("Saldo", TypeName = "decimal")]
        [Required]
        public decimal Saldo { get; set; }

        [Column("Pagada", TypeName = "bit")]
        [Required]
        public bool Pagada { get; set; }

        [Column("Anulada", TypeName = "bit")]
        [Required]
        public bool Anulada { get; set; }

        [Column("FechaEmision", TypeName = "datetime")]
        [Required]
        public DateTime FechaEmision { get; set; }

        [Column("FechaVencimiento", TypeName = "datetime")]
        [Required]
        public DateTime FechaVencimiento { get; set; }

        [Column("FechaPago", TypeName = "datetime")]
        public DateTime? FechaPago { get; set; }

        [Column("Autor", TypeName = "int")]
        [Required]
        public int Autor { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        [Required]
        public DateTime FechaCreacion { get; set; }

        [Column("AutorModificacion", TypeName = "int")]
        public int? AutorModificacion { get; set; }

        [Column("FechaModificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }

        [Column("Activo", TypeName = "bit")]
        [Required]
        public bool Activo { get; set; }

        [ForeignKey("IdSuministro")]
        public Suministro Suministro { get; set; }
        public ICollection<AvisosDeudasDetalle> AvisosDeudasDetalles { get; set; }
    }


}
