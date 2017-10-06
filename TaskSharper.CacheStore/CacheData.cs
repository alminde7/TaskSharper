using System;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.CacheStore
{
    public class CacheData
    {
        public Event Event { get; set; }
        public DateTime Updated { get; set; }
        public bool ForceUpdate { get; set; }

        public CacheData(Event caleEvent, DateTime updated, bool forceUpdate)
        {
            Event = caleEvent;
            Updated = updated;
            ForceUpdate = forceUpdate;
        }
    }
}