using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("v_ObtenerSuministroProcesosNegocios")]
    public class v_ObtenerSuministroProcesosNegocios
    {
        [Column("IdComunicacion", TypeName = "bigint")]
        [Required]
        public long IdComunicacion { get; set; }

        [Column("IdSuministro", TypeName = "bigint")]
        [Required]
        public long IdSuministro { get; set; }

        [Column("Descripcion", TypeName = "varchar(50)")]
        public string Descripcion { get; set; }

        [Column("FechaEnvio", TypeName = "datetime")]
        public DateTime? FechaEnvio { get; set; }

        [Column("Canal", TypeName = "varchar(50)")]
        public string Canal { get; set; }

        [Column("Contacto", TypeName = "varchar(50)")]
        public string Contacto { get; set; }

        [Column("Dato", TypeName = "varchar(50)")]
        public string Dato { get; set; }

        [Column("IdCanal", TypeName = "bigint")]
        public long IdCanal { get; set; }
    }
}
