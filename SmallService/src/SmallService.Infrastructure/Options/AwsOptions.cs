namespace SmallService.Infrastructure.Options;

public sealed class AwsOptions
{
    public string AccessKey { get; set; }
    public string Region { get; set; }
    public string SecretKey { get; set; }
    public Uri ServiceUrl { get; set; }
}