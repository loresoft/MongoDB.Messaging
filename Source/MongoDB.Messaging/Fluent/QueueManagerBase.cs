using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A queue manager builder base class
    /// </summary>
    public class QueueManagerBase
    {
        private readonly IQueueManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManagerBase"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public QueueManagerBase(IQueueManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Gets the queue manager.
        /// </summary>
        /// <value>
        /// The queue manager.
        /// </value>
        public IQueueManager Manager
        {
            get { return _manager; }
        }
    }
}