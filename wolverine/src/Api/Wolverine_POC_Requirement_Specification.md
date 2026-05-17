# Requirement Specification  
## POC Application using Wolverine + Vertical Slice Architecture + Kafka + PostgreSQL Inbox/Outbox

---

# 1. Objective

Build a **Proof of Concept (POC)** ASP.NET Core application using **Wolverine** with the **Mediator Pattern** and **Clean Vertical Slice Architecture**.

The application must demonstrate:

- CQRS Commands + Handlers
- Vertical Slice feature organization
- In-process mediator using Wolverine
- Database transaction + Outbox pattern
- Inbox pattern for idempotent message consumption
- PostgreSQL persistence
- Kafka async messaging
- Serilog structured logging to Seq
- Good practices for:
  - latency
  - reliability
  - scalability
  - observability
  - low coupling
  - manageable operational complexity

The solution must be implementable in **Visual Studio Code** using `.NET 8`.

---

# 2. Business Domain

## Order Processing System

Customers place product orders through HTTP API.

Flow:

1. Validate request
2. Create Order record in PostgreSQL
3. Save outgoing event to Wolverine Outbox
4. Commit transaction
5. Publish async message to Kafka topic
6. Consumer receives event
7. Consumer updates Order status to `Confirmed`
8. Inbox prevents duplicate processing

---

# 3. Functional Requirements

## POST /orders

Create a new order.

### Request

```json
{
  "customerName": "Kannan",
  "productCode": "PRD-1001",
  "quantity": 2,
  "unitPrice": 1500
}
```

### Response

```json
{
  "orderId": "guid",
  "status": "Pending"
}
```

## GET /orders/{id}

Return order details.

---

# 4. Business Rules

- customerName required
- productCode required
- quantity > 0
- unitPrice > 0
- Total = quantity × unitPrice
- Initial Status = Pending
- After consumer success = Confirmed

---

# 5. Non-Functional Requirements

## Reliability

- PostgreSQL transactional outbox
- Durable inbox
- Retry policy
- Dead letter queue
- Idempotent consumer

## Latency

- POST response < 150 ms local
- Kafka publish async
- Non-blocking handlers

## Scalability

- Stateless API
- Horizontal scale consumers
- Connection pooling

## Coupling

- Vertical slices
- No service layer required
- Transport independent handlers

## Operational Simplicity

Run entire solution using Docker Compose.

---

# 6. Technology Stack

- .NET 8
- ASP.NET Core Minimal API
- Wolverine
- PostgreSQL
- Kafka
- Serilog
- Seq
- EF Core

---

# 7. Architecture

```text
src/
 ├── Api/
 ├── Features/
 │   ├── Orders/
 │   │   ├── CreateOrder/
 │   │   ├── GetOrder/
 │   ├── Messaging/
 ├── Infrastructure/
 └── Shared/
```

---

# 8. Wolverine Requirements

## Mediator Pattern

HTTP Endpoint → Command → Wolverine InvokeAsync() → Handler

## Outbox Pattern

DB commit + outgoing event atomic.

## Inbox Pattern

Duplicate messages processed only once.

## Retry

- 3 retries
- exponential backoff
- move to error queue

## Kafka Topic

`orders.created`

---

# 9. PostgreSQL Schema

## Orders Table

| Column | Type |
|--------|------|
| Id | UUID |
| CustomerName | TEXT |
| ProductCode | TEXT |
| Quantity | INT |
| UnitPrice | NUMERIC |
| TotalAmount | NUMERIC |
| Status | TEXT |
| CreatedUtc | TIMESTAMP |

Use Wolverine-generated inbox/outbox tables.

---

# 10. Logging

Use Serilog + Seq.

Must log:

- Request start/end
- Order created
- DB commit
- Outbox queued
- Kafka publish
- Consumer success/failure
- Retries
- Exceptions

Include:

- CorrelationId
- OrderId
- MessageId

---

# 11. Metrics / Observability

Expose lightweight metrics:

- requests/sec
- order create latency
- handler duration
- retries count
- failed messages

Health checks:

- /health
- PostgreSQL
- Kafka

---

# 12. Docker Compose Requirement (MANDATORY)

Codex must create a `docker-compose.yml` containing:

- postgres
- kafka
- zookeeper (if image requires)
- seq
- optional kafka-ui
- optional pgadmin

All services must be runnable locally with:

```bash
docker compose up -d
```

Ports should be mapped clearly.

Use persistent named volumes.

---

# 13. Deliverables

Codex must generate:

- Full .NET solution
- Source code
- README.md
- docker-compose.yml
- appsettings.Development.json
- launch.json for VS Code
- EF migrations

---

# 14. Acceptance Criteria

- POST /orders works
- GET /orders/{id} works
- Order saved in PostgreSQL
- Event published to Kafka
- Consumer updates status to Confirmed
- Duplicate messages ignored
- Retries work
- Seq receives logs
- docker compose starts dependencies successfully

---

# 15. Constraints

Avoid:

- classic layered architecture
- fat controllers
- repository over-abstraction
- synchronous Kafka publish in request path
- tight coupling to infrastructure

---

# 16. Final Goal

Demonstrate a production-style modern .NET messaging application using Wolverine with strong reliability and clean architecture.
