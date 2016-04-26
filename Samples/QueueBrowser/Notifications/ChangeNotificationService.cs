using System;
using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Messaging;
using MongoDB.Messaging.Change;

namespace QueueBrowser.Notifications
{
    public class ChangeNotificationService
    {
        private readonly Lazy<ChangeNotifier> _notififer;
        private readonly Lazy<ChangeNotificationHubHandler> _handler;
        private ISubscription _subscription;

        public ChangeNotificationService() : this(MessageQueue.Default.QueueManager.ConnectionName)
        {
        }

        public ChangeNotificationService(string connectionName)
        {
            _handler = new Lazy<ChangeNotificationHubHandler>(() =>
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChangeNotificationHub, INotifyChange>();
                return new ChangeNotificationHubHandler(hubContext);
            });
            _notififer = new Lazy<ChangeNotifier>(() => new ChangeNotifier(connectionName));
        }

        public void Start()
        {
            // filter subscription to only message queue database
            var filter = MessageQueue.Default.QueueManager.Database.DatabaseNamespace.DatabaseName;
            filter += ".*";

            _subscription = _notififer.Value.Subscribe(_handler.Value, filter);
            _notififer.Value.Start();
        }

        public void Stop()
        {
            if (!_notififer.IsValueCreated)
                return;

            if (_subscription != null)
                _notififer.Value.Unsubscribe(_subscription);

            _notififer.Value.Stop();
        }
    }
}