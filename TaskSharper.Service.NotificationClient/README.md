# TaskSharper Notification Client

This package contains an object for communcating with the TaskSharper.Service SignalR socket connection. 

## NotificationClient
NotificationClient is an object that helps estabilishing connection to the socket server, aswell as 
providing a method for subscribing to events. 

The client implements _INotificationClient_ which has the following members:
```csharp
public interface INotificationClient
{
    bool IsConnected { get; }
    int ConnectionRetries { get; set; }
    int ConnectionIntervalInMs { get; set; }

    Task Connect();
    void Subscribe<T>(Action<T> callback);
    void Dispose();
}
```
Buildin to the client is a connection retry functionality, which mean that it will try to establish 
connection to the server a specified amount of times. `ConnectionRetries` sspecifies the number of 
times the client will try to connect before giving in, the default value is 5. `ConnectionIntervalInMs`
is the amount of time waited between earch retry, default value is 1000 ms.

`Connect()` will establish the connection to the server. 

`Subscribe<T>(callback)` configures a 
subscription on the server. `T` is the event that the client subscribes to, and can be both simple
and complex types.

`Dispose` will shut down the connection to the server.    


## HubConnectionProxy
HubConnectionProxy is a proxy for the SignalR HubConnection. The reason for creating a wrapper around 
SignalR HubConnection, is that the interface for HubConnection, does not contain the method 
`CreateHubProxy()` which is needed to connect to a SignalR hub. The effect of that is that
HubConnection has to be injected into the _NotificationClient_ like this:

```csharp
public NotificationClient(HubConnection connection)
{
    this._connection = connection;
    _notificationHub = _connection.CreateHubProxy(HubName);
}
```

The result being that the _NotificationClient_ being very hard to unit test, as `HubConnection`
cannot be mocked. 

The solution found to make _NotificationClient_ unit testable, has been to create a wrapper/proxy 
around `HubConnection` - namely `HubConnectionProxy` which implements the interface 
`IHubConnectionProxy` thereby making _NotificationClient_ unit testable.

`HubConnectionProxy` implements the methods needed to succesfully establish a connection to a
SignalR hub. `HubConnectionProxy` does nothing but calls methods on `HubConnection`, thereby
eliminating the immediate need to unit test the proxy - after all, the onlt thing the proxy does
is to call methods on another object. By making the assumption that the SignalR team has done
the needed amount of tests on the `HubConnection`, the implemented proxy will __NOT__ be unit tested.

The implementation of `HubConnectionProxy` can be seen here:

```csharp
public class HubConnectionProxy : IHubConnectionProxy
{
    private readonly HubConnection _connection;

    public HubConnectionProxy(string url)
    {
        _connection = new HubConnection(url);
    }

    public Task Start()
    {
        return _connection.Start();
    }

    public IHubProxy CreateHubProxy(string hubName)
    {
        return _connection.CreateHubProxy(hubName);
    }

    public void Stop()
    {
        _connection.Stop();
    }
}
```

From the implementation can be seen that nothing fancy happens - only direct method calls to 
`HubConnection`. If more methods is needed from `HubConnection` these methods can just be 
implemented in `HubConnectionProxy` and `IHubConnectionProxy` to make the methods available. 

After the creation of `HubConnectionProxy` the constructor for _NotificationClient_ looks 
like this:

```csharp
public NotificationClient(IHubConnectionProxy connection)
{
    this._connection = connection;
    _notificationHub = _connection.CreateHubProxy(HubName);
}
```

The `IHubConnectionProxy` is now injected into the constructor and thereby making _NotificationClient_
unit testable.