using System;
using System.Collections.Generic;

namespace TaskSharper.Domain.Calendar
{
    public interface ICalendarService
    {
        List<Event> GetEvents(string calendarId);
        List<Event> GetEvents(DateTime start, string calendarId);
        List<Event> GetEvents(DateTime start, DateTime end, string calendarId);
    }
}
