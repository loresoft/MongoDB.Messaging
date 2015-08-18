using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A queue subscriber configuration builder
    /// </summary>
    public class SubscriberQueueBuilder : QueueManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriberQueueBuilder"/> class.
        /// </summary>
        /// <param name="manager">The queue manager.</param>
        public SubscriberQueueBuilder(IQueueManager manager)
            : base(manager)
        {
        }

        /// <summary>
        /// The queue name to configure a subscriber for
        /// </summary>
        /// <param name="name">The queue name.</param>
        /// <returns></returns>
        public SubscriberBuilder Queue(string name)
        {
            // load queue, apply defaults to message
            var queue = Manager.Load(name);
            var configuration = queue.Configuration;

            return new SubscriberBuilder(configuration);
        }
    }
}