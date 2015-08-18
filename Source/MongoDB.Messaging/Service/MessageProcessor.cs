using System;
using System.Collections.Generic;
using System.Threading;

using MongoDB.Messaging.Logging;
using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// A message queue processor
    /// </summary>
    public class MessageProcessor : IMessageProcessor
    {
        

        private readonly Lazy<IList<IMessageWorker>> _workers;
        private readonly IMessageService _service;
        private readonly IQueueContainer _container;
        private readonly IQueueConfiguration _configuration;

        private int _activeWorkers;
        

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
                Logger.Trace()
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

            // Stop Works
            foreach (var worker in _workers.Value)
            {
                Logger.Trace()
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


        private IList<IMessageWorker> CreateWorkers()
        {
            var workers = new List<IMessageWorker>();

            int count = _configuration.WorkerCount;

            // limit worker count to 1 to 25
            count = Math.Max(1, count);
            count = Math.Min(25, count);

            for (int i = 0; i < count; i++)
            {
                string name = string.Format("{0}-Worker-{1:00}", _configuration.Name, i + 1);
                var worker = new MessageWorker(this, name);

                Logger.Trace().Message("Created worker '{0}'.", worker.Name).Write();
                workers.Add(worker);
            }

            // add health work 
            string healthName = string.Format("{0}-Worker-Health", _configuration.Name);
            var healthWorker = new HealthWorker(this, healthName);
            workers.Add(healthWorker);

            return workers;
        }

    }
}