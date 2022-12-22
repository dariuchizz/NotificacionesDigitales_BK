using Newtonsoft.Json;

namespace NotificacionesDigitalesApi.Model
{
    public class EventProcessGeneratorDto
    {
        [JsonProperty("proceso")]
        public string Proceso { get; set; }

        [JsonProperty("fecha-a-procesar")]
        public string FechaAProcesar { get; set; }

        [JsonProperty("aviso")]
        public bool? Aviso { get; set; }
    }
}
