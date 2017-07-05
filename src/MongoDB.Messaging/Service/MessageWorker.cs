using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Messaging.Logging;
using MongoDB.Messaging.Subscription;

namespace MongoDB.Messaging.Service
{
    /// <summary>
    /// A message queue worker
    /// </summary>
    public class MessageWorker : MessageWorkerBase
    {
        private static readonly ILogger _logger = Logger.CreateLogger<MessageWorker>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWorker"/> class.
        /// </summary>
        /// <param name="processor">The parent processor.</param>
        /// <param name="name">The name of the worker.</param>
        public MessageWorker(IMessageProcessor processor, string name)
            : base(processor, name)
        {
        }


        /// <summary>
        /// Process the underlying queue.
        /// </summary>
        protected override void Process()
        {
            var message = Repository.Dequeue().Result;

            // keep looping till queue is empty
            while (message != null)
            {
                ProcessMessage(message);

                // support graceful shutdown
                if (IsAwaitingShutdown)
                    break;

                // next item
                message = Repository.Dequeue().Result;
            }
        }


        private void ProcessMessage(Message message)
        {
            if (message == null)
                return;

            string statusMessage;
            DateTime? expireDate;
            string id = message.Id;

            try
            {
                Logger.ThreadProperties.Set("Message", id);
                statusMessage = string.Format("Processing message '{0}' ...", message.Name);

                _logger.Debug()
                    .Message(statusMessage)
                    .Write();

                Repository.UpdateStatus(id, statusMessage)
                    .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);

                Stopwatch watch = Stopwatch.StartNew();
                var result = ProcessSubscriber(message);
                watch.Stop();

                statusMessage = string.Format("Message '{0}' completed with {1}: {2} ms", message.Name, result, watch.Elapsed);
                expireDate = GetExpire(result);
                var logLevel = GetLevel(result);

                _logger.Log(logLevel)
                    .Message(statusMessage)
                    .Write();

                Repository.MarkComplete(id, result, statusMessage, expireDate)
                    .ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                statusMessage = "Error processing request: " + ex.GetBaseException().Message;
                expireDate = GetExpire(MessageResult.Error);

                _logger.Error()
                    .Message(statusMessage)
                    .Exception(ex)
                    .Write();

                var task = Repository.MarkComplete(id, MessageResult.Error, statusMessage, expireDate);

                // only retry when successfully set result to error
                task.ContinueWith(t => RetryMessage(t.Result, ex), TaskContinuationOptions.OnlyOnRanToCompletion);
                task.ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);
            }
            finally
            {
                Logger.ThreadProperties.Remove("Message");
            }
        }

        private void RetryMessage(Message message, Exception exception)
        {
            if (message == null)
                return;

            if (Configuration.RetryFactory == null)
                return;

            // create message retry 
            var retry = Configuration.RetryFactory();
            if (retry == null)
                return;

            var context = new ProcessContext(message, Container);
            bool shouldRetry = retry.ShouldRetry(context, exception);
            if (!shouldRetry)
                return;

            var nextAttempt = retry.NextAttempt(context);

            _logger.Debug()
                .Message("Message {0} scheduled for retry on {1}.", message.Id, nextAttempt)
                .Write();

            // schedule retry 
            Repository.Schedule(message.Id, nextAttempt);
        }

        private MessageResult ProcessSubscriber(Message message)
        {
            var context = new ProcessContext(message, Container);

            using (var subscriber = CreateSubscriber())
                return subscriber.Process(context);
        }

        private IMessageSubscriber CreateSubscriber()
        {
            var subscriber = Configuration.SubscriberFactory();

            if (subscriber != null)
                return subscriber;

            // can't process messages without subscriber, don't start again
            Shutdown();

            throw new InvalidOperationException(string.Format("Error creating Subscriber for queue '{0}'.", Configuration.Name));
        }

        private DateTime? GetExpire(MessageResult result)
        {
            switch (result)
            {
                case MessageResult.Successful:
                    return DateTime.UtcNow.Add(Configuration.ExpireSuccessful);
                case MessageResult.Warning:
                    return DateTime.UtcNow.Add(Configuration.ExpireWarning);
                case MessageResult.Error:
                    return DateTime.UtcNow.Add(Configuration.ExpireError);
                default:
                    return null;
            }
        }

        private static LogLevel GetLevel(MessageResult result)
        {
            switch (result)
            {
                case MessageResult.Warning:
                    return LogLevel.Warn;
                case MessageResult.Error:
                    return LogLevel.Error;
                default:
                    return LogLevel.Debug;
            }
        }

        private static void LogTaskError(Task task)
        {
            if (task == null)
                return;

            var ex = task.Exception;
            if (ex == null)
                return;

            _logger.Error()
                .Message("Task Error: " + ex.GetBaseException().Message)
                .Exception(ex)
                .Write();
        }
    }
}