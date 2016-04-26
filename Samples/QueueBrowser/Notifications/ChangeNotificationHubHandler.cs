using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using MongoDB.Messaging.Change;

namespace QueueBrowser.Notifications
{
    public class ChangeNotificationHubHandler : IHandleChange
    {
        private readonly IHubContext<INotifyChange> _context;

        public ChangeNotificationHubHandler(IHubContext<INotifyChange> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        public void HandleChange(ChangeRecord change)
        {
            var notification = new ChangeNotification(change);
            _context.Clients.All.Change(notification);
        }
    }
}
