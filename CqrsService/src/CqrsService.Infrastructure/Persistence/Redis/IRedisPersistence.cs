namespace CqrsService.Infrastructure.Persistence.Redis;

public interface IRedisPersistence
{
    Task Set(string key, string value);
    Task Set<TModel>(string key, TModel model) where TModel : class;
    Task Set(string key, string value, int ttlInSeconds);
    Task Set<TModel>(string key, TModel model, int ttlInSeconds) where TModel : class;
    Task<string> Get(string key);
    Task<TModel> Get<TModel>(string key) where TModel : class;
    Task<bool> Delete(string key);
}