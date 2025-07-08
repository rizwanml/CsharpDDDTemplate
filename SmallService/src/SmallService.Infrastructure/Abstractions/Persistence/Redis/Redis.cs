using System.Runtime.ExceptionServices;
using System.Text.Json;
using SmallService.Shared;
using StackExchange.Redis;

namespace SmallService.Infrastructure.Abstractions.Persistence.Redis;

public sealed class Redis : IRedis
{
    private readonly IConnectionMultiplexer _redis;

    public Redis(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task Set(string key, string value)
    {
        try
        {
            var redisDatabase = _redis.GetDatabase();

            await redisDatabase.StringSetAsync(key, value);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(Redis), methodName: nameof(Set));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task Set<TModel>(string key, TModel model) where TModel : class
    {
        try
        {
            var redisDatabase = _redis.GetDatabase();

            var value = JsonSerializer.Serialize(model);
            await redisDatabase.StringSetAsync(key, value);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(Redis), methodName: nameof(Set));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task Set(string key, string value, int ttlInSeconds)
    {
        try
        {
            var redisDatabase = _redis.GetDatabase();

            await redisDatabase.StringSetAsync(key, value);
            await redisDatabase.KeyExpireAsync(key, TimeSpan.FromSeconds(ttlInSeconds));
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(Redis), methodName: nameof(Set));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task Set<TModel>(string key, TModel model, int ttlInSeconds) where TModel : class
    {
        try
        {
            var redisDatabase = _redis.GetDatabase();

            var value = JsonSerializer.Serialize(model);
            await redisDatabase.StringSetAsync(key, value);

            await redisDatabase.KeyExpireAsync(key, TimeSpan.FromSeconds(ttlInSeconds));
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(Redis), methodName: nameof(Set));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<string> Get(string key)
    {
        try
        {
            var redisDatabase = _redis.GetDatabase();

            return await redisDatabase.StringGetAsync(key);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(Redis), methodName: nameof(Get));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> Get<TModel>(string key) where TModel : class
    {
        try
        {
            var redisDatabase = _redis.GetDatabase();

            var response = await redisDatabase.StringGetAsync(key);

            if (response.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<TModel>(response);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(Redis), methodName: nameof(Get));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<bool> Delete(string key)
    {
        try
        {
            var redisDatabase = _redis.GetDatabase();

            return await redisDatabase.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(Redis), methodName: nameof(Delete));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
}