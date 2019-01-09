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
        /// The queue names to configure a subscriber for
        /// </summary>
        /// <param name="nameToListen">The queue listen name.</param>
        /// <param name="nameToWrite">The queue write name.</param>
        /// <returns></returns>
        public SubscriberBuilder Queue(string nameToListen, string nameToWrite)
        {
            // load queue, apply defaults to message
            var queue = Manager.Load(nameToListen, nameToWrite);
            var configuration = queue.Configuration;

            return new SubscriberBuilder(configuration);
        }
    }
}