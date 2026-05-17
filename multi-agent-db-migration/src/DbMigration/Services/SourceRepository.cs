using System.Data;
using System.Runtime.CompilerServices;
using DbMigration.Configuration;
using DbMigration.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DbMigration.Services;

public sealed class SourceRepository : ISourceRepository
{
    private readonly MigrationOptions _options;
    private readonly ILogger<SourceRepository> _logger;

    public SourceRepository(IOptions<MigrationOptions> options, ILogger<SourceRepository> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async IAsyncEnumerable<DataTable> ReadBatchesAsync(
        int batchSize,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_options.SourceConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(_options.SourceQuery, connection)
        {
            CommandTimeout = 0
        };

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var schemaColumns = Enumerable
            .Range(0, reader.FieldCount)
            .Select(i => (Name: reader.GetName(i), Type: reader.GetFieldType(i)))
            .ToArray();

        var batchNumber = 0;

        while (true)
        {
            var batch = new DataTable();
            foreach (var (name, type) in schemaColumns)
                batch.Columns.Add(name, type);

            var rowCount = 0;
            while (rowCount < batchSize && await reader.ReadAsync(cancellationToken))
            {
                var row = batch.NewRow();
                for (var i = 0; i < reader.FieldCount; i++)
                    row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);

                batch.Rows.Add(row);
                rowCount++;
            }

            if (rowCount == 0)
                break;

            batchNumber++;
            _logger.LogInformation(
                "Batch {BatchNumber} read from source: {RowCount} rows",
                batchNumber, rowCount);

            yield return batch;
        }
    }
}
