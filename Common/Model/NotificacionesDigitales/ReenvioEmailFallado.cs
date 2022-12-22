using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("ReenviosEmailsFallados")]
    public class ReenvioEmailFallado
    {
        [Key]
        [Column("IdReenvioEmailFallado", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdReenvioEmailFallado { get; set; }

        [Column("IdComunicacion", TypeName = "bigint")]
        public long? IdComunicacion { get; set; }

        [Column("IdEventoEmail", TypeName = "bigint")]
        public long? IdEventoEmail { get; set; }

        [Column("IdConfiguracionEmail", TypeName = "bigint")]
        public long? IdConfiguracionEmail { get; set; }

        [Column("Procesado", TypeName = "bit")]
        public bool? Procesado { get; set; }

        [Column("FechaProcesado", TypeName = "datetime")]
        public DateTime FechaProcesado { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        public DateTime FechaCreacion { get; set; }

    }
}
