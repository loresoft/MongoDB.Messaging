using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace QueueBrowser.Models
{
    [BsonIgnoreExtraElements(true, Inherited = true)]
    public class LogEvent
    {
        public LogEvent()
        {
            Properties = new Dictionary<string, string>();
        }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        public string Level { get; set; }

        public string Logger { get; set; }

        public string Message { get; set; }

        [BsonIgnoreIfNull]
        public string Source { get; set; }

        [BsonIgnoreIfNull]
        public string Correlation { get; set; }

        [BsonIgnoreIfNull]
        public LogError Exception { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public bool ShouldSerializeProperties()
        {
            return Properties != null && Properties.Count > 0;
        }
    }
}