using System.Diagnostics;
using System.Text.Json;
using Confluent.Kafka;
using JasperFx;
using JasperFx.Resources;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.Kafka;
using Wolverine.Postgresql;
using WolverinePoc.Api.Features.Messaging;
using WolverinePoc.Api.Features.Orders.CreateOrder;
using WolverinePoc.Api.Features.Orders.GetOrder;
using WolverinePoc.Api.Infrastructure;
using WolverinePoc.Api.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "WolverinePoc");
});

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");
var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    ?? throw new InvalidOperationException("Kafka:BootstrapServers is required.");
var ordersTopic = builder.Configuration["Kafka:OrdersCreatedTopic"] ?? "orders.created";
var consumerGroup = builder.Configuration["Kafka:ConsumerGroup"] ?? "wolverine-poc-orders";

builder.Services.AddOpenApi();
builder.Services.AddSingleton<AppMetrics>();
builder.Services.AddSingleton(TimeProvider.System);

builder.Host.UseWolverine(options =>
{
    options.PersistMessagesWithPostgresql(connectionString, "wolverine");
    options.Policies.UseDurableInboxOnAllListeners();
    options.Policies.AutoApplyTransactions();
    options.OnException<Exception>()
        .RetryWithCooldown(TimeSpan.FromMilliseconds(250), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5))
        .Then.MoveToErrorQueue();

    options.Services.AddDbContextWithWolverineIntegration<OrdersDbContext>(
        db => db.UseNpgsql(connectionString));

    options.UseKafka(kafkaBootstrapServers)
        .AutoProvision()
        .DeadLetterQueueTopicName("orders.dead-letter");

    options.PublishMessage<OrderCreated>()
        .ToKafkaTopic(ordersTopic);

    options.ListenToKafkaTopic(ordersTopic)
        .ConfigureConsumer(consumer =>
        {
            consumer.GroupId = consumerGroup;
            consumer.AutoOffsetReset = AutoOffsetReset.Earliest;
        })
        .UseDurableInbox()
        .EnableNativeDeadLetterQueue();

    options.Services.AddResourceSetupOnStartup();
});

builder.Services
    .AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgres")
    .AddKafka(new ProducerConfig { BootstrapServers = kafkaBootstrapServers }, name: "kafka");

var app = builder.Build();

if (app.Configuration.GetValue("Database:ApplyMigrationsOnStartup", true))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    var migrationLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("DatabaseStartup");

    await MigrateDatabaseWithRetryAsync(dbContext, migrationLogger);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path);
    };
});

app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers.TryGetValue("X-Correlation-Id", out var header)
        ? header.ToString()
        : Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");

    context.TraceIdentifier = correlationId;
    context.Response.Headers["X-Correlation-Id"] = correlationId;

    using (LogContext.PushProperty("CorrelationId", correlationId))
    {
        await next();
    }
});

app.UseMiddleware<RequestMetricsMiddleware>();

app.MapPost("/orders", async (
        CreateOrderRequest request,
        IMessageBus bus,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken) =>
    {
        var logger = loggerFactory.CreateLogger("OrdersEndpoints");
        logger.LogInformation("Dispatching CreateOrder command");

        try
        {
            var response = await bus.InvokeAsync<CreateOrderResponse>(
                request.ToCommand(),
                cancellationToken);

            return Results.Created($"/orders/{response.OrderId}", response);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "CreateOrder validation failed");
            return Results.ValidationProblem(ex.Errors);
        }
    })
    .WithName("CreateOrder")
    .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
    .ProducesValidationProblem();

app.MapGet("/orders/{id:guid}", async (
        Guid id,
        IMessageBus bus,
        CancellationToken cancellationToken) =>
    {
        var order = await bus.InvokeAsync<OrderDetails?>(
            new GetOrderQuery(id),
            cancellationToken);

        return order is null ? Results.NotFound() : Results.Ok(order);
    })
    .WithName("GetOrder")
    .Produces<OrderDetails>()
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/metrics", (AppMetrics metrics) => Results.Json(metrics.Snapshot()));

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                durationMs = entry.Value.Duration.TotalMilliseconds,
                error = entry.Value.Exception?.Message
            })
        };

        await JsonSerializer.SerializeAsync(context.Response.Body, payload);
    }
});

return await app.RunJasperFxCommands(args);

static async Task MigrateDatabaseWithRetryAsync(
    OrdersDbContext dbContext,
    Microsoft.Extensions.Logging.ILogger logger)
{
    const int maxAttempts = 12;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            var delay = TimeSpan.FromSeconds(Math.Min(30, attempt * 5));
            logger.LogWarning(ex,
                "Database migration failed on attempt {Attempt}/{MaxAttempts}. Retrying in {DelaySeconds} seconds",
                attempt,
                maxAttempts,
                delay.TotalSeconds);

            await Task.Delay(delay);
        }
    }

    await dbContext.Database.MigrateAsync();
}
