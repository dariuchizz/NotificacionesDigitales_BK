using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("EnvioSMSGenericos")]
    public class EnvioSmsGenerico
    {
        [Column("IdEnvioSMSGenerico", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required()]
        public long IdEnvioSMSGenerico { get; set; }

        [Column("NroCelular", TypeName = "varchar(15)")]
        [MaxLength(15)]
        [StringLength(15)]
        public string NroCelular { get; set; }

        [Column("TextoMensaje", TypeName = "varchar(160)")]
        [MaxLength(160)]
        public string TextoMensaje { get; set; }

        [Column("IdSistema", TypeName = "bigint")]
        public long IdSistema { get; set; }

        [Column("IdProcesoNegocio", TypeName = "bigint")]
        public long IdProcesoNegocio { get; set; }

        [Column("Datos", TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string Datos { get; set; }

        [Column("FechaProceso", TypeName = "date")]
        public DateTime? FechaProceso { get; set; }

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

        [ForeignKey("IdProcesoNegocio")]
        public ProcesoNegocio ProcesosNegocio { get; set; }
    }
}
