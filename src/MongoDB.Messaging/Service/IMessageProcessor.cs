using System;
using System.Collections.Generic;
using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// Gets the name of the processor.
        /// </summary>
        /// <value>
        /// The name of the processor.
        /// </value>
        string Name { get; }


        /// <summary>
        /// Gets a value indicating whether the processor is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        bool IsBusy { get; }

        /// <summary>
        /// The number active workers
        /// </summary>
        int ActiveWorkers { get; }


        /// <summary>
        /// Gets the parent service.
        /// </summary>
        /// <value>
        /// The parent service.
        /// </value>
        IMessageService Service { get; }

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
        /// Gets the list of workers.
        /// </summary>
        /// <value>
        /// The list of workers.
        /// </value>
        IList<IMessageWorker> Workers { get; }


        /// <summary>
        /// Start the processor and all the <see cref="Workers"/>.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the processor and all the <see cref="Workers"/>.
        /// </summary>
        void Stop();


        /// <summary>
        /// Signal the processor that a worker has begun.
        /// </summary>
        void BeginWork();

        /// <summary>
        /// Signal the processor that a worker has ended.
        /// </summary>
        void EndWork();
    }
}