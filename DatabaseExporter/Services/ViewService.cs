using DatabaseExporter.Models;

namespace DatabaseExporter.Services;

public class ViewService
{
    private SqlExecutor sql;

    public void Open()
    {
        sql = new SqlExecutor();
        sql.Open(Settings.ConnectionString);
    }

    public async Task<IEnumerable<string>> GetViewNames()
    {
        return await sql.Select<string>("SELECT name FROM sys.views");
    }

    public async Task<string> GetViewDefinition(string viewName)
    {
        return await sql.Get<string>("SELECT m.definition FROM sys.views v INNER JOIN sys.sql_modules m ON m.object_id = v.object_id WHERE name = @viewName",
            new
            {
                viewName
            });
    }
}