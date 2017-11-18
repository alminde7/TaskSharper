﻿using System;
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
        public IEventCache EventCache { get; }
        public IEventCategoryCache EventCategoryCache { get; }
        public INotification Notification { get; }
        public ILogger Logger { get; set; }

        public EventManager(ICalendarService calendarService, IEventCache cache, INotification notification, ILogger logger, INotificationPublisher notificationPublisher, IEventCategoryCache categoryCache)
        {
            _notificationPublisher = notificationPublisher;
            CalendarService = calendarService;
            EventCache = cache;
            Notification = notification;
            EventCategoryCache = categoryCache;
            Logger = logger.ForContext<EventManager>();
        }

        #region Synchronous methods
        public Event GetEvent(string id)
        {
            var calEvent = EventCache.GetEvent(id);
            if (calEvent == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                calEvent = CalendarService.GetEvent(id, Constants.DefaultGoogleCalendarId);
                EventCache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return calEvent;
        }

        public Event GetEvent(string id, DateTime date)
        {
            var calEvent = EventCache.GetEvent(id, date);
            if (calEvent == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                calEvent = CalendarService.GetEvent(id, Constants.DefaultGoogleCalendarId);
                EventCache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return calEvent;
        }

        public IList<Event> GetEvents(DateTime start)
        {
            var events = EventCache.GetEvents(start);
            if (events == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                events = CalendarService.GetEvents(start.StartOfDay(), start.EndOfDay());
                EventCache.UpdateCacheStore(events, start, null);
                Notification.Attach(events);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return events;
        }

        public IList<Event> GetEvents(DateTime start, DateTime end)
        {
            var events = EventCache.GetEvents(start, end);
            if (events == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                events = CalendarService.GetEvents(start.StartOfDay(), end.EndOfDay());
                EventCache.UpdateCacheStore(events, start, end);
                Notification.Attach(events);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return events;
        }

        public Event UpdateEvent(Event eventObj)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var updatedEvent = CalendarService.UpdateEvent(eventObj);
            EventCache.AddOrUpdateEvent(updatedEvent);
            Notification.Attach(updatedEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return updatedEvent;
        }

        public void UpdateCacheStore(DateTime start, DateTime end)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var events = CalendarService.GetEvents(start.StartOfDay(), end.EndOfDay());
            EventCache.UpdateCacheStore(events, start, end);
            Notification.Attach(events);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            Logger.Information("Cache has been updated with {@NrOfEvents} events from {@Start} to {@End}", events.Count, start, end);

        }
        #endregion

        #region Asynchronous methods
        public async Task<Event> GetEventAsync(string id)
        {
            var calEvent = EventCache.GetEvent(id);
            if (calEvent == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                calEvent = await CalendarService.GetEventAsync(id, Constants.DefaultGoogleCalendarId);
                EventCache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return calEvent;
        }

        public async Task<Event> GetEventAsync(string id, DateTime date)
        {
            var calEvent = EventCache.GetEvent(id, date);
            if (calEvent == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                calEvent = await CalendarService.GetEventAsync(id, Constants.DefaultGoogleCalendarId);
                EventCache.AddOrUpdateEvent(calEvent);
                Notification.Attach(calEvent);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return calEvent;
        }

        public async Task<IList<Event>> GetEventsAsync(DateTime start)
        {
            var events = EventCache.GetEvents(start);
            if (events == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                events = await CalendarService.GetEventsAsync(start.StartOfDay(), start.EndOfDay());
                EventCache.UpdateCacheStore(events, start, null);
                Notification.Attach(events);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return events;
        }

        public async Task<IList<Event>> GetEventsAsync(DateTime start, DateTime end)
        {
            var events = EventCache.GetEvents(start, end);
            if (events == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());

                events = await CalendarService.GetEventsAsync(start.StartOfDay(), end.EndOfDay());
                EventCache.UpdateCacheStore(events, start, end);
                Notification.Attach(events);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return events;
        }

        public async Task<Event> UpdateEventAsync(Event eventObj)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var oldEvent = await GetEventAsync(eventObj.Id);
            if (oldEvent.Category.Id != eventObj.Category.Id)
            {
                await CalendarService.ChangeCategoryAsync(eventObj.Id, oldEvent.Category.Id, eventObj.Category.Id);
            }

            var updatedEvent = await CalendarService.UpdateEventAsync(eventObj);
            EventCache.AddOrUpdateEvent(updatedEvent);
            Notification.Attach(updatedEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return updatedEvent;
        }

        public async Task UpdateCacheStoreAsync(DateTime start, DateTime end)
        {
            var events = await CalendarService.GetEventsAsync(start.StartOfDay(), end.EndOfDay());
            EventCache.UpdateCacheStore(events, start, end);
            Notification.Attach(events);
            Logger.Information("Cache has been updated with {@NrOfEvents} events from {@Start} to {@End}", events.Count, start, end);

        }

        public async Task DeleteEventAsync(string id, string calendarId)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            await CalendarService.DeleteEventAsync(calendarId, id);
            EventCache.RemoveEvent(id);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
        }

        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var createdEvent = await CalendarService.InsertEventAsync(newEvent);
            EventCache.AddOrUpdateEvent(createdEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return createdEvent;
        }

        public IList<EventCategory> GetCategories()
        {
            var categories = EventCategoryCache.GetEventCategories();

            if (categories == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());
                categories = CalendarService.GetCategories();

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return categories;
        }

        public async Task<IList<EventCategory>> GetCategoriesAsync()
        {
            var categories = EventCategoryCache.GetEventCategories();

            if (categories == null)
            {
                _notificationPublisher.Publish(new GettingExternalDataEvent());
                categories = await CalendarService.GetCategoriesAsync();

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return categories;
        }

        #endregion
    }
}
