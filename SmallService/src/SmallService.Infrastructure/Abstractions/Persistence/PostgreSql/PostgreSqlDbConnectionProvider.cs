using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using SmallService.Infrastructure.Options;

namespace SmallService.Infrastructure.Abstractions.Persistence.PostgreSql;

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