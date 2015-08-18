using System;
using MongoDB.Bson.Serialization;
using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Subscription
{
    /// <summary>
    /// The message processing context.
    /// </summary>
    public class ProcessContext
    {
        private readonly Message _message;
        private readonly IQueueContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessContext"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="container">The collection.</param>
        public ProcessContext(Message message, IQueueContainer container)
        {
            _message = message;
            _container = container;
        }

        /// <summary>
        /// Gets the queue message to process.
        /// </summary>
        /// <value>
        /// The queue message to process.
        /// </value>
        public Message Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Gets the message queue collection.
        /// </summary>
        /// <value>
        /// The message queue collection.
        /// </value>
        public IQueueContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// Get data of type <typeparamref name="TData"/> from the <see cref="Message"/>.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <returns>An instance of type <typeparamref name="TData"/>.</returns>
        public TData Data<TData>()
        {
            return BsonSerializer.Deserialize<TData>(_message.Data);
        }

        /// <summary>
        /// Writes a status message back to the <see cref="Container"/> for the current request.
        /// </summary>
        /// <param name="message">The status message.</param>
        /// <param name="step">The current step.</param>
        /// <returns></returns>
        public void UpdateStatus(string message, int? step = null)
        {
            if (_message == null || _container == null)
                return;

            // fire and forget or block?
            _container.Repository.UpdateStatus(_message.Id, message, step);
        }
    }
}