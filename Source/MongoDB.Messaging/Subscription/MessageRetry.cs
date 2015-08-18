using System;

namespace MongoDB.Messaging.Subscription
{
    /// <summary>
    /// Default implementation of retry logic
    /// </summary>
    public class MessageRetry : IMessageRetry
    {
        /// <summary>
        /// Determine if the the message should be retried.
        /// </summary>
        /// <param name="processContext">The process context.</param>
        /// <param name="exception">The exception thrown while processing message.</param>
        /// <returns>
        ///   <c>true</c> if the message should be retried; otherwise <c>false</c>.
        /// </returns>
        public virtual bool ShouldRetry(ProcessContext processContext, Exception exception)
        {
            var message = processContext.Message;

            return message.ErrorCount < message.RetryCount;
        }

        /// <summary>
        /// Get the next <see cref="DateTime" /> to attempt retry.
        /// </summary>
        /// <param name="processContext">The process context.</param>
        /// <returns>
        ///   <see cref="DateTime" /> the message should be retried.
        /// </returns>
        public virtual DateTime NextAttempt(ProcessContext processContext)
        {
            var message = processContext.Message;

            // retry weight, 1 = 1 min, 2 = 30 min, 3 = 2 hrs, 4+ = 8 hrs
            if (message.ErrorCount > 3)
                return DateTime.Now.AddHours(8);

            if (message.ErrorCount == 3)
                return DateTime.Now.AddHours(2);

            if (message.ErrorCount == 2)
                return DateTime.Now.AddMinutes(30);

            // default
            return DateTime.Now.AddMinutes(1);
        }
    }
}