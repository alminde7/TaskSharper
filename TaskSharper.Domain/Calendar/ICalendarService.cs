using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Calendar
{
    public interface ICalendarService
    {
        Event GetEvent(string id, string calendarId);
        List<Event> GetEvents(string calendarId);
        List<Event> GetEvents(DateTime start, string calendarId);
        List<Event> GetEvents(DateTime start, DateTime end, string calendarId);
        Event InsertEvent(Event eventObj, string calendarId);
        Event UpdateEvent(Event eventObj, string calendarId);
        void DeleteEvent(string calendarId, string eventId);

        Task<Event> GetEventAsync(string id, string calendarId);
        Task<List<Event>> GetEventsAsync(string calendarId);
        Task<List<Event>> GetEventsAsync(DateTime start, string calendarId);
        Task<List<Event>> GetEventsAsync(DateTime start, DateTime end, string calendarId);
        Task<Event> InsertEventAsync(Event eventObj, string calendarId);
        Task<Event> UpdateEventAsync(Event eventObj, string calendarId);
        Task DeleteEventAsync(string calendarId, string eventId);
    }
}
