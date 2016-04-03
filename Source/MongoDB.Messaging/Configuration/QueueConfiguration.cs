using System;
using MongoDB.Messaging.Subscription;

namespace MongoDB.Messaging.Configuration
{
    /// <summary>
    /// A <c>class</c> defining a queue configuration
    /// </summary>
    public class QueueConfiguration : IQueueConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueConfiguration"/> class.
        /// </summary>
        public QueueConfiguration()
        {
            PollTime = TimeSpan.FromSeconds(10);
            ProcessTimeout = TimeSpan.FromMinutes(30);
            TimeoutPolicy = TimeoutPolicy.Fail;
            Priority = MessagePriority.Normal;
            WorkerCount = 1;

            RetryFactory = () => new MessageRetry();

            ExpireSuccessful = TimeSpan.FromHours(1);
            ExpireWarning = TimeSpan.FromDays(2);
            ExpireError = TimeSpan.FromDays(7);

            HealthCheck = TimeSpan.FromMinutes(1);
            LockCollection = "ServiceLock";
        }


        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        /// <value>
        /// The name of the queue.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user description for the queue.
        /// </summary>
        /// <value>
        /// The description of the queue.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the number of times the message should retry on error. Use zero to prevent retry.
        /// </summary>
        /// <value>
        /// The number of times the message should retry on error.
        /// </value>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the priority of the message in the queue.
        /// </summary>
        /// <value>
        /// The priority of the message in the queue.
        /// </value>
        public MessagePriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the response queue.
        /// </summary>
        /// <value>
        /// The response queue.
        /// </value>
        public string ResponseQueue { get; set; }


        /// <summary>
        /// Gets or sets the number of workers to run for this queue.
        /// </summary>
        /// <value>
        /// The number of workers to run for this queue.
        /// </value>
        public int WorkerCount { get; set; }

        /// <summary>
        /// Gets or sets the worker poll time.
        /// </summary>
        /// <value>
        /// The worker poll time.
        /// </value>
        public TimeSpan PollTime { get; set; }


        /// <summary>
        /// Gets or sets the process timeout.
        /// </summary>
        /// <value>
        /// The process timeout.
        /// </value>
        public TimeSpan ProcessTimeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout policy.
        /// </summary>
        /// <value>
        /// The timeout policy.
        /// </value>
        public TimeoutPolicy TimeoutPolicy { get; set; }

        /// <summary>
        /// Gets or sets the subscriber factory.
        /// </summary>
        /// <value>
        /// The subscriber factory.
        /// </value>
        public Func<IMessageSubscriber> SubscriberFactory { get; set; }

        /// <summary>
        /// Gets or sets the retry message on error factory.
        /// </summary>
        /// <value>
        /// The retry message on error factory.
        /// </value>
        public Func<IMessageRetry> RetryFactory { get; set; }

        /// <summary>
        /// Gets or sets the time successful messages will kept before expiring.
        /// </summary>
        /// <value>
        /// The time successful messages will kept before expiring.
        /// </value>
        public TimeSpan ExpireSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the time warning messages will kept before expiring.
        /// </summary>
        /// <value>
        /// The time warning messages will kept before expiring.
        /// </value>
        public TimeSpan ExpireWarning { get; set; }

        /// <summary>
        /// Gets or sets the time error messages will kept before expiring.
        /// </summary>
        /// <value>
        /// The time error messages will kept before expiring.
        /// </value>
        public TimeSpan ExpireError { get; set; }


        /// <summary>
        /// Gets or sets the time between health checks.
        /// </summary>
        /// <value>
        /// The time between health checks.
        /// </value>
        public TimeSpan HealthCheck { get; set; }

        /// <summary>
        /// Gets or sets the name lock collection.
        /// </summary>
        /// <value>
        /// The name lock collection.
        /// </value>
        public string LockCollection { get; set; }


        /// <summary>
        /// Gets or sets a value indicating weather the worker is triggered from a change notification.
        /// </summary>
        /// <value>
        ///   <c>true</c> to trigger worker on change; otherwise <c>false</c>.
        /// </value>
        public bool Trigger { get; set; }
    }
}