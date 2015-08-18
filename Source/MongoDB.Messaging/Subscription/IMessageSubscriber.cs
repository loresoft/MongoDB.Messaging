using System;

namespace MongoDB.Messaging.Subscription
{
    /// <summary>
    /// An <see langword="interface"/> to implement to process message from a queue.
    /// </summary>
    public interface IMessageSubscriber : IDisposable
    {
        /// <summary>
        /// Process a message with the specified process context.
        /// </summary>
        /// <param name="processContext">The process context.</param>
        /// <returns>The result to report after processing the message.</returns>
        MessageResult Process(ProcessContext processContext);
    }
}