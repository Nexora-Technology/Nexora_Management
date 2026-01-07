# ClickUp Hierarchy Migration Guide

**Version:** 1.0
**Date:** 2026-01-07
**Phase:** Backend Database Migration (Phase 2)
**Risk Level:** 🔴 HIGH
**Estimated Downtime:** 1-2 hours

---

## Executive Summary

This guide documents the migration from **Workspace → Project → Task** hierarchy to **Workspace → Space → Folder (optional) → TaskList → Task** hierarchy, following ClickUp's proven structure.

### Migration Scope

- **Database:** PostgreSQL 16
- **Backend:** .NET 9.0 / EF Core 9.0
- **Affected Tables:** Projects, TaskLists, Tasks
- **New Entities:** Space, Folder, TaskList
- **Deprecated:** Project entity (kept for 30-day rollback)

---

## Migration Overview

### Before Migration

```
Workspace
  └─ Project
       └─ Task
```

### After Migration

```
Workspace
  └─ Space (Required)
       ├─ Folder (Optional)
       │    └─ TaskList
       │         └─ Task
       └─ TaskList (Can exist directly under Space)
            └─ Task
```

---

## Prerequisites Checklist

### 1. Environment Setup

- [ ] Staging environment configured
- [ ] Database backup completed (full dump)
- [ ] Backup restoration tested
- [ ] Migration scripts reviewed and approved
- [ ] Maintenance window scheduled
- [ ] Stakeholders notified 24h in advance

### 2. Database Health Checks

```sql
-- Check Projects count
SELECT COUNT(*) FROM "Projects";

-- Check Tasks count
SELECT COUNT(*) FROM "Tasks";

-- Verify all Tasks have valid ProjectId
SELECT COUNT(*) FROM "Tasks" t
LEFT JOIN "Projects" p ON t."ProjectId" = p."Id"
WHERE p."Id" IS NULL;
-- Expected: 0
```

### 3. Application Status

- [ ] Backend application builds successfully
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] Frontend updated to use TaskListId
- [ ] API documentation updated

---

## Migration Steps

### Step 1: Pre-Migration Backup (15 min)

**Script:** `/apps/backend/scripts/MigrateProjectsToTaskLists.sql` (Lines 1-22)

```bash
# Connect to database
psql -h localhost -U postgres -d nexora_management

# Create backup schema
CREATE SCHEMA IF NOT EXISTS _backup_projects;

# Backup Projects table
CREATE TABLE _backup_projects."Projects" AS
SELECT * FROM "Projects";

# Backup Tasks table
CREATE TABLE _backup_projects."Tasks" AS
SELECT * FROM "Tasks";

# Verify backups
SELECT 'Projects' as table_name, COUNT(*) as row_count FROM _backup_projects."Projects"
UNION ALL
SELECT 'Tasks', COUNT(*) FROM _backup_projects."Tasks";
```

**Verification:** Ensure backup counts match production counts.

---

### Step 2: Migrate Projects → TaskLists (30 min)

**Script:** `/apps/backend/scripts/MigrateProjectsToTaskLists.sql` (Lines 24-169)

```bash
# Run migration in transaction
psql -h localhost -U postgres -d nexora_management -f scripts/MigrateProjectsToTaskLists.sql
```

**What It Does:**

1. Creates default "General" Space per Workspace
2. Copies all Projects to TaskLists (preserving IDs)
3. Runs 3 verification queries
4. Displays summary report

**Expected Output:**

```
✓ Verification 1 PASSED: TaskLists count = Projects count (N)
✓ Verification 2 PASSED: All TaskLists have valid SpaceId
✓ Verification 3 PASSED: All Project IDs preserved in TaskLists
```

**If Verification Fails:**

```sql
-- Rollback immediately
ROLLBACK;

-- Check what went wrong
SELECT * FROM "Spaces";
SELECT * FROM "TaskLists";
SELECT COUNT(*) FROM "Projects";
SELECT COUNT(*) FROM "TaskLists";
```

---

### Step 3: Migrate Tasks (45 min)

**Script:** `/apps/backend/scripts/MigrateTasksToTaskLists.sql`

```bash
# ⚠️ ONLY run after Step 2 succeeds
psql -h localhost -U postgres -d nexora_management -f scripts/MigrateTasksToTaskLists.sql
```

**What It Does:**

1. Verifies TaskLists exist
2. Backs up Tasks table
3. Adds temporary column TaskListId_New
4. Copies ProjectId → TaskListId_New
5. Verifies no orphaned Tasks
6. Drops old FK constraint
7. Drops ProjectId column
8. Renames TaskListId_New → TaskListId
9. Creates new FK to TaskLists
10. Creates index

**Expected Output:**

```
✓ Prerequisite check passed: N TaskLists found
✓ All Tasks have valid ProjectId references
✓ Verification PASSED: All Tasks have valid TaskListId
✓ Verification 1 PASSED: Tasks count unchanged (M)
✓ Verification 2 PASSED: All Tasks have valid TaskListId
✓ Verification 3 PASSED: FK constraint exists
✓ Verification 4 PASSED: Index created
```

**Critical Moment:** After Step 8 (column rename), the application will break until Step 10 (FK creation) completes.

---

### Step 4: Post-Migration Validation (30 min)

**Script:** `/apps/backend/scripts/ValidateMigration.sql`

```bash
# Run comprehensive validation
psql -h localhost -U postgres -d nexora_management -f scripts/ValidateMigration.sql
```

**Expected Results:**

| Check                | Expected Value            |
| -------------------- | ------------------------- |
| Spaces per Workspace | ≥ 1                       |
| TaskLists count      | = original Projects count |
| Tasks count          | = original Tasks count    |
| Orphaned Tasks       | 0                         |
| Invalid SpaceId      | 0                         |
| FK constraints exist | YES                       |
| Indexes created      | YES                       |
| Query execution time | < 100ms                   |

---

### Step 5: Application Restart (10 min)

```bash
# Stop backend
dotnet-stop Nexora.Management.API

# Start backend
dotnet-start Nexora.Management.API

# Check logs for errors
tail -f logs/nexora.log
```

**Expected:** Application starts without errors, Tasks load successfully.

---

### Step 6: Smoke Testing (30 min)

### Backend API Tests

```bash
# Test Spaces endpoint
curl https://api.nexora.com/api/spaces -H "Authorization: Bearer $TOKEN"

# Test TaskLists endpoint
curl https://api.nexora.com/api/tasklists -H "Authorization: Bearer $TOKEN"

# Test Tasks endpoint
curl https://api.nexora.com/api/tasks?taskListId={id} -H "Authorization: Bearer $TOKEN"

# Create Task
curl -X POST https://api.nexora.com/api/tasks \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"title": "Test Task", "taskListId": "..."}'
```

### Frontend Tests

- [ ] Spaces page loads space tree
- [ ] TaskList detail page shows tasks
- [ ] Task modal shows TaskList selector
- [ ] Tasks can be created/edited/deleted
- [ ] No console errors
- [ ] Real-time updates work (SignalR)

---

## Rollback Procedures

### When to Rollback

- ❌ Data loss detected
- ❌ Orphaned Tasks > 0
- ❌ Application fails to start
- ❌ Critical bugs in production
- ❌ Performance degradation > 50%

### Rollback Steps

**Script:** `/apps/backend/scripts/RollbackMigration.sql`

```bash
# Run rollback
psql -h localhost -U postgres -d nexora_management -f scripts/RollbackMigration.sql
```

**What It Does:**

1. Restores Tasks.TaskListId → Tasks.ProjectId
2. Recreates FK to Projects
3. Drops TaskLists table
4. Restores Projects functionality

**Post-Rollback Verification:**

```bash
# Restart application
dotnet-stop && dotnet-start

# Verify Projects work
curl https://api.nexora.com/api/projects

# Verify Tasks work
curl https://api.nexora.com/api/tasks?projectId={id}
```

---

## Troubleshooting

### Issue 1: Orphaned Tasks After Migration

**Symptom:** `ValidateMigration.sql` shows orphaned_tasks > 0

**Cause:** Some Tasks referenced Projects that were deleted

**Solution:**

```sql
-- Find orphaned tasks
SELECT t."Id", t."Title"
FROM "Tasks" t
LEFT JOIN "TaskLists" tl ON t."TaskListId" = tl."Id"
WHERE tl."Id" IS NULL;

-- Assign to default TaskList
UPDATE "Tasks"
SET "TaskListId" = (SELECT "Id" FROM "TaskLists" LIMIT 1)
WHERE "TaskListId" IS NULL;
```

---

### Issue 2: FK Constraint Errors

**Symptom:** Migration fails at constraint creation

**Cause:** Constraint name mismatch

**Solution:**

```sql
-- Check existing constraint names
SELECT constraint_name
FROM information_schema.table_constraints
WHERE table_name = 'Tasks'
AND constraint_type = 'FOREIGN KEY';

-- Drop manually if needed
ALTER TABLE "Tasks"
DROP CONSTRAINT IF EXISTS {old_constraint_name};
```

---

### Issue 3: Application Won't Start

**Symptom:** Backend crashes on startup

**Cause:** Missing migrations or configuration

**Solution:**

```bash
# Check pending migrations
dotnet ef migrations list

# Apply pending migrations
dotnet ef database update

# Rebuild application
dotnet build
dotnet run
```

---

### Issue 4: Performance Degradation

**Symptom:** Queries slower than before migration

**Cause:** Missing indexes or inefficient queries

**Solution:**

```sql
-- Check query execution plan
EXPLAIN ANALYZE
SELECT t.*, tl."Name", s."Name"
FROM "Tasks" t
JOIN "TaskLists" tl ON t."TaskListId" = tl."Id"
JOIN "Spaces" s ON tl."SpaceId" = s."Id";

-- Create missing indexes if needed
CREATE INDEX IF NOT EXISTS "IX_Tasks_TaskListId"
ON "Tasks"("TaskListId");
```

---

## Post-Migration Cleanup

### Day 1-7: Monitor

- [ ] Check application logs daily
- [ ] Monitor query performance
- [ ] Track error rates
- [ ] Gather user feedback

### Day 7-14: Optimization

- [ ] Add missing indexes based on query analysis
- [ ] Optimize slow queries
- [ ] Update API documentation
- [ ] Train users on new hierarchy

### Day 14-30: Deprecation

- [ ] Remove ProjectId from frontend
- [ ] Update all documentation
- [ ] Remove Project API endpoints
- [ ] Drop Projects table (after 30-day validation)

### Day 30: Final Cleanup

```sql
-- Drop Projects table (after validation complete)
DROP TABLE IF EXISTS "Projects" CASCADE;

-- Remove ProjectId from Task entity (C# code)
-- Update EF Core migrations
-- Create final migration to remove Projects
```

---

## Support Contacts

### Technical Team

- **Backend Lead:** [Contact info]
- **DBA:** [Contact info]
- **DevOps:** [Contact info]

### Emergency Contacts

- **On-Call Engineer:** [Contact info]
- **Product Manager:** [Contact info]

---

## Additional Resources

- [Original Plan](../../plans/260107-0051-clickup-hierarchy-implementation/plan.md)
- [System Architecture](../../../../../docs/system-architecture.md)
- [Codebase Summary](../../../../../docs/codebase-summary.md)

---

**Document Version:** 1.0
**Last Updated:** 2026-01-07
**Maintained By:** Development Team
