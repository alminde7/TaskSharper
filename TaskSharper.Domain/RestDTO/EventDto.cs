using System;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.RestDTO
{
    public class EventDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public EventType EventType { get; set; }
        public EventStatus EventStatus { get; set; }
        public EventCategory EventCategory { get; set; }
    }
}
