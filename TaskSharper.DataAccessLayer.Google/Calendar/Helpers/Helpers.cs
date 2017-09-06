using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using GoogleEvent = Google.Apis.Calendar.v3.Data.Event;
using Event = TaskSharper.Domain.Calendar.Event;

namespace TaskSharper.DataAccessLayer.Google.Calendar.Helpers
{
    public class Helpers
    {
        public static Event GoogleEventParser(GoogleEvent googleEvent)
        {
            return new Event
            {
                Id = googleEvent.Id,
                Title = googleEvent.Summary,
                Description = googleEvent.Description,
                Start = googleEvent.Start?.DateTime,
                End = googleEvent.End?.DateTime,
                Status = Enum.TryParse(googleEvent.Status, out Event.EventStatus statusValue) ? statusValue : Event.EventStatus.Confirmed,
                Created = googleEvent.Created,
                OriginalStartTime = googleEvent.OriginalStartTime?.DateTime,
                Updated = googleEvent.Updated,
                Recurrence = googleEvent.Recurrence,
                Type = Enum.TryParse(googleEvent.ExtendedProperties?.Shared["Type"], out Event.EventType typeValue) ? typeValue : Event.EventType.None
            };
        }

        internal static List<Event> GoogleEventParser(List<GoogleEvent> googleEvents)
        {
            return googleEvents.Select(googleEvent => new Event
            {
                Id = googleEvent.Id,
                Title = googleEvent.Summary,
                Description = googleEvent.Description,
                Start = googleEvent.Start?.DateTime,
                End = googleEvent.End?.DateTime,
                Status = Enum.TryParse(googleEvent.Status, out Event.EventStatus statusValue) ? statusValue : Event.EventStatus.Confirmed,
                Created = googleEvent.Created,
                OriginalStartTime = googleEvent.OriginalStartTime?.DateTime,
                Updated = googleEvent.Updated,
                Recurrence = googleEvent.Recurrence,
                Type = Enum.TryParse(googleEvent.ExtendedProperties?.Shared["Type"], out Event.EventType typeValue) ? typeValue : Event.EventType.None
            }).ToList();
        }

        internal static GoogleEvent GoogleEventParser(Event eventObj)
        {
            return new GoogleEvent
            {
                Summary = eventObj.Title,
                Description = eventObj.Description,
                Start = new EventDateTime { DateTime = eventObj.Start },
                End = new EventDateTime { DateTime = eventObj.End },
                Status = eventObj.Status.ToString(),
                ExtendedProperties = new GoogleEvent.ExtendedPropertiesData { Shared = new Dictionary<string, string>
                {
                    { "Type", eventObj.Type.ToString() }
                }}
            };
        }

        internal static List<GoogleEvent> GoogleEventParser(List<Event> eventObjs)
        {
            return eventObjs.Select(eventObj => new GoogleEvent
            {
                Summary = eventObj.Title,
                Description = eventObj.Description,
                Start = new EventDateTime { DateTime = eventObj.Start },
                End = new EventDateTime { DateTime = eventObj.End },
                Status = eventObj.Status.ToString(),
                ExtendedProperties = new GoogleEvent.ExtendedPropertiesData { Shared = new Dictionary<string, string>
                {
                    { "Type", eventObj.Type.ToString() }
                }}
            }).ToList();
        }
    }
}
