using System;

namespace MongoDB.Messaging.Change
{
    /// <summary>
    /// An <see langword="interface"/> for a change subscription
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Gets MongoDB collection namespace wildcard filter.
        /// </summary>
        /// <value>
        /// The MongoDB collection namespace wildcard filter.
        /// </value>
        string Filter { get; }
        /// <summary>
        /// Gets the change notification handler.
        /// </summary>
        /// <value>
        /// The change notification handler.
        /// </value>
        IHandleChange Handler { get; }
        
        /// <summary>
        /// Begin invoke of <see cref="Handler"/> on the thread-pool background thread.
        /// </summary>
        /// <param name="change">The change record to send.</param>
        /// <returns><c>true</c> if the Handler is still alive and was able to be invoked; otherwise <c>false</c>.</returns>
        bool BeginInvoke(ChangeRecord change);
    }
}