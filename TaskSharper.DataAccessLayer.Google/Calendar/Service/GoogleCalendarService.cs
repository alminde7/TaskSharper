using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.DataAccessLayer.Google.Calendar.Service
{
    public class GoogleCalendarService : BaseService, ICalendarService
    {
        private readonly CalendarService _service;
        public GoogleCalendarService(CalendarService service = null)
        {
            Authenticate();
            if (service == null)
            {
                _service = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = UserCredential,
                    ApplicationName = "TaskSharper"
                });
            }
            else
            {
                _service = service;
            }
        }

        public List<Event> GetEvents(string calendarId = "primary")
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

        public List<Event> GetEvents(DateTime start, string calendarId = "primary")
        {
            var request = _service.Events.List(calendarId);
            request.TimeMin = start;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = request.Execute();

            return Helpers.Helpers.GoogleEventParser(events.Items.ToList());
        }

        public List<Event> GetEvents(DateTime start, DateTime end, string calendarId = "primary")
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

        public Event InsertEvent(Event eventObj, string calendarId = "primary")
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Insert(googleEvent, calendarId);
            var response = request.Execute();

            return Helpers.Helpers.GoogleEventParser(response);
        }

        public Event UpdateEvent(Event eventObj, string calendarId)
        {
            var googleEvent = Helpers.Helpers.GoogleEventParser(eventObj);

            var request = _service.Events.Update(googleEvent, calendarId, googleEvent.Id);
            var response = request.Execute();

            return Helpers.Helpers.GoogleEventParser(response);
        }

        public void DeleteEvent(string calendarId, string eventId)
        {
            var request = _service.Events.Delete(calendarId, eventId);
            request.Execute();
        }
    }
}