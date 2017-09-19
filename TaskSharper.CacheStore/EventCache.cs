﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.CacheStore
{
    public class EventCache : ICacheStore
    {
        public ConcurrentDictionary<DateTime, List<Event>> Events { get; set; }

        public DateTime LastUpdated { get; set; }

        public EventCache()
        {
            Events = new ConcurrentDictionary<DateTime, List<Event>>();
        }

        public bool HasData(DateTime date)
        {
            return Events.ContainsKey(date.Date);
        }

        public void UpdateCacheStore(IList<Event> events, DateTime fromDate, DateTime? toDate)
        {
            UpdateEventsContainer(fromDate, toDate);

            foreach (var calEvent in events)
            {
                var date = calEvent.Start.Value.Date;

                Events[date].Add(calEvent);
            }

            LastUpdated = DateTime.Now;
        }

        public IList<Event> GetEvents(DateTime date)
        {
            if (Events.ContainsKey(date.Date))
            {
                return Events[date.Date];
            }
            else
            {
                return new List<Event>();
            }
        }

        private void UpdateEventsContainer(DateTime start, DateTime? end)
        {
            if (end.HasValue)
            {
                var timeSpanDays = (end.Value.Date - start.Date).Days;

                for (int index = 0; index <= timeSpanDays; index++)
                {
                    var date = start.Date.AddDays(index);
                    if (!Events.ContainsKey(date))
                    {
                        Events.TryAdd(date, new List<Event>());
                    }
                }
            }
            else
            {
                var date = start.Date;
                if (!Events.ContainsKey(date))
                {
                    Events.TryAdd(date, new List<Event>());
                }
            }
            
        }
    }
}
