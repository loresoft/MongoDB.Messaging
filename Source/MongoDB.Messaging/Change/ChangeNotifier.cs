using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Messaging.Extensions;
using MongoDB.Messaging.Logging;
using MongoDB.Messaging.Storage;

namespace MongoDB.Messaging.Change
{
    /// <summary>
    /// Notification of MongoDB changes by tailing the oplog collection and publishing a change record.
    /// </summary>
    public class ChangeNotifier
    {
        private static readonly ILogger _logger = Logger.CreateLogger<ChangeNotifier>();  

        private readonly List<ISubscription> _subscribers = new List<ISubscription>();
        private readonly Lazy<IMongoCollection<ChangeRecord>> _collection;

        private CancellationTokenSource _tokenSource = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNotifier"/> class.
        /// </summary>
        /// <param name="connectionName">The name of the connection string in the application config.</param>
        /// <exception cref="ArgumentException"><paramref name="connectionName"/> is <see langword="null" /> or empty</exception>
        public ChangeNotifier(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
                throw new ArgumentException("Argument is null or empty", nameof(connectionName));

            _collection = new Lazy<IMongoCollection<ChangeRecord>>(() => GetCollection(connectionName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNotifier"/> class.
        /// </summary>
        /// <param name="mongoClient">The MongoClient connection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mongoClient"/> is <see langword="null" />.</exception>
        public ChangeNotifier(IMongoClient mongoClient)
        {
            if (mongoClient == null)
                throw new ArgumentNullException(nameof(mongoClient));

            _collection = new Lazy<IMongoCollection<ChangeRecord>>(() => GetCollection(mongoClient));
        }


        /// <summary>
        /// Gets or sets the last timestamp that was notified.  When notification is started, it will start notifications from this point.
        /// </summary>
        /// <value>
        /// The last timestamp that was notified.
        /// </value>
        public BsonTimestamp LastNotification { get; set; }

        /// <summary>
        /// Gets the change notification subscribers.
        /// </summary>
        /// <value>
        /// The change notification subscribers.
        /// </value>
        public IReadOnlyCollection<ISubscription> Subscribers => _subscribers;


        /// <summary>
        /// Start listening for changes in the oplog. 
        /// </summary>
        public void Start()
        {
            // start a new source
            var tokenSource = new CancellationTokenSource();
            if (Interlocked.CompareExchange(ref _tokenSource, tokenSource, null) != null)
                return; // if wasn't null, already running

            _logger.Trace()
                .Message("Start change notifier.")
                .Write();

            var token = _tokenSource.Token;

            // start async
            Task.Factory.StartNew(() => StartCursorAsync(token), token, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Stop listening for changes in the oplog. 
        /// </summary>
        public void Stop()
        {
            _logger.Trace()
                .Message("Stop change notifier.")
                .Write();

            // trigger stop by using cancellation source
            _tokenSource?.Cancel();

            // clear cancellation source
            Interlocked.Exchange(ref _tokenSource, null);
        }


        /// <summary>
        /// Subscribe to change notification with the specified <paramref name="subscriber"/> and collection namespace <paramref name="filter"/>.
        /// </summary>
        /// <param name="subscriber">The change notification subscriber.</param>
        /// <param name="filter">The MongoDB collection namespace wildcard filter.</param>
        /// <returns>The subscription instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subscriber"/> is <see langword="null" />.</exception>
        public ISubscription Subscribe(IHandleChange subscriber, string filter = null)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            var subscription = new Subscription(subscriber, filter);
            lock (_subscribers)
                _subscribers.Add(subscription);

            return subscription;
        }

        /// <summary>
        /// Unsubscribe the specified <paramref name="subscription"/>.
        /// </summary>
        /// <param name="subscription">The subscription to remove.</param>
        /// <returns><c>true</c> if successsfully remove; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="subscription"/> is <see langword="null" />.</exception>
        public bool Unsubscribe(ISubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            lock (_subscribers)
                return _subscribers.Remove(subscription);
        }


        /// <summary>
        /// Publish the specified change record to all <see cref="Subscribers"/>.
        /// </summary>
        /// <param name="changeRecord">The change record to publish.</param>
        /// <exception cref="ArgumentNullException"><paramref name="changeRecord"/> is <see langword="null" />.</exception>
        public void Publish(ChangeRecord changeRecord)
        {
            if (changeRecord == null)
                throw new ArgumentNullException(nameof(changeRecord));

            // local snapshot of subscribers
            ISubscription[] subscribers;
            lock (_subscribers)
                subscribers = _subscribers.ToArray();

            // notify all subscribers that are active and match filter
            var deadSubscribers = subscribers
                .Where(h => IsMatch(changeRecord, h.Filter))
                .Where(h => !h.BeginInvoke(changeRecord))
                .ToList();

            if (deadSubscribers.Count == 0)
                return;

            // remove dead subscribers
            lock (_subscribers)
                foreach (var d in deadSubscribers)
                    _subscribers.Remove(d);
        }


        private async Task StartCursorAsync(CancellationToken token)
        {
            var collection = _collection.Value;

            var options = new FindOptions<ChangeRecord> { CursorType = CursorType.TailableAwait };


            while (!token.IsCancellationRequested)
            {
                _logger.Trace()
                    .Message("Starting tailable cursor on oplog.")
                    .Write();

                var timestamp = GetTimestamp();
                var filter = Builders<ChangeRecord>.Filter.Gte(m => m.Timestamp, timestamp);

                // Start the cursor and wait for the initial response
                using (var cursor = await collection.FindAsync(filter, options, token).ConfigureAwait(false))
                {
                    await cursor.ForEachAsync(document =>
                    {
                        LastNotification = document.Timestamp;
                        Publish(document);
                    }, token).ConfigureAwait(false);
                }

                // cursor died, restart it
            }
        }


        private BsonTimestamp GetTimestamp()
        {
            if (LastNotification != null && LastNotification != default(BsonTimestamp))
                return LastNotification;

            int unixTime = Convert.ToInt32(DateTime.UtcNow.ToUnixTimeSeconds());
            var timestamp = new BsonTimestamp(unixTime, 0);

            return timestamp;
        }

        private bool IsMatch(ChangeRecord changeRecord, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return true;

            return changeRecord.Namespace.Like(filter);
        }


        private IMongoCollection<ChangeRecord> GetCollection(string connectionName)
        {
            var mongoUrl = MongoFactory.GetMongoUrl(connectionName);
            var client = new MongoClient(mongoUrl);
            return GetCollection(client);
        }

        private IMongoCollection<ChangeRecord> GetCollection(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("local");
            VerifyCollection(database);

            var collection = database.GetCollection<ChangeRecord>("oplog.rs");
            return collection;
        }


        private static void VerifyCollection(IMongoDatabase database)
        {
            var filter = new BsonDocument("name", "oplog.rs");
            var collections = database.ListCollections(new ListCollectionsOptions {Filter = filter});
            bool exists = collections.ToList().Any();
            if (exists)
                return;

            _logger.Error()
                .Message("Could not find MongoDB 'oplog.rs' collection in 'local' database.")
                .Write();

            throw new InvalidOperationException("Could not find MongoDB 'oplog.rs' collection in 'local' database.");
        }

        private static void LogTaskError(Task task)
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
