using System;

namespace QueueBrowser.Query
{
    public class QueryRequest
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string Sort { get; set; }

        public bool Descending { get; set; }

        public string Filter { get; set; }
    }
}
