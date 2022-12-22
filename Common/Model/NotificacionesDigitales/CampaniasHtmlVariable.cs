using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("CampaniasHtmlVariables")]
    public class CampaniasHtmlVariable
    {
        [Key]
        [Column("IdCampaniaHtmlVariable", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdCampaniaHtmlVariable { get; set; }
        [Column("IdCampania", TypeName = "bigint")]
        public long IdCampania { get; set; }
        [Column("Nombre", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Column("Apellido", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string Apellido { get; set; }
        [Column("NombreApellido", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string NombreApellido { get; set; }
        [Column("Domicilio", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string Domicilio { get; set; }

        public Campania Campania { get; set; }

        [Column("Html", TypeName = "varchar(MAX)")]
        [MaxLength]
        public string Html { get; set; }
    }

}
