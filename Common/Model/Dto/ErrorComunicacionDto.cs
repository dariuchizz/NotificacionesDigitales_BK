using Newtonsoft.Json;

namespace Common.Model.Dto
{
    public class ErrorComunicacionDto
    {
        [JsonProperty("cantidad")]
        public int Cantidad { get; set; }
    }
}
