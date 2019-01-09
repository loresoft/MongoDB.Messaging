using MongoDB.Messaging.Storage;
using System;

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
        public QueueContainer(IQueueConfiguration configuration, IQueueRepository repositoryToListen, IQueueRepository repositoryToWrite)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (repositoryToListen == null)
                throw new ArgumentNullException(nameof(repositoryToListen));

            // If only one repository is given, it will be used to read and write
            if (repositoryToWrite == null)
                repositoryToWrite = repositoryToListen;

            NameToListen = configuration.NameToListen;
            NameToWrite = configuration.NameToWrite;
            Configuration = configuration;
            RepositoryToListen = repositoryToListen;
            RepositoryToWrite = repositoryToWrite;
        }

        /// <summary>
        /// Gets the name of the listen queue.
        /// </summary>
        /// <value>
        /// The name of the listen queue.
        /// </value>
        public string NameToListen { get; }

        /// <summary>
        /// Gets the name of the write queue.
        /// </summary>
        /// <value>
        /// The name of the write queue.
        /// </value>
        public string NameToWrite { get; }

        /// <summary>
        /// Gets the listen repository.
        /// </summary>
        /// <value>
        /// The listen repository.
        /// </value>
        public IQueueRepository RepositoryToListen { get; }

        /// <summary>
        /// Gets the write repository.
        /// </summary>
        /// <value>
        /// The write repository.
        /// </value>
        public IQueueRepository RepositoryToWrite { get; }

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
            message.Name = $"{Configuration.NameToListen}-{Configuration.NameToWrite}";
            message.RetryCount = Configuration.RetryCount;
            message.Priority = (int)Configuration.Priority;
            message.ResponseQueue = Configuration.ResponseQueue;
        }
    }
}