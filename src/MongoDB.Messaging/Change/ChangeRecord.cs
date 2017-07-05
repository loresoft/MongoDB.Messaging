using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Messaging.Change
{
    /// <summary>
    /// MongoDB change record for the oplog.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ChangeRecord
    {
        /// <summary>
        /// Gets or sets the timestamp for the operation.
        /// </summary>
        /// <value>
        /// The timestamp for the change.
        /// </value>
        /// <remarks>
        /// The 'ts' field in the oplog document.
        /// </remarks>
        [BsonElement("ts")]
        public BsonTimestamp Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the operation.
        /// </summary>
        /// <value>
        /// The unique identifier for the operation.
        /// </value>
        /// <remarks>
        /// The 'h' field in the oplog document.
        /// </remarks>
        [BsonElement("h")]
        public long UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the version of the oplog format.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        /// <remarks>
        /// The 'v' field in the oplog document.
        /// </remarks>
        [BsonElement("v")]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the operation that took place. Known values can be 'i' for insert, 
        /// 'u' for update, 'd' for delete, 'c' for commands and 'n' for no-ops". 
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        /// <remarks>
        /// The 'op' field in the oplog document.
        /// </remarks>
        [BsonElement("op")]
        public string Operation { get; set; }

        /// <summary>
        /// Gets or sets the namespace, database and collection, for the operation.
        /// </summary>
        /// <value>
        /// The namespace for the operation.
        /// </value>
        /// <remarks>
        /// The 'ns' field in the oplog document.
        /// </remarks>
        [BsonElement("ns")]
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the query for an update operation. 
        /// </summary>
        /// <value>
        /// The query for an update operation.
        /// </value>
        /// <remarks>
        /// The 'o2' field in the oplog document.
        /// </remarks>
        [BsonElement("o2")]
        public BsonDocument Query { get; set; }

        /// <summary>
        /// Gets or sets the document for the operation. The document will be different based on the operation.
        /// </summary>
        /// <value>
        /// The document for the operation.
        /// </value>
        /// <remarks>
        /// The 'o' field in the oplog document.
        /// </remarks>
        [BsonElement("o")]
        public BsonDocument Document { get; set; }
    }
}