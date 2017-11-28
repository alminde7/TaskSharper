 # TaskSharper
TaskShaper is a collection of WPF applications that allow the user to manage their Google Calendar through a touch-friendly interface.
The applications is meant to be included in the [CAMI project](http://www.aal-europe.eu/projects/cami/) which provides
solutions for health management. 

## Applications
The TaskShaper project consist of 4 WPF application and one Windows Service, a brief description of the services can be seen here:
- An Task application to manage tasks
- An Appointment application to manage appointments
- A Calendar application to see all calendar events for a given day, week or month. 
- A Launcher application which is a interface to open the aforementioned applications.
- A windows service that handles communication with Google, as well as caching data and handling event notifications. 

## Getting started
Some steps has to be taken in order to succesfully get the solution up and running.
#### Prerequisites
Make sure to have the following before attempting to run the application:
- Internet connection.
- The file _Client_secret.json_ containing client id and client secret. Can be obtained by writing an email to one of the developers. 
- A Google account.

#### Setup guide
1. Clone repository
2. Open solution in Visual Studio with administrator priviliges
3. Build solution
4. Right-click on Solution and choose _Set StartUp Projects..._
5. Select _Multiple startup projects_
6. Set __TaskSharper.Launcher__ and __TaskSharper.Service__ to _Start_
7. Run the application and watch it fail
8. Go to your user document folder, and open the folder _TaskSharper_. Path to folder: _C:/Users/[user]/Documents/TaskSharper_
9. Open the folder _.credentials_ and place the _Client_secret.json_ there.
10. Go back to Visual Studio and start the solution again. 
11. At startup you should be prompted for a Google account. 
12. Fill in Google credentials. 
13. Application can now handle your calendar events.

The application is still in development phase, and this setup progree should not be considered valid for the final release. 
In the future an installer will be created to handle must of the setup steps. 

## Configuration options
After completing the _Getting started_ guide, there will be three new files in _C:/Users/[user]/Documents/TaskShaper/Config_ namely:
- _ClientSettings.json_
- _LoggingSettings.json_
- _LoggingSettings.json_

#### ClientSettings
Client settings include on two configuration options: 
- __APIServerUrl__ - specify which endpoint the application must request to get data.
- __NotificationServerUrl__ - speicy which SignalR server the application must connect to in order to get notification about upcomming events. 

```json
{
    "APIServerUrl": "http://localhost:8000/api/",
    "NotificationServerUrl": "http://localhost:8000"
}
```

#### LoggingSettings
Logging settings include the following: 
```json
{
    "EnableLoggingToFile": true,
    "EnableElasticsearchLogging": true,
    "MinimumLogLevel": "Information",
    "ElasticsearchConfig": {
        "Url": "http://my.elasticsearch.server:9200",
        "EnableAuthentication": false,
        "Username": null,
        "Password": null
    }
}
```

#### ServiceSettings

The service settings include options for disabling cache and notifications, as well as configure how often notifications
for a certain type of event should fire. 

```json
{
    "Cache": {
        "EnableCache": true,
        "AllowedTimeInCache": "00:05:00"
    },
    "Notification": {
        "EnableNotifications": true,
        "Appointments": {
            "EnableAppointmentNotifications": true,
            "NotificationOffsets": [-15, -5, 0, 5, 10]
        },
        "Tasks": {
            "EnableTaskNotifications": true,
            "NotificationOffsets": [ -15, -5, 0, 5, 10]
        }
    }
}
```

`Cache.AllowedTimeInCache` specify how long time data must reside in the cache before it is considered to be invalid.
It is configured as a TimeSpan in C#. In the configuration file it must be configued as `"[hours].[minutes].[seconds]"`

`Notification.Appointments.NotificationOffsets` is implemented as a list of interger values, specifying the notification times
for an appointment type calendar event. Negative numbers will configure the notification to be fired before the event. 
Positive numbers will fire a notification after the event has happend. 0 is at the time of the event. Notifications will not
be fired if the user has completed the task/appointment.

## Techinal references
Build status can be found on continous integration server here: [Go CI/CD](http://alminde1.mynetgear.com:8153/go/pipelines)
