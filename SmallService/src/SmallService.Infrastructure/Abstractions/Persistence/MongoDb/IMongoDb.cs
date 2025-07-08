using System.Linq.Expressions;

namespace SmallService.Infrastructure.Abstractions.Persistence.MongoDb;

public interface IMongoDb
{
    Task<TModel> Create<TModel>(TModel model) where TModel : class;
    Task<TModel> QuerySingle<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class;
    Task<IEnumerable<TModel>> QueryMultiple<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class;
    Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : class;
    Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class;
}