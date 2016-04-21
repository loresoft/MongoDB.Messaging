using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Messaging;
using QueueBrowser.Data;
using QueueBrowser.Models;
using QueueBrowser.Query;

namespace QueueBrowser.Services
{
    [RoutePrefix("api/Queue")]
    public class QueueController : ApiController
    {
        private readonly QueueRepository _repository = new QueueRepository();

        [HttpGet]
        [Route("", Name = "QueueList")]
        public IHttpActionResult Get()
        {
            var model = _repository.GetQueues(@"[\w]+Queue");

            return Ok(model);
        }

        [HttpGet]
        [Route("{name}/Status", Name = "QueueStatus")]
        public IHttpActionResult GetStatus(string name)
        {
            var model = _repository.GetStatus(name);

            return Ok(model);
        }

        [HttpGet]
        [Route("{name}/Messages", Name = "QueueMessages")]
        public IHttpActionResult GetMessages(string name, int? page = null, int? pageSize = null, string sort = null, bool? descending = null, string filter = null)
        {
            var result = _repository.GetMessages(name, page, pageSize, sort, descending, filter);

            return Ok(result);
        }

        [HttpPost]
        [Route("{name}/Requeue", Name = "QueueRequeue")]
        public IHttpActionResult Requeue(string name, QueueActionModel model)
        {
            _repository.Requeue(name, model.Ids);

            return Ok();
        }

        [HttpDelete]
        [Route("{name}/Messages", Name = "QueueDeleteMessages")]
        public IHttpActionResult DeleteMessages(string name, QueueActionModel model)
        {
            _repository.Delete(name, model.Ids);

            return Ok();
        }

    }
}
