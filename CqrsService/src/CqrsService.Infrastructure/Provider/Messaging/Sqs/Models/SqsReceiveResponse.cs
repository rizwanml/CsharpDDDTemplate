using Amazon.SQS.Model;

namespace CqrsService.Infrastructure.Provider.Messaging.Sqs.Models;

public sealed class SqsReceiveResponse<TModel>
{
    public Message Message { get; set; }
    public TModel Body { get; set; } 
}