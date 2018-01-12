using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Google;
using Serilog;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.Notification;
using TaskSharper.Domain.RestDTO;
using TaskSharper.Domain.ServerEvents;

namespace TaskSharper.Service.Controllers
{
    [Attributes.Log]
    public class TasksController : ApiController
    {
        private readonly IEventManager _eventManager;
        private readonly INotificationPublisher _notificationPublisher;
        public ILogger Logger { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventManager">EventManager is used to get events</param>
        /// <param name="logger"></param>
        public TasksController(IEventManager eventManager, ILogger logger, INotificationPublisher notificationPublisher)
        {
            _eventManager = eventManager;
            _notificationPublisher = notificationPublisher;
            Logger = logger.ForContext<TasksController>();
        }

        /// <summary>
        /// Get a task by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Get(string id, string calendarId)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid id");

            try
            {
                var calEvent = await _eventManager.GetEventAsync(id, calendarId);
                if (calEvent.Type != EventType.Task)
                {
                    throw new KeyNotFoundException();
                }
                return Ok(calEvent);
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
            catch (KeyNotFoundException e)
            {
                var errmsg = $"Task with id {id} was not found";
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

        /// <summary>
        /// Get tasks between two dates
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<Event>))]
        public async Task<IHttpActionResult> Get(DateTime from, DateTime to)
        {
            if (!IsValidTimespan(from, to))
                return BadRequest("'To' date must be later than 'from' date");

            try
            {
                var data = await _eventManager.GetEventsAsync(from, to);
                var tasks = data.Where(e => e.Type == EventType.Task);
                return Ok(tasks);
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

        /// <summary>
        /// Create a new task
        /// </summary>
        /// <param name="calEvent"></param>
        /// <returns></returns>
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
                    Type = EventType.Task,
                    Category = calEvent.EventCategory
                };

                var createdEvent = await _eventManager.CreateEventAsync(newEvent);
                return Created("/api/tasks/" + createdEvent.Id, createdEvent);
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

        /// <summary>
        /// Update an exsisting task
        /// </summary>
        /// <param name="calEvent"></param>
        /// <returns></returns>
        [HttpPut]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Put(Event calEvent)
        {
            try
            {
                var updatedEvent = await _eventManager.UpdateEventAsync(calEvent);
                return Created("/api/tasks/" + updatedEvent.Id, updatedEvent);
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
                var errmsg = $"Failed to update event with id: {calEvent}";
                Logger.Error(e, errmsg);
                return Content(HttpStatusCode.InternalServerError, errmsg);
            }

        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
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

        [HttpPost]
        [Route("api/Tasks/{category}/Complete")]
        public async Task<IHttpActionResult> CompleteTask(Categories category)
        {
            try
            {
                var events = await _eventManager.GetEventsAsync(DateTime.Today);
                events = events.Where(i => i.Type == EventType.Task && i.Category.Name == category.ToString() && !i.MarkedAsDone &&
                                     (i.Start?.Ticks > DateTime.Now.AddMinutes(-15).Ticks && i.Start?.Ticks < DateTime.Now.AddMinutes(15).Ticks ||
                                      i.End?.Ticks < DateTime.Now.AddMinutes(15).Ticks && i.End?.Ticks > DateTime.Now.AddMinutes(-15).Ticks))
                    .OrderBy(o => o.Start).ThenBy(o => o.End).ToList();

                // Find closest event
                Event closestEvent = null;
                foreach (var @event in events)
                {
                    if (closestEvent == null)
                        closestEvent = @event;

                    if (Math.Abs(DateTime.Now.Ticks - (long) @event.Start?.Ticks) <
                        Math.Abs(DateTime.Now.Ticks - (long) closestEvent.Start?.Ticks))
                    {
                        closestEvent = @event;
                    }
                }

                if (closestEvent != null)
                {
                    closestEvent.MarkedAsDone = true;
                    var updatedEvent = await _eventManager.UpdateEventAsync(closestEvent);
                    _notificationPublisher.Publish(new TaskMarkedAsDoneEvent{Event = updatedEvent});
                }
                else
                {
                    throw new KeyNotFoundException();
                }

                return Ok(closestEvent);
            }
            catch (KeyNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, $"There are no incomplete tasks in category {category} from {DateTime.Now.AddMinutes(-15):G} to {DateTime.Now.AddMinutes(15):G}");
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        public enum Categories
        {
            Medication,
            Hygiene,
            Social
        }

        private bool IsValidTimespan(DateTime from, DateTime to)
        {
            if ((to - from).Ticks <= 0) return false;
            return true;
        }
    }
}
