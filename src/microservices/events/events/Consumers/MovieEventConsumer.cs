using System.Text.Json;
using events.Models;

namespace events.Consumers
{
    public class MovieEventConsumer(string bootstrapServers, string topic, ILoggerFactory loggerFactory)
        : BaseConsumer(bootstrapServers, topic, loggerFactory)
    {
        protected override async Task ProcessMessage(string message)
        {
            try
            {
                var movieEvent = JsonSerializer.Deserialize<Event>(message);
                _logger.LogInformation($"Received movie event: {movieEvent.Id}, Type: {movieEvent.Type}, Payload: {movieEvent.Payload}");
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error deserializing movie event: {ex.Message}");
            }
        }
    }
}
