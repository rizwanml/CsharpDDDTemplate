namespace SmallService.Infrastructure.Abstractions.Messaging.Kafka;

public interface IKafkaProducerService<TEvent>
{
    Task ProduceEvent(TEvent @event, string eventKey);
}