using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Serilog;

namespace TaskSharper.Service.Controllers
{
    public class StatusController : ApiController
    {
        public ILogger Logger { get; set; }

        public StatusController(ILogger logger)
        {
            Logger = logger.ForContext<StatusController>();
        }

        /// <summary>
        /// Check whether the service is alive.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IHttpActionResult))]
        public IHttpActionResult Get()
        {
            var statusmsg = "Connected to service";
            return Content(HttpStatusCode.OK, statusmsg);
        }
        
    }
}
