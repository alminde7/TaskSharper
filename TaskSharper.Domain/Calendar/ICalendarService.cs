using System;
using System.Collections.Generic;

namespace TaskSharper.Domain.Calendar
{
    public interface ICalendarService
    {
        Event GetEvent(string id, string calendarId);
        List<Event> GetEvents(string calendarId);
        List<Event> GetEvents(DateTime start, string calendarId);
        List<Event> GetEvents(DateTime start, DateTime end, string calendarId);
        Event InsertEvent(Event eventObj, string calendarId);
        Event UpdateEvent(Event eventObj, string calendarId);
        void DeleteEvent(string calendarId, string eventId);
    }
}
