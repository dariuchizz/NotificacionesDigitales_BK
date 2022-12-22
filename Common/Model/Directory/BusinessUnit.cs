using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.Directory
{
    [Table("UNEGOCIO")]
    public class BusinessUnit
    {
        [Column("id_un")]
        [Key]
        public int Id { get; set; }

        [Column("nombre_un")]
        public string Name { get; set; }
        [Column("nombre_abr_un", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string NameAbbr { get; set; }

        [Column("codigo_un_ag")]
        public string Code { get; set; }

        [Column("empresa_un")]
        public int EnterpriseId { get; set; }

        [ForeignKey("EnterpriseId")]
        public Enterprise Enterprise { get; set; }

        public IEnumerable<OperationCenter> OperationCenters { get; set; }
    }
}
