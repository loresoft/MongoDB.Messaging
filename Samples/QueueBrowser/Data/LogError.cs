using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Messaging.Storage;

namespace QueueBrowser.Data
{
    [BsonIgnoreExtraElements(true, Inherited = true)]
    public class LogError
    {
        public string Message { get; set; }

        public string BaseMessage { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public string Source { get; set; }

        public string MethodName { get; set; }

        public string ModuleName { get; set; }

        public string ModuleVersion { get; set; }

        public int? ErrorCode { get; set; }
    }
}