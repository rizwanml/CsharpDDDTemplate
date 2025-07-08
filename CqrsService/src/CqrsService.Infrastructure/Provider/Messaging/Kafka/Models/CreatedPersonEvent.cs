using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsService.Infrastructure.Provider.Messaging.Kafka.Models;

public class CreatedPersonEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}