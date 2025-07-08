using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using CqrsService.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace CqrsService.Infrastructure.Persistence.InMemory;

public sealed class InMemoryPersistence : IInMemoryPersistence
{
    private readonly IMemoryCache _memoryCache;

    public InMemoryPersistence(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<TModel> Create<TModel>(TModel model) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            if (_memoryCache.TryGetValue(collectionName, out List<TModel> cacheValue))
            {
                cacheValue.Add(model);

                _memoryCache.Set(collectionName, cacheValue);
            }
            else
            {
                List<TModel> modelList = new List<TModel>
                {
                    model
                };
                
                _memoryCache.Set(collectionName, modelList);
            }

            return model;
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(InMemoryPersistence), methodName: nameof(Create));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> QuerySingle<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            if (_memoryCache.TryGetValue(collectionName, out IEnumerable<TModel> cacheValue))
            {
                return cacheValue.SingleOrDefault(expression.Compile());
            }

            return default;
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(InMemoryPersistence), methodName: nameof(QuerySingle));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<IEnumerable<TModel>> QueryMultiple<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            if (_memoryCache.TryGetValue(collectionName, out IEnumerable<TModel> cacheValue))
            {
                return cacheValue.Where(expression.Compile()).ToList();
            }

            return Enumerable.Empty<TModel>();
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(InMemoryPersistence), methodName: nameof(QueryMultiple));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            if (_memoryCache.TryGetValue(collectionName, out List<TModel> cacheValue))
            {
                var item = cacheValue.SingleOrDefault(expression.Compile());

                if (item is null)
                {
                    return default;
                }

                cacheValue.Remove(item);
                cacheValue.Add(model);

                _memoryCache.Set(collectionName, cacheValue);
            }

            return model;
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(InMemoryPersistence), methodName: nameof(Update));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            if (_memoryCache.TryGetValue(collectionName, out List<TModel> cacheValue))
            {
                var item = cacheValue.SingleOrDefault(expression.Compile());

                if (item is null)
                {
                    return default;
                }

                cacheValue.Remove(item);

                _memoryCache.Set(collectionName, cacheValue);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(InMemoryPersistence), methodName: nameof(Delete));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
}