using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Serilog;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Extensions;
using TaskSharper.Shared.Wrappers;

namespace TaskSharper.CacheStore
{
    public class EventCache : IEventCache
    {
        public ILogger Logger { get; set; }
        public ConcurrentDictionary<DateTime, Dictionary<string, CacheData<Event>>> Events { get; }
        public TimeSpan UpdatedOffset { get; set; } = TimeSpan.FromMinutes(5);

        private Timer DailyCleanUpTimer { get; set; }

        public EventCache(ILogger logger)
        {
            Logger = logger.ForContext<EventCache>();
            Events = new ConcurrentDictionary<DateTime, Dictionary<string, CacheData<Event>>>();

            DailyCleanUpTimer = new Timer()
                .SetDailyScheduler(new TimeSpan(0,2,0,0), CleanUp)
                .StartTimer();
        }

        private void CleanUp()
        {
            try
            {
                Logger.ForContext("CleanUp", typeof(EventCache)).Information("Doing daily cleaning for cache");
                Events.Clear();
            }
            catch (Exception e)
            {
                Logger.ForContext("CleanUp", typeof(EventCache)).Error(e, "Error while doing daily cleaning of cache");
            }
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

                //Events[date].AddOrUpdate(calEvent.Id, new CacheData(calEvent, DateTime.Now, false));

                var diff = (calEvent.End.Value - calEvent.Start.Value).Days;

                for (int i = 0; i <= diff; i++)
                {
                    Events[date.AddDays(i)].AddOrUpdate(calEvent.Id, new CacheData<Event>(calEvent, DateTime.Now, false));
                }
            }
        }

        /// <summary>
        /// Returns all events on a given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>
        /// Return <see cref="null"/> 
        /// Return <see cref="IList{Event}"/> 
        /// </returns>
        public IList<Event> GetEvents(DateTime date)
        {
            if (HasData(date.StartOfDay()))
            {
                var data = Events[date.StartOfDay()].Values.ToList();
                
                if (data.Any(x => x.ForceUpdate || DataTooOld(x.Updated))) return null;

                return data.Select(x => x.Data).ToList();
            }

            return null;
        }

        /// <summary>
        /// Returns all events on a given date
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>
        /// Return <see cref="null"/> 
        /// Return <see cref="IList{Event}"/> 
        /// </returns>
        public IList<Event> GetEvents(DateTime start, DateTime end)
        {
            List<Event> events = new List<Event>();
            var diff = (end - start).Days;

            for (int i = 0; i <= diff; i++)
            {
                var date = start.AddDays(i).StartOfDay();

                if (!Events.ContainsKey(date))
                    return null;
                else
                {
                    if (Events[date].Values.Any(x => x.ForceUpdate || DataTooOld(x.Updated)))
                        return null;

                    events.AddRange(Events[date].Values.Select(x => x.Data).ToList());
                }
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

            return Events[date][id].Data;
        }

        /// <summary>
        /// Return an event with a given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Event GetEvent(string id)
        {
            CacheData<Event> cacheData = null;
            var dummy = Events.FirstOrDefault(x => x.Value.TryGetValue(id, out cacheData));

            if (cacheData != null)
            {
                if (cacheData.ForceUpdate || DataTooOld(cacheData.Updated)) return null;
            }
            
            return cacheData?.Data;
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

            foreach (var @event in Events)
            {
                foreach (var calEvent in @event.Value)
                {
                    if (calEvent.Key == calendarEvent.Id)
                    {
                        Events[@event.Key].Remove(calEvent.Key);
                        break;
                    }
                }
            } // removes event if exist
            
            Events[date].AddOrUpdate(calendarEvent.Id, new CacheData<Event>(calendarEvent, DateTime.Now, false));
        }

        public void RemoveEvent(string id)
        {
            foreach (var @event in Events)
            {
                foreach (var calEvent in @event.Value)
                {
                    if (calEvent.Key == id)
                    {
                        Events[@event.Key].Remove(calEvent.Key);
                        break;
                    }
                }
            } // removes event if exist
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
                        Events.TryAdd(date, new Dictionary<string, CacheData<Event>>());
                    }
                }
            }
            else
            {
                var date = start.StartOfDay();
                if (!Events.ContainsKey(date))
                {
                    Events.TryAdd(date, new Dictionary<string, CacheData<Event>>());
                }
            }
        }

        private bool DataTooOld(DateTime lastUpdated)
        {
            return (lastUpdated + UpdatedOffset) < DateTime.Now;
        }
    }
}
