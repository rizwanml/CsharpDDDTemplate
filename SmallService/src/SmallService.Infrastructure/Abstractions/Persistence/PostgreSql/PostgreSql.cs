using System.Data;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using Dapper;
using Dapper.Contrib.Extensions;
using SmallService.Shared;

namespace SmallService.Infrastructure.Abstractions.Persistence.PostgreSql;

public sealed class PostgreSql : IPostgreSql
{
    private readonly PostgreSqlDbConnectionProvider _dbConnectionProvider;

    public PostgreSql(PostgreSqlDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
        SqlMapper.Settings.CommandTimeout = 30;
    }

    public async Task<TModel> Create<TModel>(TModel model) where TModel : class
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                await connection.InsertAsync(model);

                return model;
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(Create));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> QuerySingle<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                return await connection.GetAsync<TModel>(expression.Compile());
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(QuerySingle));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<IEnumerable<TModel>> QueryMultiple<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                return await connection.GetAsync<IEnumerable<TModel>>(expression.Compile());
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(QueryMultiple));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : class
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                var item = await connection.GetAsync<TModel>(expression.Compile());

                if (item == null)
                {
                    return default;
                }

                item = model;

                await connection.UpdateAsync(item);

                return model;
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(Update));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<bool> Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                var item = await connection.GetAsync<TModel>(expression.Compile());

                if (item == null)
                {
                    return default;
                }

                await connection.DeleteAsync(item);

                return true;
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(Delete));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task CallProcedure(string procedureName, DynamicParameters procedureParameters)
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                await connection.ExecuteAsync(procedureName, param: procedureParameters, commandType: CommandType.StoredProcedure);
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(CallProcedure), operationDetail: procedureName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> CallProcedureQuerySingle<TModel>(string procedureName, DynamicParameters procedureParameters) where TModel : class
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                return await connection.QuerySingleAsync<TModel>(procedureName, param: procedureParameters, commandType: CommandType.StoredProcedure);
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(CallProcedureQuerySingle), operationDetail: procedureName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<IEnumerable<TModel>> CallProcedureQueryMultiple<TModel>(string procedureName, DynamicParameters procedureParameters) where TModel : class
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                return await connection.QueryAsync<IEnumerable<TModel>>(procedureName, param: procedureParameters, commandType: CommandType.StoredProcedure) as IEnumerable<TModel>;
            }

            return Enumerable.Empty<TModel>();
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(CallProcedureQueryMultiple), operationDetail: procedureName);
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task ExecuteCommand(string command)
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                await connection.ExecuteAsync(command);
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(ExecuteCommand));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<TModel> ExecuteCommandQuerySingle<TModel>(string command) where TModel : class
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                return await connection.QuerySingleAsync(command) as TModel;
            }
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(ExecuteCommandQuerySingle));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }

    public async Task<IEnumerable<TModel>> ExecuteCommandQueryMultiple<TModel>(string command) where TModel : class
    {
        try
        {
            using (IDbConnection connection = _dbConnectionProvider.GetDbConnection())
            {
                return await connection.QueryMultipleAsync(command) as IEnumerable<TModel>;
            }

            return Enumerable.Empty<TModel>();
        }
        catch (Exception ex)
        {
            ExceptionHandlerHelpers.HandleException(exception: ex, className: nameof(PostgreSql), methodName: nameof(ExecuteCommandQueryMultiple));
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw; //to satisfy the compiler
        }
    }
}