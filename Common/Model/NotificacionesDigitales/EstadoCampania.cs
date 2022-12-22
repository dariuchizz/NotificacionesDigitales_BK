using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("EstadoCampanias")]
    public class EstadoCampania
    {
        public EstadoCampania()
        {
            this.Campanias = new HashSet<Campania>();
        }

        [Key]
        [Column("IdEstadoCampania", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEstadoCampania { get; set; }
        [Column("Descripcion", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        [Required]
        public string Descripcion { get; set; }

        public ICollection<Campania> Campanias { get; set; }

    }
}
