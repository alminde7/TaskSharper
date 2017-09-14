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
        /// Possible types an event can be.
        /// </summary>
        public enum EventType
        {
            None,
            Appointment,
            Task
        }

        /// <summary>
        /// Possible statuses that can be assigned to an event.
        /// </summary>
        public enum EventStatus
        {
            Confirmed,
            Tentative,
            Cancelled,
            Completed
        }

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

        /// <summary>
        /// The (inclusive) start time of the event.
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// The (exclusive) end time of the event
        /// </summary>
        public DateTime? End { get; set; }

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

        /// <summary>
        /// Reminders for the event. In minutes.
        /// </summary>
        public List<int?> Reminders { get; set; }
    }
}
