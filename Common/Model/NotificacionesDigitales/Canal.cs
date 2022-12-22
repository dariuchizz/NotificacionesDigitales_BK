using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Canales")]
    public class Canal
    {
        public Canal()
        {
            this.Comunicaciones = new List<Comunicacion>();
            this.CsvCampanias = new HashSet<CsvCampania>();
        }

        [Column("IdCanal", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdCanal { get; set; }

        [Column("Descripcion", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Descripcion { get; set; }

        [Column("Autor", TypeName = "int")]
        [Required]
        public int Autor { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        [Required]
        public DateTime FechaCreacion { get; set; }

        [Column("AutorModificacion", TypeName = "int")]
        public int? AutorModificacion { get; set; }

        [Column("FechaModificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }

        [Column("Activo", TypeName = "bit")]
        [Required]
        public bool Activo { get; set; }

        public IEnumerable<Comunicacion> Comunicaciones { get; set; }
        public ICollection<CsvCampania> CsvCampanias { get; set; }
    }

}