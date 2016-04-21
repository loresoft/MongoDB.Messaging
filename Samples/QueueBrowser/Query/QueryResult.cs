using System;
using System.Collections.Generic;

namespace QueueBrowser.Query
{
    public class QueryResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int PageCount { get; set; }


        public string Sort { get; set; }

        public bool Descending { get; set; }


        public string Filter { get; set; }


        public int Total { get; set; }
    }
}