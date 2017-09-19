using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.CacheStore
{
    public interface ICacheStore
    {
        DateTime LastUpdated { get; }

        IList<Event> GetEvents(DateTime date);
        Event GetEvent(string id, DateTime date);
        Event GetEvent(string id);

        bool HasData(DateTime date);
        bool HasEvent(string id, DateTime date);
        bool HasEvent(string id);

        void UpdateCacheStore(IList<Event> events, DateTime fromDate, DateTime? toDate);
        void AddOrUpdateEvent(Event calendarEvent);


    }
}
