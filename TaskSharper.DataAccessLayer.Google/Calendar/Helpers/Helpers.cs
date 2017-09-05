using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using GoogleEvent = Google.Apis.Calendar.v3.Data.Event;
using Event = TaskSharper.Domain.Calendar.Event;

namespace TaskSharper.DataAccessLayer.Google.Calendar.Helpers
{
    internal class Helpers
    {
        internal static Event GoogleEventParser(GoogleEvent googleEvent)
        {
            return new Event
            {
                Id = googleEvent.Id,
                Title = googleEvent.Summary,
                Description = googleEvent.Description,
                Start = googleEvent.Start?.DateTime,
                End = googleEvent.End?.DateTime,
                Status = googleEvent.Status,
                Created = googleEvent.Created,
                OriginalStartTime = googleEvent.OriginalStartTime?.DateTime,
                Updated = googleEvent.Updated,
                Recurrence = googleEvent.Recurrence
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
                Status = googleEvent.Status,
                Created = googleEvent.Created,
                OriginalStartTime = googleEvent.OriginalStartTime?.DateTime,
                Updated = googleEvent.Updated,
                Recurrence = googleEvent.Recurrence
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
                Status = eventObj.Status
            };
        }

        internal static List<GoogleEvent> GoogleEventParser(List<Event> eventObjs)
        {
            return eventObjs.Select(eventObj => new GoogleEvent
            {
                Summary = eventObj.Title,
                Description = eventObj.Description,
                Start = new EventDateTime { Date = eventObj.Start.ToString(), DateTime = eventObj.Start },
                End = new EventDateTime { Date = eventObj.End.ToString(), DateTime = eventObj.End },
                Status = eventObj.Status
            }).ToList();
        }
    }
}
