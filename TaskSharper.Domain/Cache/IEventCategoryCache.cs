using System.Collections.Generic;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.Domain.Cache
{
    public interface IEventCategoryCache : ICacheStore
    {
        IList<EventCategory> GetEventCategories();
        void UpdateEventCategories(IList<EventCategory> eventCategories);
    }
}