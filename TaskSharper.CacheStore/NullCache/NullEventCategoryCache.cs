using System;
using System.Collections.Generic;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.CacheStore.NullCache
{
    // NullObject pattern
    // Used when cache is disabled. Instead of adding a bunch of if/else to the EventManager, 
    // making the code ugly, this implementation can be injected, thereby forcing nothing to be cached.
    public class NullEventCategoryCache : IEventCategoryCache
    {
        public TimeSpan UpdatedOffset { get; set; }
        public IList<EventCategory> GetEventCategories()
        {
            return null;
        }

        public void UpdateEventCategories(IList<EventCategory> eventCategories)
        {
            
        }
    }
}
