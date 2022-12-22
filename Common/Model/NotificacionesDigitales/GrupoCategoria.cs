using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Common.Model.NotificacionesDigitales
{
    [Table("GrupoCategorias")]
    public class GrupoCategoria
    {
        [Key]
        [Column("IdGrupoCategoria", TypeName = "bigint", Order = 1)]
        public long IdGrupoCategoria { get; set; }
        [Key]
        [Column("Categoria", TypeName = "varchar(4)", Order = 2)]
        [MaxLength(4)]
        [StringLength(4)]
        public string Categoria { get; set; }

        public ClienteGrupoCategoria ClienteGrupoCategoria { get; set; }
    }

}
