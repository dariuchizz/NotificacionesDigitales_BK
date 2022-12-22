using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("ConfiguracionesEmails")]
    public class ConfiguracionEmail
    {
        [Key]
        [Column("IdConfiguracionEmail", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "IdConfiguracionEmail is required")]
        public long IdConfiguracionEmail { get; set; }

        [Column("Reason", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string Reason { get; set; }

        [Column("Code", TypeName = "int")]
        [Required(ErrorMessage = "Code is required")]
        public int Code { get; set; }

        [Column("BounceCode", TypeName = "varchar(10)")]
        [MaxLength(10)]
        [StringLength(10)]
        public string BounceCode { get; set; }

        [Column("Severity", TypeName = "varchar(10)")]
        [MaxLength(10)]
        [StringLength(10)]
        public string Severity { get; set; }

        [Column("IdEventoResultanteEmail", TypeName = "bigint")]
        public long? IdEventoResultanteEmail { get; set; }

        [Column("Autor", TypeName = "int")]
        [Required(ErrorMessage = "Autor is required")]
        public int Autor { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        [Required(ErrorMessage = "Fecha Creacion is required")]
        public DateTime FechaCreacion { get; set; }

        [Column("AutorModificacion", TypeName = "int")]
        public int? AutorModificacion { get; set; }

        [Column("FechaModificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }

        [Column("Activo", TypeName = "bit")]
        [Required(ErrorMessage = "Activo is required")]
        public bool Activo { get; set; }
    }

}