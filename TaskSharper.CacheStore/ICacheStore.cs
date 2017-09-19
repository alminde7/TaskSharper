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
        IList<Event> GetEvents(DateTime date);

        bool HasData(DateTime date);

        void UpdateCacheStore(IList<Event> events, DateTime fromDate, DateTime? toDate);
        
    }
}
