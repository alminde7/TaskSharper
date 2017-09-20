# TaskSharper Cache store

The reason to implement a cache feature is to minimize the amount of requests on the network. As per the current 
design of the Calendar application, one request is made per day. That results in:

- 1 request for TodayView
- 7 requests for WeekView
- 28-31 requests for MonthView

And that is done everytime the user decides to increase/decrese day/week/month.  

With the implementation of a cache mechanism, every request will search for data in the cache store, before making
a network request. To avoid unnessesary network call when starting the application, the cache is filled with data
corresponding to:
- 14 days **prior** current date
- 14 days **past** current date

However the filling of the cache is not controlled by the CacheStore project, but by the WPF application.

## Rules
- If cahce has not been updated within 5 min of last request, and new request must be made to ensure data is in sync.

## Interface
```csharp
public interface ICacheStore
{
    DateTime LastUpdated { get; }

    IList<Event> GetEvents(DateTime date);
    Event GetEvent(string id, DateTime date);
    Event GetEvent(string id);

    bool HasData(DateTime date);
    bool HasEvent(string id, DateTime date);
    bool HasEvent(string id);

    void UpdateCacheStore(IList<Event> events, DateTime fromDate, DateTime? toDate);
    void AddOrUpdateEvent(Event calendarEvent);
}
```


## Future work
- Add `LastUpdated` attribute to every event id in the EventCollectionStore, to controle when each individual event
    must be updated
- Perfomance testing
  - Different collection types
    - `ConcurrentDictionary<DateTime, List<Event>>()`
    - `ConcurrentDictionary<DateTime, Dictionary<string, Event>>()`
  - Methods
    - `Event GetEvent(string id, DateTime date)`
    - `Event GetEvent(string id)`
    - `bool HasEvent(string id, DateTime date)`
    - `bool HasEvent(string id)`