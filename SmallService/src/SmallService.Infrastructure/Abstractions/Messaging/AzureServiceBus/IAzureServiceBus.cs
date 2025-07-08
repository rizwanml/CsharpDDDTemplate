using SmallService.Infrastructure.Abstractions.Messaging.AzureServiceBus.Models;

namespace SmallService.Infrastructure.Abstractions.Messaging.AzureServiceBus;

public interface IAzureServiceBus
{
    Task Send<TModel>(string connectionString, string queueName, TModel message, AzureServiceBusMessageOptions? options = null) where TModel : class;
        
    Task<IReadOnlyList<AzureServiceBusReceiveResponse<TModel>>> Receive<TModel>(string connectionString, string queueName, string? identifier = null, int? prefetchCount = null, int maxNumberOfMessages = 10, int waitTimeSeconds = 20) where TModel : class;
       
    Task<bool> Delete(string connectionString, string queueName, long messageId);
        
    Task BatchDelete(string connectionString, string queueName, IReadOnlyList<AzureServiceBusBatchDeleteRequest> azureServiceBusBatchDeleteRequest);
}