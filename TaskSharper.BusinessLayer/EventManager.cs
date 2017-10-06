using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.BusinessLayer
{
    public class EventManager : IEventManager
    {
        public ICalendarService CalendarService { get; }
        public ICacheStore Cache { get; }
        public INotification Notification { get; }
        public ILogger Logger { get; }

        public EventManager(ICalendarService calendarService, ICacheStore cache, INotification notification, ILogger logger)
        {
            CalendarService = calendarService;
            Cache = cache;
            Notification = notification;
            Logger = logger.ForContext<EventManager>();
        }
        
        public Event GetEvent(string id)
        {
            var calEvent = Cache.GetEvent(id);
            if (calEvent == null)
            {
                calEvent = CalendarService.GetEvent(id, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);
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
                Notification.Attach(calEvent);
            }

            return calEvent;
        }

        public IList<Event> GetEvents(DateTime start)
        {
            var events = Cache.GetEvents(start);
            if (events == null)
            {
                events = CalendarService.GetEvents(start.StartOfDay(), start.EndOfDay(), Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, null);
                Notification.Attach(events);
            }
            
            return events;
        }

        public IList<Event> GetEvents(DateTime start, DateTime end)
        {
            var events = Cache.GetEvents(start, end);
            if (events == null)
            {
                events = CalendarService.GetEvents(start.StartOfDay(), end.EndOfDay(), Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, end);
                Notification.Attach(events);
            }
            
            return events;
        }

        public Event UpdateEvent(Event eventObj)
        {
            try
            {
                var updatedEvent = CalendarService.UpdateEvent(eventObj, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(updatedEvent);
                Notification.Attach(updatedEvent);
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
            var events = CalendarService.GetEvents(start.StartOfDay(), end.EndOfDay(), Constants.DefaultGoogleCalendarId);
            Cache.UpdateCacheStore(events, start, end);
            Notification.Attach(events);
            Logger.Information("Cache has been updated with {@NrOfEvents} events from {@Start} to {@End}", events.Count, start, end);
        }

        public async Task<Event> GetEventAsync(string id)
        {
            var calEvent = Cache.GetEvent(id);
            if (calEvent == null)
            {
                calEvent = await CalendarService.GetEventAsync(id, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);
            }

            return calEvent;
        }

        public async Task<Event> GetEventAsync(string id, DateTime date)
        {
            var calEvent = Cache.GetEvent(id, date);
            if (calEvent == null)
            {
                calEvent = await CalendarService.GetEventAsync(id, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);
            }

            return calEvent;
        }

        public async Task<IList<Event>> GetEventsAsync(DateTime start)
        {
            var events = Cache.GetEvents(start);
            if (events == null)
            {
                events = await CalendarService.GetEventsAsync(start.StartOfDay(), start.EndOfDay(), Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, null);
                Notification.Attach(events);
            }

            return events;
        }

        public async Task<IList<Event>> GetEventsAsync(DateTime start, DateTime end)
        {
            var events = Cache.GetEvents(start, end);
            if (events == null)
            {
                events = await CalendarService.GetEventsAsync(start.StartOfDay(), end.EndOfDay(), Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, end);
                Notification.Attach(events);
            }

            return events;
        }

        public async Task<Event> UpdateEventAsync(Event eventObj)
        {
            try
            {
                var updatedEvent = await CalendarService.UpdateEventAsync(eventObj, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(updatedEvent);
                Notification.Attach(updatedEvent);
                return updatedEvent;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to update event with id {eventObj.Id}");
                return null;
            }
        }

        public async Task UpdateCacheStoreAsync(DateTime start, DateTime end)
        {
            var events = await CalendarService.GetEventsAsync(start.StartOfDay(), end.EndOfDay(), Constants.DefaultGoogleCalendarId);
            Cache.UpdateCacheStore(events, start, end);
            Notification.Attach(events);
            Logger.Information("Cache has been updated with {@NrOfEvents} events from {@Start} to {@End}", events.Count, start, end);
        }
    }
}
