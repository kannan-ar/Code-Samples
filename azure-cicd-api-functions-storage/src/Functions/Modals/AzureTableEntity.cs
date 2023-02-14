using Azure;
using Azure.Data.Tables;
using System;

namespace Functions.Modals
{
    public abstract class AzureTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public abstract void SetPartitionAndRowKeys();
    }
}
