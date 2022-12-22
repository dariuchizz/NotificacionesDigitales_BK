using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("EstadosSuministro")]
    public class EstadosSuministro
    {
        [Column("IdEstadoSuministro", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEstadoSuministro { get; set; }
        [Column("Codigo", TypeName = "char(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        [Required]
        public string Codigo { get; set; }
        [Column("Descripcion", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Descripcion { get; set; }
        [Column("ConGas", TypeName = "bit")]
        [Required]
        public bool ConGas { get; set; }
        [Column("Grupo", TypeName = "varchar(25)")]
        [MaxLength(25)]
        [StringLength(25)]
        [Required]
        public string Grupo { get; set; }
        [Column("Orden", TypeName = "smallint")]
        [Required]
        public short Orden { get; set; }
    }

}
