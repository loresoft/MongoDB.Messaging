using System;

namespace MongoDB.Messaging.Subscription
{
    /// <summary>
    /// An <see langword="interface"/> to implement message error retry logic.
    /// </summary>
    public interface IMessageRetry
    {
        /// <summary>
        /// Determine if the the message should be retried.
        /// </summary>
        /// <param name="processContext">The process context.</param>
        /// <param name="exception">The exception thrown while processing message.</param>
        /// <returns><c>true</c> if the message should be retried; otherwise <c>false</c>.</returns>
        bool ShouldRetry(ProcessContext processContext, Exception exception);

        /// <summary>
        /// Get the next <see cref="DateTime"/> to attempt retry.
        /// </summary>
        /// <param name="processContext">The process context.</param>
        /// <returns><see cref="DateTime"/> the message should be retried.</returns>
        DateTime NextAttempt(ProcessContext processContext);
    }
}