using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Emails")]
    public class Email
    {
        public Email()
        {
            this.SegmentosEmails = new List<SegmentoEmail>();
        }

        [Column("IdEmail", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEmail { get; set; }

        [Column("IdSuministro", TypeName = "bigint")]
        public long? IdSuministro { get; set; }

        [Column("IdExterno", TypeName = "bigint")]
        public long? IdExterno { get; set; }

        [Column("Email", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string DEmail { get; set; }

        [Column("TieneNotificacionDigital", TypeName = "bit")]
        public bool? TieneNotificacionDigital { get; set; }

        [Column("IdMotivoBaja", TypeName = "bigint")]
        public long? IdMotivoBaja { get; set; }

        [Column("EsTitular", TypeName = "bit")]
        public bool? EsTitular { get; set; }

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

        [ForeignKey("IdMotivoBaja")]
        public MotivoBaja MotivosBaja { get; set; }

        public IEnumerable<SegmentoEmail> SegmentosEmails { get; set; }
    }

}
