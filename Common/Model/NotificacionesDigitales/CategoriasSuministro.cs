using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("CategoriasSuministro")]
    public class CategoriasSuministro
    {
        [Key]
        [Column("IdCategoriaSuministro", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdCategoriaSuministro { get; set; }
        [Column("Codigo", TypeName = "varchar(4)")]
        [MaxLength(4)]
        [StringLength(4)]
        public string Codigo { get; set; }
        [Column("Descripcion", TypeName = "varchar(25)")]
        [MaxLength(25)]
        [StringLength(25)]
        public string Descripcion { get; set; }
    }

}
