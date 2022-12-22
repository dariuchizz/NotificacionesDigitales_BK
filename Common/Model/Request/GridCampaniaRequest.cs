using System;

namespace Common.Model.Request
{
    public class GridCampaniaRequest: GridRequest
    {
        public int ClaseCampania { get; set; }
        public string Campania { get; set; }
        public int Estado { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public override string ToString()
        {
            return $"PageIndex={PageIndex}&PageSize={PageSize}&Active={Active}&Direction={Direction}" +
                    $"Campania={Campania}&Estado={Estado}&ClaseCampania={ClaseCampania}&FechaDesde={FechaDesde:yyyy/MM/dd}&&FechaDesde={FechaHasta:yyyy/MM/dd}";
        }
    }
}
