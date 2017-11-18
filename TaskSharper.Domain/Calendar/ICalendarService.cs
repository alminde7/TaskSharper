using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Calendar
{
    public interface ICalendarService
    {
        Event GetEvent(string id, string calendarId);
        List<Event> GetEvents();
        List<Event> GetEvents(DateTime start);
        List<Event> GetEvents(DateTime start, DateTime end);
        Event InsertEvent(Event eventObj);
        Event UpdateEvent(Event eventObj);
        void DeleteEvent(string calendarId, string eventId);

        Task<Event> GetEventAsync(string id, string calendarId);
        Task<List<Event>> GetEventsAsync();
        Task<List<Event>> GetEventsAsync(DateTime start);
        Task<List<Event>> GetEventsAsync(DateTime start, DateTime end);
        Task<Event> InsertEventAsync(Event eventObj);
        Task<Event> UpdateEventAsync(Event eventObj);
        Task DeleteEventAsync(string calendarId, string eventId);

        List<EventCategory> GetCategories();
        Task<List<EventCategory>> GetCategoriesAsync();
        Event ChangeCategory(string eventId, string categoryId, string newCategoryId);
        Task<Event> ChangeCategoryAsync(string eventId, string categoryId, string newCategoryId);
    }
}
