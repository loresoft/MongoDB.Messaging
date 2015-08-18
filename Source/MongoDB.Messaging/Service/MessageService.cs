using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using MongoDB.Messaging.Logging;
using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// A message queue processing service.
    /// </summary>
    public class MessageService : IMessageService
    {
        

        private readonly Lazy<IList<IMessageProcessor>> _processors;
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
        /// Start the service and all the <see cref="Processors" />.
        /// </summary>
        public void Start()
        {
            _activeProcesses = 0;

            foreach (var process in _processors.Value)
            {
                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    Logger.Error()
                        .Message("Error trying to start processor '{0}'.", process.Name)
                        .Exception(ex)
                        .Write();
                }
            }
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
    }
}
