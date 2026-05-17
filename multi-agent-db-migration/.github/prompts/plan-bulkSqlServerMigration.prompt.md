# Plan: Bulk SQL Server-to-Server Data Migration (Stage 1 — Architecture)

## TL;DR
Design a .NET 8 console application that streams batches from a source SQL Server database and bulk-inserts them into a target SQL Server database using `SqlBulkCopy`. Services are cleanly separated behind interfaces, wired through DI, with retry and structured logging baked in.

---

## Project Structure

```
multi-agent-db-migration/
├── src/
│   └── DbMigration/
│       ├── Program.cs                         ← DI setup, IHost bootstrap, entry point
│       ├── DbMigration.csproj
│       ├── Configuration/
│       │   ├── MigrationOptions.cs            ← Strongly-typed config (IOptions<T>)
│       ├── Interfaces/
│       │   ├── ISourceRepository.cs           ← Streams batches from source DB
│       │   ├── IBulkCopyService.cs            ← Writes a DataTable batch to target DB
│       │   ├── IRetryService.cs               ← Wraps operations with retry logic
│       │   └── IMigrationOrchestrator.cs      ← Top-level RunAsync coordination
│       ├── Services/
│       │   ├── SourceRepository.cs
│       │   ├── BulkCopyService.cs
│       │   ├── RetryService.cs
│       │   └── MigrationOrchestrator.cs
│       └── Models/
│           └── MigrationResult.cs             ← Value object: rows copied, duration, errors
├── appsettings.json                           ← Connection strings, batch size, retry config
└── tests/
    └── DbMigration.Tests/
        └── DbMigration.Tests.csproj
```

---

## Interface Definitions

### `IMigrationOrchestrator`
- `Task RunAsync(CancellationToken cancellationToken)`

### `ISourceRepository`
- `IAsyncEnumerable<DataTable> ReadBatchesAsync(int batchSize, CancellationToken cancellationToken)`
- Streams rows from source using `SqlDataReader`; yields a filled `DataTable` per batch

### `IBulkCopyService`
- `Task CopyAsync(DataTable batch, CancellationToken cancellationToken)`
- Wraps `SqlBulkCopy`; uses `SqlBulkCopyOptions.KeepIdentity` as opt-in config

### `IRetryService`
- `Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)`
- Manual exponential backoff; retries only on transient `SqlException` error numbers

---

## Configuration — `MigrationOptions`
| Property | Type | Default |
|---|---|---|
| SourceConnectionString | string | required |
| DestinationConnectionString | string | required |
| SourceQuery | string | required |
| DestinationTable | string | required |
| BatchSize | int | 1000 |
| MaxRetryAttempts | int | 3 |
| RetryDelayMs | int | 500 |

Fail fast at startup if required fields are missing (validated via `IOptions` + `DataAnnotations`).

---

## Orchestration Sequence

1. `Program.cs` builds `IHost` with DI, reads `appsettings.json`, resolves `IMigrationOrchestrator`
2. `MigrationOrchestrator.RunAsync` calls `ISourceRepository.ReadBatchesAsync(batchSize, ct)`
3. For each yielded `DataTable` batch:
   a. Log batch start (batch number, row count)
   b. Call `IRetryService.ExecuteAsync(ct => IBulkCopyService.CopyAsync(batch, ct), ct)`
   c. Log batch end
4. Log total rows copied + elapsed time
5. All connections disposed via `await using`

---

## Batching Strategy
- `SqlDataReader` used in `SourceRepository` to avoid loading full result set into memory
- Each batch materializes as a `DataTable` of `BatchSize` rows max
- `IAsyncEnumerable<DataTable>` enables lazy pipeline — next batch fetched only after previous is written

---

## Retry Strategy
- `RetryService` catches `SqlException`; inspects `Number` property against known transient error codes (e.g., 1205 deadlock, -2 timeout, 49920 service busy)
- Exponential backoff: `RetryDelayMs * 2^attempt` per retry
- Non-transient exceptions propagate immediately (no swallowing)
- Log each retry attempt with attempt number and error code

---

## Key Technology Choices
- `Microsoft.Extensions.Hosting` — DI, configuration, logging
- `Microsoft.Extensions.Logging` — `ILogger<T>`, structured logging
- `Microsoft.Data.SqlClient` — `SqlDataReader`, `SqlBulkCopy`
- `IAsyncEnumerable<T>` — streaming batch pipeline
- No Polly dependency — manual retry keeps the footprint minimal per repo rules

---

## Risks
| Risk | Mitigation |
|---|---|
| Large result sets exhausting memory | Stream via `SqlDataReader` + `IAsyncEnumerable`; never load full set |
| Transient SQL failures | `RetryService` with exponential backoff on known error codes |
| Connection leaks | `await using` on all `SqlConnection` and `SqlBulkCopy` instances |
| Schema mismatch between source/target | Fail fast on first `SqlBulkCopy` failure with descriptive log |
| Partial migration on crash | Document as known limitation; full-transaction bulk copy is optional future work |

---

## Decisions
- Scope: source-to-target copy for a single table per run (multi-table is out of scope)
- No Polly — manual retry to minimize dependencies
- `DataTable` as the batch transfer object (compatible with `SqlBulkCopy` API)
- Config via `appsettings.json` + `IOptions<MigrationOptions>` (no env-var fallback for now)

---

## Relevant Files to Create
- `src/DbMigration/Program.cs`
- `src/DbMigration/DbMigration.csproj`
- `src/DbMigration/Configuration/MigrationOptions.cs`
- `src/DbMigration/Interfaces/ISourceRepository.cs`
- `src/DbMigration/Interfaces/IBulkCopyService.cs`
- `src/DbMigration/Interfaces/IRetryService.cs`
- `src/DbMigration/Interfaces/IMigrationOrchestrator.cs`
- `src/DbMigration/Models/MigrationResult.cs`
- `src/DbMigration/Services/SourceRepository.cs`
- `src/DbMigration/Services/BulkCopyService.cs`
- `src/DbMigration/Services/RetryService.cs`
- `src/DbMigration/Services/MigrationOrchestrator.cs`
- `appsettings.json`
- `tests/DbMigration.Tests/DbMigration.Tests.csproj`
