namespace events.Models
{
    public class PaymentEventDto
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public string? MethodType { get; set; }
    }
}
