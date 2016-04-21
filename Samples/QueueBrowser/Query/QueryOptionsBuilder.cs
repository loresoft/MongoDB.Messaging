using System;
using System.Linq.Expressions;

namespace QueueBrowser.Query
{
    public class QueryOptionsBuilder<TSource, TResult>
    {
        private readonly QueryOptions<TSource, TResult> _queryOptions;

        public  QueryOptions<TSource, TResult> QueryOptions
        {
            get { return _queryOptions; }
        }
        
        public QueryOptionsBuilder(QueryOptions<TSource, TResult> queryOptions)
        {
            _queryOptions = queryOptions;
        }

        public QueryOptionsBuilder<TSource, TResult> Request(QueryRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            _queryOptions.Page = request.Page;
            _queryOptions.PageSize = request.PageSize;
            _queryOptions.Sort = request.Sort;
            _queryOptions.Descending = request.Descending;
            _queryOptions.Filter = request.Filter;

            return this;
        }

        public QueryOptionsBuilder<TSource, TResult> Page(int page)
        {
            _queryOptions.Page = page;
            return this;
        }
        
        public QueryOptionsBuilder<TSource, TResult> PageSize(int pageSize)
        {
            _queryOptions.PageSize = pageSize;
            return this;
        }
        
        public QueryOptionsBuilder<TSource, TResult> Sort(string sortField)
        {
            _queryOptions.Sort = sortField;
            return this;
        }

        public QueryOptionsBuilder<TSource, TResult> Filter(string filter)
        {
            _queryOptions.Filter = filter;
            return this;
        }
        
        public QueryOptionsBuilder<TSource, TResult> Descending(bool value = true)
        {
            _queryOptions.Descending = value;
            return this;
        }

        public QueryOptionsBuilder<TSource, TResult> Selector(Expression<Func<TSource, TResult>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            _queryOptions.Selector = selector;
            return this;
        }

        
    }
}