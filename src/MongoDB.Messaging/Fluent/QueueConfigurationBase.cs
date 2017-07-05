using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A base queue configuration builder 
    /// </summary>
    public class QueueConfigurationBase
    {
        private readonly IQueueConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueConfigurationBase"/> class.
        /// </summary>
        /// <param name="configuration">The queue configuration.</param>
        public QueueConfigurationBase(IQueueConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gets the queue configuration.
        /// </summary>
        /// <value>
        /// The queue configuration.
        /// </value>
        public IQueueConfiguration Configuration
        {
            get { return _configuration; }
        }
    }
}