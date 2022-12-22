using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("v_ObtenerSuministroCampanias")]
    public class v_ObtenerSuministroCampanias
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

        [Column("IdCanalCampania", TypeName = "bigint")]
        public long IdCanalCampania { get; set; }

        [Column("Canal", TypeName = "canal")]
        public string Canal { get; set; }

        [Column("Contacto", TypeName = "contacto")]
        public string Contacto { get; set; }

        
    }
}
