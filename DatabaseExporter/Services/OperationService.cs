using DatabaseExporter.Models;
using System.Text.Json;

namespace DatabaseExporter.Services;

internal static class OperationService
{
    private static string FileName => $"{nameof(OperationService)}.json";

    private async static Task<OperationInfo?> Get()
    {
        if (!File.Exists(FileName)) return new OperationInfo();

        using StreamReader sr = new(FileName);
        return JsonSerializer.Deserialize<OperationInfo>(await sr.ReadToEndAsync());
    }

    private async static Task Set(OperationInfo info)
    {
        using StreamWriter sw = new(FileName);

        var json = JsonSerializer.Serialize(info);

        await sw.WriteAsync(json);
    }

    public async static Task<long> GetTableRecordFetched(string  tableName)
    {
        var operationInfo = await Get();

        if (operationInfo.DataInfo == null) return 0;

        foreach(var info in operationInfo!.DataInfo)
        {
            if(info.TableName == tableName) return info.RecordFetched;
        }

        return 0;
    }

    public async static Task SaveTableRecordFetched(string tableName, long count)
    {
        var operationInfo = await Get();

        if(operationInfo.DataInfo == null)
        {
            operationInfo.DataInfo =
            [
                new()
                {
                    TableName = tableName,
                    RecordFetched = count
                }
            ];

            await Set(operationInfo);
            return;
        }

        foreach (var info in operationInfo!.DataInfo)
        {
            if (info.TableName == tableName)
            {
                info.RecordFetched = count;
                await Set(operationInfo);
                return;
            }
        }

        
    }
}