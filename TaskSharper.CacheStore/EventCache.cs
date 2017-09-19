using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.CacheStore
{
    public class EventCache : ICacheStore
    {
        public ConcurrentDictionary<DateTime, Dictionary<string, Event>> Events { get; }
        public DateTime LastUpdated { get; set; }

        public EventCache()
        {
            Events = new ConcurrentDictionary<DateTime, Dictionary<string, Event>>();
        }

        public bool HasData(DateTime date)
        {
            return Events.ContainsKey(date.Date);
        }

        public bool HasEvent(string id, DateTime date)
        {
            if (HasData(date.Date))
            {
                return Events[date.Date].ContainsKey(id);
            }
            return false;
        }

        public bool HasEvent(string id)
        {
            return Events.Any(x => x.Value.ContainsKey(id));
        }

        /// <summary>
        /// 
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
                var date = calEvent.Start.Value.Date;

                Events[date].AddOrUpdate(calEvent.Id, calEvent);
            }

            LastUpdated = DateTime.Now;
        }

        public IList<Event> GetEvents(DateTime date)
        {
            if (HasData(date))
            {
                return Events[date.Date].Values.ToList();
            }
            return null;
        }

        public Event GetEvent(string id, DateTime date)
        {
            date = date.Date;

            if (!HasData(date)) return null;
            if (!Events[date].ContainsKey(id)) return null;

            return Events[date][id];
        }

        public Event GetEvent(string id)
        {
            Event calEvent = null;
            var dummy = Events.FirstOrDefault(x => x.Value.TryGetValue(id, out calEvent));

            return calEvent;
        }

        public void AddOrUpdateEvent(Event calendarEvent)
        {
            var date = calendarEvent.Start.Value.Date;

            if(!Events.ContainsKey(date))
                InitializeEventsDictionary(date, null);

            Events[date].AddOrUpdate(calendarEvent.Id, calendarEvent);

        }

        private void InitializeEventsDictionary(DateTime start, DateTime? end)
        {
            if (end.HasValue)
            {
                var timeSpanDays = (end.Value.Date - start.Date).Days;

                for (int index = 0; index <= timeSpanDays; index++)
                {
                    var date = start.Date.AddDays(index);
                    if (!Events.ContainsKey(date))
                    {
                        Events.TryAdd(date, new Dictionary<string, Event>());
                    }
                }
            }
            else
            {
                var date = start.Date;
                if (!Events.ContainsKey(date))
                {
                    Events.TryAdd(date, new Dictionary<string, Event>());
                }
            }
        }
    }
}
