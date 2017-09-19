# TaskSharper Cache store

## Future work
- Perfomance testing
  - Different collection types
    - `ConcurrentDictionary<DateTime, List<Event>>()`
    - `ConcurrentDictionary<DateTime, Dictionary<string, Event>>()`
  - Methods
    - `Event GetEvent(string id, DateTime date)`
    - `Event GetEvent(string id)`
    - `bool HasEvent(string id, DateTime date)`
    - `bool HasEvent(string id)`