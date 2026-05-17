using System.ComponentModel.DataAnnotations;

namespace DbMigration.Configuration;

public sealed class MigrationOptions
{
    public const string SectionName = "Migration";

    [Required]
    public string SourceConnectionString { get; init; } = string.Empty;

    [Required]
    public string DestinationConnectionString { get; init; } = string.Empty;

    [Required]
    public string SourceQuery { get; init; } = string.Empty;

    [Required]
    public string DestinationTable { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int BatchSize { get; init; } = 1000;

    [Range(1, 10)]
    public int MaxRetryAttempts { get; init; } = 3;

    [Range(0, int.MaxValue)]
    public int RetryDelayMs { get; init; } = 500;
}
