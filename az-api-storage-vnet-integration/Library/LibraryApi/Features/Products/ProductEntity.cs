using Azure;
using Azure.Data.Tables;

namespace LibraryApi.Features.Products
{
    public class ProductEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Name { get; set; }
    }
}
