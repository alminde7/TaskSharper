using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Google;
using Serilog;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.RestDTO;

namespace TaskSharper.Service.Controllers
{
    [Attributes.Log]
    public class AppointmentsController : ApiController
    {
        private readonly IEventManager _eventManager;
        public ILogger Logger { get; set; }

        public AppointmentsController(IEventManager eventManager, ILogger logger)
        {
            _eventManager = eventManager;
            Logger = logger.ForContext<AppointmentsController>();
        }

        [HttpGet]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid id");

            try
            {
                var calEvent = await _eventManager.GetEventAsync(id);
                if (calEvent.Type != EventType.Appointment)
                {
                    throw new KeyNotFoundException();
                }
                return Ok(calEvent);
            }
            catch (HttpRequestException e)
            {
                return Content((HttpStatusCode) 599, e);
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
            catch (KeyNotFoundException e)
            {
                var errmsg = $"Appointment with id {id} was not found";
                Logger.Error(e, errmsg);
                return Content(HttpStatusCode.NotFound, errmsg);
            }
            catch (Exception e)
            {
                var errmsg = $"Failed to retrieve events for id {id}";
                Logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<Event>))]
        public async Task<IHttpActionResult> Get(DateTime from, DateTime to)
        {
            if (!IsValidTimespan(from, to))
                return BadRequest("'To' date must be later than 'from' date");

            try
            {
                var data = await _eventManager.GetEventsAsync(from, to);
                var appointments = data.Where(e => e.Type == EventType.Appointment);
                return Ok(appointments);
            }
            catch (HttpRequestException e)
            {
                return Content((HttpStatusCode)599, e);
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
                var errmsg = $"Failed to retrieve events between dates {from} and {to}";
                Logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }
        }

        [HttpPost]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Post(EventDto calEvent)
        {
            if (!IsValidTimespan(calEvent.Start, calEvent.End))
                return BadRequest("Invalid timespan");
            if (string.IsNullOrWhiteSpace(calEvent.Title))
                return BadRequest("No title provided");

            try
            {
                var newEvent = new Event()
                {
                    Title = calEvent.Title,
                    Description = calEvent.Description,
                    Start = calEvent.Start,
                    End = calEvent.End,
                    Status = calEvent.EventStatus,
                    Type = EventType.Appointment,
                    Category = calEvent.EventCategory
                };

                var createdEvent = await _eventManager.CreateEventAsync(newEvent);
                return Created("/api/appointments/" + createdEvent.Id, createdEvent);
            }
            catch (HttpRequestException e)
            {
                return Content((HttpStatusCode)599, e);
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
                var errmsg = $"Failed to create event";
                Logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }
        }

        [HttpPut]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Put(Event calEvent)
        {
            try
            {
                var updatedEvent = await _eventManager.UpdateEventAsync(calEvent);
                return Created("/api/appointments/" + updatedEvent.Id, updatedEvent);
            }
            catch (HttpRequestException e)
            {
                return Content((HttpStatusCode)599, e);
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
                var errmsg = $"Failed to update event with id: {calEvent.Id}";
                Logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }

        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id, string calendarId)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(calendarId))
                return BadRequest("Invalid id or calendarId");

            try
            {
                await _eventManager.DeleteEventAsync(id, calendarId);
                return Ok();
            }
            catch (HttpRequestException e)
            {
                return Content((HttpStatusCode)599, e);
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
                var errmsg = $"Failed to delete event with id: {id}";
                Logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }
        }

        private bool IsValidTimespan(DateTime from, DateTime to)
        {
            if ((to - from).Ticks <= 0) return false;
            return true;
        }
    }
}
