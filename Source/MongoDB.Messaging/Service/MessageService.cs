using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Messaging.Change;
using MongoDB.Messaging.Configuration;
using MongoDB.Messaging.Logging;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// A message queue processing service.
    /// </summary>
    public class MessageService : IMessageService
    {
        private static readonly ILogger _logger = Logger.CreateLogger<MessageService>();

        private readonly Lazy<IList<IMessageProcessor>> _processors;
        private readonly Lazy<ChangeNotifier> _notifier;
        private readonly IQueueManager _manager;

        private int _activeProcesses;


        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class.
        /// </summary>
        public MessageService()
            : this(MessageQueue.Default.QueueManager)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class.
        /// </summary>
        /// <param name="manager">The queue manager.</param>
        /// <exception cref="ArgumentNullException"><paramref name="manager"/> is <see langword="null" />.</exception>
        public MessageService(IQueueManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            _processors = new Lazy<IList<IMessageProcessor>>(CreateProcesses);
            _notifier = new Lazy<ChangeNotifier>(CreateNotifier);
            _manager = manager;
            _activeProcesses = 0;

        }


        /// <summary>
        /// Gets the queue manager for the service.
        /// </summary>
        /// <value>
        /// The queue manager for the service.
        /// </value>
        public IQueueManager Manager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Gets the list of message processors for the service.
        /// </summary>
        /// <value>
        /// The list of message processors for the service.
        /// </value>
        public IList<IMessageProcessor> Processors
        {
            get { return _processors.Value; }
        }

        /// <summary>
        /// The number active processes
        /// </summary>
        public int ActiveProcesses
        {
            get { return _activeProcesses; }
        }

        /// <summary>
        /// Gets the change notifier service.
        /// </summary>
        /// <value>
        /// The change notifier service.
        /// </value>
        public ChangeNotifier Notifier
        {
            get { return _notifier.Value; }
        }


        /// <summary>
        /// Start the service and all the <see cref="Processors" />.
        /// </summary>
        public void Start()
        {
            _activeProcesses = 0;

            bool notify = false;

            foreach (var process in _processors.Value)
            {
                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    _logger.Error()
                        .Message("Error trying to start processor '{0}'.", process.Name)
                        .Exception(ex)
                        .Write();
                }

                // track if anyone has trigger set
                if (process.Configuration.Trigger)
                    notify = true;
            }

            // start notifier
            if (notify)
                _notifier.Value.Start();
        }

        /// <summary>
        /// Stop the service and all the <see cref="Processors" />.
        /// </summary>
        public void Stop()
        {
            if (_processors == null || _processors.Value.Count == 0)
                return;

            foreach (var monitor in _processors.Value)
                monitor.Stop();

            if (_notifier.IsValueCreated)
                _notifier.Value.Stop();

            // Safe shutdown, wait for active processes
            DateTimeOffset timeout = DateTimeOffset.Now.AddSeconds(30);
            while (_activeProcesses > 0 && timeout > DateTimeOffset.Now)
                Thread.Sleep(500);
        }


        /// <summary>
        /// Signal the processor that a worker has begun.
        /// </summary>
        public void BeginWork()
        {
            Interlocked.Increment(ref _activeProcesses);
        }

        /// <summary>
        /// Signal the processor that a worker has ended.
        /// </summary>
        public void EndWork()
        {
            Interlocked.Decrement(ref _activeProcesses);
        }


        private IList<IMessageProcessor> CreateProcesses()
        {
            // create processor for all subscribers
            var list = _manager.Subscriptions
                .Select(s => new MessageProcessor(this, s))
                .ToList<IMessageProcessor>();

            // TODO create control queue processor

            return list;
        }

        private ChangeNotifier CreateNotifier()
        {
            var connectionName = _manager.NotificationConnection ?? _manager.ConnectionName;
            var notifier = new ChangeNotifier(connectionName);

            return notifier;
        }

    }
}
