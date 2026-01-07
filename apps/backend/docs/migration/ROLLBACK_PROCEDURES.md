# Emergency Rollback Procedures

**Version:** 1.0
**Date:** 2026-01-07
**Purpose:** Emergency rollback from TaskList hierarchy back to Project hierarchy
**Risk Level:** 🔴 CRITICAL
**Estimated Time:** 20-30 minutes

---

## When to Use This Guide

### Critical Scenarios Requiring Rollback

- ❌ **Data Loss:** Tasks or Projects missing after migration
- ❌ **Application Failure:** Backend won't start or crashes repeatedly
- ❌ **Performance Severe:** Queries taking > 10x longer than baseline
- ❌ **User Impact:** Critical functionality broken (e.g., cannot create/view Tasks)
- ❌ **Data Corruption:** Orphaned Tasks > 1% of total Tasks

### Non-Rollback Scenarios (Fix Forward)

- ⚠️ Minor UI bugs (fix with hotfix)
- ⚠️ Missing indexes (add post-migration)
- ⚠️ API contract mismatches (update frontend)
- ⚠️ SignalR group naming (update clients)
- ⚠️ Documentation errors (update docs)

---

## Pre-Rollback Checklist

### 1. Verify Backup Availability

```sql
-- Check backup tables exist
SELECT
    table_schema,
    table_name,
    (SELECT COUNT(*) FROM information_schema.columns WHERE table_name = _backup_projects.table_name) as column_count
FROM information_schema.tables
WHERE table_schema = '_backup_projects'
ORDER BY table_name;

-- Expected: Projects, Tasks tables exist
```

If backups missing: **STOP** - Use full database backup instead.

### 2. Document Current State

```sql
-- Record current counts for comparison
SELECT
    'TaskLists' as table_name, COUNT(*) as row_count FROM "TaskLists"
UNION ALL
SELECT 'Tasks', COUNT(*) FROM "Tasks"
UNION ALL
SELECT 'Spaces', COUNT(*) FROM "Spaces";

-- Save this output for rollback verification
```

### 3. Notify Stakeholders

- [ ] Product Manager notified
- [ ] Support team notified
- [ ] Users notified via maintenance page
- [ ] Rollback ETA communicated (20-30 min)

---

## Rollback Execution

### Step 1: Application Shutdown (5 min)

```bash
# Stop backend to prevent new writes
systemctl stop nexora-api
# OR
dotnet-stop Nexora.Management.API

# Verify stopped
ps aux | grep Nexora.Management.API
```

**Critical:** Ensure no active database connections before proceeding.

---

### Step 2: Database Rollback (10 min)

**Script:** `/apps/backend/scripts/RollbackMigration.sql`

```bash
# Connect to database
psql -h localhost -U postgres -d nexora_management

# Run rollback in transaction
psql -h localhost -U postgres -d nexora_management -f scripts/RollbackMigration.sql
```

**What It Does:**

1. Verifies backup tables exist
2. Drops FK_Tasks_TaskLists_TaskListId
3. Drops IX_Tasks_TaskListId
4. Adds ProjectId column back
5. Copies TaskListId → ProjectId
6. Sets ProjectId to NOT NULL
7. Recreates FK_Tasks_Projects_ProjectId
8. Creates IX_Tasks_ProjectId
9. Drops TaskListId column
10. Verifies Tasks restored correctly
11. Drops TaskLists table

**Expected Output:**

```
✓ Backup tables verified
Dropped FK: FK_Tasks_TaskLists_TaskListId
Dropped index: IX_Tasks_TaskListId
Added column: ProjectId
Restored ProjectId from TaskListId: N rows
Set ProjectId to NOT NULL
Created FK: FK_Tasks_Projects_ProjectId
Created index: IX_Tasks_ProjectId
Dropped column: TaskListId
✓ All Tasks have valid ProjectId references
Dropped table: TaskLists
```

**If Rollback Fails:**

```sql
-- Check current state
\d "Tasks"
SELECT COUNT(*) FROM "Tasks";
SELECT COUNT(*) FROM "Tasks" WHERE "ProjectId" IS NULL;

-- Manual fix if needed
ALTER TABLE "Tasks" ADD COLUMN "ProjectId" UUID;
UPDATE "Tasks" SET "ProjectId" = (SELECT "Id" FROM _backup_projects."Tasks" bt WHERE bt."Id" = "Tasks"."Id");
```

---

### Step 3: Post-Rollback Validation (5 min)

```sql
-- Verify Projects table accessible
SELECT COUNT(*) as projects_count FROM "Projects";

-- Verify Tasks restored
SELECT
    (SELECT COUNT(*) FROM _backup_projects."Tasks") as backup_count,
    (SELECT COUNT(*) FROM "Tasks") as current_count;

-- Verify no orphaned Tasks
SELECT COUNT(*) as orphaned_tasks
FROM "Tasks" t
LEFT JOIN "Projects" p ON t."ProjectId" = p."Id"
WHERE p."Id" IS NULL;
-- Expected: 0

-- Verify FK exists
SELECT constraint_name
FROM information_schema.table_constraints
WHERE table_name = 'Tasks'
AND constraint_name = 'FK_Tasks_Projects_ProjectId';
-- Expected: 1 row
```

**If Validation Fails:**

- Check rollback script output for errors
- Review application logs
- Contact DBA for assistance

---

### Step 4: Code Rollback (5 min)

```bash
# Revert to pre-migration commit
cd /apps/backend
git revert <migration-commit-hash>
git push origin main

# Rebuild application
dotnet restore
dotnet build
dotnet test

# Expected: All tests pass
```

**If Build Fails:**

- Check for merge conflicts
- Resolve conflicts manually
- Rebuild and retest

---

### Step 5: Application Restart (5 min)

```bash
# Start backend
systemctl start nexora-api
# OR
dotnet-start Nexora.Management.API

# Check logs
tail -f logs/nexora.log

# Verify health endpoint
curl https://api.nexora.com/health
# Expected: {"status":"healthy"}
```

**Expected:** Application starts without errors.

---

### Step 6: Smoke Testing (10 min)

### Backend API Tests

```bash
# Test Projects endpoint (should work)
curl https://api.nexora.com/api/projects -H "Authorization: Bearer $TOKEN"
# Expected: 200 OK, returns Projects list

# Test Tasks endpoint with ProjectId
curl https://api.nexora.com/api/tasks?projectId={id} -H "Authorization: Bearer $TOKEN"
# Expected: 200 OK, returns Tasks

# Create Task with ProjectId
curl -X POST https://api.nexora.com/api/tasks \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title": "Rollback Test", "projectId": "..."}'
# Expected: 201 Created
```

### Frontend Tests

- [ ] Projects page loads
- [ ] Tasks page loads
- [ ] Task creation works
- [ ] Task editing works
- [ ] No console errors

### Database Verification

```sql
-- Verify Projects working
SELECT COUNT(*) FROM "Projects";

-- Verify Tasks working
SELECT COUNT(*) FROM "Tasks";

-- Verify FK constraints working
SELECT constraint_name FROM information_schema.table_constraints
WHERE table_name = 'Tasks' AND constraint_type = 'FOREIGN KEY';
```

---

## Post-Rollback Actions

### Immediate (Within 1 Hour)

- [ ] Monitor application logs for errors
- [ ] Check error rates (should be < baseline)
- [ ] Verify critical functionality works
- [ ] Update incident status
- [ ] Notify stakeholders rollback complete

### Short-Term (Within 24 Hours)

- [ ] Root cause analysis (why migration failed)
- [ ] Document lessons learned
- [ ] Update migration scripts
- [ ] Schedule retry migration
- [ ] Hotfix any discovered bugs

### Long-Term (Within 1 Week)

- [ ] Review rollback procedures
- [ ] Improve rollback testing
- [ ] Update runbooks
- [ ] Train team on rollback process
- [ ] Schedule migration retry

---

## Rollback Decision Tree

```
Migration Issue Detected
    │
    ├─→ Data Loss Detected?
    │   ├─→ YES → ROLLBACK IMMEDIATELY
    │   └─→ NO → Continue
    │
    ├─→ Application Won't Start?
    │   ├─→ YES → Check logs
    │   │   ├─→ Configuration error? → Fix config, restart
    │   │   └─→ Code error? → ROLLBACK
    │   └─→ NO → Continue
    │
    ├─→ Performance Degradation?
    │   ├─→ YES → Check indexes
    │   │   ├─→ Missing indexes? → Add indexes, continue
    │   │   └─→ Query issue? → Optimize query, continue
    │   └─→ NO → Continue
    │
    ├─→ User Impact?
    │   ├─→ Critical functionality broken? → ROLLBACK
    │   └─→ Minor UI bugs? → Hotfix, continue
    │
    └─→ Error Rate > 5x Baseline?
        ├─→ YES → ROLLBACK
        └─→ NO → Monitor closely
```

---

## Common Rollback Scenarios

### Scenario 1: Orphaned Tasks After Migration

**Symptom:** `ValidateMigration.sql` shows orphaned_tasks > 100

**Decision:** ROLLBACK (data integrity issue)

**Rollback Steps:**

1. Stop application
2. Run RollbackMigration.sql
3. Verify Tasks restored
4. Restart application
5. Investigate why Tasks became orphaned

---

### Scenario 2: Application Won't Start

**Symptom:** Backend crashes with "Invalid column name TaskListId"

**Decision:** Check if migrations applied

**Diagnosis:**

```bash
# Check if migration applied
dotnet ef migrations list
# Look for "AddClickUpHierarchyTables"

# Check database schema
\d "Tasks"
# Look for TaskListId column
```

**Rollback Steps:**

- If migration not applied: Fix code, rebuild
- If migration applied: Run RollbackMigration.sql

---

### Scenario 3: Performance Degradation

**Symptom:** Tasks query takes 5 seconds (baseline: 100ms)

**Decision:** Investigate first

**Diagnosis:**

```sql
EXPLAIN ANALYZE
SELECT t.*, tl."Name"
FROM "Tasks" t
JOIN "TaskLists" tl ON t."TaskListId" = tl."Id"
LIMIT 100;
```

**Rollback Decision:**

- If missing index: Add index, continue
- If inefficient query: Optimize query, continue
- If table scan: ROLLBACK

---

## Rollback Testing

### Pre-Migration Testing

**Test rollback in staging first:**

```bash
# 1. Apply migration in staging
psql -h staging-db -U postgres -d nexora_staging -f scripts/MigrateProjectsToTaskLists.sql
psql -h staging-db -U postgres -d nexora_staging -f scripts/MigrateTasksToTaskLists.sql

# 2. Verify migration succeeds
psql -h staging-db -U postgres -d nexora_staging -f scripts/ValidateMigration.sql

# 3. Test rollback
psql -h staging-db -U postgres -d nexora_staging -f scripts/RollbackMigration.sql

# 4. Verify rollback succeeds
SELECT COUNT(*) FROM "Projects";  -- Should equal baseline
SELECT COUNT(*) FROM "Tasks";    -- Should equal baseline
```

**Expected:** All steps complete without errors.

---

## Emergency Contacts

### Primary Contacts

- **On-Call DBA:** [Phone] | [Email]
- **Backend Lead:** [Phone] | [Email]
- **DevOps Engineer:** [Phone] | [Email]

### Escalation Path

1. **Level 1:** On-Call Engineer (first 15 min)
2. **Level 2:** Backend Lead (15-30 min)
3. **Level 3:** CTO (30+ min, critical issues only)

---

## Rollback Report Template

After rollback, complete this report:

```markdown
# Rollback Report - [Date]

## Summary

- **Trigger:** [Why rollback initiated]
- **Duration:** [Start time] - [End time]
- **Decision Maker:** [Who authorized rollback]

## Issues Encountered

1. [Issue 1 description]
2. [Issue 2 description]

## Rollback Steps Taken

1. [Step 1 completed at HH:MM]
2. [Step 2 completed at HH:MM]
   ...

## Verification Results

- [ ] Projects count restored (N rows)
- [ ] Tasks count restored (M rows)
- [ ] Orphaned Tasks (0)
- [ ] Application starts successfully
- [ ] Smoke tests passed

## Post-Rollback Status

- Application Status: [Healthy/Unhealthy]
- Error Rate: [% vs baseline]
- User Impact: [Description]

## Next Steps

1. [Root cause analysis scheduled]
2. [Migration retry planned]
3. [Hotfix deployed]

## Lessons Learned

- [What went wrong]
- [How to prevent next time]
```

---

## Additional Resources

- [Migration README](./MIGRATION_README.md)
- [Validation Queries](../../scripts/ValidateMigration.sql)
- [Original Migration Plan](../../plans/260107-0051-clickup-hierarchy-implementation/plan.md)

---

**Document Version:** 1.0
**Last Updated:** 2026-01-07
**Maintained By:** Development Team
