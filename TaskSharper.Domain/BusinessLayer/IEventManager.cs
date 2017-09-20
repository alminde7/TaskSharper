using System;
using System.Collections.Generic;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.BusinessLayer
{
    public interface IEventManager
    {
        Event GetEvent(string id);
        Event GetEvent(string id, DateTime date);
        IList<Event> GetEvents(DateTime start);
        IList<Event> GetEvents(DateTime start, DateTime end);
        void UpdateCacheStore(DateTime start, DateTime end);
    }
}
