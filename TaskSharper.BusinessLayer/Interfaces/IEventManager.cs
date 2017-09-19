using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.BusinessLayer.Interfaces
{
    public interface IEventManager
    {
        IList<Event> GetEvents(DateTime start, DateTime end);
        void UpdateCacheStore(DateTime start, DateTime end);
    }
}
