using System.Data;
using CqrsService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CqrsService.Infrastructure.Persistence.PostgreSql;

public sealed class PostgreSqlDbConnectionProvider
{
    private readonly IOptions<PostgreSqlOptions> _options;

    public PostgreSqlDbConnectionProvider(IOptions<PostgreSqlOptions> options)
    {
        _options = options;
    }

    public IDbConnection GetDbConnection()
    {
        return new NpgsqlConnection(_options.Value.ConnectionString);
    }
}