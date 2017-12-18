using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Serilog;
using TaskSharper.Domain.Calendar;
using Event = TaskSharper.Domain.Models.Event;

namespace TaskSharper.DataAccessLayer.Google.Calendar
{
    /// <summary>
    /// Handles Event operations on Google Calendar
    /// </summary>
    public class GoogleCalendarEventRepository : GoogleCalendarBase, IEventRepository
    {
        private readonly CalendarService _service;
        public ILogger Logger { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Google Calendar service used to create requests to Google</param>
        /// <param name="logger"></param>
        public GoogleCalendarEventRepository(CalendarService service, ILogger logger) : base(service, logger)
        {
            _service = service;
            Logger = logger.ForContext<GoogleCalendarEventRepository>();
        }
        
        /// <summary>
        /// Get event by id and calendarid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public Event GetEvent(string id, string calendarId)
        {
            // Define parameters of request.
            var request = _service.Events.Get(calendarId, id);
            Logger.Information("Requesting event with id {0} in Google Calendar." + id);

            // Execute request to retrieve events
            var response = request.Execute();
            var events = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful");

            return events;
        }

        /// <summary>
        /// Get all events
        /// </summary>
        /// <returns></returns>
        public List<Event> GetEvents()
        {
            var calendarList = GetCalendars();

            var events = new List<Event>();

            foreach (var calendarListEntry in calendarList)
            {
                var request = _service.Events.List(calendarListEntry.Id);
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Logger.Information($"Requesting all events in Google Calendar {calendarListEntry.Summary}.");

                var response = request.Execute();
                events.AddRange(Helpers.Helpers.GoogleEventParser(response.Items.ToList()));
            }
            Logger.Information("Google Calendar request was successful");

            return events;
        }

        /// <summary>
        /// Get events on a given date
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public List<Event> GetEvents(DateTime start)
        {
            var calendarList = GetCalendars();

            var events = new List<Event>();

            foreach (var calendarListEntry in calendarList)
            {
                var request = _service.Events.List(calendarListEntry.Id);
                request.TimeMin = start;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Logger.Information($"Requesting all events in Google Calendar {calendarListEntry.Summary} from {start}.");

                var response = request.Execute();
                events.AddRange(Helpers.Helpers.GoogleEventParser(response.Items.ToList()));
            }
            Logger.Information("Google Calendar request was successful");

            return events;
        }

        /// <summary>
        /// Get events between two dates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Event> GetEvents(DateTime start, DateTime end)
        {
            var calendarList = GetCalendars();

            var events = new List<Event>();

            foreach (var calendarListEntry in calendarList)
            {
                var request = _service.Events.List(calendarListEntry.Id);
                request.TimeMin = start;
                request.TimeMax = end;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Logger.Information($"Requesting all events in Google Calendar {calendarListEntry.Summary} from {start} to {end}.");

                var response = request.Execute();
                events.AddRange(Helpers.Helpers.GoogleEventParser(response.Items.ToList()));
            }
            Logger.Information("Google Calendar request was successful");

            return events;
        }

        /// <summary>
        /// Create a new event
        /// </summary>
        /// <param name="eventObj"></param>
        /// <returns></returns>
        public Event InsertEvent(Event eventObj)
        {
            var calendarList = GetCalendars();
            var calendarId = calendarList.FirstOrDefault(i => i.Summary == eventObj.Category?.Name)?.Id ?? Constants.DefaultGoogleCalendarId;

            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Insert(googleEvent, calendarId);
            Logger.Information("Inserting {@0} into Google Calendar.", eventObj);

            var response = request.Execute();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful");

            return retval;
        }

        /// <summary>
        /// Update an existing event
        /// </summary>
        /// <param name="eventObj"></param>
        /// <returns></returns>
        public Event UpdateEvent(Event eventObj)
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Update(googleEvent, eventObj.Category?.Id ?? Constants.DefaultGoogleCalendarId, googleEvent.Id);
            Logger.Information("Updating {@0} in Google Calendar.", eventObj);

            var response = request.Execute();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful");

            return retval;
        }

        /// <summary>
        /// Update the category for an event. 
        /// </summary>
        /// <param name="eventId">Event to be updated</param>
        /// <param name="categoryId">Current category</param>
        /// <param name="newCategoryId">New category</param>
        /// <returns></returns>
        public Event UpdateEventCategory(string eventId, string categoryId, string newCategoryId)
        {
            var request = _service.Events.Move(categoryId, eventId, newCategoryId);
            Logger.Information("Changing Calendar for event with ID {Id} from {OldCalendar} to {NewCalendar}", eventId, categoryId, newCategoryId);

            var response = request.Execute();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful");

            return retval;
        }

        /// <summary>
        /// Delete a event.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        public void DeleteEvent(string calendarId, string eventId)
        {
            var request = _service.Events.Delete(calendarId, eventId);
            Logger.Information("Deleting event with {@0} in Google Calendar.", eventId);

            request.Execute();
        }

        /// <summary>
        /// Get an event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public async Task<Event> GetEventAsync(string id, string calendarId)
        {
            // Define parameters of request.
            var request = _service.Events.Get(calendarId, id);
            Logger.Information("Requesting event with id {0} in Google Calendar." + id);

            // Execute request to retrieve events
            var response = await request.ExecuteAsync();
            var events = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful");

            return events;
        }

        /// <summary>
        /// Get all events
        /// </summary>
        /// <returns></returns>
        public async Task<List<Event>> GetEventsAsync()
        {
            var calendarList = await GetCalendarsAsync();

            var events = new List<Event>();

            foreach (var calendarListEntry in calendarList)
            {
                var request = _service.Events.List(calendarListEntry.Id);
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Logger.Information($"Requesting all events in Google Calendar {calendarListEntry.Summary}.");

                var response = await request.ExecuteAsync();
                events.AddRange(Helpers.Helpers.GoogleEventParser(response.Items.ToList()));
            }
            Logger.Information("Google Calendar request was successful");

            return events;
        }

        /// <summary>
        /// Get events on a given date
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public async Task<List<Event>> GetEventsAsync(DateTime start)
        {
            var calendarList = await GetCalendarsAsync();

            var events = new List<Event>();

            foreach (var calendarListEntry in calendarList)
            {
                var request = _service.Events.List(calendarListEntry.Id);
                request.TimeMin = start;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Logger.Information($"Requesting all events in Google Calendar {calendarListEntry.Summary} from {start}.");

                var response = await request.ExecuteAsync();
                events.AddRange(Helpers.Helpers.GoogleEventParser(response.Items.ToList()));
            }
            Logger.Information("Google Calendar request was successful");

            return events;
        }

        /// <summary>
        /// Get events between two dates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<List<Event>> GetEventsAsync(DateTime start, DateTime end)
        {
            var calendarList = await GetCalendarsAsync();

            var events = new List<Event>();

            foreach (var calendarListEntry in calendarList)
            {
                var request = _service.Events.List(calendarListEntry.Id);
                request.TimeMin = start;
                request.TimeMax = end;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Logger.Information($"Requesting all events in Google Calendar {calendarListEntry.Summary} from {start} to {end}.");

                var response = await request.ExecuteAsync();
                events.AddRange(Helpers.Helpers.GoogleEventParser(response.Items.ToList()));
            }
            Logger.Information("Google Calendar request was successful");

            return events;
        }

        /// <summary>
        /// Create event
        /// </summary>
        /// <param name="eventObj"></param>
        /// <returns></returns>
        public async Task<Event> InsertEventAsync(Event eventObj)
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var calendarList = await GetCalendarsAsync();
            var calendarId = calendarList.FirstOrDefault(i => i.Summary == eventObj.Category?.Name)?.Id ?? Constants.DefaultGoogleCalendarId;

            var request = _service.Events.Insert(googleEvent, calendarId);
            Logger.Information("Inserting {@0} into Google Calendar.", eventObj);

            var response = await request.ExecuteAsync();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful");

            return retval;
        }

        /// <summary>
        /// Update exsisting event
        /// </summary>
        /// <param name="eventObj"></param>
        /// <returns></returns>
        public async Task<Event> UpdateEventAsync(Event eventObj)
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Update(googleEvent, eventObj.Category?.Id, googleEvent.Id);
            Logger.Information("Updating {@0} in Google Calendar.", eventObj);

            var response = await request.ExecuteAsync();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful");

            return retval;
        }

        /// <summary>
        /// Update the category for an event. 
        /// </summary>
        /// <param name="eventId">Event to be updated</param>
        /// <param name="categoryId">Current category</param>
        /// <param name="newCategoryId">New category</param>
        /// <returns></returns>
        public async Task<Event> UpdateEventCategoryAsync(string eventId, string categoryId, string newCategoryId)
        {
            var request = _service.Events.Move(categoryId, eventId, newCategoryId);
            Logger.Information("Changing Calendar for event with ID {Id} from {OldCalendar} to {NewCalendar}", eventId, categoryId, newCategoryId);

            var response = await request.ExecuteAsync();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful");

            return retval;
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task DeleteEventAsync(string calendarId, string eventId)
        {
            var request = _service.Events.Delete(calendarId, eventId);
            Logger.Information("Deleting event with {@0} in Google Calendar.", eventId);

            await request.ExecuteAsync();
        }

    }
}