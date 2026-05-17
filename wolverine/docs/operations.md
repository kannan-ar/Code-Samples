# Operations

## Local Startup

```bash
docker compose up -d
dotnet run --project src/Api/WolverinePoc.Api.csproj
```

`docker compose up -d` starts the API container plus PostgreSQL, Kafka, Zookeeper, Seq, Kafka UI, and pgAdmin. To run only infrastructure and start the API from the CLI or IDE, stop the `wolverinepoc.api` service and run the API project directly.

The API needs PostgreSQL and Kafka to be reachable before startup. If startup fails while Docker services are still warming up, wait for Compose health checks to settle and run the API again.

## Visual Studio Compose Startup

The solution includes a Visual Studio Docker Compose project:

```text
docker-compose.dcproj
```

In Visual Studio:

1. Open `WolverinePoc.sln`.
2. Set `docker-compose` as the startup project.
3. Press F5 or Ctrl+F5.

Visual Studio will build `src/Api/Dockerfile`, run `docker-compose.yml` plus `docker-compose.override.yml`, and launch the API health endpoint.

API URL:

```text
http://localhost:5012
```

## Database

Connection string:

```text
Host=localhost;Port=5432;Database=wolverine_poc;Username=wolverine;Password=wolverine
```

Domain schema:

- `orders.orders`

Wolverine schema:

- `wolverine` durable inbox/outbox and error tables, created by JasperFx resource setup.

EF migration command:

```bash
dotnet ef database update --project src/Api/WolverinePoc.Api.csproj --startup-project src/Api/WolverinePoc.Api.csproj
```

## Kafka

Topic:

- `orders.created`

Dead letter topic:

- `orders.dead-letter`

Consumer group:

- `wolverine-poc-orders-dev` in Development
- `wolverine-poc-orders` otherwise

Kafka UI is available at `http://localhost:8080`.

## Logging

Seq is available at `http://localhost:5341`.

Useful properties:

- `CorrelationId`
- `OrderId`
- `RequestPath`
- `Application`

## Health and Metrics

```bash
curl http://localhost:5012/health
curl http://localhost:5012/metrics
```

The `/metrics` endpoint is intentionally lightweight for the POC. Production deployments should export the same counters through OpenTelemetry or Prometheus.
