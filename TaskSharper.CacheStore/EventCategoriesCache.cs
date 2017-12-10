using System;
using System.Collections.Generic;
using Serilog;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.CacheStore
{
    public class EventCategoriesCache : IEventCategoryCache
    {
        private readonly ILogger _logger;

        private CacheData<IList<EventCategory>> EventCategoriesCacheData { get; set; }

        public EventCategoriesCache(ILogger logger)
        {
            _logger = logger.ForContext<EventCategoriesCache>();
        }


        public TimeSpan UpdatedOffset { get; set; } = TimeSpan.FromHours(2);
        public IList<EventCategory> GetEventCategories()
        {
            if (EventCategoriesCacheData == null || EventCategoriesCacheData.ForceUpdate || DataTooOld(EventCategoriesCacheData.Updated) )
            {
                return null;
            }
            else
            {
                return EventCategoriesCacheData.Data;
            }
        }

        public void UpdateEventCategories(IList<EventCategory> eventCategories)
        {
            EventCategoriesCacheData = new CacheData<IList<EventCategory>>(eventCategories, DateTime.Now, false);
        }

        private bool DataTooOld(DateTime lastUpdated)
        {
            return (lastUpdated + UpdatedOffset) < DateTime.Now;
        }

    }
}
