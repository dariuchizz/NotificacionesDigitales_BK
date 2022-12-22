using Newtonsoft.Json;
using System;

namespace Common.Model.Dto
{
    public class NotificacionCampaniaDto
    {
        [JsonProperty("idComunicacion")]
        public long IdComunicacion { get; set; }

        [JsonProperty("idSuministro")]
        public long IdSuministro { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("fechaEnvio")]
        public DateTime? FechaEnvio { get; set; }

        [JsonProperty("contacto")]
        public string Contacto { get; set; }

        [JsonProperty("canal")]
        public string Canal { get; set; }

        [JsonProperty("idCanalCampania")]
        public long IdCanalCampania { get; set; }
    }
}
