using System;
using System.Collections.Generic;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.Cache
{
    public interface ICacheStore
    {
        DateTime LastUpdated { get; }
        TimeSpan UpdatedOffset { get; set; }

        IList<Event> GetEvents(DateTime date);
        IList<Event> GetEvents(DateTime start, DateTime end);
        Event GetEvent(string id, DateTime date);
        Event GetEvent(string id);

        bool HasData(DateTime date);
        bool HasEvent(string id, DateTime date);
        bool HasEvent(string id);

        void UpdateCacheStore(IList<Event> events, DateTime fromDate, DateTime? toDate);
        void AddOrUpdateEvent(Event calendarEvent);
    }
}
