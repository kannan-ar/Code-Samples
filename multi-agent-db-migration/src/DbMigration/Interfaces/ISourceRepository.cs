using System.Data;

namespace DbMigration.Interfaces;

public interface ISourceRepository
{
    IAsyncEnumerable<DataTable> ReadBatchesAsync(int batchSize, CancellationToken cancellationToken);
}
