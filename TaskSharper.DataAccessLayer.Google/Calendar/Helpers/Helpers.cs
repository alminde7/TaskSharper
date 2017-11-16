using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using TaskSharper.Domain.Calendar;
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
                Start = googleEvent.Start.DateTime ?? DateTime.Parse(googleEvent.Start.Date),
                End = googleEvent.End.DateTime ?? DateTime.Parse(googleEvent.End.Date).AddTicks(-1),
                AllDayEvent = googleEvent.Start != null && googleEvent.Start.DateTime == null ? DateTime.Parse(googleEvent.Start.Date) : (DateTime?) null,
                Status = Enum.TryParse(googleEvent.Status?.First().ToString().ToUpper() + googleEvent.Status?.Substring(1), out EventStatus statusValue) ? statusValue : EventStatus.Confirmed,
                Created = googleEvent.Created,
                OriginalStartTime = googleEvent.OriginalStartTime?.DateTime,
                Updated = googleEvent.Updated,
                Recurrence = googleEvent.Recurrence,
                Type = Enum.TryParse(googleEvent.ExtendedProperties?.Shared["Type"], out EventType typeValue) ? typeValue : EventType.None,
                Category = new EventCategory { Id = googleEvent.Organizer.Email, Name = googleEvent.Organizer.DisplayName },
                Reminders = googleEvent.Reminders?.Overrides?.Select(i => i.Minutes).ToList(),
                MarkedAsDone = bool.TryParse(googleEvent.ExtendedProperties?.Shared.FirstOrDefault(i => i.Key == "MarkedAsDone").Value, out bool markedAsDoneValue) && markedAsDoneValue
            };
        }

        public static List<Event> GoogleEventParser(List<GoogleEvent> googleEvents)
        {
            return googleEvents.Select(googleEvent => new Event
            {
                Id = googleEvent.Id,
                Title = googleEvent.Summary,
                Description = googleEvent.Description,
                Start = googleEvent.Start.DateTime ?? DateTime.Parse(googleEvent.Start.Date),
                End = googleEvent.End.DateTime ?? DateTime.Parse(googleEvent.End?.Date).AddTicks(-1),
                AllDayEvent = googleEvent.Start != null && googleEvent.Start?.DateTime == null ? DateTime.Parse(googleEvent.Start.Date) : (DateTime?) null,
                Status = Enum.TryParse(googleEvent.Status?.First().ToString().ToUpper() + googleEvent.Status?.Substring(1), out EventStatus statusValue) ? statusValue : EventStatus.Confirmed,
                Created = googleEvent.Created,
                OriginalStartTime = googleEvent.OriginalStartTime?.DateTime,
                Updated = googleEvent.Updated,
                Recurrence = googleEvent.Recurrence,
                Type = Enum.TryParse(googleEvent.ExtendedProperties?.Shared["Type"], out EventType typeValue) ? typeValue : EventType.None,
                Category = new EventCategory { Id = googleEvent.Organizer.Email, Name = googleEvent.Organizer.DisplayName },
                Reminders = googleEvent.Reminders?.Overrides?.Select(i => i.Minutes).ToList(),
                MarkedAsDone = bool.TryParse(googleEvent.ExtendedProperties?.Shared.FirstOrDefault(i => i.Key == "MarkedAsDone").Value, out bool markedAsDoneValue) && markedAsDoneValue
            }).ToList();
        }

        public static GoogleEvent GoogleEventParser(Event eventObj)
        {
            return new GoogleEvent
            {
                Id = eventObj.Id,
                Summary = eventObj.Title,
                Description = eventObj.Description,
                Start = eventObj.AllDayEvent.HasValue ? new EventDateTime {Date = eventObj.AllDayEvent.Value.ToString("yyyy-MM-dd") } : new EventDateTime {DateTime = eventObj.Start},
                End = eventObj.AllDayEvent.HasValue ? new EventDateTime { Date = eventObj.AllDayEvent.Value.AddDays(1).ToString("yyyy-MM-dd") } : new EventDateTime {DateTime = eventObj.End},
                Status = eventObj.Status.ToString().ToLower(),
                ExtendedProperties = new GoogleEvent.ExtendedPropertiesData
                {
                    Shared = new Dictionary<string, string>
                    {
                        { "Type", eventObj.Type.ToString() },
                        { "MarkedAsDone", eventObj.MarkedAsDone.ToString() }
                    }
                },
                Reminders = new GoogleEvent.RemindersData
                {
                    Overrides = eventObj.Reminders?.Select(reminder => new EventReminder
                    {
                        Minutes = reminder,
                        Method = "popup"
                    }).ToList(),
                    UseDefault = eventObj.Reminders?.Count == 0
                }
            };
        }

        public static List<GoogleEvent> GoogleEventParser(List<Event> eventObjs)
        {
            return eventObjs.Select(eventObj => new GoogleEvent
            {
                Id = eventObj.Id,
                Summary = eventObj.Title,
                Description = eventObj.Description,
                Start = eventObj.AllDayEvent.HasValue ? new EventDateTime { Date = eventObj.AllDayEvent.Value.ToString("yyyy-MM-dd") } : new EventDateTime { DateTime = eventObj.Start },
                End = eventObj.AllDayEvent.HasValue ? new EventDateTime { Date = eventObj.AllDayEvent.Value.AddDays(1).ToString("yyyy-MM-dd") } : new EventDateTime { DateTime = eventObj.End },
                Status = eventObj.Status.ToString().ToLower(),
                ExtendedProperties = new GoogleEvent.ExtendedPropertiesData { Shared = new Dictionary<string, string>
                {
                    { "Type", eventObj.Type.ToString() },
                    { "MarkedAsDone", eventObj.MarkedAsDone.ToString() }
                }
                },
                Reminders = new GoogleEvent.RemindersData
                {
                    Overrides = eventObj.Reminders?.Select(reminder => new EventReminder
                    {
                        Minutes = reminder,
                        Method = "popup"
                    }).ToList(),
                    UseDefault = eventObj.Reminders?.Count == 0
                }
            }).ToList();
        }
    }
}
