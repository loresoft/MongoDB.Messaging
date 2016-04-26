using System;
using Microsoft.AspNet.SignalR;
using MongoDB.Messaging.Change;

namespace QueueBrowser.Notifications
{
    public class ChangeNotificationHub : Hub<INotifyChange>
    {
    }
}