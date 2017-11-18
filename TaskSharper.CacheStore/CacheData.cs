using System;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.CacheStore
{
    public class CacheData<T>
    {
        public T Data { get; set; }
        public DateTime Updated { get; set; }
        public bool ForceUpdate { get; set; }

        public CacheData(T data, DateTime updated, bool forceUpdate)
        {
            Data = data;
            Updated = updated;
            ForceUpdate = forceUpdate;
        }
    }
}