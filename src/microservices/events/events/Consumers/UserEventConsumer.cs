using System.Text.Json;
using events.Models;

namespace events.Consumers
{
    public class UserEventConsumer(string bootstrapServers, string topic, ILoggerFactory loggerFactory)
        : BaseConsumer(bootstrapServers, topic, loggerFactory)
    {
        protected override async Task ProcessMessage(string message)
        {
            try
            {
                var userEvent = JsonSerializer.Deserialize<Event>(message);
                _logger.LogInformation($"Received user event: {userEvent.Id}, Type: {userEvent.Type}, Payload: {userEvent.Payload}");
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error deserializing user event: {ex.Message}");
            }
        }
    }
}
