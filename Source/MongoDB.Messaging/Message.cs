using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Messaging.Security;

namespace MongoDB.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        public Message()
        {
            State = MessageState.None;
            Result = MessageResult.None;
            Priority = (int)MessagePriority.Normal;
            UserName = UserHelper.Current();
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }


        /// <summary>
        /// Gets or sets the identifier for the message.
        /// </summary>
        /// <value>
        /// The identifier for the message.
        /// </value>
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the system name for the message
        /// </summary>
        /// <value>
        /// The system name for the message.
        /// </value>
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display description for the message.
        /// </summary>
        /// <value>
        /// The display description for the message.
        /// </value>
        [BsonIgnoreIfNull]
        public string Description { get; set; }


        /// <summary>
        /// Gets or sets the correlation identifier used to track messages.
        /// </summary>
        /// <value>
        /// The correlation identifier used to track messages.
        /// </value>
        [BsonIgnoreIfNull]
        public string Correlation { get; set; }


        /// <summary>
        /// Gets or sets the queue processing state of the message.
        /// </summary>
        /// <value>
        /// The queue processing state of the message.
        /// </value>
        [BsonRepresentation(BsonType.String)]
        public MessageState State { get; set; }

        /// <summary>
        /// Gets or sets the processing result status.
        /// </summary>
        /// <value>
        /// The processing result status.
        /// </value>
        [BsonRepresentation(BsonType.String)]
        public MessageResult Result { get; set; }

        /// <summary>
        /// Gets or sets the response queue.
        /// </summary>
        /// <value>
        /// The response queue.
        /// </value>
        [BsonIgnoreIfNull]
        public string ResponseQueue { get; set; }

        /// <summary>
        /// Gets or sets the processing step of the message.
        /// </summary>
        /// <value>
        /// The processing step of the message.
        /// </value>
        [BsonIgnoreIfNull]
        public int? Step { get; set; }

        /// <summary>
        /// Gets or sets the message processing status text.
        /// </summary>
        /// <value>
        /// The  message processing status text.
        /// </value>
        [BsonIgnoreIfNull]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the data for the message.
        /// </summary>
        /// <value>
        /// The data for the message.
        /// </value>
        [BsonIgnoreIfNull]
        public BsonDocument Data { get; set; }

        /// <summary>
        /// Gets or sets the priority of the message in the queue.
        /// </summary>
        /// <value>
        /// The priority of the message in the queue.
        /// </value>
        public int Priority { get; set; }


        /// <summary>
        /// Gets or sets the number of times the message should retry on error. Use zero to prevent retry.
        /// </summary>
        /// <value>
        /// The number of times the message should retry on error.
        /// </value>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the number of times this messages has been processed with an error.
        /// </summary>
        /// <value>
        /// The number of times this messages has been processed with an error.
        /// </value>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        public int ErrorCount { get; set; }

        /// <summary>
        /// Gets or sets the scheduled start UTC time for processing this message.
        /// </summary>
        /// <value>
        /// The scheduled start UTC time for processing this message.
        /// </value>
        [BsonIgnoreIfNull]
        public DateTime? Scheduled { get; set; }


        /// <summary>
        /// Gets or sets the processing start UTC time.
        /// </summary>
        /// <value>
        /// The processing start UTC time.
        /// </value>
        [BsonIgnoreIfNull]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the processing end UTC time.
        /// </summary>
        /// <value>
        /// The processing end UTC time.
        /// </value>
        [BsonIgnoreIfNull]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time that this message will expire from the queue.
        /// </summary>
        /// <value>
        /// The date and time that this message will expire from the queue.
        /// </value>
        [BsonIgnoreIfNull]
        public DateTime? Expire { get; set; }



        /// <summary>
        /// Gets or sets the name of the user who queued the message.
        /// </summary>
        /// <value>
        /// The name of the user who queued the message.
        /// </value>
        [BsonIgnoreIfNull]
        public string UserName { get; set; }


        /// <summary>
        /// Gets or sets the date the entity was created in UTC time.
        /// </summary>
        /// <value>
        /// The date the entity was created in UTC time.
        /// </value>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date the entity was updated in UTC time.
        /// </summary>
        /// <value>
        /// The date the entity was updated in UTC time.
        /// </value>
        public DateTime Updated { get; set; }
    }
}