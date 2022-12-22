using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{

    [Table("CambiosContactos")]
    public class CambioContacto
    {
        [Column("IdCambioContacto", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdCambioContacto { get; set; }

        [Column("IdContacto", TypeName = "bigint")]
        public long? IdContacto { get; set; }

        [Column("IdConfiguracionEmail", TypeName = "bigint")]
        public long? IdConfiguracionEmail { get; set; }

        [Column("IdCanal", TypeName = "bigint")]
        [Required]
        public long IdCanal { get; set; }

        [Column("Spam", TypeName = "bit")]
        [Required] 
        public bool Spam { get; set; }

        [Column("Procesado", TypeName = "char(1)")]
        [MaxLength(1)]
        [StringLength(1)]
        [Required] 
        public string Procesado { get; set; }

        [Column("FechaProcesado", TypeName = "datetime")]
        public DateTime? FechaProcesado { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        [Required] 
        public DateTime FechaCreacion { get; set; }

    }
}
