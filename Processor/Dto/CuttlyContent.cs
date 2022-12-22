using Newtonsoft.Json;

namespace Processor.Dto
{
    public class CuttlyContent
    {
        [JsonProperty("Url")]
        public Url Url { get; set; }
    }

    public class Url
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("FullLink")]
        public string FullLink { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("shortLink")]
        public string ShortLink { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}