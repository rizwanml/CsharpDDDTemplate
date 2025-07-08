using System.Linq.Expressions;
using Dapper.Oracle;

namespace SmallService.Infrastructure.Abstractions.Persistence.Oracle;

public interface IOracle
{
    Task<TModel> Create<TModel>(TModel model) where TModel : class;
    Task<TModel> QuerySingle<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class;
    Task<IEnumerable<TModel>> QueryMultiple<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class;
    Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : class;
    Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class;
    Task CallProcedure(string procedureName, OracleDynamicParameters procedureParameters);
    Task<TModel> CallProcedureQuerySingle<TModel>(string procedureName, OracleDynamicParameters procedureParameters) where TModel : class;
    Task<IEnumerable<TModel>> CallProcedureQueryMultiple<TModel>(string procedureName, OracleDynamicParameters procedureParameters) where TModel : class;
    Task ExecuteCommand(string command);
    Task<TModel> ExecuteCommandQuerySingle<TModel>(string command) where TModel : class;
    Task<IEnumerable<TModel>> ExecuteCommandQueryMultiple<TModel>(string command) where TModel : class;
}