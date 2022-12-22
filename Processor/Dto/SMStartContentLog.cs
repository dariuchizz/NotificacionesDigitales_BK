using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Processor.Dto
{
    public partial class SMStartContentLog
    {
        [JsonProperty("cel")]
        public string Cel { get; set; }

        [JsonProperty("nom")]
        public string Nom { get; set; }

        [JsonProperty("dato")]
        public string Dato { get; set; }

        [JsonProperty("men")]
        public string Men { get; set; }

        [JsonProperty("hora")]
        public DateTimeOffset Hora { get; set; }

        [JsonProperty("telefonica")]
        public string Telefonica { get; set; }

        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("res")]
        public string Res { get; set; }
    }

    public partial class SMStartContentLog
    {
        public static SMStartContentLog[] FromJson(string json) => JsonConvert.DeserializeObject<SMStartContentLog[]>(json, Processor.Dto.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SMStartContentLog[] self) => JsonConvert.SerializeObject(self, Processor.Dto.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}