using Newtonsoft.Json;

namespace Common.Model.Services
{
    public class ServiceResponseError
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "inner_error")]
        public ServiceResponseError InnerError { get; set; }
    }
}
