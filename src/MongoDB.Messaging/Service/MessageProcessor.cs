using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Messaging.Change;
using MongoDB.Messaging.Logging;
using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// A message queue processor
    /// </summary>
    public class MessageProcessor : IMessageProcessor, IHandleChange
    {
        private static readonly ILogger _logger = Logger.CreateLogger<MessageProcessor>();

        private readonly Lazy<IList<IMessageWorker>> _workers;
        private readonly IMessageService _service;
        private readonly IQueueContainer _container;
        private readonly IQueueConfiguration _configuration;

        private int _activeWorkers;
        private int _triggerAttempts = 0;


        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        /// <param name="service">The parent message service.</param>
        /// <param name="container">The queue container to process.</param>
        public MessageProcessor(IMessageService service, IQueueContainer container)
        {
            _workers = new Lazy<IList<IMessageWorker>>(CreateWorkers);
            _service = service;
            _container = container;
            _configuration = container.Configuration;

            if (!container.Configuration.Trigger)
                return;

            // subscribe to notifications
            var filter = container.Repository.Collection.CollectionNamespace.FullName;
            service.Notifier.Subscribe(this, filter);
        }


        /// <summary>
        /// Gets the name of the processor.
        /// </summary>
        /// <value>
        /// The name of the processor.
        /// </value>
        public string Name
        {
            get { return _configuration.Name; }
        }

        /// <summary>
        /// Gets a value indicating whether the processor is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get { return ActiveWorkers > 0; }
        }

        /// <summary>
        /// The number active workers.
        /// </summary>
        public int ActiveWorkers
        {
            get { return _activeWorkers; }
        }


        /// <summary>
        /// Gets the parent service.
        /// </summary>
        /// <value>
        /// The parent service.
        /// </value>
        public IMessageService Service
        {
            get { return _service; }
        }

        /// <summary>
        /// Gets the queue container for the processor.
        /// </summary>
        /// <value>
        /// The queue container for the processor.
        /// </value>
        public IQueueContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// Gets the queue configuration.
        /// </summary>
        /// <value>
        /// The queue configuration.
        /// </value>
        public IQueueConfiguration Configuration
        {
            get { return _configuration; }
        }

        /// <summary>
        /// Gets the list of workers.
        /// </summary>
        /// <value>
        /// The list of workers.
        /// </value>
        public IList<IMessageWorker> Workers
        {
            get { return _workers.Value; }
        }


        /// <summary>
        /// Start the processor and all the <see cref="Workers" />.
        /// </summary>
        public void Start()
        {

            // Start workers
            foreach (var worker in _workers.Value)
            {
                _logger.Trace()
                    .Message("Starting worker '{0}'.", worker.Name)
                    .Write();

                worker.Start();
            }
        }

        /// <summary>
        /// Stop the processor and all the <see cref="Workers" />.
        /// </summary>
        public void Stop()
        {
            if (!_workers.IsValueCreated)
                return;

            // Stop Workers
            foreach (var worker in _workers.Value)
            {
                _logger.Trace()
                    .Message("Stopping worker '{0}'.", worker.Name)
                    .Write();

                worker.Stop();
            }
        }


        /// <summary>
        /// Signal the processor that a worker has begun.
        /// </summary>
        public void BeginWork()
        {
            Interlocked.Increment(ref _activeWorkers);
            _service.BeginWork();
        }

        /// <summary>
        /// Signal the processor that a worker has ended.
        /// </summary>
        public void EndWork()
        {
            Interlocked.Decrement(ref _activeWorkers);
            _service.EndWork();
        }


        /// <summary>
        /// Handle a MongoDB change record.
        /// </summary>
        /// <param name="change">The change record.</param>
        /// <remarks>
        /// This method is called on a thread-pool background thread.
        /// </remarks>
        void IHandleChange.HandleChange(ChangeRecord change)
        {
            // workers not started
            if (!_workers.IsValueCreated)
                return;

            // look for state changed to queued
            if (!IsQueued(change))
                return;

            // find first free worker and trigger work
            var worker = NextWorker();
            if (worker == null)
            {
                _logger.Trace()
                    .Message("Change notification trigger: All workers busy")
                    .Write();

                return;
            }

            _logger.Trace()
                .Message("Change notification trigger: {0}", worker.Name)
                .Write();

            worker.Trigger();
        }


        private IMessageWorker NextWorker()
        {
            if (!_workers.IsValueCreated)
                return null;

            var workers = _workers.Value;

            int attempt = 0;
            int workerCount = workers.Count;

            while (attempt < workerCount)
            {
                // round robin through workers
                int nextNumber = Interlocked.Increment(ref _triggerAttempts);
                int index = nextNumber % workerCount;

                var worker = workers[index];
                if (!worker.IsBusy && worker is MessageWorker)
                    return worker;

                attempt++;
            }

            return null;
        }

        private IList<IMessageWorker> CreateWorkers()
        {
            var workers = new List<IMessageWorker>();

            int count = _configuration.WorkerCount;

            // limit worker count to 1 to 25
            count = Math.Max(1, count);
            count = Math.Min(25, count);

            for (int i = 0; i < count; i++)
            {
                string name = $"{_configuration.Name}-Worker-{i + 1:00}";
                var worker = new MessageWorker(this, name);

                _logger.Trace().Message("Created worker '{0}'.", worker.Name).Write();
                workers.Add(worker);
            }

            // add health work 
            string healthName = $"{_configuration.Name}-Worker-Health";
            var healthWorker = new HealthWorker(this, healthName);
            workers.Add(healthWorker);

            return workers;
        }


        private static bool IsQueued(ChangeRecord change)
        {
            if (change?.Document == null)
                return false;

            // only insert or update
            if (change.Operation == "i")
                return IsInsertQueued(change);

            if (change.Operation == "u")
                return IsUpdateQueued(change);

            return false;
        }

        private static bool IsQueued(BsonDocument document)
        {
            if (!document.Contains("State"))
                return false;

            return document["State"] == "Queued";
        }

        private static bool IsInsertQueued(ChangeRecord change)
        {
            return IsQueued(change.Document);
        }

        private static bool IsUpdateQueued(ChangeRecord change)
        {
            if (change.Document == null)
                return false;

            if (!change.Document.Contains("$set"))
                return false;

            var document = change.Document["$set"].AsBsonDocument;
            return IsQueued(document);
        }
    }
}