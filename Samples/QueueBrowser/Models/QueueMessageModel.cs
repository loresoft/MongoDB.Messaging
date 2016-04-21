using System;
using MongoDB.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QueueBrowser.Models
{
    public class QueueMessageModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Correlation { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MessageState State { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MessageResult Result { get; set; }

        public string ResponseQueue { get; set; }

        public int? Step { get; set; }

        public string Status { get; set; }

        public int Priority { get; set; }

        public int RetryCount { get; set; }

        public int ErrorCount { get; set; }

        public DateTime? Scheduled { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? Expire { get; set; }

        public string UserName { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}