using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using MongoDB.Driver;
using SmallService.Shared;

namespace SmallService.Infrastructure.Abstractions.Persistence.MongoDb;

public sealed class MongoDb : IMongoDb
{
    private readonly MongoDbConnectionProvider _dbConnectionProvider;

    public MongoDb(MongoDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    public async Task<TModel> Create<TModel>(TModel model) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            var context = _dbConnectionProvider.GetDbContext<TModel>(collectionName);

            await context.InsertOneAsync(model);

            return model;
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(MongoDb), methodName: nameof(Create));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> QuerySingle<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            var context = _dbConnectionProvider.GetDbContext<TModel>(collectionName);

            var response = context.AsQueryable().Where(expression).FirstOrDefault();

            return response;

        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(MongoDb), methodName: nameof(QuerySingle));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<IEnumerable<TModel>> QueryMultiple<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            var context = _dbConnectionProvider.GetDbContext<TModel>(collectionName);

            return context.AsQueryable().Where(expression).ToList();

        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(MongoDb), methodName: nameof(QueryMultiple));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            var context = _dbConnectionProvider.GetDbContext<TModel>(collectionName);

            var result = await context.ReplaceOneAsync<TModel>(expression, model);

            if (result.IsAcknowledged)
            {
                return model;
            }
            else
            {
                return default;
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(MongoDb), methodName: nameof(Update));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class
    {
        try
        {
            var collectionName = typeof(TModel).Name;

            var context = _dbConnectionProvider.GetDbContext<TModel>(collectionName);

            var result = await context.DeleteOneAsync(expression);

            return result.IsAcknowledged;

        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(MongoDb), methodName: nameof(Delete));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
}