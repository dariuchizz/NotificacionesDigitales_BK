using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("SegmentosEmails")]
    public class SegmentoEmail
    {
        [Column("IdSegmento", TypeName = "bigint", Order = 1)]
        [Required]
        public long IdSegmento { get; set; }

        [Column("IdEmail", TypeName = "bigint", Order = 2)]
        [Required]
        public long IdEmail { get; set; }

        [ForeignKey("IdSegmento")]
        public Segmento Segmento { get; set; }

        [ForeignKey("IdEmail")]
        public Email Email { get; set; }
    }

}
