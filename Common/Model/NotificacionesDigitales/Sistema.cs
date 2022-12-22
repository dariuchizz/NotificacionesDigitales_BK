using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Sistemas")]
    public class Sistema
    {
        public Sistema()
        {
            this.EnvioSmsGenericos = new List<EnvioSmsGenerico>();
            this.ProcesosNegocios = new List<ProcesoNegocio>();
        }

        [Column("IdSistema", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdSistema { get; set; }

        [Column("Descripcion", TypeName = "varchar(50)")]
        [MaxLength]
        public string Descripcion { get; set; }

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

        public IEnumerable<EnvioSmsGenerico> EnvioSmsGenericos { get; set; }

        public IEnumerable<ProcesoNegocio> ProcesosNegocios { get; set; }
    }
}
