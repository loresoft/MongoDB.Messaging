using System;
using MongoDB.Bson;
using MongoDB.Messaging.Change;
using Newtonsoft.Json;

namespace QueueBrowser.Notifications
{
    public class ChangeNotification
    {
        public ChangeNotification()
        {
        }

        public ChangeNotification(ChangeRecord change)
        {
            if (change == null)
                throw new ArgumentNullException(nameof(change));

            Timestamp = change.Timestamp.Timestamp;
            UniqueId = change.UniqueId;
            Version = change.Version;
            Operation = change.Operation;
            Namespace = change.Namespace;
            Key = GetKey(change)?.ToString();
        } 

        public int Timestamp { get; set; }

        public long UniqueId { get; set; }

        public int Version { get; set; }

        public string Operation { get; set; }

        public string Namespace { get; set; }

        public string Key { get; set; }


        private static BsonValue GetKey(ChangeRecord change)
        {
            if (change?.Document == null)
                return false;

            // insert, delete or update
            if (change.Operation == "i" || change.Operation == "d")
                return GetKey(change.Document);

            if (change.Operation == "u")
                return GetKey(change.Query);

            return null;
        }

        private static BsonValue GetKey(BsonDocument document)
        {
            if (!document.Contains("_id"))
                return null;

            return document["_id"];
        }
    }
}