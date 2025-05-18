using System.Text.Json;
using events.Models;

namespace events.Consumers
{
    public class PaymentEventConsumer(string bootstrapServers, string topic, ILoggerFactory loggerFactory)
        : BaseConsumer(bootstrapServers, topic, loggerFactory)
    {
        protected override async Task ProcessMessage(string message)
        {
            try
            {
                var paymentEvent = JsonSerializer.Deserialize<Event>(message);
                _logger.LogInformation($"Received payment event: {paymentEvent.Id}, Type: {paymentEvent.Type}, Payload: {paymentEvent.Payload}");
                // Here you can add additional processing logic
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error deserializing payment event: {ex.Message}");
            }
        }
    }
}
