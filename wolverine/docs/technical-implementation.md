# Technical Implementation Details

This document explains how the Wolverine POC is implemented and how each requirement maps to the code.

## Runtime and Project Shape

The solution contains a single ASP.NET Core API project:

```text
WolverinePoc.sln
src/Api/WolverinePoc.Api.csproj
```

The project targets `net10.0` because the available SDK in this workspace is .NET `10.0.101` and the request asked to use the latest framework. The requirement specification mentions .NET 8, but all selected packages have compatible .NET 10 assets.

The API follows vertical-slice organization:

```text
src/Api/
  Features/
    Orders/
      CreateOrder/
      GetOrder/
    Messaging/
  Infrastructure/
  Shared/
```

There are no controllers, repositories, or service-layer abstractions. Minimal API endpoints dispatch commands and queries through Wolverine.

## HTTP Endpoints

The endpoints are defined in `src/Api/Program.cs`.

### POST /orders

The endpoint accepts `CreateOrderRequest`, converts it to `CreateOrderCommand`, and invokes Wolverine:

```csharp
await bus.InvokeAsync<CreateOrderResponse>(request.ToCommand(), cancellationToken);
```

The handler is:

```text
src/Api/Features/Orders/CreateOrder/CreateOrderHandler.cs
```

It validates:

- `customerName` is required.
- `productCode` is required.
- `quantity > 0`.
- `unitPrice > 0`.

Then it creates an `Order` with:

- `Status = Pending`
- `TotalAmount = Quantity * UnitPrice`
- `CreatedUtc = current UTC time`

It also publishes an `OrderCreated` integration event through Wolverine.

### GET /orders/{id}

The endpoint invokes `GetOrderQuery` through Wolverine and returns a projection from EF Core:

```text
src/Api/Features/Orders/GetOrder/GetOrderHandler.cs
```

It uses `AsNoTracking()` because the query is read-only.

## Domain Model

The order entity is implemented in:

```text
src/Api/Features/Orders/Order.cs
```

Important fields:

- `Id`
- `CustomerName`
- `ProductCode`
- `Quantity`
- `UnitPrice`
- `TotalAmount`
- `Status`
- `CreatedUtc`

The status enum is:

```text
src/Api/Features/Orders/OrderStatus.cs
```

Supported values:

- `Pending`
- `Confirmed`

## PostgreSQL Persistence

EF Core persistence is configured in:

```text
src/Api/Infrastructure/OrdersDbContext.cs
```

The domain table is:

```text
orders.orders
```

Column mapping:

| Property | Column | Type |
| --- | --- | --- |
| `Id` | `id` | `uuid` |
| `CustomerName` | `customer_name` | `text` |
| `ProductCode` | `product_code` | `text` |
| `Quantity` | `quantity` | `integer` |
| `UnitPrice` | `unit_price` | `numeric(18,2)` |
| `TotalAmount` | `total_amount` | `numeric(18,2)` |
| `Status` | `status` | `varchar(32)` |
| `CreatedUtc` | `created_utc` | `timestamp with time zone` |

EF migration files are under:

```text
src/Api/Infrastructure/Migrations/
```

The API applies EF migrations on startup when this setting is enabled:

```json
"Database": {
  "ApplyMigrationsOnStartup": true
}
```

This is done in `Program.cs` with:

```csharp
await dbContext.Database.MigrateAsync();
```

## Wolverine Mediator

Wolverine is used as an in-process mediator for HTTP requests.

The endpoint does not call the handler directly. It sends a command/query to Wolverine:

```csharp
IMessageBus.InvokeAsync<TResponse>(message)
```

This keeps the HTTP transport separate from business logic. The handlers are plain static handler methods and can be used independently of HTTP.

## Transactional Outbox

The create-order handler is marked transactional:

```csharp
[Transactional]
public static async Task<CreateOrderResponse> Handle(...)
```

Wolverine is configured with EF Core integration:

```csharp
options.Services.AddDbContextWithWolverineIntegration<OrdersDbContext>(
    db => db.UseNpgsql(connectionString));
```

And automatic transactional middleware:

```csharp
options.Policies.AutoApplyTransactions();
```

When `CreateOrderHandler` adds the order and publishes `OrderCreated`, Wolverine stores the outgoing message in the durable outbox as part of the same database transaction. That gives atomic behavior:

1. Insert order.
2. Store outgoing event envelope.
3. Commit once.
4. Publish to Kafka asynchronously after commit.

The handler therefore does not call `SaveChangesAsync()` manually. Wolverine's transactional middleware owns the commit.

## Kafka Publishing

The integration event is:

```text
src/Api/Features/Messaging/OrderCreated.cs
```

It includes:

- `MessageId`
- `OrderId`
- customer/product details
- totals
- creation timestamp

Kafka publishing is configured in `Program.cs`:

```csharp
options.PublishMessage<OrderCreated>()
    .ToKafkaTopic(ordersTopic);
```

The default topic is:

```text
orders.created
```

The `POST /orders` request does not synchronously publish to Kafka. It queues the message in Wolverine's outbox, commits the transaction, and lets Wolverine deliver it asynchronously.

## Kafka Consumer

The consumer handler is:

```text
src/Api/Features/Messaging/OrderCreatedHandler.cs
```

Kafka listener configuration is in `Program.cs`:

```csharp
options.ListenToKafkaTopic(ordersTopic)
    .ConfigureConsumer(consumer =>
    {
        consumer.GroupId = consumerGroup;
        consumer.AutoOffsetReset = AutoOffsetReset.Earliest;
    })
    .UseDurableInbox()
    .EnableNativeDeadLetterQueue();
```

When an `OrderCreated` message is received, the handler loads the order:

```csharp
var order = await dbContext.Orders.FindAsync([message.OrderId], cancellationToken);
```

If the order is found and is not already confirmed, the handler updates:

```csharp
order.Status = OrderStatus.Confirmed;
```

This is persisted to PostgreSQL because:

- The entity is tracked by EF Core.
- The handler is marked `[Transactional]`.
- Wolverine EF Core transactional middleware commits `SaveChangesAsync()` after the handler succeeds.

The handler also includes idempotent behavior:

```csharp
if (order.Status == OrderStatus.Confirmed)
{
    logger.LogInformation("Duplicate OrderCreated message ignored for already confirmed order");
    return;
}
```

This means duplicate messages do not repeatedly mutate the order.

## Durable Inbox

The Kafka listener uses:

```csharp
.UseDurableInbox()
```

This lets Wolverine track incoming message processing in PostgreSQL. If the same durable message is redelivered, Wolverine can prevent duplicate processing. The handler also performs a domain-level idempotency check by skipping already-confirmed orders.

## Wolverine Envelope Tables

The DbContext maps Wolverine envelope storage:

```csharp
modelBuilder.MapWolverineEnvelopeStorage("wolverine");
```

The EF model snapshot shows these envelope entities, but they are excluded from EF migrations. Wolverine/JasperFx resource setup creates and manages the durable messaging tables in the `wolverine` schema.

Resource setup is enabled in `Program.cs`:

```csharp
options.Services.AddResourceSetupOnStartup();
```

## Retry and Error Handling

The global failure policy is configured in `Program.cs`:

```csharp
options.OnException<Exception>()
    .RetryWithCooldown(
        TimeSpan.FromMilliseconds(250),
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(5))
    .Then.MoveToErrorQueue();
```

This gives three retries with increasing delays. If processing still fails, Wolverine moves the message to error storage.

Kafka native dead-letter queue support is enabled:

```csharp
.EnableNativeDeadLetterQueue();
```

The configured Kafka DLQ topic is:

```text
orders.dead-letter
```

## Logging

Serilog is configured in `Program.cs` and `appsettings.json`.

Sinks:

- Console
- Seq at `http://localhost:5341`

The request middleware assigns a correlation ID from `X-Correlation-Id` or generates one:

```csharp
context.Response.Headers["X-Correlation-Id"] = correlationId;
```

Structured log properties include:

- `CorrelationId`
- `OrderId`
- `MessageId`
- `RequestPath`
- `Application`

The order create handler logs when the event is queued in the outbox. The consumer logs when an order is confirmed, when a duplicate is ignored, and when processing fails.

## Health Checks

Health checks are exposed at:

```text
GET /health
```

Configured checks:

- PostgreSQL
- Kafka

The endpoint returns JSON with check name, status, duration, and error if one exists.

## Metrics

Lightweight metrics are exposed at:

```text
GET /metrics
```

Implementation:

```text
src/Api/Infrastructure/AppMetrics.cs
src/Api/Infrastructure/RequestMetricsMiddleware.cs
```

Tracked values:

- Total requests
- Approximate requests per second
- Last order creation latency
- Last handler duration
- Failed message count
- Uptime

The app also creates .NET `System.Diagnostics.Metrics` counters and histograms. The HTTP `/metrics` endpoint is intentionally simple for the POC.

## Docker Compose

The local dependency stack is defined in:

```text
docker-compose.yml
```

Services:

- `wolverinepoc.api`
- `postgres`
- `zookeeper`
- `kafka`
- `seq`
- `kafka-ui`
- `pgadmin`

Run with:

```bash
docker compose up -d
```

Important ports:

- API: `5012`
- PostgreSQL: `5432`
- Kafka: `9092`
- Seq: `5341`
- Kafka UI: `8080`
- pgAdmin: `5050`

Named volumes are used for durable local data.

The API image is built from:

```text
src/Api/Dockerfile
```

The Dockerfile uses a multi-stage build:

1. `mcr.microsoft.com/dotnet/sdk:10.0` restores, builds, and publishes.
2. `mcr.microsoft.com/dotnet/aspnet:10.0` runs the final published app.

Container-specific environment variables are in:

```text
docker-compose.override.yml
```

Those settings point the API at Docker network hostnames:

- PostgreSQL: `postgres:5432`
- Kafka: `kafka:29092`
- Seq: `http://seq`

## Visual Studio Docker Support

The API project includes Docker metadata:

```xml
<DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>
<DockerfileContext>..\..</DockerfileContext>
```

The solution also includes a Visual Studio Docker Compose project:

```text
docker-compose.dcproj
```

Set `docker-compose` as the startup project in Visual Studio to run the complete stack. The Compose project uses:

- `docker-compose.yml`
- `docker-compose.override.yml`
- `.dockerignore`
- `src/Api/Dockerfile`

## Configuration

Main config:

```text
src/Api/appsettings.json
```

Development overrides:

```text
src/Api/appsettings.Development.json
```

Important settings:

```json
"ConnectionStrings": {
  "Postgres": "Host=localhost;Port=5432;Database=wolverine_poc;Username=wolverine;Password=wolverine"
},
"Kafka": {
  "BootstrapServers": "localhost:9092",
  "OrdersCreatedTopic": "orders.created",
  "ConsumerGroup": "wolverine-poc-orders"
}
```

## VS Code Support

VS Code launch configuration is in:

```text
.vscode/launch.json
```

Build task:

```text
.vscode/tasks.json
```

The app runs on:

```text
http://localhost:5012
```

## Verification Performed

The solution was built successfully:

```bash
dotnet build WolverinePoc.sln --no-restore
```

Result:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

Docker Compose configuration was also validated with:

```bash
docker compose config
```

The full end-to-end runtime flow requires starting Docker dependencies and running the API locally.
