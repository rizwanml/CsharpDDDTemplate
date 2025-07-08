using CqrsService.Infrastructure.Provider.Messaging.Kafka.Models;
using KafkaFlow;

namespace CqrsService.Infrastructure.Provider.Messaging.Kafka;

public class KafkaConsumerService : IMessageHandler<CreatedPersonEvent>
{
    public KafkaConsumerService()
    {

        }

    public Task Handle(IMessageContext context, CreatedPersonEvent message)
    {
            Console.WriteLine(
              "Partition: {0} | Offset: {1} | Message: {2} | Json",
              context.ConsumerContext.Partition,
              context.ConsumerContext.Offset,
              message);
            return Task.CompletedTask;
        }
}