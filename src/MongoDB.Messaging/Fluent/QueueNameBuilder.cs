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
        /// The name of the queue to configure.
        /// </summary>
        /// <param name="name">The queue name.</param>
        /// <returns></returns>
        public QueueConfigurationBuilder Name(string name)
        {
            // load queue
            var queue = Manager.Load(name);
            var configuration = queue.Configuration;

            return new QueueConfigurationBuilder(configuration);
        }
    }
}