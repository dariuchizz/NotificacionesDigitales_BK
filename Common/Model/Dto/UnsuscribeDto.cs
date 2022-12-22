using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Model.Dto
{
    public class UnsuscribeDto
    {
        [Required]
        public long IdComunicacion { get; set; }

        [Required]
        public Guid GUID { get; set; }

        [Required]
        public long IdMotivo { get; set; }

        [MaxLength(50)]
        public string ObservacionCliente { get; set; }

        [Required]
        [MaxLength(10)]
        public string Origen { get; set; }
    }
}
