# System Overview

The application performs bulk data migration between SQL Server databases.

High-Level Flow:

1. Read source records
2. Transform records if needed
3. Batch records
4. Bulk insert into target database
5. Log progress
6. Retry transient failures

Primary Components:
- Configuration Loader
- Source Repository
- Bulk Copy Service
- Retry Service
- Logging Service
- Application Orchestrator