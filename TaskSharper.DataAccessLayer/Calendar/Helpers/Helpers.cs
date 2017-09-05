using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.DataAccessLayer.Calendar.Model;

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
                Start = googleEvent.Start.DateTime,
                End = googleEvent.End.DateTime,
                Status = googleEvent.Status,
                Created = googleEvent.Created,
                OriginalStartTime = googleEvent.OriginalStartTime.DateTime,
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
    }
}
