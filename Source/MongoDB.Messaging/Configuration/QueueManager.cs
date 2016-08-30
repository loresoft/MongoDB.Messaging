using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Messaging.Logging;
using MongoDB.Messaging.Storage;

namespace MongoDB.Messaging.Configuration
{
    /// <summary>
    /// A class to manage message queues.
    /// </summary>
    public class QueueManager : IQueueManager
    {
        private static readonly ILogger _logger = Logger.CreateLogger<QueueManager>();
        private readonly ConcurrentDictionary<string, IQueueContainer> _queues;

        private readonly object _databaseLock;
        private IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManager"/> class.
        /// </summary>
        public QueueManager()
        {
            _databaseLock = new object();
            _queues = new ConcurrentDictionary<string, IQueueContainer>(StringComparer.OrdinalIgnoreCase);

            ConnectionName = "Messaging";
            ControlName = "ServiceControlQueue";
            LockCollection = "ServiceLock";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManager"/> class.
        /// </summary>
        /// <param name="database">The database connection to use.</param>
        public QueueManager(IMongoDatabase database)
            : this()
        {
            _database = database;
        }


        /// <summary>
        /// Gets or sets the name of the connection string.
        /// </summary>
        /// <value>
        /// The name of the connection.
        /// </value>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Gets or sets the name of the connection string for change notification.
        /// </summary>
        /// <value>
        /// The name of the connection for change notification.
        /// </value>
        public string NotificationConnection { get; set; }

        /// <summary>
        /// Gets or sets the name of the service control queue.
        /// </summary>
        /// <value>
        /// The name of the service control queue.
        /// </value>
        public string ControlName { get; set; }

        /// <summary>
        /// Gets or sets the name of the lock control collection.
        /// </summary>
        /// <value>
        /// The name of the lock control collection.
        /// </value>
        public string LockCollection { get; set; }

        /// <summary>
        /// Gets the configured queues.
        /// </summary>
        /// <value>
        /// The configured queues.
        /// </value>
        public ConcurrentDictionary<string, IQueueContainer> Queues
        {
            get { return _queues; }
        }

        /// <summary>
        /// Gets the queues with an active subscriber.
        /// </summary>
        /// <value>
        /// The queues with an active subscriber..
        /// </value>
        public IEnumerable<IQueueContainer> Subscriptions
        {
            get { return _queues.Values.Where(q => q.Configuration.SubscriberFactory != null); }
        }

        /// <summary>
        /// Gets the underlying storage database.
        /// </summary>
        /// <value>
        /// The underlying storage database.
        /// </value>
        public IMongoDatabase Database
        {
            get
            {
                CreateDatabase();
                return _database;
            }
        }


        /// <summary>
        /// Registers a queue with the specified configuration.
        /// </summary>
        /// <param name="queueConfiguration">The queue configuration to register.</param>
        /// <returns>
        /// An instance of <see cref="IQueueContainer" /> that was registered.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">queueConfiguration</exception>
        public IQueueContainer Register(IQueueConfiguration queueConfiguration)
        {
            if (queueConfiguration == null)
                throw new ArgumentNullException(nameof(queueConfiguration));


            var queue = Load(queueConfiguration.Name);

            // update config
            var config = queue.Configuration;

            // copy values
            config.Description = queueConfiguration.Description;
            config.PollTime = queueConfiguration.PollTime;
            config.Priority = queueConfiguration.Priority;
            config.ProcessTimeout = queueConfiguration.ProcessTimeout;
            config.ResponseQueue = queueConfiguration.ResponseQueue;
            config.RetryCount = queueConfiguration.RetryCount;
            config.SubscriberFactory = queueConfiguration.SubscriberFactory;
            config.TimeoutPolicy = queueConfiguration.TimeoutPolicy;
            config.WorkerCount = queueConfiguration.WorkerCount;

            return queue;
        }

        /// <summary>
        /// Loads the specified queue by name. If the queue has not been configured, it will be created.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>
        /// An instance of <see cref="IQueueContainer" /> with the queue name.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">queueName</exception>
        /// <exception cref="System.ArgumentException">The queue name is invalid.;queueName</exception>
        public IQueueContainer Load(string queueName)
        {
            if (queueName == null)
                throw new ArgumentNullException(nameof(queueName));

            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("The queue name is invalid.", nameof(queueName));

            return _queues.GetOrAdd(queueName, key =>
            {
                var queue = new QueueConfiguration { Name = key, LockCollection = LockCollection };
                return CreateContainer(queue);
            });
        }

        /// <summary>
        /// Sets the underlying storage <paramref name="database"/>.
        /// </summary>
        /// <param name="database">The underlying storage database.</param>
        public void SetDatabase(IMongoDatabase database)
        {
            // thread safe database creation
            lock (_databaseLock)
                _database = database;
        }

        private void CreateDatabase()
        {
            if (_database != null)
                return;

            // thread safe database creation
            lock (_databaseLock)
            {
                if (_database != null)
                    return;

                _database = string.IsNullOrEmpty(ConnectionName)
                    ? MongoFactory.GetDatabaseFromConnectionString("mongodb://localhost/Messaging")
                    : MongoFactory.GetDatabaseFromConnectionName(ConnectionName);
            }
        }

        private IQueueContainer CreateContainer(IQueueConfiguration configuration)
        {
            var collection = GetCollection(configuration.Name);
            var repository = new QueueRepository(collection);

            var queue = new QueueContainer(configuration, repository);

            return queue;
        }

        private IMongoCollection<Message> GetCollection(string name)
        {
            CreateDatabase();

            var collection = _database.GetCollection<Message>(name);
            InitializeCollection(collection);

            return collection;
        }


        private void InitializeCollection(IMongoCollection<Message> collection)
        {
            // create indexes
            DefaultSortIndex(collection);
            DequeueIndex(collection);
            StateIndex(collection);
            ResultIndex(collection);
            ExpireIndex(collection);
            TimeoutIndex(collection);
            ScheduleIndex(collection);
        }


        private void ScheduleIndex(IMongoCollection<Message> collection)
        {
            var scheduleIndex = Builders<Message>.IndexKeys
                .Ascending("State") // issue with enum type
                .Ascending(m => m.Scheduled);

            var scheduleOptions = new CreateIndexOptions();
            scheduleOptions.Name = "ScheduleIndex";

            // fire and forget
            collection.Indexes.CreateOneAsync(scheduleIndex, scheduleOptions)
                .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void TimeoutIndex(IMongoCollection<Message> collection)
        {
            var timeoutIndex = Builders<Message>.IndexKeys
                .Ascending("State") // issue with enum type
                .Ascending(m => m.Updated);

            var timeoutOptions = new CreateIndexOptions();
            timeoutOptions.Name = "TimeoutIndex";

            // fire and forget
            collection.Indexes.CreateOneAsync(timeoutIndex, timeoutOptions)
                .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void ExpireIndex(IMongoCollection<Message> collection)
        {
            var expireIndex = Builders<Message>.IndexKeys
                .Ascending(m => m.Expire);

            var expireOptions = new CreateIndexOptions();
            expireOptions.Name = "ExpireIndex";
            expireOptions.ExpireAfter = TimeSpan.FromSeconds(1);

            // fire and forget
            collection.Indexes.CreateOneAsync(expireIndex, expireOptions)
                .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void ResultIndex(IMongoCollection<Message> collection)
        {
            var resultIndex = Builders<Message>.IndexKeys
                .Ascending("Result"); // issue with enum type

            var resultOptions = new CreateIndexOptions();
            resultOptions.Name = "ResultIndex";

            // fire and forget
            collection.Indexes.CreateOneAsync(resultIndex, resultOptions)
                .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void StateIndex(IMongoCollection<Message> collection)
        {
            var stateIndex = Builders<Message>.IndexKeys
                .Ascending("State"); // issue with enum type

            var stateOptions = new CreateIndexOptions();
            stateOptions.Name = "StateIndex";

            // fire and forget
            collection.Indexes.CreateOneAsync(stateIndex, stateOptions)
                .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void DequeueIndex(IMongoCollection<Message> collection)
        {
            var dequeueIndex = Builders<Message>.IndexKeys
                .Ascending(m => m.Priority)
                .Ascending(m => m.Id);

            var dequeueOptions = new CreateIndexOptions();
            dequeueOptions.Name = "DequeueIndex";

            // fire and forget
            collection.Indexes.CreateOneAsync(dequeueIndex, dequeueOptions)
                .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void DefaultSortIndex(IMongoCollection<Message> collection)
        {
            var index = Builders<Message>.IndexKeys
                .Descending(m => m.Updated);

            var indexOptions = new CreateIndexOptions();
            indexOptions.Name = "DefaultSortIndex";

            // fire and forget
            collection.Indexes.CreateOneAsync(index, indexOptions)
                .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void LogTaskError(Task task)
        {
            var exception = task.Exception;
            var errorMessage = exception?.Message ?? string.Empty;

            _logger.Error()
                .Message("Error: " + errorMessage)
                .Exception(exception)
                .Write();
        }
    }
}
