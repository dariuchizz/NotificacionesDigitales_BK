using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("AvisosDeudas")]

    public class AvisosDeuda
    {
        public AvisosDeuda()
        {
            this.AvisosDeudasDetalles = new HashSet<AvisosDeudasDetalle>();
        }

        [Column("IdAvisoDeuda", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdAvisoDeuda { get; set; }
        [Column("IdSuministro", TypeName = "bigint")]
        [Required]
        public long IdSuministro { get; set; }
        [Column("Emisor", TypeName = "int")]
        [Required]
        public int Emisor { get; set; }
        [Column("Numero", TypeName = "varchar(16)")]
        [MaxLength(16)]
        [StringLength(16)]
        public string Numero { get; set; }
        [Column("Empresa", TypeName = "smallint")]
        [Required]
        public short Empresa { get; set; }
        [Column("Total", TypeName = "decimal")]
        [Required]
        public decimal Total { get; set; }
        [Column("Saldo", TypeName = "decimal")]
        [Required]
        public decimal Saldo { get; set; }
        [Column("FechaEmision", TypeName = "date")]
        [Required]
        public DateTime FechaEmision { get; set; }
        [Column("FechaPago", TypeName = "date")]
        public DateTime? FechaPago { get; set; }
        [Column("FechaVencimiento", TypeName = "date")]
        public DateTime? FechaVencimiento { get; set; }
        [Column("Estado", TypeName = "varchar(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        public string Estado { get; set; }
        [Column("Pagado", TypeName = "bit")]
        public bool? Pagado { get; set; }
        [Column("Anulado", TypeName = "bit")]
        public bool? Anulado { get; set; }
        [Column("FechaCreacion", TypeName = "datetime")]
        [Required]
        public DateTime FechaCreacion { get; set; }
        [Column("FechaModificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }

        public ICollection<AvisosDeudasDetalle> AvisosDeudasDetalles { get; set; }

    }
}
