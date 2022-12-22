using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("AvisosDeudasDetalles")]
    public class AvisosDeudasDetalle
    {
        [Column("IdAvisoDeuda", TypeName = "bigint")]
        [Required]
        public long IdAvisoDeuda { get; set; }
        [Column("IdFactura", TypeName = "bigint")]
        [Required]
        public long IdFactura { get; set; }

        public AvisosDeuda AvisosDeuda { get; set; }
        public Factura Factura { get; set; }

    }
}
