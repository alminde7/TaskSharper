using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Calendar
{
    public interface IEventRestClient
    {
        Event Get(string id);
        Task<Event> GetAsync(string id);

        Task<IEnumerable<Event>> GetAsync(DateTime date);

        Task<IEnumerable<Event>> GetAsync(DateTime from, DateTime to);

        Task<Event> CreateAsync(Event newEvent);

        Task<Event> UpdateAsync(Event updatedEvent);

        Task DeleteAsync(string id, string calendarId);

        Task<IEnumerable<EventCategory>> GetAsync();
    }
}
