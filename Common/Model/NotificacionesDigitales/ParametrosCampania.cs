using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("ParametrosCampanias")]
    public class ParametrosCampania
    {
        [Key]
        [Column("IdParametro", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdParametro { get; set; }
        [Column("IdCampania", TypeName = "bigint")]
        [Required]
        public long IdCampania { get; set; }
        [Column("Categorias", TypeName = "varchar(max)")]
        [MaxLength]
        public string Categorias { get; set; }
        [Column("Estados", TypeName = "varchar(max)")]
        [MaxLength]
        public string Estados { get; set; }
        [Column("GranCliente", TypeName = "bit")]
        //[Required]
        public bool? GranCliente { get; set; }
        [Column("EnteOficial", TypeName = "bit")]
        //[Required]
        public bool? EnteOficial { get; set; }
        [Column("TieneDebitoAutomatico", TypeName = "bit")]
        //[Required]
        public bool? TieneDebitoAutomatico { get; set; }
        [Column("IdMotivoBaja", TypeName = "int")]
        public int? IdMotivoBaja { get; set; }
        [Column("TieneNotificacionDigital", TypeName = "bit")]
        //[Required]
        public bool? TieneNotificacionDigital { get; set; }
        [Column("Localidades", TypeName = "varchar(max)")]
        [MaxLength]
        public string Localidades { get; set; }

        public Campania Campania { get; set; }
    }


}
