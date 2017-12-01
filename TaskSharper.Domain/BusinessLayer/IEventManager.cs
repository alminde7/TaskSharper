using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.BusinessLayer
{
    public interface IEventManager
    {
        Task<Event> GetEventAsync(string id);
        Task<Event> GetEventAsync(string id, DateTime date);
        Task<IList<Event>> GetEventsAsync(DateTime start);
        Task<IList<Event>> GetEventsAsync(DateTime start, DateTime end);
        Task<Event> UpdateEventAsync(Event eventObj);
        Task UpdateCacheStoreAsync(DateTime start, DateTime end);

        Task DeleteEventAsync(string id, string calendarId);
        Task<Event> CreateEventAsync(Event newEvent);

        IList<EventCategory> GetCategories();
        Task<IList<EventCategory>> GetCategoriesAsync();
    }
}
