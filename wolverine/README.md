# Wolverine POC

Proof-of-concept order processing API using ASP.NET Core Minimal API, Wolverine mediator/messaging, Kafka, PostgreSQL durable inbox/outbox, EF Core, Serilog, and Seq.

The requirement spec asked for .NET 8, but the implementation targets `net10.0` because the workspace has .NET SDK `10.0.101` and the user requested the latest framework.

## Prerequisites

- .NET SDK 10
- Docker Desktop or compatible Docker Compose runtime

## Run Dependencies

```bash
docker compose up -d
```

Exposed ports:

- API: `http://localhost:5012`
- PostgreSQL: `localhost:5432`
- Kafka: `localhost:9092`
- Seq: `http://localhost:5341`
- Kafka UI: `http://localhost:8080`
- pgAdmin: `http://localhost:5050`

The Compose stack includes the API container plus all infrastructure. Visual Studio can run it by setting the `docker-compose` project as the startup project.

## Run the API

To run the API outside Docker while still using Compose infrastructure:

```bash
dotnet run --project src/Api/WolverinePoc.Api.csproj
```

The API applies the EF order-table migration on startup. Wolverine/JasperFx resource setup creates the durable inbox/outbox tables in the `wolverine` schema.

## Visual Studio Docker Compose

Open `WolverinePoc.sln` in Visual Studio and set `docker-compose` as the startup project. Visual Studio uses:

- `docker-compose.dcproj`
- `docker-compose.yml`
- `docker-compose.override.yml`
- `src/Api/Dockerfile`

The API Compose service is `wolverinepoc.api`. It listens on port `8080` internally and is mapped to `http://localhost:5012`.

## Try the Flow

Create an order:

```bash
curl -X POST http://localhost:5012/orders \
  -H "Content-Type: application/json" \
  -H "X-Correlation-Id: demo-001" \
  -d "{\"customerName\":\"Kannan\",\"productCode\":\"PRD-1001\",\"quantity\":2,\"unitPrice\":1500}"
```

Get an order:

```bash
curl http://localhost:5012/orders/{orderId}
```

The order is initially persisted as `Pending`, an `OrderCreated` message is queued through Wolverine's transactional outbox, Kafka receives it on `orders.created`, and the durable consumer updates the order to `Confirmed`.

## Useful Endpoints

- `POST /orders`
- `GET /orders/{id}`
- `GET /health`
- `GET /metrics`
- `GET /openapi/v1.json` in Development

## Project Layout

```text
src/
  Api/
    Features/
      Messaging/
      Orders/
        CreateOrder/
        GetOrder/
    Infrastructure/
    Shared/
```

See [docs/architecture.md](docs/architecture.md) and [docs/operations.md](docs/operations.md) for implementation details.
