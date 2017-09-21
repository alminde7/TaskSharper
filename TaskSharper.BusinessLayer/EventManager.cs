using System;
using System.Collections.Generic;
using Serilog;
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
        public ILogger Logger { get; }

        public EventManager(ICalendarService calendarService, ICacheStore cache, ILogger logger)
        {
            CalendarService = calendarService;
            Cache = cache;
            Logger = logger;
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
                events = CalendarService.GetEvents(start, start.Date.AddDays(1).AddTicks(-1), Constants.DefaultGoogleCalendarId);
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

        public Event UpdateEvent(Event eventObj)
        {
            try
            {
                var updatedEvent = CalendarService.UpdateEvent(eventObj, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(updatedEvent);
                return updatedEvent;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to update event with id {eventObj.Id}");
                return null;
            }
        }

        public void UpdateCacheStore(DateTime start, DateTime end)
        {
            Cache.UpdateCacheStore(CalendarService.GetEvents(start, end, Constants.DefaultGoogleCalendarId), start, end);
        }
    }
}
