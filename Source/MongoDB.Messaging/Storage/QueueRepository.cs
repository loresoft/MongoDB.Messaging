using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Messaging.Storage
{
    /// <summary>
    /// The queue storage repository
    /// </summary>
    public class QueueRepository : IQueueRepository
    {
        private readonly IMongoCollection<Message> _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueRepository"/> class.
        /// </summary>
        /// <param name="collection">The underlying storage collection.</param>
        public QueueRepository(IMongoCollection<Message> collection)
        {
            _collection = collection;
        }

        /// <summary>
        /// Gets the underlying storage collection.
        /// </summary>
        /// <value>
        /// The underlying storage collection.
        /// </value>
        public IMongoCollection<Message> Collection
        {
            get { return _collection; }
        }


        /// <summary>
        /// Start an <see cref="IQueryable" /> of all messages.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable" /> of all messages.
        /// </returns>
        public IQueryable<Message> All()
        {
            return _collection.AsQueryable();
        }


        /// <summary>
        /// Finds message with the specified <paramref name="id" /> as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier of the message.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public Task<Message> Find(string id)
        {
            return _collection
                .Find(m => m.Id == id)
                .FirstOrDefaultAsync();
        }


        /// <summary>
        /// Finds one message with the specified <paramref name="criteria" /> as an asynchronous operation.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public Task<Message> FindOne(Expression<Func<Message, bool>> criteria)
        {
            return _collection
                .Find(criteria)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Finds all message with the specified <paramref name="criteria" /> as an asynchronous operation.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public Task<List<Message>> FindAll(Expression<Func<Message, bool>> criteria)
        {
            return _collection
                .Find(criteria)
                .ToListAsync();
        }


        /// <summary>
        /// Gets the number of message as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public Task<long> Count()
        {
            // empty document to get all
            var filter = new BsonDocument();

            return _collection
                .CountAsync(filter);
        }

        /// <summary>
        /// Gets the number of message with the specified <paramref name="criteria" /> as an asynchronous operation.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public Task<long> Count(Expression<Func<Message, bool>> criteria)
        {
            return _collection
                .Find(criteria)
                .CountAsync();
        }


        /// <summary>
        /// Save the specified <paramref name="message" /> as an asynchronous operation.
        /// </summary>
        /// <param name="message">The message to save.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null" />.</exception>
        public Task<Message> Save(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            // generate id if null, upsert requires id
            if (string.IsNullOrEmpty(message.Id))
            {
                message.Id = ObjectId.GenerateNewId().ToString();
                message.Created = DateTime.UtcNow;
            }

            message.Updated = DateTime.UtcNow;

            var updateOptions = new UpdateOptions { IsUpsert = true };

            return _collection
                .ReplaceOneAsync(m => m.Id == message.Id, message, updateOptions)
                .ContinueWith(t => message);
        }


        /// <summary>
        /// Delete the message with the specified <paramref name="id" /> as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier of the message.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null" />.</exception>
        public Task<long> Delete(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return _collection
                .DeleteOneAsync(m => m.Id == id)
                .ContinueWith(t => t.Result.DeletedCount);
        }

        /// <summary>
        /// Delete one message with the specified <paramref name="criteria" /> as an asynchronous operation.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public Task<long> DeleteOne(Expression<Func<Message, bool>> criteria)
        {
            return _collection
                .DeleteOneAsync(criteria)
                .ContinueWith(t => t.Result.DeletedCount);
        }

        /// <summary>
        /// Delete all message with the specified <paramref name="criteria" /> as an asynchronous operation.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public Task<long> DeleteAll(Expression<Func<Message, bool>> criteria)
        {
            return _collection
                .DeleteManyAsync(criteria)
                .ContinueWith(t => t.Result.DeletedCount);
        }


        /// <summary>
        /// Enqueue the specified <paramref name="message" /> for processing as an asynchronous operation.
        /// </summary>
        /// <param name="message">The message to queue.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null" />.</exception>
        public Task<Message> Enqueue(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            // mark state as queued
            message.State = MessageState.Queued;

            return Save(message);
        }

        /// <summary>
        /// Dequeues the next queued message for processing as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        public Task<Message> Dequeue()
        {
            var filter = Builders<Message>.Filter
                .Eq(m => m.State, MessageState.Queued);

            var update = Builders<Message>.Update
                .Set(m => m.State, MessageState.Processing)
                .Set(p => p.Status, "Begin processing ...")
                .Set(p => p.StartTime, DateTime.UtcNow)
                .Set(p => p.Updated, DateTime.UtcNow);

            // sort by priority then by insert order
            var sort = Builders<Message>.Sort
                .Ascending(m => m.Priority)
                .Ascending(m => m.Id);

            var options = new FindOneAndUpdateOptions<Message, Message>();
            options.IsUpsert = false;
            options.ReturnDocument = ReturnDocument.After;
            options.Sort = sort;

            return _collection.FindOneAndUpdateAsync(filter, update, options);
        }

        /// <summary>
        /// Requeue the message with the specified <paramref name="id"/> as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier of the message.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null" />.</exception>
        public Task<Message> Requeue(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            var filter = Builders<Message>.Filter
                .Eq(p => p.Id, id);

            var update = Builders<Message>.Update
                .Set(p => p.State, MessageState.Queued)
                .Set(p => p.Updated, DateTime.UtcNow);

            var options = new FindOneAndUpdateOptions<Message, Message>();
            options.IsUpsert = false;
            options.ReturnDocument = ReturnDocument.After;

            return _collection.FindOneAndUpdateAsync(filter, update, options);
        }


        /// <summary>
        /// Schedules the message with specified identifier for processing on the <paramref name="scheduled"/> date and time.
        /// </summary>
        /// <param name="id">The message identifier to schedule.</param>
        /// <param name="scheduled">The date and time of the scheduled processing.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null" />.</exception>
        public Task<Message> Schedule(string id, DateTime scheduled)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            var filter = Builders<Message>.Filter
                .Eq(p => p.Id, id);

            var update = Builders<Message>.Update
                .Set(p => p.State, MessageState.Scheduled)
                .Set(p => p.Scheduled, scheduled.ToUniversalTime())
                .Set(p => p.Updated, DateTime.UtcNow)
                .Unset(p => p.Expire);  


            var options = new FindOneAndUpdateOptions<Message, Message>();
            options.IsUpsert = false;
            options.ReturnDocument = ReturnDocument.After;

            return _collection.FindOneAndUpdateAsync(filter, update, options);
        }

        /// <summary>
        /// Schedules the specified <paramref name="message" /> for future processing as an asynchronous operation.
        /// </summary>
        /// <param name="message">The message to queue.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">The Scheduled property can't be null when scheduling a message.</exception>
        public Task<Message> Schedule(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (message.Scheduled == null)
                throw new ArgumentException("The Scheduled property can't be null when scheduling a message.", "message");

            // mark state as queued
            message.State = MessageState.Scheduled;

            return Save(message);
        }


        /// <summary>
        /// Updates the status of the message with specified <paramref name="id" /> as an asynchronous operation..
        /// </summary>
        /// <param name="id">The identifier of the message.</param>
        /// <param name="status">The status display mesage.</param>
        /// <param name="step">The current processing step.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null" />.</exception>
        public Task<Message> UpdateStatus(string id, string status, int? step = null)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            var filter = Builders<Message>.Filter
                .Eq(p => p.Id, id);

            var update = Builders<Message>.Update
                .Set(p => p.Updated, DateTime.UtcNow);

            if (!string.IsNullOrEmpty(status))
                update.Set(p => p.Status, status);

            if (step.HasValue)
                update.Set(p => p.Step, step.Value);

            var options = new FindOneAndUpdateOptions<Message, Message>();
            options.IsUpsert = false;
            options.ReturnDocument = ReturnDocument.After;

            return _collection.FindOneAndUpdateAsync(filter, update, options);
        }

        /// <summary>
        /// Marks the processing complete for the message with specified <paramref name="id" /> as an asynchronous operation..
        /// </summary>
        /// <param name="id">The identifier of the message.</param>
        /// <param name="messageResult">The result of the processing.</param>
        /// <param name="status">The status display mesage.</param>
        /// <param name="expireDate">The expire date.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null" />.</exception>
        public Task<Message> MarkComplete(string id, MessageResult messageResult, string status = null, DateTime? expireDate = null)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            var filter = Builders<Message>.Filter
                .Eq(p => p.Id, id);

            var update = Builders<Message>.Update
                .Set(p => p.State, MessageState.Complete)
                .Set(p => p.Result, messageResult)
                .Set(p => p.EndTime, DateTime.UtcNow)
                .Set(p => p.Updated, DateTime.UtcNow);

            update = expireDate.HasValue 
                ? update.Set(p => p.Expire, expireDate.Value) 
                : update.Unset(p => p.Expire);

            if (!string.IsNullOrEmpty(status))
                update = update.Set(p => p.Status, status);

            if (messageResult == MessageResult.Error)
                update = update.Inc(p => p.ErrorCount, 1);

            var options = new FindOneAndUpdateOptions<Message, Message>();
            options.IsUpsert = false;
            options.ReturnDocument = ReturnDocument.After;

            return _collection.FindOneAndUpdateAsync(filter, update, options);
        }
    }
}