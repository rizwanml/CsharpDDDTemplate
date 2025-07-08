namespace SmallService.Infrastructure.Abstractions.Messaging.AzureServiceBus.Models;

public sealed class AzureServiceBusMessageOptions
{
    public string? Identifier { get; set; }
    public string? ContentType { get; set; }
    public string? CorrelationId { get; set; }
    public string? MessageId { get; set; }
    public string? PartitionKey { get; set; }
    public string? ReplyTo { get; set; }
    public string? ReplyToSessionId { get; set; }
    public DateTimeOffset? ScheduledEnqueueTime { get; set; }
    public string? Subject { get; set; }
    public int? TimeToLiveSeconds { get; set; }
    public string? TransactionPartitionKey { get; set; }
}