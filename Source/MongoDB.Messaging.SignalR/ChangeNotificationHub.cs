using System;
using Microsoft.AspNet.SignalR;

namespace MongoDB.Messaging.SignalR
{
    /// <summary>
    /// A strongly typed SignalR hub for change notifications.
    /// </summary>
    public class ChangeNotificationHub : Hub<IChangeNotificationClient>
    {

    }
}