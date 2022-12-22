using Newtonsoft.Json;

namespace Common
{
    public static class Utils
    {
        public static T ConvertToObject<T>(dynamic dyn)
        {
            var json = JsonConvert.SerializeObject(dyn);
            var converted = JsonConvert.DeserializeObject<T>(json);
            return converted;
        }

        public static string ConvertToJson(dynamic dyn)
        {
            var json = JsonConvert.SerializeObject(dyn);
            return json;
        }

        public const string STORAGE_ACCOUNT_NAME = "STORAGE_ACCOUNT_NAME";

        public const string STORAGE_ACCOUNT_EMULATOR = "devstoreaccount1";

        public const string STORAGE_EMULATOR_ACCOUNT_CONNECTION_STRING = "UseDevelopmentStorage=true";

        public const string STORAGE_CONNECTION_STRING_TEMPLATE = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net";

        public const string INDEXED_EVENTS_QUEUE = "indexedreceiptevents";

        public const string STORAGE_ACCOUNT_KEY = "STORAGE_ACCOUNT_KEY";
    }
}
