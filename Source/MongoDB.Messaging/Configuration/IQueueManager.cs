using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MongoDB.Driver;

namespace MongoDB.Messaging.Configuration
{
    /// <summary>
    /// An <c>interface</c> defining the queue manager.
    /// </summary>
    public interface IQueueManager
    {
        /// <summary>
        /// Gets or sets the name of the connection string.
        /// </summary>
        /// <value>
        /// The name of the connection.
        /// </value>
        string ConnectionName { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the connection string for change notification.
        /// </summary>
        /// <value>
        /// The name of the connection for change notification.
        /// </value>
        string NotificationConnection { get; set; }

        /// <summary>
        /// Gets or sets the name of the service control queue.
        /// </summary>
        /// <value>
        /// The name of the service control queue.
        /// </value>
        string ControlName { get; set; }

        /// <summary>
        /// Gets the configured queues.
        /// </summary>
        /// <value>
        /// The configured queues.
        /// </value>
        ConcurrentDictionary<string, IQueueContainer> Queues { get; }

        /// <summary>
        /// Gets the queues with an active subscriber.
        /// </summary>
        /// <value>
        /// The queues with an active subscriber.
        /// </value>
        IEnumerable<IQueueContainer> Subscriptions { get; }

        /// <summary>
        /// Gets the underlying storage database.
        /// </summary>
        /// <value>
        /// The underlying storage database.
        /// </value>
        IMongoDatabase Database { get; }


        /// <summary>
        /// Registers a queue with the specified configuration.
        /// </summary>
        /// <param name="queueConfiguration">The queue configuration to register.</param>
        /// <returns>An instance of <see cref="IQueueContainer"/> that was registered.</returns>
        IQueueContainer Register(IQueueConfiguration queueConfiguration);

        /// <summary>
        /// Loads the specified queue by name. If the queue has not been configured, it will be created.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>An instance of <see cref="IQueueContainer"/> with the queue name.</returns>
        IQueueContainer Load(string queueName);

        /// <summary>
        /// Sets the underlying storage <paramref name="database"/>.
        /// </summary>
        /// <param name="database">The underlying storage database.</param>
        void SetDatabase(IMongoDatabase database);
    }
}