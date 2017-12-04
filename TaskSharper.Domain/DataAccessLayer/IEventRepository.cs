using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskSharper.Domain.Models;

namespace TaskSharper.Domain.Calendar
{
    public interface IEventRepository
    {
        Event GetEvent(string id, string calendarId);
        List<Event> GetEvents();
        List<Event> GetEvents(DateTime start);
        List<Event> GetEvents(DateTime start, DateTime end);
        Event InsertEvent(Event eventObj);
        Event UpdateEvent(Event eventObj);
        Event UpdateEventCategory(string eventId, string categoryId, string newCategoryId);
        void DeleteEvent(string calendarId, string eventId);

        Task<Event> GetEventAsync(string id, string calendarId);
        Task<List<Event>> GetEventsAsync();
        Task<List<Event>> GetEventsAsync(DateTime start);
        Task<List<Event>> GetEventsAsync(DateTime start, DateTime end);
        Task<Event> InsertEventAsync(Event eventObj);
        Task<Event> UpdateEventAsync(Event eventObj);
        Task<Event> UpdateEventCategoryAsync(string eventId, string categoryId, string newCategoryId);
        Task DeleteEventAsync(string calendarId, string eventId);
    }
}
