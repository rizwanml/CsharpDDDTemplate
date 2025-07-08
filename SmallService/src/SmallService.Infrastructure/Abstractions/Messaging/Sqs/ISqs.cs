using SmallService.Infrastructure.Abstractions.Messaging.Sqs.Models;

namespace SmallService.Infrastructure.Abstractions.Messaging.Sqs;

public interface ISqs
{
    Task Send<TModel>(string queueName, TModel message, int? delaySeconds = null, string? messageDeduplicationId = null, string? messageGroupId = null, Dictionary<string, string>? messageAttributes = null) where TModel : class;

    Task<IReadOnlyList<SqsReceiveResponse<TModel>>> Receive<TModel>(string queueName, int maxNumberOfMessages = 10, int waitTimeSeconds = 20, int visibilityTimeout = 30) where TModel : class;

    Task<bool> Delete(string queueName, string messageId);

    Task BatchDelete(string queueName, IReadOnlyList<SqsBatchDeleteRequest> sqsBatchDeleteRequest);
}