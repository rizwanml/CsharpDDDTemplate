namespace CqrsService.Infrastructure.Provider.Messaging.AzureServiceBus.Models;

public sealed class AzureServiceBusBatchDeleteRequest
{
    public long MessageId { get; set; }
}