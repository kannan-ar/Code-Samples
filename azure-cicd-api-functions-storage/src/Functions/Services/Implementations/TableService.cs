using Azure.Data.Tables;
using Functions.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions.Services.Implementations
{
    public class TableService : ITableService
    {
        private readonly TableServiceClient serviceClient;

        public TableService(TableServiceClient serviceClient)
        {
            this.serviceClient = serviceClient;
        }

        public async Task Insert<T>(string tableName, T entity)
            where T : AzureTableEntity
        {
            entity.SetPartitionAndRowKeys();

            var tableClient = serviceClient.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();
            await tableClient.AddEntityAsync(entity);
        }
    }
}
