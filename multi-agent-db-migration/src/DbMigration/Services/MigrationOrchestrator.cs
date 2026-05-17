using System.Diagnostics;
using DbMigration.Configuration;
using DbMigration.Interfaces;
using DbMigration.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DbMigration.Services;

public sealed class MigrationOrchestrator : IMigrationOrchestrator
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IBulkCopyService _bulkCopyService;
    private readonly IRetryService _retryService;
    private readonly MigrationOptions _options;
    private readonly ILogger<MigrationOrchestrator> _logger;

    public MigrationOrchestrator(
        ISourceRepository sourceRepository,
        IBulkCopyService bulkCopyService,
        IRetryService retryService,
        IOptions<MigrationOptions> options,
        ILogger<MigrationOrchestrator> logger)
    {
        _sourceRepository = sourceRepository;
        _bulkCopyService = bulkCopyService;
        _retryService = retryService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<MigrationResult> RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Migration started. Source query: {SourceQuery} | Destination: {DestinationTable} | BatchSize: {BatchSize}",
            _options.SourceQuery, _options.DestinationTable, _options.BatchSize);

        var stopwatch = Stopwatch.StartNew();
        var totalRowsCopied = 0;
        var batchNumber = 0;

        await foreach (var batch in _sourceRepository
            .ReadBatchesAsync(_options.BatchSize, cancellationToken)
            .ConfigureAwait(false))
        {
            batchNumber++;
            var rowCount = batch.Rows.Count;

            _logger.LogInformation(
                "Batch {BatchNumber} started: {RowCount} rows to write",
                batchNumber, rowCount);

            try
            {
                await _retryService.ExecuteAsync(
                    ct => _bulkCopyService.CopyAsync(batch, ct),
                    cancellationToken);

                totalRowsCopied += rowCount;

                _logger.LogInformation(
                    "Batch {BatchNumber} completed: {RowCount} rows written. Total: {TotalRows}",
                    batchNumber, rowCount, totalRowsCopied);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Batch {BatchNumber} failed after all retry attempts",
                    batchNumber);
                throw;
            }
            finally
            {
                batch.Dispose();
            }
        }

        stopwatch.Stop();

        _logger.LogInformation(
            "Migration finished. Batches: {TotalBatches} | Rows: {TotalRows} | Elapsed: {Elapsed}",
            batchNumber, totalRowsCopied, stopwatch.Elapsed);

        return new MigrationResult
        {
            TotalBatches = batchNumber,
            TotalRowsCopied = totalRowsCopied,
            Elapsed = stopwatch.Elapsed
        };
    }
}
