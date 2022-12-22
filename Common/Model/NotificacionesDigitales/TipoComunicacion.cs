using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("TipoComunicaciones")]
    public class TipoComunicacion
    {
        public TipoComunicacion()
        {
            Envios = new HashSet<Envio>();
        }
        [Key]
        [Column("IdTipoComunicacion", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdTipoComunicacion { get; set; }

        [Column("Aviso", TypeName = "bit")]
        [Required]
        public bool Aviso { get; set; }

        [Column("Descripcion", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Descripcion { get; set; }

        [Column("Template", TypeName = "varchar(50)")]
        [Required]
        public string Template { get; set; }

        [Column("Asunto", TypeName = "varchar(255)")]
        [MaxLength(255)]
        [StringLength(255)]
        public string Asunto { get; set; }

        [Column("Tag", TypeName = "varchar(30)")]
        [MaxLength]
        [Required]
        public string Tag { get; set; }

        [Column("StoreGenerarComunicaciones", TypeName = "varchar(50)")]
        [MaxLength]
        [Required]
        public string StoreGenerarComunicaciones { get; set; }

        [Column("StoreObtenerLote", TypeName = "varchar(50)")]
        [MaxLength]
        [Required]
        public string StoreObtenerLote { get; set; }

        [Column("TopeLectura", TypeName = "int")]
        public int TopeLectura { get; set; }

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

        public ICollection<Envio> Envios { get; set; }

    }
}
