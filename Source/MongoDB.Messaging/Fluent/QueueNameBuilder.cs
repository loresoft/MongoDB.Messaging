using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A queue configuration builder
    /// </summary>
    public class QueueNameBuilder : QueueManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueNameBuilder"/> class.
        /// </summary>
        /// <param name="manager">The queue manager.</param>
        public QueueNameBuilder(IQueueManager manager)
            : base(manager)
        {
        }

        /// <summary>
        /// The names of the queues to configure.
        /// </summary>
        /// <param name="nameToListen">The queue listen name.</param>
        /// <param name="nameToWrite">The queue write name.</param>
        /// <returns></returns>
        public QueueConfigurationBuilder Name(string nameToListen, string nameToWrite)
        {
            // load queue
            var queue = Manager.Load(nameToListen, nameToWrite);
            var configuration = queue.Configuration;

            return new QueueConfigurationBuilder(configuration);
        }
    }
}