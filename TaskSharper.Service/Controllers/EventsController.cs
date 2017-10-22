using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Serilog;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.RestDTO;

namespace TaskSharper.Service.Controllers
{
    public class EventsController : ApiController
    {
        private readonly IEventManager _eventManager;
        private readonly ILogger _logger;

        public EventsController(IEventManager eventManager, ILogger logger)
        {
            _eventManager = eventManager;
            _logger = logger;
        }
        
        [Attributes.Log]
        [HttpGet]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return Content(HttpStatusCode.BadRequest, "Invalid id");

            try
            {
                var calEvent = await _eventManager.GetEventAsync(id);
                return Content(HttpStatusCode.OK, calEvent);
            }
            catch (Exception e)
            {
                var errmsg = $"Failed to retrieve events for id {id}";
                _logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }
        }

        [Attributes.Log]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<Event>))]
        public async Task<IHttpActionResult> Get(DateTime from, DateTime to)
        {
            if (!IsValidTimespan(from, to))
                return Content(HttpStatusCode.BadRequest, "'To' date must be later than 'from' date");

            try
            {
                var data = await _eventManager.GetEventsAsync(from, to);
                return Content(HttpStatusCode.OK, data);
            }
            catch (Exception e)
            {
                var errmsg = $"Failed to retrieve events between dates {from} and {to}";
                _logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }
        }

        [Attributes.Log]
        [HttpPost]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Post(EventDto calEvent)
        {
            if (!IsValidTimespan(calEvent.End, calEvent.Start))
                return Content(HttpStatusCode.BadRequest, "Invalid timespan");
            if (string.IsNullOrWhiteSpace(calEvent.Title))
                return Content(HttpStatusCode.BadRequest, "No title provided");

            try
            {
                var newEvent = new Event()
                {
                    Title = calEvent.Title,
                    Description = calEvent.Description,
                    Start = calEvent.Start,
                    End = calEvent.End,
                    Status = calEvent.EventStatus,
                    Type = calEvent.EventType
                };

                var createdEvent = await _eventManager.CreateEventAsync(newEvent);
                return Content(HttpStatusCode.Created, createdEvent);
            }
            catch (Exception e)
            {
                var errmsg = $"Failed to create event";
                _logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }
        }

        [Attributes.Log]
        [HttpPut]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Put(Event calEvent)
        {
            try
            {
                var updatedEvent = await _eventManager.UpdateEventAsync(calEvent);
                return Content(HttpStatusCode.Created, updatedEvent);
            }
            catch (Exception e)
            {
                var errmsg = $"Failed to update event with id: {calEvent.Id}";
                _logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }

        }

        [Attributes.Log]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            try
            {
                await _eventManager.DeleteEventAsync(id);
                return Ok();
            }
            catch (Exception e)
            {
                var errmsg = $"Failed to delete event with id: {id}";
                _logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }
        }


        private bool IsValidTimespan(DateTime to, DateTime from)
        {
            if ((from - to).Ticks < 0) return false;
            return true;
        }
    }
}
