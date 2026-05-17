using System.Data;
using System.Runtime.CompilerServices;
using DbMigration.Configuration;
using DbMigration.Interfaces;
using DbMigration.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DbMigration.Tests;

public sealed class MigrationOrchestratorTests
{
    private readonly Mock<ISourceRepository> _sourceRepoMock = new();
    private readonly Mock<IBulkCopyService> _bulkCopyMock = new();
    private readonly Mock<IRetryService> _retryServiceMock = new();

    public MigrationOrchestratorTests()
    {
        // Default: retry service transparently invokes the operation delegate.
        _retryServiceMock
            .Setup(r => r.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>(
                (op, ct) => op(ct));
    }

    private MigrationOrchestrator CreateOrchestrator(int batchSize = 100)
    {
        var options = Options.Create(new MigrationOptions
        {
            SourceConnectionString = "s",
            DestinationConnectionString = "d",
            SourceQuery = "SELECT 1",
            DestinationTable = "T",
            BatchSize = batchSize
        });
        return new MigrationOrchestrator(
            _sourceRepoMock.Object,
            _bulkCopyMock.Object,
            _retryServiceMock.Object,
            options,
            NullLogger<MigrationOrchestrator>.Instance);
    }

    private static DataTable CreateBatch(int rowCount)
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        for (var i = 0; i < rowCount; i++)
            table.Rows.Add(i);
        return table;
    }

    private static async IAsyncEnumerable<DataTable> ToAsyncEnumerable(
        IEnumerable<DataTable> items,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var item in items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
            await Task.Yield();
        }
    }

    [Fact]
    public async Task RunAsync_SingleBatch_ReturnsCorrectRowAndBatchCounts()
    {
        _sourceRepoMock
            .Setup(r => r.ReadBatchesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable([CreateBatch(5)]));

        var result = await CreateOrchestrator().RunAsync(CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.TotalRowsCopied);
        Assert.Equal(1, result.TotalBatches);
    }

    [Fact]
    public async Task RunAsync_MultipleBatches_AccumulatesRowCount()
    {
        var batches = new[] { CreateBatch(100), CreateBatch(100), CreateBatch(50) };
        _sourceRepoMock
            .Setup(r => r.ReadBatchesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(batches));

        var result = await CreateOrchestrator().RunAsync(CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(250, result.TotalRowsCopied);
        Assert.Equal(3, result.TotalBatches);
    }

    [Fact]
    public async Task RunAsync_EmptySource_ReturnsZeroCounts()
    {
        _sourceRepoMock
            .Setup(r => r.ReadBatchesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable([]));

        var result = await CreateOrchestrator().RunAsync(CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.TotalRowsCopied);
        Assert.Equal(0, result.TotalBatches);
        Assert.True(result.Elapsed >= TimeSpan.Zero);
    }

    [Fact]
    public async Task RunAsync_BulkCopyThrows_LogsAndRethrowsException()
    {
        _sourceRepoMock
            .Setup(r => r.ReadBatchesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable([CreateBatch(10)]));

        _bulkCopyMock
            .Setup(b => b.CopyAsync(It.IsAny<DataTable>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Copy failed"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateOrchestrator().RunAsync(CancellationToken.None));
    }

    [Fact]
    public async Task RunAsync_PassesConfiguredBatchSizeToRepository()
    {
        _sourceRepoMock
            .Setup(r => r.ReadBatchesAsync(500, It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable([]));

        await CreateOrchestrator(batchSize: 500).RunAsync(CancellationToken.None);

        _sourceRepoMock.Verify(
            r => r.ReadBatchesAsync(500, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RunAsync_EachBatch_RoutedThroughRetryService()
    {
        var batches = new[] { CreateBatch(10), CreateBatch(10), CreateBatch(10) };
        _sourceRepoMock
            .Setup(r => r.ReadBatchesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(batches));

        await CreateOrchestrator().RunAsync(CancellationToken.None);

        _retryServiceMock.Verify(
            r => r.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task>>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task RunAsync_PartialFailure_RowsBeforeFailureNotCounted()
    {
        // Two batches: first succeeds (10 rows), second fails.
        var batches = new[] { CreateBatch(10), CreateBatch(10) };
        _sourceRepoMock
            .Setup(r => r.ReadBatchesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(ToAsyncEnumerable(batches));

        var callCount = 0;
        _bulkCopyMock
            .Setup(b => b.CopyAsync(It.IsAny<DataTable>(), It.IsAny<CancellationToken>()))
            .Returns<DataTable, CancellationToken>((_, _) =>
            {
                if (++callCount == 2)
                    throw new InvalidOperationException("Second batch failed");
                return Task.CompletedTask;
            });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateOrchestrator().RunAsync(CancellationToken.None));

        // Bulk copy was called twice — once per batch before the throw.
        _bulkCopyMock.Verify(
            b => b.CopyAsync(It.IsAny<DataTable>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}
