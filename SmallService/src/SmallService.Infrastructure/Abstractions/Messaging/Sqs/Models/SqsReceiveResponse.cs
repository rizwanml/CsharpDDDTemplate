using Amazon.SQS.Model;

namespace SmallService.Infrastructure.Abstractions.Messaging.Sqs.Models;

public sealed class SqsReceiveResponse<TModel>
{
    public Message Message { get; set; }
    public TModel Body { get; set; } 
}