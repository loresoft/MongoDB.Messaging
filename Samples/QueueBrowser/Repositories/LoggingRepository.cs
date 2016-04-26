using System;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Messaging.Storage;
using QueueBrowser.Models;
using QueueBrowser.Query;

namespace QueueBrowser.Repositories
{
    public class LoggingRepository 
    {
        private readonly Lazy<IMongoCollection<LogEvent>> _collection;

        public LoggingRepository()
        {
            _collection = new Lazy<IMongoCollection<LogEvent>>(CreateCollection);
        }

        public QueryResult<LogEvent> GetLogs(int? page = null, int? pageSize = null, string sort = null, bool? descending = null, string filter = null)
        {
            var result = _collection.Value.AsQueryable()
                .ToDataResult<LogEvent>(config => config
                    .Page(page ?? 1)
                    .PageSize(pageSize ?? 50)
                    .Sort(sort ?? "Date")
                    .Descending(descending ?? true)
                    .Filter(filter)
                );

            return result;
        }


        private IMongoCollection<LogEvent> CreateCollection()
        {
            string collectionName = ConfigurationManager.AppSettings["LoggingCollectionName"];
            if (string.IsNullOrEmpty(collectionName))
                collectionName = "Logging";

            var database = MongoFactory.GetDatabaseFromConnectionName("LoggingConnection");

            var mongoCollection = database.GetCollection<LogEvent>(collectionName);

            mongoCollection.Indexes.CreateOneAsync(Builders<LogEvent>.IndexKeys.Descending(s => s.Date));
            mongoCollection.Indexes.CreateOneAsync(Builders<LogEvent>.IndexKeys.Ascending(s => s.Level));
            mongoCollection.Indexes.CreateOneAsync(Builders<LogEvent>.IndexKeys.Ascending(s => s.Source));
            mongoCollection.Indexes.CreateOneAsync(Builders<LogEvent>.IndexKeys.Ascending(s => s.Correlation));

            return mongoCollection;
        }
    }
}