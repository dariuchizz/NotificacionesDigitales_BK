using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Segmentos")]
    public class Segmento
    {
        public Segmento()
        {
            this.SegmentosEmails = new List<SegmentoEmail>();
        }

        [Column("IdSegmento", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdSegmento { get; set; }

        [Column("Descripcion", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Descripcion { get; set; }

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

        public IEnumerable<SegmentoEmail> SegmentosEmails { get; set; }
    }

}