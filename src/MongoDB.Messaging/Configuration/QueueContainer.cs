using System;
using MongoDB.Messaging.Storage;

namespace MongoDB.Messaging.Configuration
{
    /// <summary>
    /// A <c>class</c> defining the components that make a up queue.
    /// </summary>
    public class QueueContainer : IQueueContainer
    {
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
                throw new ArgumentNullException(nameof(configuration));

            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            Name = configuration.Name;
            Configuration = configuration;
            Repository = repository;
        }

        /// <summary>
        /// Gets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the storage repository.
        /// </summary>
        /// <value>
        /// The storage repository.
        /// </value>
        public IQueueRepository Repository { get; }

        /// <summary>
        /// Gets the queue configuration.
        /// </summary>
        /// <value>
        /// The queue configuration.
        /// </value>
        public IQueueConfiguration Configuration { get; }


        /// <summary>
        /// Apply default settings to the specified <paramref name="message" />.
        /// </summary>
        /// <param name="message">The message to update.</param>
        public void ApplyDefaults(Message message)
        {
            message.Name = Configuration.Name;
            message.RetryCount = Configuration.RetryCount;
            message.Priority = (int)Configuration.Priority;
            message.ResponseQueue = Configuration.ResponseQueue;
        }
    }
}