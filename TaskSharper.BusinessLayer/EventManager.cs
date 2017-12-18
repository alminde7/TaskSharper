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
    /// <summary>
    /// Handle cache, notifications and data collection from external data providers. 
    /// </summary>
    public class EventManager : IEventManager
    {
        private readonly INotificationPublisher _notificationPublisher;
        public IEventRepository EventRepository { get; }
        public IEventCache EventCache { get; }
        public INotification Notification { get; }
        public ILogger Logger { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventRepository">Event repository used to get events from external data providers</param>
        /// <param name="cache"> IEventCache used to cache events</param>
        /// <param name="notification">INotification used to handle event reminders</param>
        /// <param name="notificationPublisher">NotificationPublisher used to publish events from the service</param>
        /// <param name="logger">Logger used to log</param>
        public EventManager(IEventRepository eventRepository, IEventCache cache, INotification notification, INotificationPublisher notificationPublisher, ILogger logger)
        {
            _notificationPublisher = notificationPublisher;
            EventRepository = eventRepository;
            EventCache = cache;
            Notification = notification;
            Logger = logger.ForContext<EventManager>();
        }

        /// <summary>
        /// Get single event by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get single event by id and date. Increased performance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a list of events for a given date
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a list of events between two dates
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update a single event
        /// </summary>
        /// <param name="eventObj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates the cache with data fetched from external data provider between two dates.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task UpdateCacheStoreAsync(DateTime start, DateTime end)
        {
            var events = await EventRepository.GetEventsAsync(start.StartOfDay(), end.EndOfDay());
            EventCache.UpdateCacheStore(events, start, end);
            Notification.Attach(events);
            Logger.Information("Cache has been updated with {@NrOfEvents} events from {@Start} to {@End}", events.Count, start, end);
        }

        /// <summary>
        /// Delete an event by id and calendar id. 
        /// </summary>
        /// <param name="id">Id is the id of the event</param>
        /// <param name="calendarId">CalendarId is the identifier for the calendar in which id event reside</param>
        /// <returns></returns>
        public async Task DeleteEventAsync(string id, string calendarId)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            await EventRepository.DeleteEventAsync(calendarId, id);
            EventCache.RemoveEvent(id);
			Notification.Detatch(id);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
        }

        /// <summary>
        /// Create a new event.
        /// </summary>
        /// <param name="newEvent"></param>
        /// <returns></returns>
        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            _notificationPublisher.Publish(new GettingExternalDataEvent());

            var createdEvent = await EventRepository.InsertEventAsync(newEvent);
            EventCache.AddOrUpdateEvent(createdEvent);
			Notification.Attach(createdEvent);

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());
            return createdEvent;
        }


    }
}
