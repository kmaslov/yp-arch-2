namespace events.Models
{
    public class Event
    {
        public string Id { get; set; } = null!;
        public string Type { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public object Payload { get; set; } = null!;
    }
}
