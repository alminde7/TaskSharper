using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.CacheStore
{
    public class EventCache : ICacheStore
    {
        public ILogger Logger { get; }
        public ConcurrentDictionary<DateTime, Dictionary<string, CacheData>> Events { get; }
        public DateTime LastUpdated { get; set; }
        public TimeSpan UpdatedOffset { get; set; }

        public EventCache(ILogger logger)
        {
            Logger = logger.ForContext<EventCache>();
            Events = new ConcurrentDictionary<DateTime, Dictionary<string, CacheData>>();

            UpdatedOffset = TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// Return true if the given date has updated data. Return false if the given date is not in the cache store.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool HasData(DateTime date)
        {
            return Events.ContainsKey(date.StartOfDay());
        }

        /// <summary>
        /// Return true if an event contains on a given day with a given id. Return false otherwise
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool HasEvent(string id, DateTime date)
        {
            if (HasData(date.StartOfDay()))
            {
                return Events[date.StartOfDay()].ContainsKey(id);
            }
            return false;
        }

        /// <summary>
        /// Return true id cache contains an event with the given id. Return false otherwise
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasEvent(string id)
        {
            return Events.Any(x => x.Value.ContainsKey(id));
        }

        /// <summary>
        /// Add or update a collection of events
        /// </summary>
        /// <param name="events"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void UpdateCacheStore(IList<Event> events, DateTime fromDate, DateTime? toDate)
        {
            InitializeEventsDictionary(fromDate, toDate);

            foreach (var calEvent in events)
            {
                var date = calEvent.Start.Value.StartOfDay();

                Events[date].AddOrUpdate(calEvent.Id, new CacheData(calEvent, DateTime.Now, false));
            }

            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Returns all events on a given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IList<Event> GetEvents(DateTime date)
        {
            if (HasData(date.StartOfDay()))
            {
                var data = Events[date.StartOfDay()].Values.ToList();

                var dateNow = DateTime.Now;
                if (data.Any(x => x.ForceUpdate || DataTooOld(x.Updated))) return null;

                return data.Select(x => x.Event).ToList();
            }

            return null;
        }

        public IList<Event> GetEvents(DateTime start, DateTime end)
        {
            List<Event> events = new List<Event>();
            var eventsDictionaries = Events.Where(x => x.Key >= start.StartOfDay() && x.Key <= end.StartOfDay()).Select(x => x.Value);
            
            foreach (var calEvent in eventsDictionaries)
            {
                if (calEvent.Values.Any(x => x.ForceUpdate || DataTooOld(x.Updated))) return null;
                events.AddRange(calEvent.Values.Select(x => x.Event).ToList());
            }

            return events;
        }

        /// <summary>
        /// Returns an event on a given date with a given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public Event GetEvent(string id, DateTime date)
        {
            date = date.StartOfDay();

            if (!HasData(date)) return null;
            if (!Events[date].ContainsKey(id)) return null;

            var data = Events[date][id];

            if (data.ForceUpdate || DataTooOld(data.Updated)) return null;

            return Events[date][id].Event;
        }

        /// <summary>
        /// Return an event with a given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Event GetEvent(string id)
        {
            CacheData cacheData = null;
            var dummy = Events.FirstOrDefault(x => x.Value.TryGetValue(id, out cacheData));

            if (cacheData != null)
            {
                if (cacheData.ForceUpdate || DataTooOld(cacheData.Updated)) return null;
            }
            
            return cacheData?.Event;
        }

        /// <summary>
        /// Add the event if it does not exist, and updates the event if is does exist
        /// </summary>
        /// <param name="calendarEvent"></param>
        public void AddOrUpdateEvent(Event calendarEvent)
        {
            var date = calendarEvent.Start.Value.StartOfDay();

            if(!Events.ContainsKey(date))
                InitializeEventsDictionary(date, null);

            Events[date].AddOrUpdate(calendarEvent.Id, new CacheData(calendarEvent, DateTime.Now, false));
        }

        private void InitializeEventsDictionary(DateTime start, DateTime? end)
        {
            if (end.HasValue)
            {
                var timeSpanDays = (end.Value.StartOfDay() - start.StartOfDay()).Days;

                for (int index = 0; index <= timeSpanDays; index++)
                {
                    var date = start.StartOfDay().AddDays(index);
                    if (!Events.ContainsKey(date))
                    {
                        Events.TryAdd(date, new Dictionary<string, CacheData>());
                    }
                }
            }
            else
            {
                var date = start.StartOfDay();
                if (!Events.ContainsKey(date))
                {
                    Events.TryAdd(date, new Dictionary<string, CacheData>());
                }
            }
        }

        private bool DataTooOld(DateTime lastUpdated)
        {
            return (lastUpdated + UpdatedOffset) < DateTime.Now;
        }
    }
}
