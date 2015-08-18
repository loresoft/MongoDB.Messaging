using System;

namespace MongoDB.Messaging
{
    /// <summary>
    /// The state of a message in the processing queue.
    /// </summary>
    public enum MessageState
    {
        /// <summary>
        /// The messaging is being built and is not ready for processing.
        /// </summary>
        None,

        /// <summary>
        /// The message is queued and ready for processing.
        /// </summary>
        Queued,

        /// <summary>
        /// The message is currently being processed.
        /// </summary>
        Processing,

        /// <summary>
        /// The message has completed processing
        /// </summary>
        Complete,

        /// <summary>
        /// The message processing timed out
        /// </summary>
        Timeout,

        /// <summary>
        /// The message is scheduled for future processing.
        /// </summary>
        Scheduled
    }
}