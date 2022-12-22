using Newtonsoft.Json;

namespace NotificacionesDigitalesApi.Model
{
    public class EventProcessDto
    {
        [JsonProperty("fecha-a-procesar")]
        public string FechaAProcesar { get; set; }

        [JsonProperty("Hora-a-procesar")]
        public int HoraAProcesar { get; set; }

        [JsonProperty("Id-registro")]
        public string IdRegistro { get; set; }

        [JsonProperty("aviso")]
        public bool? Aviso { get; set; }
        [JsonProperty("tipo-envio")]
        public int TipoEnvio { get; set; }
    }
}
