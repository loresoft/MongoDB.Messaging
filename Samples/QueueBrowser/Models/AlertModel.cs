using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QueueBrowser.Models
{
    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AlertModel
    {
        public const int SuccessTimeout = 8000;
        public const int ErrorTimeout = 20000;

        public const string DataKey = "Novus.Alert";

        [JsonProperty(PropertyName = "type", DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public AlertTypes Type { get; set; }

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "timeOut")]
        public int Timeout { get; set; }

        public string ToJson()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.None);
            return json;
        }
    }
}