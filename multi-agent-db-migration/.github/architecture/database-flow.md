# Database Flow

## Source Database

The source database contains large tables that must be copied efficiently.

Requirements:
- support batching
- avoid full table memory loading
- support cancellation
- support retries

---

## Target Database

The target database receives bulk insert operations using SqlBulkCopy.

Requirements:
- preserve transactional consistency
- support configurable batch sizes
- log batch failures

---

## Migration Flow

1. Open source connection
2. Read records in batches
3. Map records if needed
4. Bulk copy to target database
5. Commit batch
6. Log results
7. Continue until completion