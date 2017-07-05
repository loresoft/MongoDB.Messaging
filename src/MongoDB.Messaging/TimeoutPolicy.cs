using System;

namespace MongoDB.Messaging
{
    /// <summary>
    /// How a message should be handled on timeout.
    /// </summary>
    public enum TimeoutPolicy
    {
        /// <summary>
        /// The message will be failed and no furture processing occurs.
        /// </summary>
        Fail,

        /// <summary>
        /// The message will be requeued for processing.
        /// </summary>
        Retry
    }
}