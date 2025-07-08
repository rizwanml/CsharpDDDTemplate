using CqrsService.Infrastructure.Provider.Messaging.Sqs.Models;

namespace CqrsService.Infrastructure.Provider.Messaging.Sqs;

public interface ISqsProvider
{
    Task Send<TModel>(string queueName, TModel message, int? delaySeconds = null, string? messageDeduplicationId = null, string? messageGroupId = null, Dictionary<string, string>? messageAttributes = null) where TModel : class;
        
    Task<IReadOnlyList<SqsReceiveResponse<TModel>>> Receive<TModel>(string queueName, int maxNumberOfMessages = 10, int waitTimeSeconds = 20, int visibilityTimeout = 30) where TModel : class;
       
    Task<bool> Delete(string queueName, string messageId);
        
    Task BatchDelete(string queueName, IReadOnlyList<SqsBatchDeleteRequest> sqsBatchDeleteRequest);
}