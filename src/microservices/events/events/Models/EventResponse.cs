namespace events.Models
{
    public class EventResponse
    {
        public string Status { get; set; } = null!;
        public int Partition { get; set; }
        public long Offset { get; set; }
        public Event Event { get; set; } = null!;
    }
}
