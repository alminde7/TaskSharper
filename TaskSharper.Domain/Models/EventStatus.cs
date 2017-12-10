namespace TaskSharper.Domain.Models
{
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
}