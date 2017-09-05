# Data Access Layer

## Calendar
A calendar service interface is defined with CRUD-functionality, making it easy to switch between Google calendar, 
MSSQL calendar and other implementations.
```csharp
public interface ICalendarService
{
    ...
}
```

### Get
Different overloads are defined, depending on the required functionality.

All events:
```csharp
    List<Event> GetEvents(string calendarId);
```

Events from a given start date:
```csharp
    List<Event> GetEvents(DateTime start, string calendarId);
```

Events between two dates:
```csharp
    List<Event> GetEvents(DateTime start, DateTime end, string calendarId);
```

### Insert


### Update


### Delete