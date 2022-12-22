using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Envios")]
    public class Envio
    {
        public Envio()
        {
            //Comunicaciones = new HashSet<Comunicacion>();
        }
        [Key]
        [Column("IdEnvio", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEnvio { get; set; }
        [Column("FechaProceso", TypeName = "datetime")]
        [Required]
        public DateTime FechaProceso { get; set; }
        [Column("CantidadComunicaciones", TypeName = "bigint")]
        [Required]
        public long CantidadComunicaciones { get; set; }
        [Column("IdTipoEnvio", TypeName = "bigint")]
        [Required]
        public long IdTipoEnvio { get; set; }

        [Column("IdRelacionado", TypeName = "bigint")]
        [Required]
        public long IdRelacionado { get; set; }
        public TipoEnvio TipoEnvio { get; set; }

        public Campania Campania { get; set; }

        public TipoComunicacion TipoComunicacion { get; set; }
        //public ICollection<Comunicacion> Comunicaciones { get; set; }
    }
}
