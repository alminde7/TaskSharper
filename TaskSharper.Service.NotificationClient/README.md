﻿# TaskSharper Notification Client

This package contains an object for communcating with the TaskSharper.Service SignalR socket connection. 

## NotificationClient
Comming soon...

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