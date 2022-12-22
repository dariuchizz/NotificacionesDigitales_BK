using System.Collections.Generic;

namespace Common.Model.Dto
{
    public class ParametrosCampaniasDto
    {
        public long IdParametro { get; set; }
        public List<string> Categorias { get; set; }
        public List<string> CategoriasDetalle { get; set; }
        public List<string> Estados { get; set; }
        public int GranCliente { get; set; }
        public int EnteOficial { get; set; }
        public int TieneDebitoAutomatico { get; set; }
        public int? IdMotivoBaja { get; set; }
        public int TieneNotificacionDigital { get; set; }
        public List<string> UnidadesNegocio { get; set; }
        public List<string> Localidades { get; set; }

    }
}
