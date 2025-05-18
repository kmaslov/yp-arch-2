namespace events.Models
{
    public class UserEventDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string Action { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
