using DatabaseExporter.Models;

namespace DatabaseExporter.Services;

public class SynonymsService
{
    private SqlExecutor sql;

    public void Open()
    {
        sql = new SqlExecutor();
        sql.Open(Settings.ConnectionString);
    }

    public async Task<IEnumerable<Synonym>> GetSynonyms()
    {
        return await sql.Select<Synonym>("SELECT [name] [Name],base_object_name [Definition] FROM sys.synonyms");
    }
}
