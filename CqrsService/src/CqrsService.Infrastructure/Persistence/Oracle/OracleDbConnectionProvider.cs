using System.Data;
using CqrsService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace CqrsService.Infrastructure.Persistence.Oracle;

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