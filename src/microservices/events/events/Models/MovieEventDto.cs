namespace events.Models
{
    public class MovieEventDto
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = null!;
        public string Action { get; set; } = null!;
        public int? UserId { get; set; }
        public float? Rating { get; set; }
        public IEnumerable<string>? Genres { get; set; }
        public string? Description { get; set; }
    }
}
