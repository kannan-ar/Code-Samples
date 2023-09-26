using Azure.Data.Tables;

namespace LibraryApi.Helpers;

public interface IStorageClient
{
    Task<T> Get<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity;
}
