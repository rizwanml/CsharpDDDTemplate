namespace SmallService.Infrastructure.Options;

public sealed class MongoDbOptions
{
    public string ConnectionString { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ReadPreference { get; set; }
    public string ClusterEndpoint { get; set; }
    public string DatabaseName { get; set; }
}