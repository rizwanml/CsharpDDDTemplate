using CqrsService.Infrastructure.Provider.Messaging.Kafka.Models;
using KafkaFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsService.Infrastructure.Provider.Messaging.Kafka;

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