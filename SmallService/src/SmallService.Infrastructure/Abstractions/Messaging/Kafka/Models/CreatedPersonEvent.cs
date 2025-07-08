namespace SmallService.Infrastructure.Abstractions.Messaging.Kafka.Models;

public class CreatedPersonEvent
{
    public string Id { get; set; }  = Guid.NewGuid().ToString();
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}