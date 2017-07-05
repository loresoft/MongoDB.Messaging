using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MongoDB.Messaging.Locks
{
    /// <summary>
    /// The status of a lock
    /// </summary>
    public class LockData
    {
        /// <summary>
        /// Gets or sets the identifier for the lock.
        /// </summary>
        /// <value>
        /// The identifier for the lock.
        /// </value>
        [BsonId(IdGenerator = typeof(NullIdChecker))]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who acquired the lock.
        /// </summary>
        /// <value>
        /// The name of the user who acquired the lock.
        /// </value>
        [BsonIgnoreIfNull]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        /// <value>
        /// The name of the machine.
        /// </value>
        [BsonIgnoreIfNull]
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        /// <value>
        /// The process identifier.
        /// </value>
        [BsonIgnoreIfNull]
        public int? Process { get; set; }

        /// <summary>
        /// Gets or sets the date the lock was created in UTC time.
        /// </summary>
        /// <value>
        /// The date the lock was created in UTC time.
        /// </value>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date the lock was updated in UTC time.
        /// </summary>
        /// <value>
        /// The date the lock was updated in UTC time.
        /// </value>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets the date and time that this lock will expire and be free to be acquired.
        /// </summary>
        /// <value>
        /// The date and time that this lock will expire and be free to be acquired.
        /// </value>
        [BsonIgnoreIfNull]
        public DateTime? Expire { get; set; }
    }
}