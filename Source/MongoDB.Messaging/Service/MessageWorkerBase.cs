using System;
using System.Diagnostics;
using System.Threading;

using MongoDB.Messaging.Logging;
using MongoDB.Messaging.Configuration;
using MongoDB.Messaging.Storage;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// A base message worker implementation 
    /// </summary>
    public abstract class MessageWorkerBase : IMessageWorker
    {
        private static readonly ILogger _logger = Logger.CreateLogger<MessageWorkerBase>();

        private readonly IMessageProcessor _processor;
        private readonly IQueueContainer _container;
        private readonly IQueueConfiguration _configuration;
        private readonly IQueueRepository _repository;
        private readonly Timer _pollTimer;
        private readonly Random _random;
        private readonly string _name;

        private volatile bool _isAwaitingShutdown;
        private int _active = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWorkerBase"/> class.
        /// </summary>
        /// <param name="processor">The parent processor.</param>
        /// <param name="name">The name of the worker.</param>
        /// <exception cref="ArgumentNullException"><paramref name="processor"/> is <see langword="null" />.</exception>
        protected MessageWorkerBase(IMessageProcessor processor, string name)
        {
            if (processor == null)
                throw new ArgumentNullException("processor");

            if (name == null)
                throw new ArgumentNullException("name");

            _name = name;

            _processor = processor;
            _container = _processor.Container;
            _configuration = _container.Configuration;
            _repository = _container.Repository;

            _random = new Random();
            _pollTimer = new Timer(PollQueue);
        }


        /// <summary>
        /// Gets the name of the worker.
        /// </summary>
        /// <value>
        /// The name of the worker.
        /// </value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets a value indicating whether the worker is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get { return _active > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the worker is awaiting shutdown.
        /// </summary>
        /// <value>
        /// <c>true</c> if worker is awaiting shutdown; otherwise, <c>false</c>.
        /// </value>
        public bool IsAwaitingShutdown
        {
            get { return _isAwaitingShutdown; }
        }


        /// <summary>
        /// Gets the parent processor.
        /// </summary>
        /// <value>
        /// The parent processor.
        /// </value>
        public IMessageProcessor Processor
        {
            get { return _processor; }
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
        /// Gets the storage repository.
        /// </summary>
        /// <value>
        /// The storage repository.
        /// </value>
        public IQueueRepository Repository
        {
            get { return _repository; }
        }


        /// <summary>
        /// Gets the poll time.
        /// </summary>
        /// <value>
        /// The poll time.
        /// </value>
        public virtual TimeSpan PollTime
        {
            get { return _configuration.PollTime; }
        }



        /// <summary>
        /// Start the worker processing messages from the queue.
        /// </summary>
        public void Start()
        {
            _logger.Trace()
                .Message("Starting '{0}' worker polling at {1}.", Name, _configuration.PollTime)
                .Write();

            _isAwaitingShutdown = false;
            StartTimer(TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Stop the worker from processing messages from the queue.
        /// </summary>
        public void Stop()
        {
            _logger.Trace()
                .Message("Stopping '{0}' worker.", Name)
                .Write();

            Shutdown();
            StopTimer();
        }

        /// <summary>
        /// Trigger immediate processing of the queue.
        /// </summary>
        public void Trigger()
        {
            StartTimer(TimeSpan.Zero);
        }


        /// <summary>
        /// Signal that a worker has begun.
        /// </summary>
        public void BeginWork()
        {
            Interlocked.Increment(ref _active);
            _processor.BeginWork();
        }

        /// <summary>
        /// Signal that a worker has ended.
        /// </summary>
        public void EndWork()
        {
            Interlocked.Decrement(ref _active);
            _processor.EndWork();
        }


        private void StartTimer(TimeSpan pollTime)
        {
            // don't start if shutting down
            if (_isAwaitingShutdown || IsBusy)
                return;

            // randomize start time to reduce resource contention with multiple workers.
            int nextRun = (int)pollTime.TotalMilliseconds;
            if (nextRun > 1000)
            {
                // pad + or - 5%
                int padding = (int)(nextRun * .05);
                int high = nextRun + padding;
                int low = nextRun - padding;
                nextRun = _random.Next(low, high);
            }

            nextRun = Math.Max(0, nextRun);

            // timer only fires once, up to call back to start timer again
            _pollTimer.Change(nextRun, Timeout.Infinite);
        }

        private void StopTimer()
        {
            _pollTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }


        // called in background thread
        private void PollQueue(object state)
        {
            try
            {
                Logger.ThreadProperties.Set("Worker", Name);
                BeginWork();

                _logger.Trace()
                    .Message("Running '{0}' worker.", Name)
                    .Write();

                var watch = Stopwatch.StartNew();

                Process();

                watch.Stop();

                _logger.Trace()
                    .Message("Completed '{0}' process in: {1} ms.", Name, watch.ElapsedMilliseconds)
                    .Write();

            }
            catch (Exception ex)
            {
                _logger.Error()
                    .Message("Error running '{0}' process. {1}", Name, ex.Message)
                    .Exception(ex)
                    .Write();
            }
            finally
            {
                EndWork();
                Logger.ThreadProperties.Remove("Worker");

                StartTimer(PollTime);
            }

        }


        /// <summary>
        /// Process the underlying queue.
        /// </summary>
        protected abstract void Process();

        /// <summary>
        /// Signal worker to shutdown.
        /// </summary>
        protected void Shutdown()
        {
            _isAwaitingShutdown = true;
        }
    }
}