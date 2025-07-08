namespace SmallService.Infrastructure.Abstractions.Messaging.Sqs.Models;

public sealed class SqsBatchDeleteRequest
{
    public string MessageId { get; set; }
    public string ReceiptHandle { get; set; } 
}