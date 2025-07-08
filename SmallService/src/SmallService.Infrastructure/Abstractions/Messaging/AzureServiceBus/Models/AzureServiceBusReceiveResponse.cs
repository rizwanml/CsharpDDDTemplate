using Azure.Messaging.ServiceBus;

namespace SmallService.Infrastructure.Abstractions.Messaging.AzureServiceBus.Models;

public sealed class AzureServiceBusReceiveResponse<TModel>
{
    public ServiceBusReceivedMessage Message { get; set; }
    public TModel Body { get; set; } 
}