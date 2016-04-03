using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A queue manager fluent builder.
    /// </summary>
    public class QueueManagerBuilder : QueueManagerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManagerBuilder"/> class.
        /// </summary>
        /// <param name="manager">The queue manager.</param>
        public QueueManagerBuilder(IQueueManager manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Sets the name of the connection string.
        /// </summary>
        /// <param name="name">The name of the connection string.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> for the queue manager.
        /// </returns>
        public QueueManagerBuilder Connection(string name)
        {
            Manager.ConnectionName = name;
            return this;
        }

        /// <summary>
        /// Sets the name of the notification connection string.
        /// </summary>
        /// <param name="name">The name of the notification connection string.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> for the queue manager.
        /// </returns>
        public QueueManagerBuilder Notification(string name)
        {
            Manager.NotificationConnection = name;
            return this;
        }

        /// <summary>
        /// Sets the name of the service control queue.
        /// </summary>
        /// <param name="name">The name of the service control queue.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> for the queue manager.
        /// </returns>
        public QueueManagerBuilder ControlQueue(string name)
        {
            Manager.ControlName = name;
            return this;
        }

        /// <summary>
        /// Configure a message queue using the specified builder.
        /// </summary>
        /// <param name="builder">The fluent builder.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> for the queue manager.
        /// </returns>
        public QueueManagerBuilder Queue(Action<QueueNameBuilder> builder)
        {
            var queueLoaderBuilder = new QueueNameBuilder(Manager);
            builder(queueLoaderBuilder);

            return this;
        }

        /// <summary>
        /// Configure a subscription to a message queue using the specified builder.
        /// </summary>
        /// <param name="builder">The fluent builder.</param>
        /// <returns>
        /// A fluent <see langword="interface" /> for the queue manager.
        /// </returns>
        public QueueManagerBuilder Subscribe(Action<SubscriberQueueBuilder> builder)
        {
            var queueLoaderBuilder = new SubscriberQueueBuilder(Manager);
            builder(queueLoaderBuilder);

            return this;
        }

    }
}
