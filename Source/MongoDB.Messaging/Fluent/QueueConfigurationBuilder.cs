using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A queue configuration builder
    /// </summary>
    public class QueueConfigurationBuilder : QueueConfigurationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueConfigurationBuilder"/> class.
        /// </summary>
        /// <param name="configuration">The queue configuration.</param>
        public QueueConfigurationBuilder(IQueueConfiguration configuration)
            : base(configuration)
        {
        }

        /// <summary>
        /// Sets the queue description.
        /// </summary>
        /// <param name="queueDescription">The queue description.</param>
        /// <returns>A fluent <see langword="interface"/> to build the queue message.</returns>
        public QueueConfigurationBuilder Description(string queueDescription)
        {
            Configuration.Description = queueDescription;
            return this;
        }

        /// <summary>
        /// Sets the number of times the message should retry on error. Use zero to prevent retry.
        /// </summary>
        /// <param name="count">The number of retries.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> to build the queue configuration.
        /// </returns>
        public QueueConfigurationBuilder Retry(int count)
        {
            Configuration.RetryCount = count;
            return this;
        }

        /// <summary>
        /// Sets the priority of the message in the queue.
        /// </summary>
        /// <param name="value">The priority of the message in the queue.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> to build the queue configuration.
        /// </returns>
        public QueueConfigurationBuilder Priority(MessagePriority value)
        {
            Configuration.Priority = value;
            return this;
        }

        /// <summary>
        /// Sets the response queue.
        /// </summary>
        /// <param name="queueName">Name of the response queue.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> to build the queue configuration.
        /// </returns>
        public QueueConfigurationBuilder ResponseQueue(string queueName)
        {
            Configuration.ResponseQueue = queueName;
            return this;
        }
    }
}