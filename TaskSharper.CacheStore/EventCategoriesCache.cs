using System;
using System.Collections.Generic;
using Serilog;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.CacheStore
{
    /// <summary>
    /// Used to hold data about catogories
    /// </summary>
    public class EventCategoriesCache : IEventCategoryCache
    {
        private readonly ILogger _logger;

        /// <summary>
        /// The datastructure that holds the cache data.
        /// </summary>
        private CacheData<IList<EventCategory>> EventCategoriesCacheData { get; set; }

        public EventCategoriesCache(ILogger logger)
        {
            _logger = logger.ForContext<EventCategoriesCache>();
        }

        /// <summary>
        /// The allowed lenght of stay in the cache for each data. 
        /// </summary>
        public TimeSpan UpdatedOffset { get; set; } = TimeSpan.FromMinutes(5);

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

        /// <summary>
        /// Update the cache with a list of categories
        /// </summary>
        /// <param name="eventCategories"></param>
        public void UpdateEventCategories(IList<EventCategory> eventCategories)
        {
            EventCategoriesCacheData = new CacheData<IList<EventCategory>>(eventCategories, DateTime.Now, false);
        }

        /// <summary>
        /// Determines whether the data is too old. 
        /// </summary>
        /// <param name="lastUpdated"></param>
        /// <returns></returns>
        private bool DataTooOld(DateTime lastUpdated)
        {
            return (lastUpdated + UpdatedOffset) < DateTime.Now;
        }

    }
}
