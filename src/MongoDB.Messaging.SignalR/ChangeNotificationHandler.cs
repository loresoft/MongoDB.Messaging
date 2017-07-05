using System;
using Microsoft.AspNet.SignalR;
using MongoDB.Messaging.Change;

namespace MongoDB.Messaging.SignalR
{
    /// <summary>
    /// A change handler that forwards the change to the underling SignalR hub context.
    /// </summary>
    /// <seealso cref="MongoDB.Messaging.Change.IHandleChange" />
    public class ChangeNotificationHandler : IHandleChange
    {
        private readonly IHubContext<IChangeNotificationClient> _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNotificationHandler"/> class.
        /// </summary>
        /// <param name="context">The SignalR hub context.</param>
        /// <exception cref="System.ArgumentNullException">context is null</exception>
        public ChangeNotificationHandler(IHubContext<IChangeNotificationClient> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        /// <summary>
        /// Handle a MongoDB change record by forwarding the change to the underling SignalR hub context.
        /// </summary>
        /// <param name="change">The change record.</param>
        /// <remarks>
        /// This method is called on a thread-pool background thread.
        /// </remarks>
        public virtual void HandleChange(ChangeRecord change)
        {
            var notification = new ChangeNotification(change);
            _context.Clients.All.SendChange(notification);
        }
    }
}
