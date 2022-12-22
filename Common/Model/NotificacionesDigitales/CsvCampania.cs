using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("CsvCampanias")]
    public class CsvCampania
    {
        [Key]
        [Column("IdCsvCampania", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdCsvCampania { get; set; }
        [Column("IdCampania", TypeName = "bigint")]
        [Required]
        public long IdCampania { get; set; }
        [Column("IdCanal", TypeName = "bigint")]
        [Required]
        public long IdCanal { get; set; }
        [Column("Dato", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        [Required]
        public string Dato { get; set; }
        [Column("Nombre", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Column("Apellido", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string Apellido { get; set; }
        [Column("NombreApellido", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        public string NombreApellido { get; set; }
        [Column("Domicilio", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        public string Domicilio { get; set; }
        [Column("Secuencia", TypeName = "bigint")]
        [Required]
        public long Secuencia { get; set; }
        public Campania Campania { get; set; }
        public Canal Canal { get; set; }
    }

}
