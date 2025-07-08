namespace SmallService.Infrastructure.Options;

public class KafkaOptions
{
    public string CreatedPersonEventTopicName { get; set; }
    public string ConsumerGroupId { get; set; }
    public string SaslUsername { get; set; }
    public string SaslPassword { get; set; }
    public string SchemaRegUname { get; set; }
    public string SchemaRegPword { get; set; }
    public Uri SchemaRegistryUrl { get; set; }
    public string SchemaRegBasicAuth => $"{SchemaRegUname}:{SchemaRegPword}";
    public string[] BoostrapServers { get; set; }
}

