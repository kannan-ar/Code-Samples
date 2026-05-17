using System.ComponentModel.DataAnnotations;
using DbMigration.Configuration;
using Xunit;

namespace DbMigration.Tests;

public sealed class MigrationOptionsTests
{
    private static List<ValidationResult> Validate(MigrationOptions options)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            options,
            new ValidationContext(options),
            results,
            validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Validate_AllRequiredFieldsPopulated_NoErrors()
    {
        var options = new MigrationOptions
        {
            SourceConnectionString = "Server=s;",
            DestinationConnectionString = "Server=d;",
            SourceQuery = "SELECT 1",
            DestinationTable = "Target"
        };

        Assert.Empty(Validate(options));
    }

    [Theory]
    [InlineData(nameof(MigrationOptions.SourceConnectionString))]
    [InlineData(nameof(MigrationOptions.DestinationConnectionString))]
    [InlineData(nameof(MigrationOptions.SourceQuery))]
    [InlineData(nameof(MigrationOptions.DestinationTable))]
    public void Validate_RequiredFieldEmpty_ReturnsValidationError(string emptyProperty)
    {
        var options = new MigrationOptions
        {
            SourceConnectionString      = emptyProperty == nameof(MigrationOptions.SourceConnectionString)      ? "" : "s",
            DestinationConnectionString = emptyProperty == nameof(MigrationOptions.DestinationConnectionString) ? "" : "d",
            SourceQuery                 = emptyProperty == nameof(MigrationOptions.SourceQuery)                 ? "" : "SELECT 1",
            DestinationTable            = emptyProperty == nameof(MigrationOptions.DestinationTable)            ? "" : "T"
        };

        var errors = Validate(options);

        Assert.Contains(errors, e => e.MemberNames.Contains(emptyProperty));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_BatchSizeLessThanOne_ReturnsValidationError(int batchSize)
    {
        var options = new MigrationOptions
        {
            SourceConnectionString = "s",
            DestinationConnectionString = "d",
            SourceQuery = "SELECT 1",
            DestinationTable = "T",
            BatchSize = batchSize
        };

        var errors = Validate(options);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(MigrationOptions.BatchSize)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Validate_MaxRetryAttemptsOutOfRange_ReturnsValidationError(int maxRetry)
    {
        var options = new MigrationOptions
        {
            SourceConnectionString = "s",
            DestinationConnectionString = "d",
            SourceQuery = "SELECT 1",
            DestinationTable = "T",
            MaxRetryAttempts = maxRetry
        };

        var errors = Validate(options);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(MigrationOptions.MaxRetryAttempts)));
    }

    [Fact]
    public void Validate_DefaultValues_AreValid()
    {
        // Verifies that the built-in defaults (BatchSize=1000, MaxRetryAttempts=3, RetryDelayMs=500)
        // all pass their range constraints out of the box.
        var options = new MigrationOptions
        {
            SourceConnectionString = "s",
            DestinationConnectionString = "d",
            SourceQuery = "SELECT 1",
            DestinationTable = "T"
            // BatchSize, MaxRetryAttempts, RetryDelayMs use property defaults
        };

        Assert.Empty(Validate(options));
    }
}
