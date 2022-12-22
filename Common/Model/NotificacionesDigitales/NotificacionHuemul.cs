using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("NotificacionesHuemul")]
    public class NotificacionHuemul
    {
        [Column("IdNotificacionHuemul", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdNotificacionHuemul { get; set; }

        [Column("CuentaUnificada", TypeName = "bigint")]
        [Required]
        public long CuentaUnificada { get; set; }

        [Column("TipoComprobante", TypeName = "varchar(16)")]
        [Required]
        public string TipoComprobante { get; set; }

        [Column("NroComprobante", TypeName = "varchar(16)")]
        [MaxLength]
        [Required]
        public string NroComprobante { get; set; }

        [Column("Canal", TypeName = "varchar(10)")]
        [Required]
        public long IdCanal { get; set; }

        [Column("LinkHuemul", TypeName = "varchar(5000)")]
        [MaxLength]
        [Required]
        public string LinkHuemul { get; set; }

        [Column("ActualizadosDesdeAG", TypeName = "bit")]
        [Required]
        public bool ActualizadosDesdeAG { get; set; }

        [Column("FechaProcesado", TypeName = "date")]
        public DateTime? FechaProcesado { get; set; }

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
    }
}