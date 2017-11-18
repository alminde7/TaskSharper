using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Calendar
{
    public class Event
    {
        /// <summary>
        /// Identifier of the event.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Title of the event.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the event.
        /// </summary>
        public string Description { get; set; }

        private DateTime? _start;
        /// <summary>
        /// The (inclusive) start time of the event.
        /// </summary>
        public DateTime? Start
        {
            get => _start?.ToLocalTime();
            set => _start = value?.ToUniversalTime();
        }

        private DateTime? _end;

        /// <summary>
        /// The (exclusive) end time of the event.
        /// </summary>
        public DateTime? End
        {
            get => _end?.ToLocalTime();
            set => _end = value?.ToUniversalTime();
        }

        /// <summary>
        /// Date object which is only set for all day events.
        /// </summary>
        public DateTime? AllDayEvent { get; set; }

        /// <summary>
        /// Status of the event. Optional. Possible values:
        /// Confirmed - The event is confirmed (default).
        /// Tentative - The event is tentatively confirmed.
        /// Cancelled - The event is cancelled.
        /// Completed - The event is completed.
        /// </summary>
        public EventStatus Status { get; set; }

        /// <summary>
        /// Creation time of the event. Read-only.
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// First start time for a recurring event. Optional.
        /// </summary>
        public DateTime? OriginalStartTime { get; set; }

        /// <summary>
        /// Last modification time of the event. Read-only.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Recurrence of the event. TODO: Look into how this looks.
        /// </summary>
        public IList<string> Recurrence { get; set; }

        /// <summary>
        /// Type of the event. Possible values:
        /// None
        /// Apointment
        /// Task
        /// </summary>
        public EventType Type { get; set; }

        public EventCategory Category { get; set; }

        /// <summary>
        /// Reminders for the event. In minutes.
        /// </summary>
        public List<int?> Reminders { get; set; }

        /// <summary>
        /// Marks whether or not the event is finished. Only applicable for Tasks.
        /// </summary>
        public bool MarkedAsDone { get; set; }
    }

    public class EventCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
