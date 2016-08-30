using System;
using System.Diagnostics;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Messaging.Locks;
using MongoDB.Messaging.Logging;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// A queue health monitoring worker.
    /// </summary>
    public class HealthWorker : MessageWorkerBase
    {
        private static readonly ILogger _logger = Logger.CreateLogger<HealthWorker>();
        private readonly ILockManager _lockManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthWorker"/> class.
        /// </summary>
        /// <param name="processor">The parent processor.</param>
        /// <param name="name">The name of the worker.</param>
        public HealthWorker(IMessageProcessor processor, string name)
            : base(processor, name)
        {
            // create throttle lock to only allow one run per schedule
            var lockCollection = Configuration.LockCollection ?? "ServiceLock";
            var collection = Repository.Collection.Database.GetCollection<LockData>(lockCollection);
            _lockManager = new ThrottleLock(collection, Configuration.HealthCheck);
        }


        /// <summary>
        /// Gets the poll time.
        /// </summary>
        /// <value>
        /// The poll time.
        /// </value>
        public override TimeSpan PollTime
        {
            get { return Configuration.HealthCheck; }
        }

        /// <summary>
        /// Process the underlying queue.
        /// </summary>
        protected override void Process()
        {
            if (!_lockManager.Acquire(Name))
                return;

            CheckTimeout();
            CheckSchedule();
        }


        private void CheckTimeout()
        {
            var watch = Stopwatch.StartNew();

            var timeout = Configuration.ProcessTimeout;

            // timeout can't be less than PollTime
            if (timeout < PollTime)
                timeout = PollTime;

            var timeoutDate = DateTime.UtcNow.Subtract(timeout);

            // all message that are processing and update before timeout date
            Expression<Func<Message, bool>> filter = m => m.State == MessageState.Processing && m.Updated < timeoutDate;

            var timeoutState = (Configuration.TimeoutPolicy == TimeoutPolicy.Retry)
                ? MessageState.Queued
                : MessageState.Timeout;

            // update all messages that are timed out
            var updateDefinition = Builders<Message>.Update
                .Set(m => m.State, timeoutState)
                .Set(p => p.Updated, DateTime.UtcNow)
                .Inc(p => p.ErrorCount, 1);

            var updateOptions = new UpdateOptions { IsUpsert = false };

            var t = Repository.Collection.UpdateManyAsync(
                filter,
                updateDefinition,
                updateOptions);

            // wait for result
            var result = t.Result;

            watch.Stop();

            // log as Warn if there are results
            _logger.Warn()
                .Message("Completed '{0}' timeout check in: {1} ms. Timeout Message Count: {2}", Name, watch.ElapsedMilliseconds, result.ModifiedCount)
                .WriteIf(result.ModifiedCount > 0);
        }

        private void CheckSchedule()
        {
            var watch = Stopwatch.StartNew();
            
            var currentDate = DateTime.UtcNow;

            // all message that are scheduled and schedule date before now
            Expression<Func<Message, bool>> filter = m => m.State == MessageState.Scheduled && m.Scheduled < currentDate;

            // update all messages to queued
            var updateDefinition = Builders<Message>.Update
                .Set(m => m.State, MessageState.Queued)
                .Set(p => p.Updated, DateTime.UtcNow);

            var updateOptions = new UpdateOptions { IsUpsert = false };

            var t = Repository.Collection.UpdateManyAsync(
                filter,
                updateDefinition,
                updateOptions);

            // wait for result
            var result = t.Result;

            watch.Stop();

            // log as Info if there are results
            _logger.Info()
                .Message("Completed '{0}' schedule check in: {1} ms. Schedule Message Count: {2}", Name, watch.ElapsedMilliseconds, result.ModifiedCount)
                .WriteIf(result.ModifiedCount > 0);

        }
    }
}