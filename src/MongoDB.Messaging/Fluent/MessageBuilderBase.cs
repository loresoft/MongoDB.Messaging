using System;
using MongoDB.Bson;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A fluent <see langword="class"/> to build a queue message
    /// </summary>
    public class MessageBuilder
    {
        private readonly Message _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBuilder"/> class.
        /// </summary>
        /// <param name="message">The message to update.</param>
        public MessageBuilder(Message message)
        {
            _message = message;
        }

        /// <summary>
        /// Gets the message to be updated.
        /// </summary>
        /// <value>
        /// The message to be updated..
        /// </value>
        public Message Message
        {
            get { return _message; }
        }


        /// <summary>
        /// Set the system name for the message.
        /// </summary>
        /// <param name="name">The system name for the message.</param>
        /// <returns>
        /// A fluent interface to build a queue message
        /// </returns>
        public MessageBuilder Name(string name)
        {
            _message.Name = name;
            return this;
        }

        /// <summary>
        /// Set the description for the message.
        /// </summary>
        /// <param name="value">The description for the message.</param>
        /// <returns>
        /// A fluent interface to build a queue message
        /// </returns>
        public MessageBuilder Description(string value)
        {
            _message.Description = value;
            return this;
        }

        /// <summary>
        /// Set the description for the message using the string format.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>
        /// A fluent interface to build a queue message
        /// </returns>
        public MessageBuilder Description(string format, params object[] args)
        {
            _message.Description = string.Format(format, args);
            return this;
        }

        /// <summary>
        /// Set the description for the message from the specified object.ToString().
        /// </summary>
        /// <param name="value">The object to get the description from.</param>
        /// <returns>
        /// A fluent interface to build a queue message
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" />.</exception>
        public MessageBuilder Description(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            _message.Description = value.ToString();
            return this;
        }

        /// <summary>
        /// Set the data to be processed with the messsage.
        /// </summary>
        /// <param name="value">The  data to be processed with the messsage.</param>
        /// <returns>
        /// A fluent interface to build a queue message
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <see langword="null" />.</exception>
        public MessageBuilder Data<TData>(TData value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (string.IsNullOrEmpty(_message.Name))
                _message.Name = value.GetType().Name;

            if (string.IsNullOrEmpty(_message.Description))
                _message.Description = value.ToString();

            _message.Data = value.ToBsonDocument();
            return this;
        }

        /// <summary>
        /// Set the correlation identifier used to track messages.
        /// </summary>
        /// <param name="value">The correlation identifier used to track messages.</param>
        /// <returns>
        /// A fluent interface to build a queue message
        /// </returns>
        public MessageBuilder Correlation(string value)
        {
            _message.Correlation = value;
            return this;
        }

        /// <summary>
        /// Sets the priority of the message in the queue.
        /// </summary>
        /// <param name="value">The priority of the message in the queue.</param>
        /// <returns>
        /// A fluent interface to build a queue message
        /// </returns>
        public MessageBuilder Priority(MessagePriority value)
        {
            _message.Priority = (int)value;
            return this;
        }

        /// <summary>
        /// Sets the number of times the message should retry on error.  Use zero to prevent retry.
        /// </summary>
        /// <param name="value">The number of times the message should retry on error.</param>
        /// <returns>
        /// A fluent interface to build a queue message
        /// </returns>
        public MessageBuilder Retry(int value)
        {
            _message.RetryCount = value;
            return this;
        }
    }
}