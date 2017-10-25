# Service.RestClient

This package contains the following
- Methods for communicating with TaskSharper.Service RestAPI
- Facotry for setting up common request headers
- Extension methods to RestSharp objects. 

## TaskSharper.Service RestAPI Client
The RestClient object fullfils the interface *IEventRestClient*. The methods available can be seen here:

```csharp
public interface IEventRestClient
{
    Task<Event> GetAsync(string id);

    Task<IEnumerable<Event>> GetAsync(DateTime date);

    Task<IEnumerable<Event>> GetAsync(DateTime from, DateTime to);

    Task<Event> CreateAsync(Event newEvent);

    Task<Event> UpdateAsync(Event updatedEvent);

    Task DeleteAsync(string id);
}
```


## Extension methods
Hereby follows a walkthrough of the extension methods created for RestSharp. The reason for these
extension methods is to "hide" certain configurations, and thereby making the application code 
more readable.

#### AddCorrelationId
As the name hints, this extension add correlationId to the request. The method can be seen here:
```csharp
public static IRestRequest AddCorrelationId(this IRestRequest request)
{
    request.AddHeader(Http.Header_CorrelationId, Guid.NewGuid().ToString());
    return request;
}
```

The method created a new header for the request, with key: *X-Correlation-Id* and value being
a GUID.

This header makes request tracing through multiple seperated systems easier. 

The extension is used like this:
```csharp
IRestRequest request = new RestRequest();
request.AddCorrelationId();
```

## Factories
The inspiration for creating these factories comes from: http://www.hackered.co.uk/articles/restsharp-and-the-factory-pattern-you-really-should 

#### RestRequestFactory
The RestRequestFactory creates instances of RestSharp IRestRequest objects. The factory bootstraps
the rest request object with headers/security etc. The reason for having the factory bootstraping
the RestRequest is to avoid unessecary code-copy. If there is general changes to a request, 
these changes only have to be made in one place. 

In the following example the RestRequestFactory creates RestRequests where correlationId is added
as a header. 
```csharp
public class RestRequestFactory : IRestRequestFactory
{
    public IRestRequest Create(string path, Method method)
    {
        // Create request
        var request = new RestRequest(path, method);

        // Bootstrap request headers etc.
        request.AddCorrelationId();

        // Return request object
        return request;
    }
}
```

This ensure that a correlationId is sent along with each request to the server. 
