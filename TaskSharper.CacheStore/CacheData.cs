using System;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.CacheStore
{
    public class CacheData<T>
    {
        public T Event { get; set; }
        public DateTime Updated { get; set; }
        public bool ForceUpdate { get; set; }

        public CacheData(T caleEvent, DateTime updated, bool forceUpdate)
        {
            Event = caleEvent;
            Updated = updated;
            ForceUpdate = forceUpdate;
        }
    }
}