using Newtonsoft.Json;
using System.Collections;

namespace Common.Model.Services
{
    public class ServerValidationErrorResponse : ServiceResponseError
    {
        [JsonProperty(PropertyName = "model_state")]
        public IEnumerable ModelState { get; set; }
    }
}
