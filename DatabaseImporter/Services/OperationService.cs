using System.Text.Json;

namespace DatabaseImporter.Services;

internal static class OperationService
{
    private static string FileName => $"{nameof(OperationService)}.json";

    private async static Task<TableInfo?> Get()
    {
        if (!File.Exists(FileName)) return null;

        using StreamReader sr = new(FileName);
        return JsonSerializer.Deserialize<TableInfo>(await sr.ReadToEndAsync());
    }

    private async static Task Set(TableInfo info)
    {
        using StreamWriter sw = new(FileName);

        var json = JsonSerializer.Serialize(info);

        await sw.WriteAsync(json);
    }

    public async static Task<long> GetTableRecordAffected(string tableName)
    {
        var tableInfo = await Get();

        return tableInfo == null || tableInfo.TableName != tableName ? 0 : tableInfo.IterationDone;
    }

    public async static Task SaveTableRecordAffected(string tableName, long count)
    {
        var tableInfo = await Get();

        if (tableInfo == null)
        {
            tableInfo = new TableInfo();
            tableInfo.TableName = tableName;
        }

        tableInfo.IterationDone = count;

        await Set(tableInfo);
    }
}