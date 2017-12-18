using System;
using System.Collections.Generic;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.CacheStore.NullCache
{
    // NullObject pattern
    // Used when cache is disabled. Instead of adding a bunch of if/else to the EventManager, 
    // making the code ugly, this implementation can be injected, thereby forcing nothing to be cached.
    /// <summary>
    /// NullCache used to disable the cache functionality for IEventCache.
    /// </summary>
    public class NullEventCache : IEventCache
    {
        public TimeSpan UpdatedOffset { get; set; }
        public IList<Event> GetEvents(DateTime date)
        {
            return null;
        }

        public IList<Event> GetEvents(DateTime start, DateTime end)
        {
            return null;
        }

        public Event GetEvent(string id, DateTime date)
        {
            return null;
        }

        public Event GetEvent(string id)
        {
            return null;
        }

        public bool HasData(DateTime date)
        {
            return false;
        }

        public bool HasEvent(string id, DateTime date)
        {
            return false;
        }

        public bool HasEvent(string id)
        {
            return false;
        }

        public void UpdateCacheStore(IList<Event> events, DateTime fromDate, DateTime? toDate)
        {
            
        }

        public void AddOrUpdateEvent(Event calendarEvent)
        {
            
        }

        public void RemoveEvent(string id)
        {
            
        }
    }
}
