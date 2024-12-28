using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatabaseExporter.Services;

internal class SqlExecutor
{
    private IDbConnection _connection;

    public IDbConnection Open(string connectionString)
    {
        _connection = new SqlConnection(connectionString);

        _connection.Open();

        return _connection;
    }

    public async Task<IEnumerable<T>> Select<T>(string sql, object? parameters = null)
    {
        return await _connection.QueryAsync<T>(sql, parameters);
    }

    public async Task<T> Get<T>(string sql, object? parameters = null)
    {
        return await _connection.QuerySingleAsync<T>(sql, parameters);
    }

    public async Task<IEnumerable<dynamic>> Select(string sql)
    {
        return await _connection.QueryAsync(sql);
    }
}