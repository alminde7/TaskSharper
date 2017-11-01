using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Domain.ServerEvents;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.BusinessLayer
{
    public class EventManager : IEventManager
    {
        private readonly INotificationPublisher _notificationPublisher;
        public ICalendarService CalendarService { get; }
        public ICacheStore Cache { get; }
        public INotification Notification { get; }
        public ILogger Logger { get; set; }

        public EventManager(ICalendarService calendarService, ICacheStore cache, INotification notification, ILogger logger, INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
            CalendarService = calendarService;
            Cache = cache;
            Notification = notification;
            Logger = logger.ForContext<EventManager>();
        }

        #region Synchronous methods
        public Event GetEvent(string id)
        {
            var calEvent = Cache.GetEvent(id);
            if (calEvent == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                calEvent = CalendarService.GetEvent(id, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return calEvent;
        }

        public Event GetEvent(string id, DateTime date)
        {
            var calEvent = Cache.GetEvent(id, date);
            if (calEvent == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                calEvent = CalendarService.GetEvent(id, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return calEvent;
        }

        public IList<Event> GetEvents(DateTime start)
        {
            var events = Cache.GetEvents(start);
            if (events == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                events = CalendarService.GetEvents(start.StartOfDay(), start.EndOfDay(), Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, null);
                Notification.Attach(events);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return events;
        }

        public IList<Event> GetEvents(DateTime start, DateTime end)
        {
            var events = Cache.GetEvents(start, end);
            if (events == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                events = CalendarService.GetEvents(start.StartOfDay(), end.EndOfDay(), Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, end);
                Notification.Attach(events);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return events;
        }

        public Event UpdateEvent(Event eventObj)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var updatedEvent = CalendarService.UpdateEvent(eventObj, Constants.DefaultGoogleCalendarId);
            Cache.AddOrUpdateEvent(updatedEvent);
            Notification.Attach(updatedEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return updatedEvent;
        }

        public void UpdateCacheStore(DateTime start, DateTime end)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var events = CalendarService.GetEvents(start.StartOfDay(), end.EndOfDay(), Constants.DefaultGoogleCalendarId);
            Cache.UpdateCacheStore(events, start, end);
            Notification.Attach(events);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            Logger.Information("Cache has been updated with {@NrOfEvents} events from {@Start} to {@End}", events.Count, start, end);

        }
        #endregion

        #region Asynchronous methods
        public async Task<Event> GetEventAsync(string id)
        {
            var calEvent = Cache.GetEvent(id);
            if (calEvent == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                calEvent = await CalendarService.GetEventAsync(id, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return calEvent;
        }

        public async Task<Event> GetEventAsync(string id, DateTime date)
        {
            var calEvent = Cache.GetEvent(id, date);
            if (calEvent == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                calEvent = await CalendarService.GetEventAsync(id, Constants.DefaultGoogleCalendarId);
                Cache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return calEvent;
        }

        public async Task<IList<Event>> GetEventsAsync(DateTime start)
        {
            var events = Cache.GetEvents(start);
            if (events == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                events = await CalendarService.GetEventsAsync(start.StartOfDay(), start.EndOfDay(), Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, null);
                Notification.Attach(events);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return events;
        }

        public async Task<IList<Event>> GetEventsAsync(DateTime start, DateTime end)
        {
            var events = Cache.GetEvents(start, end);
            if (events == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                events = await CalendarService.GetEventsAsync(start.StartOfDay(), end.EndOfDay(),
                    Constants.DefaultGoogleCalendarId);
                Cache.UpdateCacheStore(events, start, end);
                Notification.Attach(events);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return events;
        }

        public async Task<Event> UpdateEventAsync(Event eventObj)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var updatedEvent = await CalendarService.UpdateEventAsync(eventObj, Constants.DefaultGoogleCalendarId);
            Cache.AddOrUpdateEvent(updatedEvent);
            Notification.Attach(updatedEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return updatedEvent;
        }

        public async Task UpdateCacheStoreAsync(DateTime start, DateTime end)
        {
            var events = await CalendarService.GetEventsAsync(start.StartOfDay(), end.EndOfDay(), Constants.DefaultGoogleCalendarId);
            Cache.UpdateCacheStore(events, start, end);
            Notification.Attach(events);
            Logger.Information("Cache has been updated with {@NrOfEvents} events from {@Start} to {@End}", events.Count, start, end);

        }

        public async Task DeleteEventAsync(string id)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            await CalendarService.DeleteEventAsync(id, Constants.DefaultGoogleCalendarId);
            Cache.RemoveEvent(id);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
        }

        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var createdEvent = await CalendarService.InsertEventAsync(newEvent, Constants.DefaultGoogleCalendarId);
            Cache.AddOrUpdateEvent(createdEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return createdEvent;
        }
        #endregion
    }
}
