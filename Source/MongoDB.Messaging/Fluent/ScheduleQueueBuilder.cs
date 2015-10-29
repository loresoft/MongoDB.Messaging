using System;
using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A fluent builder to schedule a message for processing.
    /// </summary>
    public class ScheduleQueueBuilder : QueueManagerBase
    {
        private readonly Message _message;
        private PublishQueueBuilder _queueBuilder;


        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleQueueBuilder"/> class.
        /// </summary>
        /// <param name="manager">The queue manager.</param>
        /// <param name="message">The message to update.</param>
        public ScheduleQueueBuilder(IQueueManager manager, Message message) : base(manager)
        {
            _message = message;
        }

        /// <summary>
        /// Gets the message being built.
        /// </summary>
        /// <value>
        /// The message being build.
        /// </value>
        public Message Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Gets the queue instance.
        /// </summary>
        /// <value>
        /// The queue instance.
        /// </value>
        public IQueueContainer Container
        {
            get { return _queueBuilder?.Container; }
        }

        /// <summary>
        /// Start building a scheduled message to a queue with the specified date of processing.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> the message is scheduled for processing on.</param>
        /// <returns>
        /// A fluent interface to build the queue message.
        /// </returns>
        public PublishQueueBuilder Schedule(DateTime value)
        {
            _message.Scheduled = value;

            _queueBuilder = new PublishQueueBuilder(Manager,  _message);

            return _queueBuilder; 
        }
    }
}