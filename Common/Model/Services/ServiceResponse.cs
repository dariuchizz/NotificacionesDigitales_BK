using Common.Enums;
using Newtonsoft.Json;

namespace Common.Model.Services
{
    public class ServiceResponse<T>
    {
        [JsonProperty(PropertyName = "status")]
        public ServiceResponseStatus Status { get; set; }

        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }

        [JsonProperty(PropertyName = "errors")]
        public ServiceResponseError[] Errors { get; set; }
    }
}
