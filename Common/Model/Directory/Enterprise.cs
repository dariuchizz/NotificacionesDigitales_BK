using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.Directory
{
    [Table("EMPRESA")]
    public class Enterprise
    {
        [Column("id_empresa")]
        [Key]
        public int Id { get; set; }

        [Column("nombre_empresa")]
        public string Name { get; set; }

        [Column("nombre_abr_empresa")]
        public string Code { get; set; }

        public IEnumerable<BusinessUnit> BusinessUnits { get; set; }
    }
}
