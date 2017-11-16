using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.BusinessLayer
{
    public interface IEventManager
    {
        Event GetEvent(string id);
        Event GetEvent(string id, DateTime date);
        IList<Event> GetEvents(DateTime start);
        IList<Event> GetEvents(DateTime start, DateTime end);
        Event UpdateEvent(Event eventObj);
        void UpdateCacheStore(DateTime start, DateTime end);

        Task<Event> GetEventAsync(string id);
        Task<Event> GetEventAsync(string id, DateTime date);
        Task<IList<Event>> GetEventsAsync(DateTime start);
        Task<IList<Event>> GetEventsAsync(DateTime start, DateTime end);
        Task<Event> UpdateEventAsync(Event eventObj);
        Task UpdateCacheStoreAsync(DateTime start, DateTime end);

        Task DeleteEventAsync(string id, string calendarId);
        Task<Event> CreateEventAsync(Event newEvent);

        List<EventCategory> GetCategories();
        Task<List<EventCategory>> GetCategoriesAsync();
    }
}
