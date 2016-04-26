using System;
using MongoDB.Driver;

namespace QueueBrowser.Models
{
    public class QueueStatusModel
    {
        public string Name { get; set; }

        public string Namespace { get; set; }

        public long Queued { get; set; }
        public long Processing { get; set; }
        public long Complete { get; set; }
        public long Timeout { get; set; }
        public long Scheduled { get; set; }

        public long Successful { get; set; }
        public long Warning { get; set; }
        public long Error { get; set; }
    }
}