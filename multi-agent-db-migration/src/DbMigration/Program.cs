using DbMigration.Configuration;
using DbMigration.Interfaces;
using DbMigration.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<MigrationOptions>()
    .BindConfiguration(MigrationOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<ISourceRepository, SourceRepository>();
builder.Services.AddScoped<IBulkCopyService, BulkCopyService>();
builder.Services.AddSingleton<IRetryService, RetryService>();
builder.Services.AddScoped<IMigrationOrchestrator, MigrationOrchestrator>();

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    using var cts = new CancellationTokenSource();

    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        cts.Cancel();
        logger.LogWarning("Cancellation requested by user.");
    };

    using var scope = host.Services.CreateScope();
    var orchestrator = scope.ServiceProvider.GetRequiredService<IMigrationOrchestrator>();

    var result = await orchestrator.RunAsync(cts.Token);

    if (result.IsSuccess)
    {
        logger.LogInformation(
            "Migration succeeded. Batches: {TotalBatches} | Rows: {TotalRowsCopied} | Elapsed: {Elapsed}.",
            result.TotalBatches, result.TotalRowsCopied, result.Elapsed);
        return 0;
    }

    logger.LogError(
        "Migration completed with {ErrorCount} error(s): {Errors}",
        result.Errors.Count, result.Errors);
    return 1;
}
catch (OperationCanceledException)
{
    logger.LogWarning("Migration was cancelled.");
    return 2;
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Migration failed with an unhandled exception.");
    return 1;
}
