using MongoDB.Messaging.Storage;

namespace MongoDB.Messaging.Configuration
{
    /// <summary>
    /// An <c>interface</c> defining the components that make a up queue.
    /// </summary>
    public interface IQueueContainer
    {
        /// <summary>
        /// Gets the name of the listen queue.
        /// </summary>
        /// <value>
        /// The name of the listen queue.
        /// </value>
        string NameToListen { get; }

        /// <summary>
        /// Gets the name of the write queue.
        /// </summary>
        /// <value>
        /// The name of the write queue.
        /// </value>
        string NameToWrite { get; }

        /// <summary>
        /// Gets the listen repository.
        /// </summary>
        /// <value>
        /// The listen repository.
        /// </value>
        IQueueRepository RepositoryToListen { get; }

        /// <summary>
        /// Gets the write repository.
        /// </summary>
        /// <value>
        /// The write repository.
        /// </value>
        IQueueRepository RepositoryToWrite { get; }

        /// <summary>
        /// Gets the queue configuration.
        /// </summary>
        /// <value>
        /// The queue configuration.
        /// </value>
        IQueueConfiguration Configuration { get; }


        /// <summary>
        /// Apply default settings to the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to update.</param>
        void ApplyDefaults(Message message);
    }
}