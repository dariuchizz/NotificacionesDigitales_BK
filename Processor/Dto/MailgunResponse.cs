using Newtonsoft.Json;

namespace Processor.Dto
{
    public class MailgunResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
