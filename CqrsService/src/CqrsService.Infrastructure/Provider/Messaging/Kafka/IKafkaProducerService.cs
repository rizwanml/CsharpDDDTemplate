using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsService.Infrastructure.Provider.Messaging.Kafka;

public interface IKafkaProducerService<TEvent>
{
    Task ProduceEvent(TEvent @event, string eventKey);
}