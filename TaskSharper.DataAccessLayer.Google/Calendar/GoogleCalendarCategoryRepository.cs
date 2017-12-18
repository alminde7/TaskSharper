using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Serilog;
using Serilog.Core;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.DataAccessLayer;
using TaskSharper.Domain.Models;
using Event = TaskSharper.Domain.Models.Event;

namespace TaskSharper.DataAccessLayer.Google.Calendar
{
    /// <summary>
    /// Used to get categories from Google Calendar
    /// </summary>
    public class GoogleCalendarCategoryRepository : GoogleCalendarBase, ICategoryRepository
    {
        private readonly CalendarService _service;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Google Calendar service used to create requests to Google</param>
        /// <param name="logger"></param>
        public GoogleCalendarCategoryRepository(CalendarService service, ILogger logger) : base(service, logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of categories associated with a Google Calendar account.
        /// </summary>
        /// <returns></returns>
        public List<EventCategory> GetCategories()
        {
            return GetCalendars().Select(calendarListEntry => new EventCategory { Id = calendarListEntry.Id, Name = calendarListEntry.Summary }).ToList();
        }
        /// <summary>
        /// Get a list of categories associated with a Google Calendar account.
        /// </summary>
        /// <returns></returns>
        public async Task<List<EventCategory>> GetCategoriesAsync()
        {
            var calendarList = await GetCalendarsAsync();
            return calendarList.Select(calendarListEntry => new EventCategory { Id = calendarListEntry.Id, Name = calendarListEntry.Summary }).ToList();
        }


    }
}
