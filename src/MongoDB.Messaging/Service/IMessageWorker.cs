using System;
using MongoDB.Messaging.Configuration;
using MongoDB.Messaging.Storage;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageWorker
    {
        /// <summary>
        /// Gets the name of the worker.
        /// </summary>
        /// <value>
        /// The name of the worker.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the worker is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        bool IsBusy { get; }

        /// <summary>
        /// Gets a value indicating whether the worker is awaiting shutdown.
        /// </summary>
        /// <value>
        /// <c>true</c> if worker is awaiting shutdown; otherwise, <c>false</c>.
        /// </value>
        bool IsAwaitingShutdown { get; }


        /// <summary>
        /// Gets the parent processor.
        /// </summary>
        /// <value>
        /// The parent processor.
        /// </value>
        IMessageProcessor Processor { get; }

        /// <summary>
        /// Gets the queue container for the processor.
        /// </summary>
        /// <value>
        /// The queue container for the processor.
        /// </value>
        IQueueContainer Container { get; }

        /// <summary>
        /// Gets the queue configuration.
        /// </summary>
        /// <value>
        /// The queue configuration.
        /// </value>
        IQueueConfiguration Configuration { get; }

        /// <summary>
        /// Gets the storage repository.
        /// </summary>
        /// <value>
        /// The storage repository.
        /// </value>
        IQueueRepository Repository { get; }



        /// <summary>
        /// Start the worker processing messages from the queue.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the worker from processing messages from the queue.
        /// </summary>
        void Stop();

        /// <summary>
        /// Trigger immediate processing of the queue.
        /// </summary>
        void Trigger();


        /// <summary>
        /// Signal that the worker has begun.
        /// </summary>
        void BeginWork();

        /// <summary>
        /// Signal that the worker has ended.
        /// </summary>
        void EndWork();

    }
}