using System.Linq.Expressions;
using Dapper;

namespace SmallService.Infrastructure.Abstractions.Persistence.PostgreSql;

public interface IPostgreSql
{
    Task<TModel> Create<TModel>(TModel model) where TModel : class;
    Task<TModel> QuerySingle<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class;
    Task<IEnumerable<TModel>> QueryMultiple<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class;
    Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : class;
    Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class;
    Task CallProcedure(string procedureName, DynamicParameters procedureParameters);
    Task<TModel> CallProcedureQuerySingle<TModel>(string procedureName, DynamicParameters procedureParameters) where TModel : class;
    Task<IEnumerable<TModel>> CallProcedureQueryMultiple<TModel>(string procedureName, DynamicParameters procedureParameters) where TModel : class;
    Task ExecuteCommand(string command);
    Task<TModel> ExecuteCommandQuerySingle<TModel>(string command) where TModel : class;
    Task<IEnumerable<TModel>> ExecuteCommandQueryMultiple<TModel>(string command) where TModel : class;
}