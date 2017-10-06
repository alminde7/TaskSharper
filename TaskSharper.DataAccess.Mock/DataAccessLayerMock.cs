using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.DataAccess.Mock
{
    public class DataAccessLayerMock : ICalendarService
    {
        public Event GetEvent(string id, string calendarId)
        {
            return new Event();
        }

        public List<Event> GetEvents(string calendarId)
        {
            return new List<Event>();
        }

        public List<Event> GetEvents(DateTime start, string calendarId)
        {
            return new List<Event>();
        }

        public List<Event> GetEvents(DateTime start, DateTime end, string calendarId)
        {
            return new List<Event>();
        }

        public Event InsertEvent(Event eventObj, string calendarId)
        {
            return new Event();
        }

        public Event UpdateEvent(Event eventObj, string calendarId)
        {
            return new Event();
        }

        public void DeleteEvent(string calendarId, string eventId)
        {
            
        }

        public Task<Event> GetEventAsync(string id, string calendarId)
        {
            return new Task<Event>(() => new Event());
        }

        public Task<List<Event>> GetEventsAsync(string calendarId)
        {
            return new Task<List<Event>>(() => new List<Event>());
        }

        public Task<List<Event>> GetEventsAsync(DateTime start, string calendarId)
        {
            return new Task<List<Event>>(() => new List<Event>());
        }

        public Task<List<Event>> GetEventsAsync(DateTime start, DateTime end, string calendarId)
        {
            return new Task<List<Event>>(() => new List<Event>());
        }

        public Task<Event> InsertEventAsync(Event eventObj, string calendarId)
        {
            return new Task<Event>(() => new Event());
        }

        public Task<Event> UpdateEventAsync(Event eventObj, string calendarId)
        {
            return new Task<Event>(()=> new Event());
        }

        public Task DeleteEventAsync(string calendarId, string eventId)
        {
            return Task.CompletedTask;
        }
    }
}
