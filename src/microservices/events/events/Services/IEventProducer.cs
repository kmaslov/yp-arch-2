using Confluent.Kafka;
using events.Models;

namespace events.Services
{
    public interface IEventProducer
    {
        Task<DeliveryResult<string, string>> ProduceAsync(string topic, Event eventData);
    }
}
