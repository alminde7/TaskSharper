using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Serilog;
using TaskSharper.DataAccessLayer.Google.Authentication;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.DataAccessLayer.Google.Calendar.Service
{
    public class GoogleCalendarService : ICalendarService
    {
        private readonly CalendarService _service;
        private readonly ILogger _logger;

        public GoogleCalendarService(CalendarService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public List<Event> GetEvents(string calendarId)
        {
            // Define parameters of request.
            var request = _service.Events.List(calendarId);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            _logger.Information("Requesting all events in Google Calendar.");

            // Execute request to retrieve events
            var response = request.Execute();
            var events = Helpers.Helpers.GoogleEventParser(response.Items.ToList());
            _logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

        public List<Event> GetEvents(DateTime start, string calendarId)
        {
            var request = _service.Events.List(calendarId);
            request.TimeMin = start;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            _logger.Information($"Requesting all events in Google Calendar from {start}.");

            var response = request.Execute();
            var events = Helpers.Helpers.GoogleEventParser(response.Items.ToList());
            _logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

        public List<Event> GetEvents(DateTime start, DateTime end, string calendarId)
        {
            var request = _service.Events.List(calendarId);
            request.TimeMin = start;
            request.TimeMax = end;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            _logger.Information($"Requesting all events in Google Calendar from {start} to {end}.");

            var response = request.Execute();
            var events = Helpers.Helpers.GoogleEventParser(response.Items.ToList());
            _logger.Information("Google Calendar request was successful and returned {@0}", events);

            return events;
        }

        public Event InsertEvent(Event eventObj, string calendarId)
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Insert(googleEvent, calendarId);
            _logger.Information("Inserting {@0} into Google Calendar.", eventObj);

            var response = request.Execute();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            _logger.Information("Google Calendar request was successful and returned {@0}", retval);

            return retval;
        }

        public Event UpdateEvent(Event eventObj, string calendarId)
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Update(googleEvent, calendarId, googleEvent.Id);
            _logger.Information("Updating {@0} in Google Calendar.", eventObj);

            var response = request.Execute();
            var retval = Helpers.Helpers.GoogleEventParser(response);
            _logger.Information("Google Calendar request was successful and returned {@0}", retval);

            return retval;
        }

        public void DeleteEvent(string calendarId, string eventId)
        {
            var request = _service.Events.Delete(calendarId, eventId);
            _logger.Information("Deleting event with {@0} in Google Calendar.", eventId);

            request.Execute();
        }
    }
}