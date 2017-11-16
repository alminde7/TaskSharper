using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Serilog;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Service.Controllers
{
    [Attributes.Log]
    public class CategoriesController : ApiController
    {
        private readonly IEventManager _eventManager;
        public ILogger Logger { get; set; }

        public CategoriesController(IEventManager eventManager, ILogger logger)
        {
            _eventManager = eventManager;
            Logger = logger.ForContext<EventsController>();
        }

        [HttpGet]
        [ResponseType(typeof(List<EventCategory>))]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var categories = await _eventManager.GetCategoriesAsync();
                return Content(HttpStatusCode.OK, categories);
            }
            catch (Exception e)
            {
                var errmsg = $"Failed to retrieve EventCategories";
                Logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }
        }
    }
}
