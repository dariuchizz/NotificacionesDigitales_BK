using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("ListasGrises")]
    public class ListaGris
    {
        [Column("IdListaGris", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdListaGris { get; set; }

        [Column("IdComunicacion", TypeName = "bigint")]
        [Required(ErrorMessage = "IdComunicacion is required")]
        public long IdComunicacion { get; set; }

        [Column("IdMotivo", TypeName = "bigint")]
        [Required(ErrorMessage = "IdMotivo is required")]
        public long IdMotivo { get; set; }

        [Column("ObservacionCliente", TypeName = "varchar(50)")]
        public string ObservacionCliente { get; set; }

        [Column("Origen", TypeName = "varchar(10)")]
        [Required(ErrorMessage = "Origen is required")]
        public string Origen { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        [Required(ErrorMessage = "Fecha Creacion is required")]
        public DateTime FechaCreacion { get; set; }

        [Column("FechaModificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }

        [Column("Activo", TypeName = "bit")]
        [Required(ErrorMessage = "Activo is required")]
        public bool Activo { get; set; }

        public Comunicacion Comunicacion { get; set; }
    }
}
