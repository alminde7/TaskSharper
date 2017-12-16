using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.DataAccessLayer;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.Notification;
using TaskSharper.Domain.ServerEvents;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.BusinessLayer
{
    public class EventManager : IEventManager
    {
        private readonly INotificationPublisher _notificationPublisher;
        public IEventRepository EventRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IEventCache EventCache { get; }
        public IEventCategoryCache EventCategoryCache { get; }
        public INotification Notification { get; }
        public ILogger Logger { get; set; }

        public EventManager(IEventRepository eventRepository, ICategoryRepository categoryRepository, IEventCategoryCache categoryCache, IEventCache cache, INotification notification, INotificationPublisher notificationPublisher, ILogger logger)
        {
            _notificationPublisher = notificationPublisher;
            EventRepository = eventRepository;
            CategoryRepository = categoryRepository;
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

                calEvent = EventRepository.GetEvent(id, Constants.DefaultGoogleCalendarId);
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

                calEvent = EventRepository.GetEvent(id, Constants.DefaultGoogleCalendarId);
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

                events = EventRepository.GetEvents(start.StartOfDay(), start.EndOfDay());
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

                events = EventRepository.GetEvents(start.StartOfDay(), end.EndOfDay());
                EventCache.UpdateCacheStore(events, start, end);
                Notification.Attach(events);

                _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            }

            return events;
        }

        public Event UpdateEvent(Event eventObj)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var updatedEvent = EventRepository.UpdateEvent(eventObj);
            EventCache.AddOrUpdateEvent(updatedEvent);
            Notification.Attach(updatedEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return updatedEvent;
        }

        public void UpdateCacheStore(DateTime start, DateTime end)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var events = EventRepository.GetEvents(start.StartOfDay(), end.EndOfDay());
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

            if (calEvent != null)
            {
                Logger.Information($"Returning event from cache");
                return calEvent;
            }
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            calEvent = await EventRepository.GetEventAsync(id, Constants.DefaultGoogleCalendarId);
            EventCache.AddOrUpdateEvent(calEvent);
            Notification.Attach(calEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());

            Logger.Information("Returning event from external source");
            return calEvent;
        }

        public async Task<Event> GetEventAsync(string id, DateTime date)
        {
            var calEvent = EventCache.GetEvent(id, date);
            if (calEvent != null)
            {
                Logger.Information($"Returning event from cache");
                return calEvent;
            }
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            calEvent = await EventRepository.GetEventAsync(id, Constants.DefaultGoogleCalendarId);
            EventCache.AddOrUpdateEvent(calEvent);
            Notification.Attach(calEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());

            Logger.Information("Returning event from external source");
            return calEvent;
        }

        public async Task<IList<Event>> GetEventsAsync(DateTime start)
        {
            var events = EventCache.GetEvents(start);
            if (events != null)
            {
                Logger.Information($"Returning {events.Count} events from cache");
                return events;
            }
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            events = await EventRepository.GetEventsAsync(start.StartOfDay(), start.EndOfDay());
            EventCache.UpdateCacheStore(events, start, null);
            Notification.Attach(events);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());

            Logger.Information($"Returning {events.Count} events from external source");
            return events;
        }

        public async Task<IList<Event>> GetEventsAsync(DateTime start, DateTime end)
        {
            var events = EventCache.GetEvents(start, end);
            if (events != null)
            {
                Logger.Information($"Returning {events.Count} events from cache");
                return events;
            }
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            events = await EventRepository.GetEventsAsync(start.StartOfDay(), end.EndOfDay());
            EventCache.UpdateCacheStore(events, start, end);
            Notification.Attach(events);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());

            Logger.Information($"Returning {events.Count} events from external source");
            return events;
        }

        public async Task<Event> UpdateEventAsync(Event eventObj)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var oldEvent = await GetEventAsync(eventObj.Id);
            if (oldEvent.Category.Id != eventObj.Category.Id)
            {
                await EventRepository.UpdateEventCategoryAsync(eventObj.Id, oldEvent.Category.Id, eventObj.Category.Id);
            }

            var updatedEvent = await EventRepository.UpdateEventAsync(eventObj);
            EventCache.AddOrUpdateEvent(updatedEvent);
            Notification.Attach(updatedEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return updatedEvent;
        }

        public async Task UpdateCacheStoreAsync(DateTime start, DateTime end)
        {
            var events = await EventRepository.GetEventsAsync(start.StartOfDay(), end.EndOfDay());
            EventCache.UpdateCacheStore(events, start, end);
            Notification.Attach(events);
            Logger.Information("Cache has been updated with {@NrOfEvents} events from {@Start} to {@End}", events.Count, start, end);
        }

        public async Task DeleteEventAsync(string id, string calendarId)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            await EventRepository.DeleteEventAsync(calendarId, id);
            EventCache.RemoveEvent(id);
			Notification.Detatch(id);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
        }

        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var createdEvent = await EventRepository.InsertEventAsync(newEvent);
            EventCache.AddOrUpdateEvent(createdEvent);
			Notification.Attach(createdEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return createdEvent;
        }

        public IList<EventCategory> GetCategories()
        {
            var categories = EventCategoryCache.GetEventCategories();

            if (categories != null)
            {
                Logger.Information($"Returning {categories.Count} categories from cache");
                return categories;
            }
            _notificationPublisher.Publish(new GettingExternalDataEvent());
            categories = CategoryRepository.GetCategories();

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());

            Logger.Information($"Returning {categories.Count} categories from external source");
            return categories;
        }

        public async Task<IList<EventCategory>> GetCategoriesAsync()
        {
            var categories = EventCategoryCache.GetEventCategories();

            if (categories != null)
            {
                Logger.Information($"Returning {categories.Count} categories from cache");
                return categories;
            }
            _notificationPublisher.Publish(new GettingExternalDataEvent());
            categories = await CategoryRepository.GetCategoriesAsync();

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());

            Logger.Information($"Returning {categories.Count} categories from external source");
            return categories;
        }

        #endregion
    }
}
