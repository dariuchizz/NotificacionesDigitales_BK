using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ServiceResponseStatus
    {
        Ok,
        Error,
        ValidationError,
    }
}