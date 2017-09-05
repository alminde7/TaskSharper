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
        /// "confirmed" - The event is confirmed (default).
        /// "tentative" - The event is tentatively confirmed.
        /// "cancelled" - The event is cancelled.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Creation time of the event. Read-only.
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// First start time for a recurring event. Optional. TODO: Should this be included?
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
    }
}
