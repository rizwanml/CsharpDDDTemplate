using KafkaFlow;
using SmallService.Infrastructure.Abstractions.Messaging.Kafka.Models;

namespace SmallService.Infrastructure.Abstractions.Messaging.Kafka;

public class KafkaProducerService : IKafkaProducerService<CreatedPersonEvent>
{
    private readonly IMessageProducer<CreatedPersonEvent> _personProducer;

    public KafkaProducerService(IMessageProducer<CreatedPersonEvent> personProducer)
    {
            _personProducer = personProducer;
        }

    public async Task ProduceEvent(CreatedPersonEvent message, string messageKey)
    {
            await _personProducer.ProduceAsync(messageKey: messageKey, messageValue: message);
        }
}