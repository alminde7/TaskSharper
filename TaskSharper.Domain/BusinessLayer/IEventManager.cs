using System;
using System.Collections.Generic;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.BusinessLayer
{
    public interface IEventManager
    {
        IList<Event> GetEvents(DateTime start, DateTime end);
        void UpdateCacheStore(DateTime start, DateTime end);
    }
}
