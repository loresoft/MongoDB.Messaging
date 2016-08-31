using System;
using Microsoft.AspNet.SignalR;
using MongoDB.Messaging.Change;
using MongoDB.Messaging.Logging;

namespace MongoDB.Messaging.SignalR
{
    /// <summary>
    /// A change notification service to orchestrate changes from MongoDB to a SignalR hub.
    /// </summary>
    public class ChangeNotificationService
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ChangeNotificationService>();

        private readonly Lazy<ChangeNotifier> _notififer;
        private readonly Lazy<ChangeNotificationHandler> _handler;
        private ISubscription _subscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNotificationService" /> class.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <param name="filter">The MongoDB collection namespace filter.</param>
        public ChangeNotificationService(string connectionName, string filter = null)
        {
            _handler = new Lazy<ChangeNotificationHandler>(CreateHandler);
            _notififer = new Lazy<ChangeNotifier>(() => CreateNotifier(connectionName));

            Filter = filter ?? MessageQueue.Default.QueueManager.Database.DatabaseNamespace.DatabaseName + ".*";

        }

        /// <summary>
        /// Gets or sets the MongoDB collection namespace filter.
        /// </summary>
        /// <value>
        /// The MongoDB collection namespace filter.
        /// </value>
        /// <example>
        /// Filter to all collections in the Messaging database.
        /// <code>Messaging.*</code>
        /// </example>
        public string Filter { get; set; }


        /// <summary>
        /// Start the change notification service.
        /// </summary>
        public void Start()
        {
            _logger.Debug()
                .Message("Change Notification Service Starting with filter '{0}'", Filter)
                .Write();

            _subscription = _notififer.Value.Subscribe(_handler.Value, Filter);
            _notififer.Value.Start();
        }

        /// <summary>
        /// Stop the change notification service.
        /// </summary>
        public void Stop()
        {
            if (!_notififer.IsValueCreated)
                return;

            _logger.Debug()
                .Message("Change Notification Service Stopping")
                .Write();

            if (_subscription != null)
                _notififer.Value.Unsubscribe(_subscription);

            _notififer.Value.Stop();
        }


        /// <summary>
        /// Creates an instance of <see cref="ChangeNotificationHandler"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual ChangeNotificationHandler CreateHandler()
        {
            // get hub contact to send messages on
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChangeNotificationHub, IChangeNotificationClient>();
            return new ChangeNotificationHandler(hubContext);
        }

        /// <summary>
        /// Creates an instance of <see cref="ChangeNotifier"/> from the specified <paramref name="connectionName"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual ChangeNotifier CreateNotifier(string connectionName)
        {
            return new ChangeNotifier(connectionName);
        }

    }
}