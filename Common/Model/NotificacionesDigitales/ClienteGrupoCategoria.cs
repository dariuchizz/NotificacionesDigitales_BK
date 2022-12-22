using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Common.Model.NotificacionesDigitales
{
    [Table("ClienteGrupoCategorias")]
    public class ClienteGrupoCategoria
    {
        public ClienteGrupoCategoria()
        {
            this.GrupoCategorias = new HashSet<GrupoCategoria>();
        }

        [Key]
        [Column("IdClienteGrupo", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdClienteGrupo { get; set; }
        [Column("GranCliente", TypeName = "bit")]
        public bool GranCliente { get; set; }
        [Column("Grupo", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string Grupo { get; set; }

        public ICollection<GrupoCategoria> GrupoCategorias { get; set; }
    }

}
