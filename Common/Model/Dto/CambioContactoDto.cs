using Newtonsoft.Json;
using System;

namespace Common.Model.Dto
{
    public class CambioContactoDto
    {
        [JsonProperty("idCambioContacto")]
        public long IdCambioContacto { get; set; }

        [JsonProperty("idContacto")]
        public long? IdContacto { get; set; }

        [JsonProperty("idCanal")]
        public long IdCanal { get; set; }

        [JsonProperty("spam")]
        public bool Spam { get; set; }

        [JsonProperty("procesado")]
        public string Procesado { get; set; }

        [JsonProperty("fechaProcesado")]
        public DateTime? FechaProcesado { get; set; }

        [JsonProperty("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

    }
}