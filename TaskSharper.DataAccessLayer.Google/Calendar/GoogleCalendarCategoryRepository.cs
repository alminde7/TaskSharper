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
    public class GoogleCalendarCategoryRepository : GoogleCalendarBase, ICategoryRepository
    {
        private readonly CalendarService _service;
        private readonly ILogger _logger;

        public GoogleCalendarCategoryRepository(CalendarService service, ILogger logger) : base(service, logger)
        {
            _service = service;
            _logger = logger;
        }

        public List<EventCategory> GetCategories()
        {
            return GetCalendars().Select(calendarListEntry => new EventCategory { Id = calendarListEntry.Id, Name = calendarListEntry.Summary }).ToList();
        }

        public async Task<List<EventCategory>> GetCategoriesAsync()
        {
            var calendarList = await GetCalendarsAsync();
            return calendarList.Select(calendarListEntry => new EventCategory { Id = calendarListEntry.Id, Name = calendarListEntry.Summary }).ToList();
        }


    }
}
