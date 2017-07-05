using System;
using MongoDB.Messaging.Storage;

namespace MongoDB.Messaging.Configuration
{
    /// <summary>
    /// An <c>interface</c> defining the components that make a up queue.
    /// </summary>
    public interface IQueueContainer
    {
        /// <summary>
        /// Gets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the storage repository.
        /// </summary>
        /// <value>
        /// The storage repository.
        /// </value>
        IQueueRepository Repository { get; }

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