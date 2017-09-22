using System;
using System.Collections.Generic;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
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

        public Event GetEvent(string id)
        {
            var calEvent = Cache.GetEvent(id);
            if (calEvent == null)
            {
                calEvent = CalendarService.GetEvent(id, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(calEvent);
            }

            return calEvent;
        }

        public Event GetEvent(string id, DateTime date)
        {
            var calEvent = Cache.GetEvent(id, date);
            if (calEvent == null)
            {
                calEvent = CalendarService.GetEvent(id, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(calEvent);
            }

            return calEvent;
        }

        public IList<Event> GetEvents(DateTime start)
        {
            var events = Cache.GetEvents(start);
            if (events == null)
            {
                events = CalendarService.GetEvents(start.Date, start.Date.AddDays(1).AddTicks(-1), Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, null);
            }
            
            return events;
        }

        public IList<Event> GetEvents(DateTime start, DateTime end)
        {
            var events = Cache.GetEvents(start, end);
            if (events == null)
            {
                events = CalendarService.GetEvents(start, end, Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, end);
            }
            
            return events;
        }

        public void UpdateCacheStore(DateTime start, DateTime end)
        {
            Cache.UpdateCacheStore(CalendarService.GetEvents(start, end, Constants.DefaultGoogleCalendarId), start, end);
        }
    }
}
