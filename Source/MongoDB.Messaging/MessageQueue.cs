using System;
using System.Threading.Tasks;
using MongoDB.Messaging.Configuration;
using MongoDB.Messaging.Fluent;

namespace MongoDB.Messaging
{
    /// <summary>
    /// A class to Configure and Publish to a message queue.
    /// </summary>
    public class MessageQueue
    {
        private readonly IQueueManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueue"/> class.
        /// </summary>
        public MessageQueue()
            : this(new QueueManager())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueue"/> class.
        /// </summary>
        /// <param name="manager"> The queue manager for underlying queues.</param>
        /// <exception cref="System.ArgumentNullException">The manager argument can not be null.</exception>
        public MessageQueue(IQueueManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            _manager = manager;
        }

        /// <summary>
        /// Gets the queue manager for underlying queues.
        /// </summary>
        /// <value>
        /// The queue manager for underlying queues.
        /// </value>
        public IQueueManager QueueManager
        {
            get { return _manager; }
        }


        /// <summary>
        /// Configure the message queues with the specified fluent builder.
        /// </summary>
        /// <param name="builder">The fluent builder to configure message queues.</param>
        /// <example>
        /// Configure queue and subscriber with fluent builder.
        /// <code><![CDATA[
        /// MessageQueue.Default.Configure(c => c
        ///     .Connection("Messaging")
        ///     .Queue(q => q
        ///         .Name("queue-name")
        ///         .Description("description")
        ///         .Retry(1)
        ///         .Priority(MessagePriority.Normal)
        ///         .ResponseQueue("response-name")
        ///     )
        ///     .Subscribe(s => s
        ///         .Queue("queue-name")
        ///         .PollTime(TimeSpan.FromSeconds(10))
        ///         .Handler<MessageHandler>()
        ///         .Workers(2)
        ///         .Timeout(TimeSpan.FromMinutes(30))
        ///         .TimeoutAction(TimeoutPolicy.Retry)
        ///     )
        /// );
        /// ]]></code>
        /// </example>
        public void Configure(Action<QueueManagerBuilder> builder)
        {
            var queueManagerBuilder = new QueueManagerBuilder(_manager);
            builder(queueManagerBuilder);
        }

        /// <summary>
        /// Publish a message to a queue with the specified fluent builder as an asynchronous operation.
        /// </summary>
        /// <param name="builder">The fluent builder for creating the message to publish.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        /// <example>
        /// Publish a user message to a queue.
        /// <code><![CDATA[
        /// var message = await MessageQueue.Default.Publish(c => c
        ///     .Queue("queue-name")
        ///     .Name("UserMessage")
        ///     .Description("Update User Data")
        ///     .Data(userMessage)
        /// );
        /// ]]></code>
        /// </example>
        /// <exception cref="System.ArgumentNullException">The builder argument is <c>null</c>.</exception>
        /// <exception cref="System.InvalidOperationException">Could not find queue to publish message.</exception>
        public Task<Message> Publish(Action<PublishQueueBuilder> builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");

            // start a new message with default state
            var message = new Message();
            message.State = MessageState.Queued;

            var publishBuilder = new PublishQueueBuilder(_manager, message);
            builder(publishBuilder);

            var container = publishBuilder.Container;
            if (container == null)
                throw new InvalidOperationException("Could not find queue to publish message.");

            // enqueue on repository
            return container.Repository.Enqueue(message);
        }


        #region Singleton
        private static readonly Lazy<MessageQueue> _current = new Lazy<MessageQueue>(() => new MessageQueue());

        /// <summary>
        /// Gets the default singleton instance of <see cref="MessageQueue"/>.
        /// </summary>
        /// <value>The current singleton instance.</value>
        public static MessageQueue Default
        {
            get { return _current.Value; }
        }
        #endregion
    }
}
