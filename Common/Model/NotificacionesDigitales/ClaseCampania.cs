using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Common.Model.NotificacionesDigitales
{
    [Table("ClasesCampanias")]
    public class ClaseCampania
    {
        public ClaseCampania()
        {
            this.Campanias = new HashSet<Campania>();
        }

        [Column("IdClaseCampania", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int IdClaseCampania { get; set; }

        [Column("Descripcion", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string descripcion { get; set; }

        public ICollection<Campania> Campanias { get; set; }

    }
}
