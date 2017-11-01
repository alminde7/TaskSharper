# TaskSharper Cache store

The reason to implement a cache feature is to minimize the amount of requests on the network.

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
