using System;

namespace Common.Model.Dto
{
    public class ProcesoNegocioDto
    {
        public long IdProcesoNegocio { get; set; }

        public long? IdSistema { get; set; }

        public string DescripcionProceso { get; set; }

        public int? TopeDiario { get; set; }

        public int Autor { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int? AutorModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }

    }
}
