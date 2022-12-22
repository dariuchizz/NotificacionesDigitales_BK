using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Celulares")]
    public class Celular
    {
        [Column("IdCelular", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdCelular { get; set; }
        [Column("IdSuministro", TypeName = "bigint")]
        [Required]
        public long IdSuministro { get; set; }
        [Column("IdExterno", TypeName = "bigint")]
        [Required]
        public long IdExterno { get; set; }
        [Column("Numero", TypeName = "varchar(15)")]
        [MaxLength(15)]
        [StringLength(15)]
        public string Numero { get; set; }
        [Column("IdMotivoBaja", TypeName = "bigint")]
        [Required]
        public long IdMotivoBaja { get; set; }
        [Column("EsTitular", TypeName = "bit")]
        [Required]
        public bool EsTitular { get; set; }
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
