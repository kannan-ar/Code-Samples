using Azure.Data.Tables;

namespace LibraryApi.Helpers;

public class StorageClient : IStorageClient
{
    private readonly TableServiceClient tableClient;

    public StorageClient(TableServiceClient tableClient)
    {
        this.tableClient = tableClient;
    }

    public async Task<T> Get<T>(string tableName, string partitionKey, string rowKey)
        where T : class, ITableEntity
    {
        var table = tableClient.GetTableClient(tableName);

        var response = await table.GetEntityAsync<T>(partitionKey, rowKey);

        return response.Value;
    }
}
