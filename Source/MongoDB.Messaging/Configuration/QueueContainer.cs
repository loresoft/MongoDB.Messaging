using System;
using MongoDB.Messaging.Storage;

namespace MongoDB.Messaging.Configuration
{
    /// <summary>
    /// A <c>class</c> defining the components that make a up queue.
    /// </summary>
    public class QueueContainer : IQueueContainer
    {
        private readonly string _name;
        private readonly IQueueRepository _repository;
        private readonly IQueueConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueContainer"/> class.
        /// </summary>
        /// <param name="configuration">The queue configuration.</param>
        /// <param name="repository">The storage repository.</param>
        /// <exception cref="System.ArgumentNullException">
        /// configuration
        /// or
        /// repository
        /// </exception>
        public QueueContainer(IQueueConfiguration configuration, IQueueRepository repository)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (repository == null)
                throw new ArgumentNullException("repository");

            _name = configuration.Name;
            _configuration = configuration;
            _repository = repository;
        }

        /// <summary>
        /// Gets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the storage repository.
        /// </summary>
        /// <value>
        /// The storage repository.
        /// </value>
        public IQueueRepository Repository
        {
            get { return _repository; }
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

        /// <summary>
        /// Apply default settings to the specified <paramref name="message" />.
        /// </summary>
        /// <param name="message">The message to update.</param>
        public void ApplyDefaults(Message message)
        {
            message.Name = _configuration.Name;
            message.RetryCount = _configuration.RetryCount;
            message.Priority = (int)_configuration.Priority;
            message.ResponseQueue = _configuration.ResponseQueue;
        }
    }
}