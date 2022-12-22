using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("EventosCelulares")]
    public class EventoSMS
    {
        [Column("IdEventoSMS", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEventoSMS { get; set; }

        [Column("IdComunicacion", TypeName = "bigint")]
        public long? IdComunicacion { get; set; }

        [Column("IdEvento", TypeName = "varchar(40)")]
        public string IdEvento { get; set; }

        [Column("Fecha", TypeName = "datetime")]
        public DateTime? Fecha { get; set; }

        [Column("Evento", TypeName = "varchar(10)")]
        [MaxLength(10)]
        [StringLength(10)]
        public string DEvento { get; set; }

        [Column("IdExterno", TypeName = "varchar(100)")]
        [MaxLength]
        public string IdExterno { get; set; }

        [Column("Message", TypeName = "varchar(2000)")]
        [MaxLength]
        public string Message { get; set; }

        [Column("Razon", TypeName = "varchar(50)")]
        public string Razon { get; set; }

        [Column("Codigo", TypeName = "int")]
        public int? Codigo { get; set; }

        [Column("Subcodigo", TypeName = "varchar(10)")]
        public string Subcodigo { get; set; }

        [ForeignKey("IdComunicacion")]
        public Comunicacion Comunicacion { get; set; }
    }

}
