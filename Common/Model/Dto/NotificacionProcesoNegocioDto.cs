using Newtonsoft.Json;
using System;

namespace Common.Model.Dto
{
    public class NotificacionProcesoNegocioDto
    {
        [JsonProperty("idComunicacion")]
        public long IdComunicacion { get; set; }

        [JsonProperty("idSuministro")]
        public long IdSuministro { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("fechaEnvio")]
        public DateTime? FechaEnvio { get; set; }

        [JsonProperty("canal")]
        public string Canal { get; set; }

        [JsonProperty("contacto")]
        public string Contacto { get; set; }

        [JsonProperty("dato")]
        public string Dato { get; set; }

        [JsonProperty("idCanal")]
        public long IdCanal { get; set; }
    }
}
