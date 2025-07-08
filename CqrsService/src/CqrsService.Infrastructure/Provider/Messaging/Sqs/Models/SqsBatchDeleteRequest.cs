namespace CqrsService.Infrastructure.Provider.Messaging.Sqs.Models;

public sealed class SqsBatchDeleteRequest
{
    public string MessageId { get; set; }
    public string ReceiptHandle { get; set; } 
}