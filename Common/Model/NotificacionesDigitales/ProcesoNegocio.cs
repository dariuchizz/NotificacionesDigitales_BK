using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("ProcesosNegocios")]
    public class ProcesoNegocio
    {
        public ProcesoNegocio()
        {
            this.EnvioSmsGenericos = new List<EnvioSmsGenerico>();
        }

        [Column("IdProcesoNegocio", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required()]
        public long IdProcesoNegocio { get; set; }

        [Column("IdSistema", TypeName = "bigint")]
        public long? IdSistema { get; set; }

        [Column("DescripcionProceso", TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string DescripcionProceso { get; set; }

        [Column("TopeDiario", TypeName = "int")]
        public int? TopeDiario { get; set; }

        [Column("Autor", TypeName = "int")]
        public int Autor { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        public DateTime FechaCreacion { get; set; }

        [Column("AutorModificacion", TypeName = "int")]
        public int? AutorModificacion { get; set; }

        [Column("FechaModificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }

        [Column("Activo", TypeName = "bit")]
        public bool Activo { get; set; }

        [ForeignKey("IdSistema")]
        public Sistema Sistemaa { get; set; }

        public IEnumerable<EnvioSmsGenerico> EnvioSmsGenericos { get; set; }
    }
}
