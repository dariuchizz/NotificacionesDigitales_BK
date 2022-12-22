using Newtonsoft.Json;

namespace Processor.Dto
{
    public class VencimientoFacturaDto
    {
        [JsonProperty("nombreApellido")]
        public string NombreApellido { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }
        
        [JsonProperty("numeroFactura")]
        public string NumeroFactura { get; set; }
        
        [JsonProperty("cuentaUnificada")]
        public string CuentaUnificada { get; set; }
        
        [JsonProperty("domicilioSuministro")]
        public string DomicilioSuministro { get; set; }
        
        [JsonProperty("montoFactura")]
        public decimal Monto { get; set; }
        
        [JsonProperty("fechaVencimiento")]
        public string FechaVencimiento { get; set; }
        
        [JsonProperty("periodo")]
        public int Periodo { get; set; }

    }
}
