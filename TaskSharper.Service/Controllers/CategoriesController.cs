using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Google;
using Serilog;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.Service.Controllers
{
    [Attributes.Log]
    public class CategoriesController : ApiController
    {
        private readonly ICategoryManager _categoryManager;
        public ILogger Logger { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryManager">CategoryManager is used to get categories</param>
        /// <param name="logger"></param>
        public CategoriesController(ICategoryManager categoryManager, ILogger logger)
        {
            _categoryManager = categoryManager;
            Logger = logger.ForContext<CategoriesController>();
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]   
        [ResponseType(typeof(List<EventCategory>))]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var categories = await _categoryManager.GetCategoriesAsync();
                return Content(HttpStatusCode.OK, categories);
            }
            catch (GoogleApiException e)
            {
                if (e.HttpStatusCode == HttpStatusCode.Unauthorized)
                {
                    var errmsg = "Unauthorized attempt to call Google Calendar API.";
                    Logger.Error(e, errmsg);
                    return Content(HttpStatusCode.Unauthorized, errmsg);
                }
                return Content(HttpStatusCode.InternalServerError, e);
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
