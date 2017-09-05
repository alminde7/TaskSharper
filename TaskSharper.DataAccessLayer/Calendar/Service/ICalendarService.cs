using System;
using System.Collections.Generic;
using TaskSharper.DataAccessLayer.Calendar.Model;

namespace TaskSharper.DataAccessLayer.Calendar.Service
{
    public interface ICalendarService
    {
        List<Event> GetEvents(string calendarId);
        List<Event> GetEvents(DateTime start, string calendarId);
        List<Event> GetEvents(DateTime start, DateTime end, string calendarId);
    }
}
