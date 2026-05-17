using DbMigration.Configuration;
using DbMigration.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DbMigration.Services;

public sealed class RetryService : IRetryService
{
    // Known transient SQL Server / Azure SQL error numbers.
    private static readonly IReadOnlySet<int> TransientErrorNumbers = new HashSet<int>
    {
        -2,    // Client timeout
        1205,  // Deadlock victim
        4060,  // Cannot open database
        40197, // Service error processing request
        40501, // Service busy
        40613, // Database unavailable
        49920  // Too many requests
    };

    private readonly MigrationOptions _options;
    private readonly ILogger<RetryService> _logger;

    public RetryService(IOptions<MigrationOptions> options, ILogger<RetryService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt <= _options.MaxRetryAttempts; attempt++)
        {
            try
            {
                await operation(cancellationToken);
                return;
            }
            catch (SqlException ex) when (attempt < _options.MaxRetryAttempts && IsTransient(ex))
            {
                var delay = TimeSpan.FromMilliseconds(
                    _options.RetryDelayMs * Math.Pow(2, attempt));

                var transientErrorNumber = GetTransientErrorNumber(ex);
                _logger.LogWarning(
                    "Transient SQL error {ErrorNumber} on attempt {Attempt}/{MaxAttempts}. Retrying in {DelayMs}ms.",
                    transientErrorNumber, attempt + 1, _options.MaxRetryAttempts, delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private static bool IsTransient(SqlException ex)
    {
        foreach (SqlError error in ex.Errors)
        {
            if (TransientErrorNumbers.Contains(error.Number))
                return true;
        }
        return false;
    }

    private static int GetTransientErrorNumber(SqlException ex)
    {
        foreach (SqlError error in ex.Errors)
        {
            if (TransientErrorNumbers.Contains(error.Number))
                return error.Number;
        }
        return ex.Number;
    }
}
