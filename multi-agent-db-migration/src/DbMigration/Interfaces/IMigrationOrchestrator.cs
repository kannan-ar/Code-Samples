using DbMigration.Models;

namespace DbMigration.Interfaces;

public interface IMigrationOrchestrator
{
    Task<MigrationResult> RunAsync(CancellationToken cancellationToken);
}
