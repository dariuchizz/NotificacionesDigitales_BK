using System;

namespace Common.Model.Dto
{
    public class ConfiguracionEmailDto
    {
        public long IdConfiguracionEmail { get; set; }

        public string Reason { get; set; }

        public int Code { get; set; }

        public string BounceCode { get; set; }

        public string Severity { get; set; }

        public long? IdEventoResultanteEmail { get; set; }

        public int Autor { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int? AutorModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }
    }

}