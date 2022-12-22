using Newtonsoft.Json;

namespace Processor.Dto
{
    public class MailgunTemplateResponse
    {
        [JsonProperty("template", DefaultValueHandling = DefaultValueHandling.Ignore)] 
        public MailgunTemplateItemDto Template { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class MailgunTemplateItemDto
    {
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("version")]
        public MailgunTemplateVersionDto Version { get; set; }
    }

    public class MailgunTemplateVersionDto
    {
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }
        [JsonProperty("engine")]
        public string Engine { get; set; }
        [JsonProperty("tag")]
        public string Tag { get; set; }
        [JsonProperty("template")]
        public string Template { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
