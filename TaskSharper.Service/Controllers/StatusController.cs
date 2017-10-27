using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Serilog;
using Serilog.Core;
using TaskSharper.Domain.BusinessLayer;

namespace TaskSharper.Service.Controllers
{
    public class StatusController : ApiController
    {
        public ILogger Logger { get; set; }
        private readonly IEventManager _eventManager;

        public StatusController(IEventManager eventManager, ILogger logger)
        {
            _eventManager = eventManager;
            Logger = logger.ForContext<EventsController>();
        }

        [HttpGet]
        [ResponseType(typeof(IHttpActionResult))]
        public IHttpActionResult Get()
        {
            var statusmsg = "Connected to service";
            return Content(HttpStatusCode.OK, statusmsg);
        }
        
    }
}
