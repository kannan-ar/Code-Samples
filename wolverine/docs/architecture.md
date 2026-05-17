# Architecture

## Request Path

1. `POST /orders` receives a JSON request.
2. Minimal API converts it to `CreateOrderCommand`.
3. `IMessageBus.InvokeAsync<CreateOrderResponse>()` dispatches the command through Wolverine mediator.
4. `CreateOrderHandler` validates the request, creates the `Order`, and publishes `OrderCreated`.
5. Wolverine EF Core transactional middleware commits the order and outgoing envelope atomically.
6. Wolverine publishes the durable outbox message to Kafka topic `orders.created`.
7. `OrderCreatedHandler` consumes through a durable inbox and confirms the order.

## Reliability

- EF Core owns the domain `orders.orders` table.
- Wolverine owns durable messaging tables in the `wolverine` schema.
- `MapWolverineEnvelopeStorage("wolverine")` maps envelope tables into the DbContext for batching, but EF migrations exclude those tables.
- Kafka listener uses `UseDurableInbox()` for idempotent, at-least-once processing.
- Failure policy retries three times with cooldowns of 250 ms, 1 second, and 5 seconds, then moves failures to Wolverine error storage.
- Kafka native DLQ is enabled with topic `orders.dead-letter`.

## Vertical Slices

Features are grouped by business capability:

- `Features/Orders/CreateOrder`: request, command, handler, response
- `Features/Orders/GetOrder`: query, projection, handler
- `Features/Messaging`: integration event and Kafka consumer
- `Infrastructure`: EF Core, startup migration, metrics middleware

There is no repository or service layer; handlers depend directly on transport-independent abstractions and EF Core.

## Observability

- Serilog writes structured logs to console and Seq.
- Correlation IDs are taken from `X-Correlation-Id` or generated per request.
- Logs include order IDs where the order is known.
- `/health` checks PostgreSQL and Kafka.
- `/metrics` exposes lightweight counters and last observed latencies.
