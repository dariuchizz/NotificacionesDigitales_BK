using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("ProcesosEventos")]
    public class ProcesoEvento
    {
        [Column("IdProcesoEvento", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id Proceso Evento is required")]
        public long IdProcesoEvento { get; set; }

        [Column("Tipo", TypeName = "int")]
        [Required(ErrorMessage = "Tipo is required")]
        public int Tipo { get; set; }

        [Column("Aviso", TypeName = "bit")]
        public bool? Aviso { get; set; }

        [Column("Fecha", TypeName = "date")]
        [Required(ErrorMessage = "Fecha is required")]
        public DateTime Fecha { get; set; }

        [Column("Hora", TypeName = "int")]
        [Required(ErrorMessage = "Hora is required")]
        public int Hora { get; set; }

        [Column("Estado", TypeName = "int")]
        [Required(ErrorMessage = "Estado is required")]
        public int Estado { get; set; }

        [Column("Cantidad", TypeName = "int")]
        public int Cantidad { get; set; }

        [Column("FechaUltimaModificacion", TypeName = "datetime")]
        [Required(ErrorMessage = "Fecha Ultima Modificacion is required")]
        public DateTime FechaUltimaModificacion { get; set; }
    }
}