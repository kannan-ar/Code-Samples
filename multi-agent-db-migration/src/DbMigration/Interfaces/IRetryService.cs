namespace DbMigration.Interfaces;

public interface IRetryService
{
    Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken);
}
