using System.Data;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using SmallService.Infrastructure.Options;

namespace SmallService.Infrastructure.Abstractions.Persistence.Oracle;

public sealed class OracleDbConnectionProvider
{
    private readonly IOptions<OracleOptions> _options;

    public OracleDbConnectionProvider(IOptions<OracleOptions> options)
    {
        _options = options;
    }

    public IDbConnection GetDbConnection()
    {
        return new OracleConnection(_options.Value.ConnectionString);
    }
}