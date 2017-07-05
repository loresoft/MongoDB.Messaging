using System.Collections.Generic;
using MongoDB.Messaging.Change;
using MongoDB.Messaging.Configuration;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Gets the queue manager for the service.
        /// </summary>
        /// <value>
        /// The queue manager for the service.
        /// </value>
        IQueueManager Manager { get; }

        /// <summary>
        /// Gets the list of message processors for the service.
        /// </summary>
        /// <value>
        /// The list of message processors for the service.
        /// </value>
        IList<IMessageProcessor> Processors { get; }

        /// <summary>
        /// The number active processes
        /// </summary>
        int ActiveProcesses { get; }

        /// <summary>
        /// Gets the change notifier service.
        /// </summary>
        /// <value>
        /// The change notifier service.
        /// </value>
        ChangeNotifier Notifier { get; }


        /// <summary>
        /// Start the service and all the <see cref="Processors"/>.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the service and all the <see cref="Processors"/>.
        /// </summary>
        void Stop();


        /// <summary>
        /// Signal the service that a worker has begun.
        /// </summary>
        void BeginWork();

        /// <summary>
        /// Signal the service that a worker has ended.
        /// </summary>
        void EndWork();

    }
}