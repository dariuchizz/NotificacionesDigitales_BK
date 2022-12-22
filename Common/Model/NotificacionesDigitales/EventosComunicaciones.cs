using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("EventosComunicaciones")]
    public class EventosComunicaciones
    {
        [Column("IdEventoComunicacion", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEventoComunicacion { get; set; }
        
        [Column("IdComunicacion", TypeName = "bigint")]
        [Required]
        public long IdComunicacion { get; set; }
        
        [Column("IdEventoResultante", TypeName = "bigint")]
        [Required]
        public long IdEventoResultante { get; set; }

        [Column("FechaEvento", TypeName = "datetime")]
        public DateTime? FechaEvento { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        [Required]
        public DateTime FechaCreacion { get; set; }

        public Comunicacion Comunicacion { get; set; }

        public EventosResultantes EventosResultante { get; set; }
    }

}
