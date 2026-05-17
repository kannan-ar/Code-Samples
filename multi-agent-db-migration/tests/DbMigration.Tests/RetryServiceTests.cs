using DbMigration.Configuration;
using DbMigration.Services;
using DbMigration.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbMigration.Tests;

public sealed class RetryServiceTests
{
    // Use RetryDelayMs=0 so tests don't incur real delays.
    private static RetryService CreateService(int maxRetryAttempts = 3, int retryDelayMs = 0)
    {
        var options = Options.Create(new MigrationOptions
        {
            SourceConnectionString = "s",
            DestinationConnectionString = "d",
            SourceQuery = "SELECT 1",
            DestinationTable = "T",
            MaxRetryAttempts = maxRetryAttempts,
            RetryDelayMs = retryDelayMs
        });
        return new RetryService(options, NullLogger<RetryService>.Instance);
    }

    [Fact]
    public async Task ExecuteAsync_SuccessOnFirstAttempt_CallsOperationOnce()
    {
        var sut = CreateService();
        var callCount = 0;

        await sut.ExecuteAsync(_ =>
        {
            callCount++;
            return Task.CompletedTask;
        }, CancellationToken.None);

        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task ExecuteAsync_TransientException_RetriesAndSucceeds()
    {
        var sut = CreateService();
        var callCount = 0;

        await sut.ExecuteAsync(_ =>
        {
            callCount++;
            if (callCount == 1)
                throw SqlExceptionFactory.Create(1205); // deadlock — transient
            return Task.CompletedTask;
        }, CancellationToken.None);

        Assert.Equal(2, callCount);
    }

    [Fact]
    public async Task ExecuteAsync_TransientException_ExhaustsAllRetries_Rethrows()
    {
        // MaxRetryAttempts=3 means 1 initial + 3 retries = 4 total calls.
        var sut = CreateService(maxRetryAttempts: 3);
        var callCount = 0;

        await Assert.ThrowsAnyAsync<Exception>(() =>
            sut.ExecuteAsync(_ =>
            {
                callCount++;
                throw SqlExceptionFactory.Create(1205);
            }, CancellationToken.None));

        Assert.Equal(4, callCount);
    }

    [Fact]
    public async Task ExecuteAsync_NonTransientSqlException_PropagatesImmediatelyWithoutRetry()
    {
        var sut = CreateService();
        var callCount = 0;

        await Assert.ThrowsAnyAsync<Exception>(() =>
            sut.ExecuteAsync(_ =>
            {
                callCount++;
                throw SqlExceptionFactory.Create(50000); // non-transient error number
            }, CancellationToken.None));

        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task ExecuteAsync_NonSqlException_PropagatesImmediatelyWithoutRetry()
    {
        var sut = CreateService();
        var callCount = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.ExecuteAsync(_ =>
            {
                callCount++;
                throw new InvalidOperationException("not a SQL error");
            }, CancellationToken.None));

        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedDuringDelay_ThrowsOperationCanceledException()
    {
        // Token is pre-cancelled; after the first transient failure the delay
        // should immediately surface OperationCanceledException.
        var sut = CreateService();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            sut.ExecuteAsync(_ => throw SqlExceptionFactory.Create(1205), cts.Token));
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(1205)]
    [InlineData(4060)]
    [InlineData(40197)]
    [InlineData(40501)]
    [InlineData(40613)]
    [InlineData(49920)]
    public async Task ExecuteAsync_AllKnownTransientErrors_Retry(int errorNumber)
    {
        var sut = CreateService(maxRetryAttempts: 1);
        var callCount = 0;

        await sut.ExecuteAsync(_ =>
        {
            callCount++;
            if (callCount == 1)
                throw SqlExceptionFactory.Create(errorNumber);
            return Task.CompletedTask;
        }, CancellationToken.None);

        Assert.Equal(2, callCount);
    }
}
