using Confluent.Kafka;
using events.Models;
using System.Text.Json;

namespace events.Services
{
    public class KafkaEventProducer : IEventProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaEventProducer> _logger;

        public KafkaEventProducer(string bootstrapServers, ILogger<KafkaEventProducer> logger)
        {
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger = logger;
        }

        public async Task<DeliveryResult<string, string>> ProduceAsync(string topic, Event eventData)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key = eventData.Id,
                    Value = JsonSerializer.Serialize(eventData)
                };

                var result = await _producer.ProduceAsync(topic, message);
                _logger.LogInformation($"Delivered message to {result.TopicPartitionOffset}");
                return result;
            }
            catch (ProduceException<string, string> e)
            {
                _logger.LogError($"Delivery failed: {e.Error.Reason}");
                throw;
            }
        }
    }
}
