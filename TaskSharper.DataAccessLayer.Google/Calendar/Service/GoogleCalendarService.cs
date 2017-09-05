using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.DataAccessLayer.Calendar.Service
{
    public class GoogleCalendarService : BaseService, ICalendarService
    {
        private readonly CalendarService _service;
        public GoogleCalendarService()
        {
            Authenticate();
            _service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = base.UserCredential,
                ApplicationName = "TaskSharper"
            });
        }

        public List<Domain.Calendar.Event> GetEvents(string calendarId = "primary")
        {
            // Define parameters of request.
            var request = _service.Events.List(calendarId);
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // Execute request to retrieve events
            var events = request.Execute();

            return Helpers.Helpers.GoogleEventParser(events.Items.ToList());
        }

        public List<Domain.Calendar.Event> GetEvents(DateTime start, string calendarId = "primary")
        {
            var request = _service.Events.List(calendarId);
            request.TimeMin = start;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = request.Execute();

            return Helpers.Helpers.GoogleEventParser(events.Items.ToList());
        }

        public List<Domain.Calendar.Event> GetEvents(DateTime start, DateTime end, string calendarId = "primary")
        {
            var request = _service.Events.List(calendarId);
            request.TimeMin = start;
            request.TimeMax = end;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = request.Execute();

            return Helpers.Helpers.GoogleEventParser(events.Items.ToList());
        }

        public Domain.Calendar.Event InsertEvent(Domain.Calendar.Event eventObj, string calendarId = "primary")
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Insert(googleEvent, calendarId);
            var createdEvent = request.Execute();

            return Helpers.Helpers.GoogleEventParser(createdEvent);
        }
    }
}