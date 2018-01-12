using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskSharper.Domain.Models;

namespace TaskSharper.Domain.RestClient
{
    public interface IEventRestClient
    {
        Event Get(string id, string calendarId);
        Task<Event> GetAsync(string id, string calendarId);

        Task<IEnumerable<Event>> GetAsync(DateTime date);

        Task<IEnumerable<Event>> GetAsync(DateTime from, DateTime to);

        Task<Event> CreateAsync(Event newEvent);

        Task<Event> UpdateAsync(Event updatedEvent);

        Task DeleteAsync(string id, string calendarId);

        Task<IEnumerable<EventCategory>> GetAsync();
    }
}
