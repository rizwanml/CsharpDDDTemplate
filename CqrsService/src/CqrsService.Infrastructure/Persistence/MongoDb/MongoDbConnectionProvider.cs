using CqrsService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CqrsService.Infrastructure.Persistence.MongoDb;

public sealed class MongoDbConnectionProvider
{
    private readonly IOptions<MongoDbOptions> _options;
    private readonly IMongoClient _mongoClient;
    private IMongoDatabase _mongoDatabase;

    public MongoDbConnectionProvider(IOptions<MongoDbOptions> options, IMongoClient mongoClient)
    {
        _options = options;
        _mongoClient = mongoClient;
    }

    public void GetMongoDatabase()
    {
        _mongoDatabase = _mongoClient.GetDatabase(_options.Value.DatabaseName);
    }

    public IMongoCollection<TModel> GetDbContext<TModel>(string collectionName)
    {
        if (_mongoDatabase is null)
        {
            GetMongoDatabase();
        }

        return _mongoDatabase.GetCollection<TModel>(collectionName);
    }
}