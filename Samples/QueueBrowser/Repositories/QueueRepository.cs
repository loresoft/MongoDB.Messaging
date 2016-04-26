using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Messaging;
using MongoDB.Messaging.Configuration;
using NLog.Fluent;
using QueueBrowser.Models;
using QueueBrowser.Query;

namespace QueueBrowser.Repositories
{
    public class QueueRepository
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public IEnumerable<NameValueModel> GetQueues(string filter = null)
        {
            var db = MessageQueue.Default.QueueManager.Database;
            var names = new List<NameValueModel>();

            List<BsonDocument> result;

            using (var cursor = db.ListCollections())
                result = cursor.ToList();

            // convert to collection names
            foreach (var document in result)
            {
                BsonValue value;

                if (!document.TryGetValue("name", out value))
                    continue;

                string name = value.AsString;
                if (name == null || name.StartsWith("system."))
                    continue;

                if (!string.IsNullOrEmpty(filter) && !Regex.IsMatch(name, filter))
                    continue;

                var model = new NameValueModel { Name = name, Value = name };
                names.Add(model);
            }

            return names;
        }

        public QueueStatusModel GetStatus(string queueName)
        {
            var status = new QueueStatusModel();

            var queueContainer = MessageQueue.Default.QueueManager.Load(queueName);
            var repository = queueContainer.Repository;

            var queuedTask = repository.Count(m => m.State == MessageState.Queued);
            var processingTask = repository.Count(m => m.State == MessageState.Processing);
            var completeTask = repository.Count(m => m.State == MessageState.Complete);
            var timeoutTask = repository.Count(m => m.State == MessageState.Timeout);
            var scheduledTask = repository.Count(m => m.State == MessageState.Scheduled);

            var successfulTask = repository.Count(m => m.Result == MessageResult.Successful);
            var warningTask = repository.Count(m => m.Result == MessageResult.Warning);
            var errorTask = repository.Count(m => m.Result == MessageResult.Error);

            Task.WaitAll(
                queuedTask,
                processingTask,
                completeTask,
                timeoutTask,
                scheduledTask,
                successfulTask,
                warningTask,
                errorTask
            );

            status.Name = queueContainer.Name;
            status.Namespace = repository.Collection.CollectionNamespace.FullName;

            status.Queued = queuedTask.Result;
            status.Processing = processingTask.Result;
            status.Complete = completeTask.Result;
            status.Timeout = timeoutTask.Result;
            status.Scheduled = scheduledTask.Result;

            status.Successful = successfulTask.Result;
            status.Warning = warningTask.Result;
            status.Error = errorTask.Result;

            return status;
        }

        public QueryResult<QueueMessageModel> GetMessages(string queueName, int? page = null, int? pageSize = null, string sort = null, bool? descending = null, string filter = null)
        {
            var queue = GetQueue(queueName);
            if (queue == null)
                return null;

            var query = queue.Repository.All();
            var result = query
                .ToDataResult<Message, QueueMessageModel>(config => config
                    .Page(page ?? 1)
                    .PageSize(pageSize ?? 20)
                    .Sort(sort)
                    .Descending(descending ?? false)
                    .Filter(filter)
                    .Selector(d => new QueueMessageModel
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Description = d.Description,
                        Correlation = d.Correlation,
                        State = d.State,
                        Result = d.Result,
                        ResponseQueue = d.ResponseQueue,
                        Step = d.Step,
                        Status = d.Status,
                        Priority = d.Priority,
                        RetryCount = d.RetryCount,
                        ErrorCount = d.ErrorCount,
                        Scheduled = d.Scheduled,
                        StartTime = d.StartTime,
                        EndTime = d.EndTime,
                        Expire = d.Expire,
                        Created = d.Created,
                        Updated = d.Updated,
                        UserName = d.UserName
                    })
                );

            return result;
        }

        public IQueueContainer GetQueue(string queueName)
        {
            return MessageQueue.Default.QueueManager.Load(queueName);
        }

        public IQueueContainer Requeue(string queueName, IEnumerable<string> ids)
        {
            var queue = MessageQueue.Default.QueueManager.Load(queueName);
            var repo = queue.Repository;

            // requeue as fire and forget
            foreach (string id in ids)
                repo.Requeue(id).ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);


            return queue;
        }

        public IQueueContainer Delete(string queueName, IEnumerable<string> ids)
        {
            var queue = MessageQueue.Default.QueueManager.Load(queueName);
            var repo = queue.Repository;

            // delete as fire and forget
            foreach (string id in ids)
                repo.Delete(id).ContinueWith(LogTaskError, TaskContinuationOptions.OnlyOnFaulted);

            return queue;
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
