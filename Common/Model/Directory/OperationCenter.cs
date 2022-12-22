using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.Directory
{
    [Table("CENTROOPERATIVO")]
    public class OperationCenter
    {
        [Column("id_centrooperativo")]
        [Key]
        public int Id { get; set; }

        [Column("nombre_centrooperativo")]
        public string Name { get; set; }

        [Column("cod_centrooperativo_ag")]
        public string Code { get; set; }

        [Column("un_centrooperativo")]
        public int BusinessUnitId { get; set; }

        [ForeignKey("BusinessUnitId")]
        public BusinessUnit BusinessUnit { get; set; }

        public IEnumerable<ManagmentCenter> ManagmentCenters { get; set; }
    }
}
