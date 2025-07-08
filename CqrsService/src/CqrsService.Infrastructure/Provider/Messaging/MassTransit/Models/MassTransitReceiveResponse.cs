namespace CqrsService.Infrastructure.Provider.Messaging.MassTransit.Models;

public sealed class MassTransitReceiveResponse
{
    public string MessageId { get; set; }
}