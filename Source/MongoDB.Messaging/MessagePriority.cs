using System;

namespace MongoDB.Messaging
{
    /// <summary>
    /// The priority of the message in the queue.
    /// </summary>
    public enum MessagePriority
    {
        /// <summary>
        /// The high priority messages will be processed first.
        /// </summary>
        High = 0,
        /// <summary>
        /// The normal prioritymessages will be processed in the order received.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The low priority messages are processed last.
        /// </summary>
        Low = 2
    }
}