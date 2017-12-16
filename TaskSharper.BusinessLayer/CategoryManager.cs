using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.DataAccessLayer;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.Notification;
using TaskSharper.Domain.ServerEvents;

namespace TaskSharper.BusinessLayer
{
    public class CategoryManager : ICategoryManager
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IEventCategoryCache _categoryCache;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly ILogger _logger;

        public CategoryManager(ICategoryRepository categoryRepository, IEventCategoryCache categoryCache, INotificationPublisher notificationPublisher, ILogger logger)
        {
            _categoryRepository = categoryRepository;
            _categoryCache = categoryCache;
            _notificationPublisher = notificationPublisher;
            _logger = logger;
        }

        public async Task<IList<EventCategory>> GetCategoriesAsync()
        {
            var categories = _categoryCache.GetEventCategories();

            if (categories != null)
            {
                _logger.Information($"Returning {categories.Count} categories from cache");
                return categories;
            }
            _notificationPublisher.Publish(new GettingExternalDataEvent());
            categories = await _categoryRepository.GetCategoriesAsync();

            _notificationPublisher.Publish(new FinishedGettingExternalDataEvent());

            _logger.Information($"Returning {categories.Count} categories from external source");
            return categories;
        }
    }
}
