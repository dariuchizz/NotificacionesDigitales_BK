using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("EventosResultantes")]
    public class EventosResultantes
    {
        public EventosResultantes()
        {
            this.EventosComunicaciones = new HashSet<EventosComunicaciones>();
        }

        [Column("IdEventoResultante", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEventoResultante { get; set; }

        [Column("Nombre", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Nombre { get; set; }

        [Column("Descripcion", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        public string Descripcion { get; set; }

        public ICollection<EventosComunicaciones> EventosComunicaciones { get; set; }

    }
}
