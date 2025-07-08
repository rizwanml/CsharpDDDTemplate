using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallService.Infrastructure.Provider.Messaging.Kafka.Models
{
    public abstract class BaseEvent
    {
        /// <summary>
        /// Not unique to the instance of the object, this Id is relative to the context of the object, i.e. CustomerId, OrderId, MeterId, etc..
        /// </summary>
        public string Id { get; set; }
    }
}
