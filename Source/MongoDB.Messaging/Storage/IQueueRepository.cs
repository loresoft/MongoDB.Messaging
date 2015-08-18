using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoDB.Messaging.Storage
{
    /// <summary>
    /// The queue storage repository
    /// </summary>
    public interface IQueueRepository
    {
        /// <summary>
        /// Gets the underlying storage collection.
        /// </summary>
        /// <value>
        /// The underlying storage collection.
        /// </value>
        IMongoCollection<Message> Collection { get; }

        /// <summary>
        /// Finds message with the specified <paramref name="id"/> as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier of the message.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<Message> Find(string id);


        /// <summary>
        /// Finds one message with the specified <paramref name="criteria"/> as an asynchronous operation.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<Message> FindOne(Expression<Func<Message, bool>> criteria);

        /// <summary>
        /// Finds all message with the specified <paramref name="criteria"/> as an asynchronous operation.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<List<Message>> FindAll(Expression<Func<Message, bool>> criteria);


        /// <summary>
        /// Gets the number of message as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<long> Count();

        /// <summary>
        /// Gets the number of message with the specified <paramref name="criteria"/> as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<long> Count(Expression<Func<Message, bool>> criteria);



        /// <summary>
        /// Save the specified <paramref name="message" /> as an asynchronous operation.
        /// </summary>
        /// <param name="message">The message to save.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        Task<Message> Save(Message message);


        /// <summary>
        /// Delete the message with the specified <paramref name="id"/> as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier of the message.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<long> Delete(string id);

        /// <summary>
        /// Delete one message with the specified <paramref name="criteria"/> as an asynchronous operation.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<long> DeleteOne(Expression<Func<Message, bool>> criteria);

        /// <summary>
        /// Delete all message with the specified <paramref name="criteria"/> as an asynchronous operation.
        /// </summary>
        /// <param name="criteria">The criteria expression.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<long> DeleteAll(Expression<Func<Message, bool>> criteria);


        /// <summary>
        /// Enqueue the specified <paramref name="message" /> for processing as an asynchronous operation.
        /// </summary>
        /// <param name="message">The message to queue.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        Task<Message> Enqueue(Message message);

        /// <summary>
        /// Dequeues the next queued message for processing as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<Message> Dequeue();

        /// <summary>
        /// Requeue the message with the specified <paramref name="id"/> as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier of the message.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<Message> Requeue(string id);


        /// <summary>
        /// Schedules the message with specified identifier for processing on the <paramref name="scheduled"/> date and time.
        /// </summary>
        /// <param name="id">The message identifier to schedule.</param>
        /// <param name="scheduled">The date and time of the scheduled processing.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null" />.</exception>
        Task<Message> Schedule(string id, DateTime scheduled);

        /// <summary>
        /// Schedules the specified <paramref name="message" /> for future processing as an asynchronous operation.
        /// </summary>
        /// <param name="message">The message to queue.</param>
        /// <returns>
        /// The <see cref="Task" /> representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">The NextAttempt property can't be null when scheduling a message.</exception>
        Task<Message> Schedule(Message message);


        /// <summary>
        /// Updates the status of the message with specified <paramref name="id"/> as an asynchronous operation..
        /// </summary>
        /// <param name="id">The identifier of the message.</param>
        /// <param name="status">The status display mesage.</param>
        /// <param name="step">The current processing step.</param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task<Message> UpdateStatus(string id, string status, int? step = null);

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
        Task<Message> MarkComplete(string id, MessageResult messageResult, string status = null, DateTime? expireDate = null);
    }
}
