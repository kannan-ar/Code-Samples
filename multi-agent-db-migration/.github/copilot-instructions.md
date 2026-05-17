# Repository AI Instructions

## Project Overview

This repository contains a .NET 8 console application that performs bulk data transfer between SQL Server databases.

Primary goals:
- reliability
- performance
- maintainability
- testability

---

## Technology Stack

- .NET 8
- C#
- SQL Server
- SqlBulkCopy
- xUnit
- Microsoft.Extensions.Logging

---

## Architecture Rules

- Use dependency injection.
- Use async/await consistently.
- Avoid static mutable state.
- Separate orchestration from infrastructure logic.
- Use interfaces for external dependencies.

---

## Performance Rules

- Use batching for database operations.
- Avoid loading extremely large datasets fully into memory.
- Prefer streaming where possible.
- Dispose database connections properly.

---

## Logging Rules

- Use structured logging.
- Log batch start/end.
- Log retry attempts.
- Log failures with contextual information.

---

## Error Handling Rules

- Retry transient SQL failures.
- Fail fast on invalid configuration.
- Avoid swallowing exceptions.

---

## Unit Testing Rules

- Business logic must be unit testable.
- Avoid direct database calls inside testable services.
- Use interfaces and mocks.

---

## AI Assistant Behavior

When implementing:
- prioritize readability
- prefer clean architecture principles
- generate production-quality code

When reviewing:
- check async correctness
- check resource disposal
- check memory usage
- check exception handling

Additional instructions exist under:
- .github/agents/
- .github/architecture/
- .github/prompts/