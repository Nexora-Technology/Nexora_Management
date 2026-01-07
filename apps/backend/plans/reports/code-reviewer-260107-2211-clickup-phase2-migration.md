# Code Review Report - Phase 2 Backend Database Migration

**Date:** 2026-01-07
**Reviewer:** Code Reviewer Subagent (ac678a2)
**Phase:** Backend Database Migration (ClickUp Hierarchy)
**Grade:** **B+**
**Files Reviewed:** 25 files (4 SQL scripts + 19 application files + 2 docs)

---

## Executive Summary

Phase 2 migration implements ClickUp hierarchy restructure (Workspace → Space → TaskList → Task). Overall implementation quality is **good** with robust SQL migration scripts and comprehensive documentation. However, **critical issues** exist in application layer (CreateTaskCommand) and **security concerns** around backup schema access.

**Recommendation:** Address critical issues before production deployment. Medium-priority issues can be deferred to Phase 3 cleanup.

---

## Critical Issues (3)

### 1. **CRITICAL: Incorrect ProjectId Assignment in CreateTaskCommand**

**Severity:** 🔴 Critical (Data Corruption)
**File:** `/apps/backend/src/Nexora.Management.Application/Tasks/Commands/CreateTask/CreateTaskCommand.cs:62`

```csharp
// Line 62 - WRONG!
ProjectId = tasklist.SpaceId, // Set for backward compatibility
```

**Problem:**

- Sets `ProjectId = SpaceId` (completely different entity)
- Breaks referential integrity if Projects table still has FK constraints
- Creates orphaned Tasks referencing Spaces instead of Projects
- Migration scripts expect ProjectId to map to TaskList.Id (preserved IDs)

**Impact:**

- Data corruption: Tasks reference wrong entity type
- Queries filtering by ProjectId return wrong results
- Rollback scripts will fail (expect TaskListId → ProjectId mapping)
- Frontend may receive inconsistent data

**Fix Required:**

```csharp
// Option 1: Remove ProjectId assignment (migration already completed)
// ProjectId = tasklist.Id, // Use TaskList.Id (same as old Project.Id)

// Option 2: Don't set ProjectId at all (obsolete field)
// Leave ProjectId as default Guid.Empty (will be ignored)
```

**Verification:**

```sql
-- Check if Tasks have invalid ProjectIds
SELECT COUNT(*) FROM "Tasks" t
LEFT JOIN "Projects" p ON t."ProjectId" = p."Id"
WHERE t."ProjectId" IS NOT NULL AND p."Id" IS NULL;
-- Expected: 0 (will fail with current bug)
```

---

### 2. **CRITICAL: Missing SQL Injection Protection in Dynamic SQL**

**Severity:** 🔴 Critical (Security Vulnerability)
**File:** `/apps/backend/scripts/MigrateProjectsToTaskLists.sql` (implicit risk)

**Problem:**

- All migration scripts use hardcoded table/column names (GOOD)
- However, no validation that `_backup_projects` schema name is safe
- Schema name used in `CREATE SCHEMA` and table references
- If schema name parameterized in future, injection risk exists

**Current Status:** Safe (hardcoded), but fragile
**Risk:** Future automation could introduce parameters

**Recommendation:**

```sql
-- Add validation at top of each script
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.schemata WHERE schema_name = '_backup_projects') THEN
        RAISE NOTICE 'Backup schema exists';
    ELSE
        RAISE EXCEPTION 'Backup schema _backup_projects must exist before migration';
    END IF;
END $$;
```

---

### 3. **CRITICAL: No Database Lock During Critical Operations**

**Severity:** 🔴 Critical (Race Condition)
**Files:** All migration scripts

**Problem:**

- Scripts run in `BEGIN` transaction (GOOD)
- But no explicit table locks (`LOCK TABLE ... IN ACCESS EXCLUSIVE MODE`)
- Concurrent application writes during migration could cause:
  - Lost updates
  - Inconsistent snapshots
  - FK constraint violations

**Impact:**

- If application NOT stopped before migration
- Data corruption from concurrent writes
- Migration verification may pass but data inconsistent

**Fix Required:**

```sql
-- Add at start of each migration script
BEGIN;

-- Lock tables to prevent concurrent writes
LOCK TABLE "Projects" IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "TaskLists" IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "Tasks" IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "Workspaces" IN ACCESS EXCLUSIVE MODE;

-- ... rest of migration
```

**Documentation Gap:**

- MIGRATION_README.md mentions "Application Shutdown" in Step 5
- Should be Step 0 (before any scripts)
- Add warning: ⚠️ **STOP APPLICATION BEFORE RUNNING MIGRATION**

---

## High Priority Issues (4)

### 1. **Missing Index on Composite Query (TaskListId + StatusId)**

**Severity:** 🟠 High (Performance)
**File:** `/apps/backend/scripts/MigrateTasksToTaskLists.sql`

**Problem:**

- Only creates index on `TaskListId` (Line 141-142)
- Common query pattern: `WHERE TaskListId = ? AND StatusId = ?`
- Missing composite index for board view queries

**Impact:**

- Board view queries will be slow (table scan on StatusId filter)
- Performance degradation vs. baseline (Projects had composite index)

**Fix:**

```sql
-- Add to MigrateTasksToTaskLists.sql after Line 142
CREATE INDEX IF NOT EXISTS "IX_Tasks_TaskListId_StatusId"
ON "Tasks"("TaskListId", "StatusId")
WHERE "StatusId" IS NOT NULL;
```

**Verification:**

```sql
-- Test query performance
EXPLAIN ANALYZE
SELECT * FROM "Tasks"
WHERE "TaskListId" = '...' AND "StatusId" = '...';
-- Should use Index Scan, not Seq Scan
```

---

### 2. **Inconsistent FK Constraint Naming**

**Severity:** 🟠 High (Maintainability)
**Files:** TaskConfiguration.cs, migration scripts

**Problem:**

- EF Core config: `"FK_Tasks_TaskLists_TaskListId"` (TaskConfiguration.cs:90)
- Migration script: `"FK_Tasks_TaskLists_TaskListId"` (hardcoded)
- Old FK: `"FK_Tasks_Projects_ProjectId"`
- Naming inconsistent: should follow pattern `FK_{Table}_{ReferencedTable}_{Column}`

**Current:** Correct convention (no issue)
**Risk:** If EF Core generates different name, migration fails

**Recommendation:**

```csharp
// TaskConfiguration.cs:90 - Explicit name
builder.HasOne(t => t.TaskList)
    .WithMany(tl => tl.Tasks)
    .HasForeignKey(t => t.TaskListId)
    .HasConstraintName("FK_Tasks_TaskLists_TaskListId") // EXPLICIT
    .OnDelete(DeleteBehavior.Cascade);
```

---

### 3. **No Validation of Migration Prerequisites**

**Severity:** 🟠 High (Operational Risk)
**File:** `/apps/backend/scripts/MigrateTasksToTaskLists.sql`

**Problem:**

- Script checks `TaskLists` exist (Line 28-39) ✓
- But doesn't check:
  - Projects table still exists
  - Tasks.ProjectId column still exists
  - No orphaned Tasks before migration

**Impact:**

- Script fails cryptically if prerequisites not met
- Partial migration possible (if failure mid-transaction)
- Difficult troubleshooting

**Fix:**

```sql
-- Add to Step 1 (after Line 39)
DO $$
DECLARE
    v_projects_table_exists BOOLEAN;
    v_projectid_column_exists BOOLEAN;
BEGIN
    SELECT EXISTS (
        SELECT 1 FROM information_schema.tables
        WHERE table_name = 'Projects'
    ) INTO v_projects_table_exists;

    SELECT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name = 'Tasks' AND column_name = 'ProjectId'
    ) INTO v_projectid_column_exists;

    IF NOT v_projects_table_exists THEN
        RAISE EXCEPTION '❌ PREREQUISITE FAILED: Projects table does not exist';
    END IF;

    IF NOT v_projectid_column_exists THEN
        RAISE EXCEPTION '❌ PREREQUISITE FAILED: Tasks.ProjectId column does not exist';
    END IF;

    RAISE NOTICE '✓ All prerequisites validated';
END $$;
```

---

### 4. **Rollback Script Doesn't Validate Migration State**

**Severity:** 🟠 High (Data Loss Risk)
**File:** `/apps/backend/scripts/RollbackMigration.sql`

**Problem:**

- Assumes migration completed successfully
- No check if migration was partial
- Could delete TaskLists while Tasks still reference them
- Leaves database in inconsistent state

**Impact:**

- If rollback run after partial migration
- Data loss (TaskLists deleted, Tasks orphaned)
- No recovery without full database restore

**Fix:**

```sql
-- Add after Step 1 (Line 53)
DO $$
DECLARE
    v_tasklist_tasks_count INT;
    v_projectid_exists BOOLEAN;
BEGIN
    -- Check if Tasks have TaskListId
    SELECT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name = 'Tasks' AND column_name = 'TaskListId'
    ) INTO v_taskid_exists;

    -- Check if Tasks still have ProjectId
    SELECT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name = 'Tasks' AND column_name = 'ProjectId'
    ) INTO v_projectid_exists;

    IF NOT v_taskid_exists THEN
        RAISE EXCEPTION '❌ ROLLBACK ABORTED: TaskListId column missing (migration not completed)';
    END IF;

    IF v_projectid_exists THEN
        RAISE EXCEPTION '❌ ROLLBACK ABORTED: ProjectId column still exists (migration not completed)';
    END IF;

    RAISE NOTICE '✓ Migration state validated for rollback';
END $$;
```

---

## Medium Priority Issues (5)

### 1. **Unused Obsolete Attributes Cause Compiler Warnings**

**Severity:** 🟡 Medium (Code Quality)
**Files:** Task.cs, Project.cs

**Problem:**

- `[Obsolete]` attributes on `Task.ProjectId` (Line 7)
- `[Obsolete]` attributes on `Project` class (Line 5)
- Properties still used (navigation properties in TaskConfiguration.cs:81-84)
- Compiler warnings ignored

**Impact:**

- False sense of security (attributes suggest "don't use")
- Code still references obsolete properties
- Migration cleanup delayed

**Recommendation:**

- Remove `[Obsolete]` until Phase 3 cleanup
- Or use `#pragma warning disable CS0618` around legitimate uses
- Document migration status in README instead

---

### 2. **Missing Error Handling in Transaction Blocks**

**Severity:** 🟡 Medium (Reliability)
**Files:** All migration scripts

**Problem:**

- Scripts use `BEGIN` but no explicit exception handling
- Verification queries raise `EXCEPTION` on failure (GOOD)
- But no logging of errors before rollback
- No error codes for troubleshooting

**Impact:**

- Difficult to debug failed migrations
- No audit trail of failures
- Manual intervention required

**Recommendation:**

```sql
-- Add logging table
CREATE TABLE IF NOT EXISTS _migration_logs (
    id SERIAL PRIMARY KEY,
    script_name TEXT,
    step TEXT,
    status TEXT,
    message TEXT,
    timestamp TIMESTAMP DEFAULT NOW()
);

-- Wrap each step
DO $$
BEGIN
    INSERT INTO _migration_logs(script_name, step, status, message)
    VALUES ('MigrateProjectsToTaskLists', 'backup', 'started', 'Creating backup');
EXCEPTION WHEN OTHERS THEN
    INSERT INTO _migration_logs(script_name, step, status, message)
    VALUES ('MigrateProjectsToTaskLists', 'backup', 'failed', SQLERRM);
    RAISE;
END $$;
```

---

### 3. **SignalR Group Naming Inconsistency**

**Severity:** 🟡 Medium (Bug)
**Files:** TaskHub.cs, TaskEndpoints.cs

**Problem:**

- `TaskHub.GetTaskListGroupName()` (Line 68): `"tasklist_{taskListId}"`
- `TaskEndpoints.cs` (Line 62): `"tasklist_{request.TaskListId}"`
- Consistent (GOOD)
- But no validation that `taskListId` is valid GUID

**Impact:**

- Invalid GUIDs create invalid group names
- No error if user passes malformed ID
- Silent failures in SignalR broadcasts

**Fix:**

```csharp
// TaskHub.cs:22
public async Task JoinTaskList(Guid taskListId)
{
    if (taskListId == Guid.Empty)
    {
        throw new ArgumentException("Invalid TaskListId", nameof(taskListId));
    }
    // ... rest
}
```

---

### 4. **Performance: No Batch Size Limits in Migration Scripts**

**Severity:** 🟡 Medium (Performance)
**Files:** MigrateProjectsToTaskLists.sql, MigrateTasksToTaskLists.sql

**Problem:**

- `INSERT INTO ... SELECT ...` copies all rows in one transaction
- For large tables (>100K rows), causes:
  - Long-running transaction (locks tables)
  - High memory usage
  - Transaction log bloat

**Impact:**

- Migration timeout on large databases
- Performance degradation during migration
- Difficult to resume if interrupted

**Recommendation:**

```sql
-- Batch insert for Projects → TaskLists (example)
DO $$
DECLARE
    v_batch_size INT := 1000;
    v_offset INT := 0;
    v_total INT;
BEGIN
    SELECT COUNT(*) FROM "Projects" INTO v_total;

    WHILE v_offset < v_total LOOP
        INSERT INTO "TaskLists" (...)
        SELECT ...
        FROM "Projects"
        ORDER BY "Id"
        LIMIT v_batch_size OFFSET v_offset;

        v_offset := v_offset + v_batch_size;
        RAISE NOTICE 'Migrated % of % Projects', v_offset, v_total;
    END LOOP;
END $$;
```

---

### 5. **Missing Database Size Estimation in Docs**

**Severity:** 🟡 Medium (Planning)
**File:** `/apps/backend/docs/migration/MIGRATION_README.md`

**Problem:**

- No estimation of database size increase
- Backup schema doubles storage temporarily
- No warning about disk space requirements

**Impact:**

- Migration fails due to insufficient disk space
- Unexpected storage costs
- Production downtime longer than expected

**Fix:**

````markdown
### Database Size Requirements

**Pre-Migration Size Estimation:**

```sql
SELECT pg_size_pretty(pg_database_size('nexora_management')) as current_size;
```
````

**Expected Growth During Migration:**

- Backup schema: ~100% of current size (temporary)
- New tables (Spaces, TaskLists): ~10% of current size (permanent)
- **Total required:** ~2.1x current database size (during migration only)
- **Post-migration:** ~1.1x current database size (after cleanup)

**Example:**

- Current: 10 GB
- Required during migration: 21 GB
- Post-migration: 11 GB
- Freed after cleanup: 10 GB

````

---

## Low Priority Issues (3)

### 1. **TODO Comments in Production Code**
**Severity:** 🟢 Low (Technical Debt)
**Files:** TaskConfiguration.cs (Lines 23, 87)

```csharp
// TODO: After migration, remove ProjectId and keep only TaskListId
// TODO: After migration, remove Project relationship and keep only TaskList
````

**Recommendation:**

- Create GitHub issues for TODOs
- Link TODOs to Phase 3 plan
- Remove TODO comments from production code

---

### 2. **No Dry-Run Mode in Migration Scripts**

**Severity:** 🟢 Low (Developer Experience)
**Files:** All migration scripts

**Problem:**

- Scripts are all-or-nothing
- No way to preview changes without committing
- Difficult to test in staging

**Recommendation:**

```sql
-- Add dry-run parameter
DO $$
DECLARE
    v_dry_run BOOLEAN := COALESCE(NULLIF(:dry_run, ''), 'false')::BOOLEAN;
BEGIN
    IF v_dry_run THEN
        RAISE NOTICE 'DRY RUN MODE - No changes will be committed';
        -- Run SELECTs only, no INSERTs/UPDATEs
    ELSE
        RAISE NOTICE 'PRODUCTION MODE - Changes will be committed';
        -- Run actual migration
    END IF;
END $$;
```

---

### 3. **Inconsistent RAISE NOTICE Messages**

**Severity:** 🟢 Low (User Experience)
**Files:** All migration scripts

**Problem:**

- Some messages use emoji: `✓`, `❌`, `⚠️` (human-friendly)
- Some messages plain text (machine-readable)
- No structured logging (JSON)

**Recommendation:**

- Standardize message format
- Use structured logging for machine parsing
- Keep emoji for human-readable output

```sql
-- Standardized format
RAISE NOTICE '{"step":"backup","status":"started","message":"Creating backup schema","timestamp":"%"}', NOW();
```

---

## Security Analysis

### SQL Injection Vulnerabilities

**Status:** ✅ **NO CRITICAL VULNERABILITIES FOUND**

**Why Safe:**

- All SQL uses hardcoded identifiers (table/column names)
- No dynamic SQL concatenation
- No user input in DDL statements
- Parameterized queries in application layer (C#)

**Recommendations:**

- Add schema name validation (see Critical Issue #2)
- Use `QUOTE_IDENT()` if dynamic SQL ever needed
- Run SQL security linter (pgLint) before deployment

---

### Backup Schema Security

**Status:** ⚠️ **CONCERN**

**Issues:**

- Backup schema `_backup_projects` accessible to all database users
- No row-level security (RLS) on backup tables
- Sensitive data in plain text (no encryption at rest)

**Recommendations:**

```sql
-- Restrict backup schema access
REVOKE ALL ON SCHEMA _backup_projects FROM PUBLIC;
GRANT USAGE ON SCHEMA _backup_projects TO dba_role;
GRANT SELECT ON ALL TABLES IN SCHEMA _backup_projects TO dba_role;

-- Enable RLS (if using PostgreSQL RLS)
ALTER TABLE _backup_projects."Projects" ENABLE ROW LEVEL SECURITY;
```

---

### Transaction Safety

**Status:** ✅ **GOOD**

**Strengths:**

- All operations in `BEGIN`/`COMMIT` blocks
- Automatic rollback on error
- Verification queries before commit
- Manual commit required (prevents accidental execution)

**Gaps:**

- No explicit table locks (see Critical Issue #3)
- No savepoints for partial rollback
- No transaction timeout configuration

**Recommendations:**

```sql
-- Add savepoints for complex operations
SAVEPOINT before_tasks_migration;
-- ... migration steps ...
-- If fails, ROLLBACK TO before_tasks_migration;
```

---

### Foreign Key Constraint Safety

**Status:** ✅ **EXCELLENT**

**Strengths:**

- FK constraints created before data migration
- `ON DELETE CASCADE` prevents orphaned records
- Verification queries check referential integrity
- Rollback recreates FK constraints

**No Issues Found.**

---

## Performance Analysis

### Query Performance Expectations

**Before Migration (Projects):**

```sql
SELECT * FROM "Tasks" WHERE "ProjectId" = ?;
-- Index: IX_Tasks_ProjectId (btree)
-- Expected: 1-5ms (Index Scan)
```

**After Migration (TaskLists):**

```sql
SELECT * FROM "Tasks" WHERE "TaskListId" = ?;
-- Index: IX_Tasks_TaskListId (btree)
-- Expected: 1-5ms (Index Scan) ✓ SAME
```

**Common Query Pattern (Board View):**

```sql
SELECT * FROM "Tasks"
WHERE "TaskListId" = ? AND "StatusId" = ?;
-- Before: Composite index (ProjectId, StatusId, PositionOrder)
-- After: Single index (TaskListId) - SLOWER
-- Fix: Add composite index (see High Priority Issue #1)
```

---

### Migration Execution Time Estimates

| Script                     | Rows          | Est. Time | Bottleneck        |
| -------------------------- | ------------- | --------- | ----------------- |
| MigrateProjectsToTaskLists | 1K Projects   | 5-10s     | INSERT SELECT     |
| MigrateProjectsToTaskLists | 10K Projects  | 30-60s    | FK validation     |
| MigrateProjectsToTaskLists | 100K Projects | 5-10 min  | Transaction log   |
| MigrateTasksToTaskLists    | 1K Tasks      | 5-10s     | UPDATE statement  |
| MigrateTasksToTaskLists    | 10K Tasks     | 30-60s    | Column operations |
| MigrateTasksToTaskLists    | 100K Tasks    | 5-15 min  | Index creation    |

**Recommendations:**

- Test with production data copy in staging
- Add progress logging for large datasets
- Consider batching for >100K rows

---

### Index Strategy

**Current Indexes:**

```sql
-- Primary
CREATE INDEX "IX_Tasks_TaskListId" ON "Tasks"("TaskListId"); ✓

-- Missing (Critical)
CREATE INDEX "IX_Tasks_TaskListId_StatusId" ON "Tasks"("TaskListId", "StatusId"); ✗

-- Existing (Good)
CREATE INDEX "IX_Tasks_StatusId" ON "Tasks"("StatusId"); ✓
CREATE INDEX "idx_tasks_list" ON "Tasks"("ProjectId", "StatusId", "PositionOrder"); ✓
```

**Recommendation:**

- Add composite index before migration
- Drop old `idx_tasks_list` after migration
- Analyze query patterns with `pg_stat_statements`

---

## Architecture Compliance

### Clean Architecture Principles

**Domain Layer:**
✅ **COMPLIANT**

- `Task.cs` and `Project.cs` in `/Domain/Entities`
- No infrastructure dependencies
- `[Obsolete]` attributes appropriate for migration period

**Application Layer:**
✅ **COMPLIANT**

- Commands/Queries in `/Application/Tasks`
- DTOs in `/Application/Tasks/DTOs`
- CQRS pattern maintained
- MediatR for decoupling

**Infrastructure Layer:**
✅ **COMPLIANT**

- EF Core configurations in `/Infrastructure/Persistence/Configurations`
- Database-specific logic isolated
- Migrations in `/API/Persistence/Migrations`

**API Layer:**
✅ **COMPLIANT**

- Minimal route handlers
- No business logic
- SignalR for real-time (appropriate)

**Dependency Direction:**
✅ **CORRECT**

```
API → Application → Domain
    ↓
Infrastructure → Domain
```

---

### YAGNI / KISS / DRY Principles

**YAGNI (You Aren't Gonna Need It):**
✅ **GOOD**

- No premature optimization
- No unused features
- Migration scripts minimal (do one thing well)

**KISS (Keep It Simple, Stupid):**
✅ **GOOD**

- Straightforward SQL (no complex CTEs)
- Clear linear steps in migration
- Easy to read and maintain

**DRY (Don't Repeat Yourself):**
⚠️ **MEDIUM**

- Verification queries duplicated across scripts
- Could extract to shared function
- Not critical (verification is safety-critical, duplication OK)

**Example of DRY violation:**

```sql
-- Duplicated in 3 scripts
DO $$
DECLARE
    v_count INT;
BEGIN
    SELECT COUNT(*) INTO v_count FROM "Tasks";
    RAISE NOTICE 'Tasks count: %', v_count;
END $$;
```

---

### Separation of Concerns

**Migration Scripts:**
✅ **WELL SEPARATED**

- `MigrateProjectsToTaskLists.sql` - Projects → TaskLists
- `MigrateTasksToTaskLists.sql` - Tasks migration
- `ValidateMigration.sql` - Validation only
- `RollbackMigration.sql` - Rollback only

**Application Layer:**
✅ **WELL SEPARATED**

- Commands (write operations)
- Queries (read operations)
- DTOs (data transfer)
- No mixing of concerns

**No Issues Found.**

---

## Code Quality Assessment

### Documentation Quality

**Grade:** A (Excellent)

**Strengths:**

- Comprehensive MIGRATION_README.md (457 lines)
- Detailed ROLLBACK_PROCEDURES.md (490 lines)
- Inline comments in SQL scripts
- Step-by-step instructions
- Troubleshooting guide
- Decision trees for rollback

**Minor Gaps:**

- No database size estimation (see Medium Issue #5)
- No dry-run mode documentation
- No video walkthrough for complex steps

---

### Error Handling

**Grade:** B+ (Good)

**Strengths:**

- Exception blocks in verification queries
- Automatic rollback on failure
- Clear error messages
- Validation before destructive operations

**Gaps:**

- No logging table for audit trail
- No error codes for automation
- No retry logic for transient failures
- No partial rollback capability (savepoints)

---

### Testing Coverage

**Status:** ⚠️ **NOT ASSESSED**

**Missing:**

- No unit tests for migration scripts
- No integration tests for application changes
- No performance benchmarks
- No rollback tests in staging

**Recommendations:**

```bash
# Add to CI/CD pipeline
dotnet test Nexora.Management.Tests --filter "FullyQualifiedName~Migration"
psql -h staging-db -f scripts/MigrateProjectsToTaskLists.sql
psql -h staging-db -f scripts/ValidateMigration.sql
# Expect: All tests pass
```

---

## Recommendations

### Before Production Deployment (Must Fix)

1. **Fix CreateTaskCommand ProjectId bug** (Critical #1)
   - Change Line 62 from `ProjectId = tasklist.SpaceId` to remove assignment
   - Verify Tasks reference correct entity

2. **Add table locks to migration scripts** (Critical #3)
   - Add `LOCK TABLE` statements at start of each script
   - Update documentation: Stop application BEFORE migration

3. **Add composite index** (High #1)
   - Create `IX_Tasks_TaskListId_StatusId` index
   - Test board view query performance

4. **Validate migration state in rollback** (High #4)
   - Add checks for TaskListId/ProjectId columns
   - Prevent rollback if migration incomplete

5. **Add prerequisite validation** (High #3)
   - Check Projects table exists
   - Check ProjectId column exists
   - Verify no orphaned Tasks before migration

---

### During Migration (Operational)

1. **Test in Staging First**
   - Full migration on production data copy
   - Measure execution time
   - Test rollback procedure
   - Verify application starts successfully

2. **Add Monitoring**
   - Database connection pool
   - Query performance (pg_stat_statements)
   - Disk space usage
   - Application error logs

3. **Prepare Rollback Plan**
   - Document rollback decision tree
   - Test rollback in staging
   - Prepare hotfix branch
   - Notify stakeholders

---

### Post-Migration (Phase 3 Cleanup)

1. **Remove obsolete code**
   - Remove `[Obsolete]` attributes
   - Drop `ProjectId` column from Tasks entity
   - Remove Project navigation properties
   - Delete Projects table after 30-day validation

2. **Add tests**
   - Unit tests for migration logic
   - Integration tests for API endpoints
   - Performance benchmarks

3. **Optimize performance**
   - Analyze query patterns
   - Add missing indexes
   - Update database statistics
   - Monitor slow query log

4. **Update documentation**
   - Remove migration docs
   - Update API documentation
   - Update system architecture diagrams
   - Train users on new hierarchy

---

## Unresolved Questions

1. **Why does CreateTaskCommand set `ProjectId = tasklist.SpaceId`?**
   - Appears to be a bug (wrong entity type)
   - Should use `tasklist.Id` or not set at all
   - Needs clarification from original developer

2. **Are there integration tests for the migration?**
   - Not found in codebase
   - Should exist before production deployment
   - Critical for data safety

3. **What is the production database size?**
   - Needed to estimate migration time
   - Disk space requirements unknown
   - Performance impact unclear

4. **Is there a staging environment with production data?**
   - Required for testing
   - Not mentioned in documentation
   - Critical for safe deployment

5. **What is the rollback SLA?**
   - How quickly must rollback complete?
   - Currently estimated 20-30 min
   - May be too slow for critical production

---

## Conclusion

**Overall Grade: B+**

**Strengths:**

- Robust SQL migration scripts with comprehensive verification
- Excellent documentation (README + rollback procedures)
- Clean architecture compliance maintained
- Good separation of concerns
- Safe transaction handling

**Critical Issues:**

1. CreateTaskCommand bug (data corruption risk)
2. Missing table locks (race condition risk)
3. Missing composite index (performance regression)

**Recommendation:**

- Address critical issues before production deployment
- Test migration and rollback in staging first
- Add monitoring and alerting
- Plan Phase 3 cleanup

**Migration Readiness:** ⚠️ **NOT READY** (requires critical fixes)

**Estimated Time to Ready:** 2-3 days (fix + test + staging validation)

---

**Review Completed:** 2026-01-07 22:11 UTC
**Next Review:** After critical fixes applied
**Report Generated By:** Code Reviewer Subagent (ac678a2)
