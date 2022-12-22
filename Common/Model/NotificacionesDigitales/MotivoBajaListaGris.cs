using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("MotivosBajasListasGrises")]
    public class MotivoBajaListaGris
    {
        [Column("IdMotivoBajaListaGris", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id Motivo Baja Lista Gris is required")]
        public long IdMotivoBajaListaGris { get; set; }

        [Column("Descripcion", TypeName = "varchar(50)")]
        [MaxLength]
        public string Descripcion { get; set; }

        [Column("RequiereObservacion", TypeName = "bit")]
        [Required(ErrorMessage = "Requiere Observacion is required")]
        public bool RequiereObservacion { get; set; }

        [Column("Autor", TypeName = "int")]
        [Required(ErrorMessage = "Autor is required")]
        public int Autor { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        [Required(ErrorMessage = "Fecha Creacion is required")]
        public DateTime FechaCreacion { get; set; }

        [Column("AutorModificacion", TypeName = "int")]
        public int? AutorModificacion { get; set; }

        [Column("FechaModificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }

        [Column("Activo", TypeName = "bit")]
        [Required(ErrorMessage = "Activo is required")]
        public bool Activo { get; set; }
    }
}
