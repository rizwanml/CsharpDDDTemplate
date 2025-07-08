using Azure.Messaging.ServiceBus;

namespace CqrsService.Infrastructure.Provider.Messaging.AzureServiceBus.Models;

public sealed class AzureServiceBusReceiveResponse<TModel>
{
    public ServiceBusReceivedMessage Message { get; set; }
    public TModel Body { get; set; } 
}