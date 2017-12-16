using System.Web.Http;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.RestDTO;

namespace TaskSharper.Service.Controllers
{
    [Attributes.Log]
    public class ReportController : ApiController
    {
        private readonly IEventManager _eventManager;

        public ReportController(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        [HttpPost]
        public IHttpActionResult Post(ReportDto reportDto)
        {
            // WAT TO DO !?

            return Ok();
        }
    }
}
