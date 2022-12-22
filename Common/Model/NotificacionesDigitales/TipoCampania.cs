using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("TipoCampanias")]
    public class TipoCampania
    {
        public TipoCampania()
        {
            this.Campanias = new HashSet<Campania>();
        }

        [Key]
        [Column("IdTipoCampania", TypeName = "bigint")]
        [Required]
        public long IdTipoCampania { get; set; }
        [Column("TipoArmado", TypeName = "bigint")]
        public long? TipoArmado { get; set; }
        [Column("StoreGenerar", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        public string StoreGenerar { get; set; }
        [Column("StoreObtener", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        public string StoreObtener { get; set; }
        [Column("StoreConsultar", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        public string StoreConsultar { get; set; }

        public ICollection<Campania> Campanias { get; set; }
    }

}
