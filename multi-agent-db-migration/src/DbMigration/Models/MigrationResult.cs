namespace DbMigration.Models;

public sealed class MigrationResult
{
    public int TotalRowsCopied { get; init; }
    public int TotalBatches { get; init; }
    public TimeSpan Elapsed { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    public bool IsSuccess => Errors.Count == 0;
}
