using System;
using System.Threading;

namespace MongoDB.Messaging.Change
{
    /// <summary>
    /// A change subscription
    /// </summary>
    public class Subscription : ISubscription
    {
        private readonly WeakReference _reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="handler">The change handler.</param>
        /// <param name="filter">The MongoDB collection namespace wildcard filter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null" />.</exception>
        public Subscription(IHandleChange handler, string filter)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _reference = new WeakReference(handler);
            Filter = filter;
        }


        /// <summary>
        /// Gets MongoDB collection namespace wildcard filter.
        /// </summary>
        /// <value>
        /// The MongoDB collection namespace wildcard filter.
        /// </value>
        public string Filter { get; }

        /// <summary>
        /// Gets the change notification handler.
        /// </summary>
        /// <value>
        /// The change notification handler.
        /// </value>
        public IHandleChange Handler => _reference.Target as IHandleChange;


        /// <summary>
        /// Begin invoke of <see cref="Handler"/> on the thread-pool background thread.
        /// </summary>
        /// <param name="change">The change record to send.</param>
        /// <returns><c>true</c> if the Handler is still alive and was able to be invoked; otherwise <c>false</c>.</returns>
        public bool BeginInvoke(ChangeRecord change)
        {
            // handler might have been disposed
            var handler = _reference.Target as IHandleChange;
            if (handler == null)
                return false;

            // fire and forget
            return ThreadPool.QueueUserWorkItem(state => handler.HandleChange(change));
        }
    }
}