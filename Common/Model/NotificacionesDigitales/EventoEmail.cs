using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("EventosEmails")]
    public class EventoEmail
    {
        [Column("IdEventoEmail", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdEventoEmail { get; set; }

        [Column("IdComunicacion", TypeName = "bigint")]
        public long? IdComunicacion { get; set; }

        [Column("IdEvento", TypeName = "varchar(100)")]
        public string IdEvento { get; set; }

        [Column("Fecha", TypeName = "datetime")]
        public DateTime? Fecha { get; set; }

        [Column("Evento", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string DEvento { get; set; }

        [Column("IdExterno", TypeName = "varchar(100)")]
        [MaxLength]
        public string IdExterno { get; set; }

        [Column("Message", TypeName = "varchar(2000)")]
        [MaxLength]
        public string Message { get; set; }

        [Column("Reason", TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string Reason { get; set; }
        
        [Column("Code", TypeName = "int")]
        public int? Code { get; set; }

        [Column("BounceCode", TypeName = "varchar(10)")]
        [MaxLength(10)]
        public string BounceCode { get; set; }

        [Column("Severity", TypeName = "varchar(10)")]
        [MaxLength(10)]
        public string Severity { get; set; }

        [Column("MessageError", TypeName = "varchar(2000)")]
        [MaxLength(2000)]
        public string MessageError { get; set; }

        [ForeignKey("IdComunicacion")]
        public Comunicacion Comunicacion { get; set; }
    }

}
