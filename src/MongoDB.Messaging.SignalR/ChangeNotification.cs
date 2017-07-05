using System;
using System.Runtime.InteropServices.WindowsRuntime;
using MongoDB.Bson;
using MongoDB.Messaging.Change;

namespace MongoDB.Messaging.SignalR
{
    /// <summary>
    /// A change notification message
    /// </summary>
    public class ChangeNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNotification"/> class.
        /// </summary>
        public ChangeNotification()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNotification"/> class.
        /// </summary>
        /// <param name="change">The change.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ChangeNotification(ChangeRecord change)
        {
            if (change == null)
                throw new ArgumentNullException(nameof(change));

            Timestamp = GetDateTime(change.Timestamp);
            UniqueId = change.UniqueId;
            Version = change.Version;
            Operation = change.Operation;
            Namespace = change.Namespace;
            Key = GetKey(change)?.ToString();
        }


        /// <summary>
        /// Gets or sets the change timestamp.
        /// </summary>
        /// <value>
        /// The change timestamp.
        /// </value>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the change unique identifier.
        /// </summary>
        /// <value>
        /// The change unique identifier.
        /// </value>
        public long UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the change version.
        /// </summary>
        /// <value>
        /// The change version.
        /// </value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the change operation.
        /// </summary>
        /// <value>
        /// The change operation.
        /// </value>
        public string Operation { get; set; }

        /// <summary>
        /// Gets or sets the change namespace.
        /// </summary>
        /// <value>
        /// The change namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the change key.
        /// </summary>
        /// <value>
        /// The change key.
        /// </value>
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

        private static DateTime? GetDateTime(BsonTimestamp timestamp)
        {
            if (timestamp == null)
                return null;

            return new DateTime(1970, 1, 1).AddSeconds(timestamp.Timestamp);
        }

    }
}