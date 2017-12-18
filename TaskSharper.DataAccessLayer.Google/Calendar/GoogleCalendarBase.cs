using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Serilog;

namespace TaskSharper.DataAccessLayer.Google.Calendar
{
    /// <summary>
    /// GoogleCalendarBase defines common functionality shared between GoogleCalendarCategoryRepository and GoogleCalendarEventRepository
    /// </summary>
    public class GoogleCalendarBase
    {
        private readonly CalendarService _service;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Google Calendar service used to create requests to Google</param>
        /// <param name="logger"></param>
        public GoogleCalendarBase(CalendarService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of available calendars for the user authenticated by the CalendarService.
        /// </summary>
        /// <returns></returns>
        internal List<CalendarListEntry> GetCalendars()
        {
            // Define request
            var request = _service.CalendarList.List();
            _logger.Information("Requesting all calendars in Google Calendar");

            // Execute request to retrieve calendars
            var response = request.Execute();
            _logger.Information("Google Calendar request was successful");

            var activeCalendars = response.Items.Where(i => i.Selected == true).ToList();

            return activeCalendars;
        }

        /// <summary>
        /// Get a list of available calendars async for the user authenticated by the CalendarService.
        /// </summary>
        /// <returns></returns>
        internal async Task<List<CalendarListEntry>> GetCalendarsAsync()
        {
            // Define request
            var request = _service.CalendarList.List();
            _logger.Information("Requesting all calendars in Google Calendar");

            // Execute request to retrieve calendars
            var response = await request.ExecuteAsync();
            _logger.Information("Google Calendar request was successful");

            var activeCalendars = response.Items.Where(i => i.Selected == true).ToList();

            return activeCalendars;
        }
    }
}
