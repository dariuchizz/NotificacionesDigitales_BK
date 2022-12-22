using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("TipoEnvio")]
    public class TipoEnvio
    {
        public TipoEnvio()
        {
            this.Envios = new HashSet<Envio>();
        }

        [Key]
        [Column("IdTipoEnvio", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdTipoEnvio { get; set; }
        [Column("Dominio", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Dominio { get; set; }

        public ICollection<Envio> Envios { get; set; }
    }
}
