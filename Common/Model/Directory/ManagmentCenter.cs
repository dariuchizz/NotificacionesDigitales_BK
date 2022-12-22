using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.Directory
{
    [Table("CENTROGESTION")]
    public class ManagmentCenter
    {
        [Column("id_centrogestion")]
        [Key]
        public int Id { get; set; }

        [Column("nombre_centrogestion")]
        public string Name { get; set; }

        [Column("cod_centrogestion_ag")]
        public string Code { get; set; }

        [Column("centroopearativo_centrogestion")]
        public int OperationCenterId { get; set; }

        [ForeignKey("OperationCenterId")]
        public OperationCenter OperationCenter { get; set; }
    }
}
