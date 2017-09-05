using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3.Data;
using Event = TaskSharper.Domain.Calendar.Event;

namespace TaskSharper.DataAccessLayer.Calendar.Helpers
{
    public static class Helpers
    {
        public static Event GoogleEventParser(Google.Apis.Calendar.v3.Data.Event googleEvent)
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

        public static List<Event> GoogleEventParser(List<Google.Apis.Calendar.v3.Data.Event> googleEvents)
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

        public static Google.Apis.Calendar.v3.Data.Event GoogleEventParser(Event eventObj)
        {
            return new Google.Apis.Calendar.v3.Data.Event
            {
                Summary = eventObj.Title,
                Description = eventObj.Description,
                Start = new EventDateTime { DateTime = eventObj.Start },
                End = new EventDateTime { DateTime = eventObj.End },
                Status = eventObj.Status
            };
        }

        public static List<Google.Apis.Calendar.v3.Data.Event> GoogleEventParser(List<Event> eventObjs)
        {
            return eventObjs.Select(eventObj => new Google.Apis.Calendar.v3.Data.Event
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
