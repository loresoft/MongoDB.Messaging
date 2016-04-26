using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QueueBrowser.Repositories;

namespace QueueBrowser.Services
{
    [RoutePrefix("api/Logging")]
    public class LoggingController : ApiController
    {
        private readonly LoggingRepository _repository = new LoggingRepository();

        [HttpGet]
        [Route("", Name = "LoggingList")]
        public IHttpActionResult Get(int? page = null, int? pageSize = null, string sort = null, bool? descending = null, string filter = null)
        {
            var result = _repository.GetLogs(page, pageSize, sort, descending, filter);

            return Ok(result);
        }

    }
}
