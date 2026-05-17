using System.Data;
using DbMigration.Configuration;
using DbMigration.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DbMigration.Services;

public sealed class BulkCopyService : IBulkCopyService
{
    private readonly MigrationOptions _options;
    private readonly ILogger<BulkCopyService> _logger;

    public BulkCopyService(IOptions<MigrationOptions> options, ILogger<BulkCopyService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task CopyAsync(DataTable batch, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_options.DestinationConnectionString);
        await connection.OpenAsync(cancellationToken);

        using var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = _options.DestinationTable,
            BulkCopyTimeout = 0
        };

        foreach (DataColumn column in batch.Columns)
            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);

        _logger.LogDebug(
            "Writing {RowCount} rows to {DestinationTable}",
            batch.Rows.Count, _options.DestinationTable);

        await bulkCopy.WriteToServerAsync(batch, cancellationToken);
    }
}
