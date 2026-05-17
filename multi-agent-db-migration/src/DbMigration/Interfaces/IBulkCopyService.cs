using System.Data;

namespace DbMigration.Interfaces;

public interface IBulkCopyService
{
    Task CopyAsync(DataTable batch, CancellationToken cancellationToken);
}
