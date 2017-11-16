using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Serilog;
using TaskSharper.Domain.Calendar;
using Event = TaskSharper.Domain.Calendar.Event;

namespace TaskSharper.DataAccessLayer.Google.Calendar.Service
{
    public class GoogleCalendarService : ICalendarService
    {
        private readonly CalendarService _service;
        public ILogger Logger { get; set; }

        public GoogleCalendarService(CalendarService service, ILogger logger)
        {
            _service = service;
            Logger = logger.ForContext<GoogleApiException>();
        }

        public List<CalendarListEntry> GetCalendars()
        {
            // Define request
            var request = _service.CalendarList.List();
            Logger.Information("Requesting all calendars in Google Calendar");

            // Execute request to retrieve calendars
            var response = request.Execute();
            Logger.Information("Google Calendar request was successful and returned {@0}", response);

            var activeCalendars = response.Items.Where(i => i.Selected == true).ToList();

            return activeCalendars;
        }

        public Event GetEvent(string id, string calendarId)
        {
            // Define parameters of request.
            var request = _service.Events.Get(calendarId, id);
            Logger.Information("Requesting event with id {0} in Google Calendar." + id);

            // Execute request to retrieve events
            var response = request.Execute();
            var events = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

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
            Logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

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
            Logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

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
            Logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

        public Event InsertEvent(Event eventObj)
        {
            var calendarList = GetCalendars();
            var calendarId = calendarList.FirstOrDefault(i => i.Summary == eventObj.Category?.Name)?.Id ?? Constants.DefaultGoogleCalendarId;

            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Insert(googleEvent, calendarId);
            Logger.Information("Inserting {@0} into Google Calendar.", eventObj);

            var response = request.Execute();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful and returned {@0}", retval);

            return retval;
        }

        public Event UpdateEvent(Event eventObj)
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Update(googleEvent, eventObj.Category?.Id ?? Constants.DefaultGoogleCalendarId, googleEvent.Id);
            Logger.Information("Updating {@0} in Google Calendar.", eventObj);

            var response = request.Execute();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful and returned {@0}", retval);

            return retval;
        }

        public void DeleteEvent(string calendarId, string eventId)
        {
            var request = _service.Events.Delete(calendarId, eventId);
            Logger.Information("Deleting event with {@0} in Google Calendar.", eventId);

            request.Execute();
        }

        public async Task<List<CalendarListEntry>> GetCalendarsAsync()
        {
            // Define request
            var request = _service.CalendarList.List();
            Logger.Information("Requesting all calendars in Google Calendar");

            // Execute request to retrieve calendars
            var response = await request.ExecuteAsync();
            Logger.Information("Google Calendar request was successful and returned {@0}", response);

            var activeCalendars = response.Items.Where(i => i.Selected == true).ToList();

            return activeCalendars;
        }

        public async Task<Event> GetEventAsync(string id, string calendarId)
        {
            // Define parameters of request.
            var request = _service.Events.Get(calendarId, id);
            Logger.Information("Requesting event with id {0} in Google Calendar." + id);

            // Execute request to retrieve events
            var response = await request.ExecuteAsync();
            var events = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

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
            Logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

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
            Logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

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
            Logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

        public async Task<Event> InsertEventAsync(Event eventObj)
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var calendarList = await GetCalendarsAsync();
            var calendarId = calendarList.FirstOrDefault(i => i.Summary == eventObj.Category?.Name)?.Id ?? Constants.DefaultGoogleCalendarId;

            var request = _service.Events.Insert(googleEvent, calendarId);
            Logger.Information("Inserting {@0} into Google Calendar.", eventObj);

            var response = await request.ExecuteAsync();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful and returned {@0}", retval);

            return retval;
        }

        public async Task<Event> UpdateEventAsync(Event eventObj)
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Update(googleEvent, eventObj.Category?.Id, googleEvent.Id);
            Logger.Information("Updating {@0} in Google Calendar.", eventObj);

            var response = await request.ExecuteAsync();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            Logger.Information("Google Calendar request was successful and returned {@0}", retval);

            return retval;
        }

        public async Task DeleteEventAsync(string calendarId, string eventId)
        {
            var request = _service.Events.Delete(calendarId, eventId);
            Logger.Information("Deleting event with {@0} in Google Calendar.", eventId);

            await request.ExecuteAsync();
        }

        public List<EventCategory> GetCategories()
        {
            return GetCalendars().Select(calendarListEntry => new EventCategory {Id = calendarListEntry.Id, Name = calendarListEntry.Summary}).ToList();
        }

        public async Task<List<EventCategory>> GetCategoriesAsync()
        {
            var calendarList = await GetCalendarsAsync();
            return calendarList.Select(calendarListEntry => new EventCategory { Id = calendarListEntry.Id, Name = calendarListEntry.Summary }).ToList();
        }
    }
}