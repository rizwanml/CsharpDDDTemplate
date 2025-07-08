namespace SmallService.Infrastructure.Abstractions.Messaging.AzureServiceBus.Models;

public sealed class AzureServiceBusBatchDeleteRequest
{
    public long MessageId { get; set; }
}