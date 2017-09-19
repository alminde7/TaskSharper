using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.BusinessLayer.Interfaces;
using TaskSharper.CacheStore;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.BusinessLayer
{
    public class EventManager : IEventManager
    {
        public ICalendarService CalendarService { get; }
        public ICacheStore Cache { get; }

        public EventManager(ICalendarService calendarService, ICacheStore cache)
        {
            CalendarService = calendarService;
            Cache = cache;
        }

        //public Event GetEvent(string id)
        //{
        //}

        public IList<Event> GetEvents(DateTime start, DateTime end)
        {
            if (Cache.HasData(start))
            {
                return Cache.GetEvents(start);
            }
            else
            {
                var events = CalendarService.GetEvents(start, end, Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, end);
                return Cache.GetEvents(start);
            }
        }

        public void UpdateCacheStore(DateTime start, DateTime end)
        {
            Cache.UpdateCacheStore(CalendarService.GetEvents(start, end, Constants.DefaultGoogleCalendarId), start, end);
        }
    }
}
