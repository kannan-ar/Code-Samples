using Microsoft.Data.SqlClient;

namespace DatabaseImporter.Services;

internal class SqlExecutor
{
    private readonly string _connection;
    private SqlConnection dbConnection;

    public SqlExecutor(string connection)
    {
        _connection = connection;
    }

    public void Open()
    {
        dbConnection = new SqlConnection(_connection);
        dbConnection.Open();
    }

    public async Task<(int, SqlTransaction, bool)> Execute(string sql)
    {
        var transaction = dbConnection.BeginTransaction();

        try
        {
            using SqlCommand cmd = new(sql, dbConnection, transaction);
            return (await cmd.ExecuteNonQueryAsync(), transaction, true);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return (0, transaction, false);
        }
       
    }
}