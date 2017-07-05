using System;
using MongoDB.Messaging.Configuration;
using MongoDB.Messaging.Subscription;

namespace MongoDB.Messaging.Fluent
{
    /// <summary>
    /// A queue subscriber configuration builder
    /// </summary>
    public class SubscriberBuilder : QueueConfigurationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriberBuilder"/> class.
        /// </summary>
        /// <param name="configuration">The queue configuration.</param>
        public SubscriberBuilder(IQueueConfiguration configuration)
            : base(configuration)
        {
        }

        /// <summary>
        /// Sets the worker poll time.
        /// </summary>
        /// <param name="value">The worker poll time.</param>
        /// <returns></returns>
        public SubscriberBuilder PollTime(TimeSpan value)
        {
            Configuration.PollTime = value;
            return this;
        }

        /// <summary>
        /// Sets the number of workers to run for the queue.
        /// </summary>
        /// <param name="value">The number of workers to run for the queue.</param>
        /// <returns></returns>
        public SubscriberBuilder Workers(int value)
        {
            Configuration.WorkerCount = value;
            return this;
        }

        /// <summary>
        /// Sets the subscriber factory the handles the messages for the queue
        /// </summary>
        /// <typeparam name="TSubscriber">The type of the subscriber.</typeparam>
        /// <returns></returns>
        public SubscriberBuilder Handler<TSubscriber>()
            where TSubscriber : IMessageSubscriber, new()
        {
            Configuration.SubscriberFactory = () => new TSubscriber();
            return this;
        }

        /// <summary>
        /// Sets the subscriber factory the handles the messages for the queue
        /// </summary>
        /// <param name="factory">The subscriber factory.</param>
        /// <returns></returns>
        public SubscriberBuilder Handler(Func<IMessageSubscriber> factory)
        {
            Configuration.SubscriberFactory = factory;
            return this;
        }

        /// <summary>
        /// Sets the message processing timeout.
        /// </summary>
        /// <param name="value">The message processing timeout.</param>
        /// <returns></returns>
        public SubscriberBuilder Timeout(TimeSpan value)
        {
            Configuration.ProcessTimeout = value;
            return this;
        }

        /// <summary>
        /// Sets the timeout policy.
        /// </summary>
        /// <param name="value">The timeout policy.</param>
        /// <returns></returns>
        public SubscriberBuilder TimeoutAction(TimeoutPolicy value)
        {
            Configuration.TimeoutPolicy = value;
            return this;
        }


        /// <summary>
        /// Sets the retry factory the handles messages error retries
        /// </summary>
        /// <typeparam name="TFactory">The type of the factory.</typeparam>
        /// <returns></returns>
        public SubscriberBuilder Retry<TFactory>()
            where TFactory : IMessageRetry, new()
        {
            Configuration.RetryFactory = () => new TFactory();
            return this;
        }

        /// <summary>
        /// Sets the retry factory the handles messages error retries
        /// </summary>
        /// <param name="factory">The retry factory.</param>
        /// <returns></returns>
        public SubscriberBuilder Retry(Func<IMessageRetry> factory)
        {
            Configuration.RetryFactory = factory;
            return this;
        }


        /// <summary>
        /// Sets the time successful messages will kept before expiring.
        /// </summary>
        /// <param name="value">The time successful messages will kept before expiring.</param>
        /// <returns></returns>
        public SubscriberBuilder ExpireSuccessful(TimeSpan value)
        {
            Configuration.ExpireSuccessful = value;
            return this;
        }

        /// <summary>
        /// Sets the time warning messages will kept before expiring.
        /// </summary>
        /// <param name="value">The time warning messages will kept before expiring.</param>
        /// <returns></returns>
        public SubscriberBuilder ExpireWarning(TimeSpan value)
        {
            Configuration.ExpireWarning = value;
            return this;
        }

        /// <summary>
        /// Sets the time error messages will kept before expiring.
        /// </summary>
        /// <param name="value">The time error messages will kept before expiring.</param>
        /// <returns></returns>
        public SubscriberBuilder ExpireError(TimeSpan value)
        {
            Configuration.ExpireError = value;
            return this;
        }


        /// <summary>
        /// Sets the health check poll time.
        /// </summary>
        /// <param name="value">The health poll time.</param>
        /// <returns></returns>
        public SubscriberBuilder HeathCheck(TimeSpan value)
        {
            Configuration.HealthCheck = value;
            return this;
        }


        /// <summary>
        /// Sets weather the worker is triggered from a change notification.
        /// </summary>
        /// <param name="value"><c>true</c> to trigger worker on change; otherwise <c>false</c>.</param>
        /// <returns></returns>
        public SubscriberBuilder Trigger(bool value = true)
        {
            Configuration.Trigger = value;

            return this;
        }
    }
}