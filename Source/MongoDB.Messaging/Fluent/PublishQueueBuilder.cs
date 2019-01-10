using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A fluent builder to help publish a message to a queue
    /// </summary>
    public class PublishQueueBuilder : QueueManagerBase
    {
        private readonly Message _message;
        private IQueueContainer _queueContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishQueueBuilder" /> class.
        /// </summary>
        /// <param name="manager">The queue manager.</param>
        /// <param name="message">The message to update.</param>
        public PublishQueueBuilder(IQueueManager manager, Message message)
            : base(manager)
        {
            _message = message;
        }

        /// <summary>
        /// Gets the message being built.
        /// </summary>
        /// <value>
        /// The message being build.
        /// </value>
        public Message Message => _message;

        /// <summary>
        /// Gets the queue instance.
        /// </summary>
        /// <value>
        /// The queue instance.
        /// </value>
        public IQueueContainer Container => _queueContainer;

        /// <summary>
        /// Start building a message to a queue with the specified name.
        /// </summary>
        /// <param name="name">The name of the queue.</param>
        /// <returns>A fluent interface to build the queue message.</returns>
        public MessageBuilder Queue(string name)
        {
            // load queue, apply defaults to message
            _queueContainer = Manager.Load(name);
            _queueContainer.ApplyDefaults(_message);

            return new MessageBuilder(_message); ;
        }
    }
}