# Phase 2: Database Migration - Task Breakdown Analysis

**Report Date:** 2026-01-07 15:55
**Analyzed By:** Project Manager Agent
**Plan Reference:** /apps/frontend/plans/260107-0051-clickup-hierarchy-implementation/plan.md (Lines 260-747)
**Phase:** 2 - Database Migration (14h estimated)

---

## Executive Summary

**STATUS:** Phase 2 is **PARTIALLY COMPLETE**. EF Core entities and configurations are already implemented. Remaining work focuses on data migration scripts and validation.

**Key Findings:**

- ✅ **DONE:** All entity classes created (Space, Folder, TaskList, Task)
- ✅ **DONE:** All EF Core configurations implemented
- ✅ **DONE:** Migration file created (20260106184122_AddClickUpHierarchyTables.cs)
- ❌ **TODO:** Data migration scripts (Projects → TaskLists)
- ❌ **TODO:** Task foreign key migration (ProjectId → TaskListId)
- ❌ **TODO:** Migration validation queries

**Estimate Revision:** 14h → **4h remaining** (10h already completed)

---

## 1. Files to Create

### 1.1 Data Migration Scripts (CRITICAL - 2h)

**Directory:** `/apps/backend/scripts/` (needs creation)

| File                             | Purpose                                 | Lines | Priority  |
| -------------------------------- | --------------------------------------- | ----- | --------- |
| `MigrateProjectsToTaskLists.sql` | Copy Projects to TaskLists table        | ~70   | HIGH 🔴   |
| `MigrateTasksToTaskLists.sql`    | Update Task.ProjectId → Task.TaskListId | ~50   | HIGH 🔴   |
| `ValidateMigration.sql`          | Post-migration validation queries       | ~60   | HIGH 🔴   |
| `RollbackMigration.sql`          | Emergency rollback procedures           | ~30   | MEDIUM 🟡 |

**Total:** 4 files, ~210 SQL lines

### 1.2 Supporting Documentation (1h)

| File                     | Purpose                      | Format   |
| ------------------------ | ---------------------------- | -------- |
| `MIGRATION_README.md`    | Step-by-step migration guide | Markdown |
| `ROLLBACK_PROCEDURES.md` | Rollback documentation       | Markdown |

---

## 2. Files to Modify

### 2.1 Entity Updates (0.5h)

**Status:** Most entities already updated, minor tweaks needed.

| File                                                               | Current State                                  | Required Changes                                |
| ------------------------------------------------------------------ | ---------------------------------------------- | ----------------------------------------------- |
| `/apps/backend/src/Nexora.Management.Domain/Entities/Task.cs`      | Has both ProjectId (deprecated) and TaskListId | Add `[Obsolete]` attribute to ProjectId         |
| `/apps/backend/src/Nexora.Management.Domain/Entities/Workspace.cs` | Has Spaces collection                          | ✅ Already correct                              |
| `/apps/backend/src/Nexora.Management.Domain/Entities/Project.cs`   | Exists, needs deprecation                      | Add `[Obsolete]` attribute and warning comments |

### 2.2 AppDbContext Updates (0h)

**Status:** ✅ **COMPLETE** - No changes needed.

**File:** `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/AppDbContext.cs`

**Current State (Lines 31-33):**

```csharp
public DbSet<Space> Spaces => Set<Space>(); // NEW: ClickUp hierarchy
public DbSet<Folder> Folders => Set<Folder>(); // NEW: ClickUp hierarchy
public DbSet<TaskList> TaskLists => Set<TaskList>(); // NEW: ClickUp hierarchy (Lists)
```

**Assessment:** Already includes all new DbSets. Project DbSet retained for rollback window.

### 2.3 EF Configuration Updates (0h)

**Status:** ✅ **COMPLETE** - All configurations exist.

| Configuration File          | Status    | Notes                                        |
| --------------------------- | --------- | -------------------------------------------- |
| `SpaceConfiguration.cs`     | ✅ Exists | Workspace FK cascade delete configured       |
| `FolderConfiguration.cs`    | ✅ Exists | Space FK + unique PositionOrder index        |
| `TaskListConfiguration.cs`  | ✅ Exists | Space/Folder FKs, OwnerId, ListType defaults |
| `TaskConfiguration.cs`      | ✅ Exists | Has both Project and TaskList relationships  |
| `WorkspaceConfiguration.cs` | ✅ Exists | Spaces collection configured                 |

---

## 3. Implementation Order (Critical Dependencies)

### Phase 2A: Pre-Migration Setup (0.5h) ⏳

**Order:**

1. ✅ Verify entities exist (Space, Folder, TaskList)
2. ✅ Verify configurations exist
3. ✅ Verify migration file generated
4. ⏸️ **TODO:** Create `/apps/backend/scripts/` directory

### Phase 2B: Data Migration Scripts (2h) 🔴

**Critical Order:**

```
1. MigrateProjectsToTaskLists.sql
   ├─ Create _backup_projects schema
   ├─ Backup Projects table
   ├─ Create default "General" Space per Workspace
   ├─ Copy Projects → TaskLists (preserve IDs)
   └─ Verify counts match

2. MigrateTasksToTaskLists.sql (⚠️ RUN ONLY AFTER #1 VERIFIED)
   ├─ Backup Tasks table
   ├─ Add TaskListId_New column
   ├─ Copy ProjectId → TaskListId_New
   ├─ Verify 0 orphaned tasks
   ├─ Drop FK_Tasks_Projects_ProjectId
   ├─ Drop ProjectId column
   ├─ Rename TaskListId_New → TaskListId
   ├─ Add NOT NULL constraint
   ├─ Create FK_Tasks_TaskLists_TaskListId
   └─ Create IX_Tasks_TaskListId index

3. ValidateMigration.sql
   └─ Run all validation queries
```

**Dependencies:**

- Step 2 **MUST NOT** run until Step 1 verification passes
- Step 3 validates both steps complete successfully

### Phase 2C: Post-Migration Entity Updates (0.5h)

**Order:**

1. Mark `Task.ProjectId` as `[Obsolete]`
2. Add deprecation notice to `Project.cs`
3. Update XML documentation comments
4. Run `dotnet build` to verify no errors

### Phase 2D: Documentation (1h)

**Order:**

1. Create `MIGRATION_README.md` with step-by-step guide
2. Create `ROLLBACK_PROCEDURES.md` with emergency procedures
3. Document validation query expected results
4. Create migration checklist

---

## 4. Risk Areas (High-Severity Operations)

### 🔴 CRITICAL RISKS

#### Risk 1: Data Loss During Projects → TaskLists Migration

**Severity:** 🔴 CRITICAL
**Impact:** Permanent loss of project data if migration fails
**Probability:** LOW (with proper backups)

**Mitigation Strategies:**

1. **Pre-migration backup:**

   ```sql
   CREATE SCHEMA IF NOT EXISTS _backup_projects;
   CREATE TABLE _backup_projects."Projects" AS SELECT * FROM "Projects";
   ```

2. **Transaction wrapper:**

   ```sql
   BEGIN;
   -- migration steps
   -- Verify counts
   -- COMMIT or ROLLBACK
   ```

3. **Count verification:**

   ```sql
   -- Must return 0
   SELECT (SELECT COUNT(*) FROM "Projects") -
          (SELECT COUNT(*) FROM "TaskLists") as difference;
   ```

4. **Staging environment test:** Run migration in staging first

**Rollback Procedure:**

```sql
DROP TABLE IF EXISTS "TaskLists";
DROP TABLE IF EXISTS "Folders";
DROP TABLE IF EXISTS "Spaces";

INSERT INTO "Projects" SELECT * FROM _backup_projects."Projects";
```

#### Risk 2: Orphaned Tasks After FK Migration

**Severity:** 🔴 CRITICAL
**Impact:** Tasks with invalid references break application
**Probability:** LOW (with validation)

**Mitigation:**

1. **Pre-migration validation:**

   ```sql
   -- Verify all Tasks.ProjectId exist in Projects
   SELECT COUNT(*) FROM "Tasks" t
   LEFT JOIN "Projects" p ON t."ProjectId" = p."Id"
   WHERE p."Id" IS NULL;
   -- Must return 0
   ```

2. **Temporary column approach:**
   - Add `TaskListId_New` column (nullable)
   - Copy data while preserving `ProjectId`
   - Verify before dropping `ProjectId`

3. **Orphan check after migration:**
   ```sql
   SELECT COUNT(*) as orphaned_tasks
   FROM "Tasks" t
   LEFT JOIN "TaskLists" tl ON t."TaskListId" = tl."Id"
   WHERE tl."Id" IS NULL;
   -- Must return 0
   ```

**Rollback Procedure:**

```sql
ALTER TABLE "Tasks" DROP COLUMN IF EXISTS "TaskListId";
ALTER TABLE "Tasks" ADD COLUMN "ProjectId" uuid;
INSERT INTO "Tasks" ("ProjectId") SELECT "ProjectId" FROM _backup_projects."Tasks";
```

#### Risk 3: Foreign Key Constraint Errors

**Severity:** 🟡 MEDIUM
**Impact:** Migration fails mid-process
**Probability:** MEDIUM

**Mitigation:**

1. Check existing constraint names:

   ```sql
   SELECT constraint_name
   FROM information_schema.table_constraints
   WHERE table_name = 'Tasks';
   ```

2. Use `IF EXISTS` for constraint drops:

   ```sql
   ALTER TABLE "Tasks" DROP CONSTRAINT IF EXISTS "FK_Tasks_Projects_ProjectId";
   ```

3. Verify cascade delete behavior:
   - Task.TaskListId should have `ON DELETE CASCADE`
   - Test deleting a TaskList verifies Tasks are deleted

### 🟡 MEDIUM RISKS

#### Risk 4: Application Downtime

**Severity:** 🟡 MEDIUM
**Impact:** Users cannot access Tasks during migration
**Duration:** 1-2 hours

**Mitigation:**

- Schedule during low-traffic hours (e.g., 2 AM UTC)
- Use temporary column approach (can be done live)
- Display maintenance page during FK constraint updates
- Notify users 24h in advance

#### Risk 5: Performance Regression

**Severity:** 🟡 MEDIUM
**Impact:** Queries slower with additional JOINs
**Probability:** LOW (with proper indexes)

**Mitigation:**

1. Verify indexes created:

   ```sql
   SELECT indexname, tablename
   FROM pg_indexes
   WHERE tablename IN ('Spaces', 'Folders', 'TaskLists', 'Tasks');
   ```

2. Run EXPLAIN ANALYZE on critical queries:

   ```sql
   EXPLAIN ANALYZE
   SELECT t.*, tl."Name" as "TaskListName", s."Name" as "SpaceName"
   FROM "Tasks" t
   JOIN "TaskLists" tl ON t."TaskListId" = tl."Id"
   JOIN "Spaces" s ON tl."SpaceId" = s."Id"
   LIMIT 100;
   ```

3. Load test before/after migration

### 🟢 LOW RISKS

#### Risk 6: Entity Naming Confusion

**Severity:** 🟢 LOW
**Impact:** Developer confusion between "List" (UI) vs "TaskList" (C#)

**Mitigation:**

- XML documentation already explains display name is "List"
- Add `[Display(Name = "List")]` attribute if needed
- Document in coding standards

---

## 5. Testing Requirements

### 5.1 Pre-Migration Tests (Before Scripts Run)

**Environment:** Staging database

| Test                 | Command                                                                                                | Expected Result |
| -------------------- | ------------------------------------------------------------------------------------------------------ | --------------- |
| Backup schema exists | `SELECT schema_name FROM information_schema.schemata WHERE schema_name = '_backup_projects'`           | Returns 1 row   |
| Projects count       | `SELECT COUNT(*) FROM "Projects"`                                                                      | N projects      |
| Tasks count          | `SELECT COUNT(*) FROM "Tasks"`                                                                         | M tasks         |
| Valid Task FKs       | `SELECT COUNT(*) FROM "Tasks" t LEFT JOIN "Projects" p ON t."ProjectId" = p."Id" WHERE p."Id" IS NULL` | 0               |

### 5.2 Post-Migration Tests (After Scripts Run)

**Environment:** Staging → Production

| Test                     | Query                                                                                                                             | Expected Result               |
| ------------------------ | --------------------------------------------------------------------------------------------------------------------------------- | ----------------------------- |
| Spaces created           | `SELECT COUNT(*) FROM "Spaces"`                                                                                                   | ≥1 per Workspace              |
| TaskLists count          | `SELECT COUNT(*) FROM "TaskLists"`                                                                                                | = original Projects count     |
| Tasks count unchanged    | `SELECT COUNT(*) FROM "Tasks"`                                                                                                    | = pre-migration count         |
| No orphaned Tasks        | `SELECT COUNT(*) FROM "Tasks" WHERE "TaskListId" IS NULL`                                                                         | 0                             |
| FK constraints exist     | `SELECT constraint_name FROM information_schema.table_constraints WHERE table_name = 'Tasks' AND constraint_type = 'FOREIGN KEY'` | FK_Tasks_TaskLists_TaskListId |
| Indexes created          | `SELECT indexname FROM pg_indexes WHERE tablename = 'Tasks' AND indexname LIKE '%TaskListId%'`                                    | IX_Tasks_TaskListId           |
| TaskList-Space integrity | `SELECT COUNT(*) FROM "TaskLists" tl LEFT JOIN "Spaces" s ON tl."SpaceId" = s."Id" WHERE s."Id" IS NULL`                          | 0                             |

### 5.3 Application-Level Tests

**Backend API Tests:**

- [ ] GET /api/spaces returns 200
- [ ] GET /api/folders returns 200
- [ ] GET /api/tasklists returns 200
- [ ] GET /api/tasks?taskListId={id} returns tasks
- [ ] POST /api/tasks with TaskListId succeeds

**Frontend Integration:**

- [ ] Spaces page loads space tree
- [ ] TaskList detail page shows tasks
- [ ] Task modal shows TaskList selector
- [ ] Breadcrumbs display full hierarchy

### 5.4 Rollback Tests

**Test rollback procedure:**

1. Run migration scripts
2. Verify data migrated
3. Run rollback script
4. Verify Projects table restored
5. Verify Tasks.ProjectId restored
6. Drop backup schema
7. Verify application works with old structure

---

## 6. Ambiguities and Blockers

### 6.1 Missing Information

| Issue                                | Impact                         | Resolution Needed                                   |
| ------------------------------------ | ------------------------------ | --------------------------------------------------- |
| **Database connection string**       | Cannot run scripts             | Check appsettings.json for staging connection       |
| **Staging environment availability** | Cannot test safely             | Verify staging DB exists and is accessible          |
| **Current Projects count**           | Cannot estimate migration time | Run `SELECT COUNT(*) FROM "Projects"` on production |
| **Current Tasks count**              | Cannot estimate downtime       | Run `SELECT COUNT(*) FROM "Tasks"` on production    |
| **Maintenance window**               | Cannot schedule migration      | Coordinate with team for downtime window            |

### 6.2 Plan Ambiguities

**Ambiguity 1: Default Space Creation**

- **Plan says:** "Create default Space for each Workspace" (Line 567)
- **Question:** What if a Workspace already has Spaces?
- **Resolution:** `ON CONFLICT DO NOTHING` handles this (Line 577)

**Ambiguity 2: TaskListId vs ListId Naming**

- **Plan uses:** `ListId` in migration script (Line 640)
- **Codebase uses:** `TaskListId` in Task.cs (Line 9)
- **Resolution:** Plan is outdated - use `TaskListId` consistently

**Ambiguity 3: ProjectId Deprecation Timeline**

- **Plan says:** "Keep Projects table for 30-day rollback window" (Line 69)
- **Question:** When can we remove `[Obsolete]` ProjectId?
- **Resolution:** Document in MIGRATION_README.md with specific date

**Ambiguity 4: FolderId Default Value**

- **Plan says:** All Projects directly under Space initially (FolderId = NULL)
- **Question:** Should we create Folders to organize migrated Projects?
- **Resolution:** NO - YAGNI principle. Users can reorganize later.

### 6.3 Dependencies Not in Plan

**Dependency 1: Row-Level Security (RLS) Policies**

- **Issue:** Existing RLS policies reference WorkspaceId
- **Impact:** New Spaces/Folders/TaskLists need RLS policies
- **Resolution:** Update RLS policies to include SpaceId checks
- **Estimated effort:** +2h (not in original 14h estimate)

**Dependency 2: Application Layer Updates**

- **Issue:** Application code references Task.ProjectId
- **Impact:** Queries will break after FK migration
- **Resolution:** Update all C# queries to use TaskListId
- **Estimated effort:** +3h (not in original 14h estimate)

**Dependency 3: Frontend API Client**

- **Issue:** Frontend calls /api/projects endpoints
- **Impact:** Frontend breaks if endpoints removed
- **Resolution:** Keep /api/projects as deprecated alias
- **Estimated effort:** +1h (not in original 14h estimate)

---

## 7. Revised Implementation Plan

### Phase 2: Database Migration - Updated Estimate

**Original Estimate:** 14h
**Completed Work:** 10h (entities, configs, migration file)
**Remaining Work:** 4h (scripts, validation, docs)

**BUT:** Additional dependencies discovered:

- RLS policy updates: +2h
- Application layer updates: +3h
- Frontend API backward compatibility: +1h

**Revised Total:** 14h + 6h = **20h total** (10h done, 10h remaining)

### Detailed Task Breakdown (10h Remaining)

#### Task 2.1: Create Data Migration Scripts (3h)

- [ ] Create `/apps/backend/scripts/` directory (0.1h)
- [ ] Write `MigrateProjectsToTaskLists.sql` (1h)
- [ ] Write `MigrateTasksToTaskLists.sql` (1h)
- [ ] Write `ValidateMigration.sql` (0.5h)
- [ ] Write `RollbackMigration.sql` (0.4h)

#### Task 2.2: Test Migration Scripts in Staging (2h)

- [ ] Restore production backup to staging (0.3h)
- [ ] Run `MigrateProjectsToTaskLists.sql` (0.2h)
- [ ] Verify Projects → TaskLists migration (0.5h)
- [ ] Run `MigrateTasksToTaskLists.sql` (0.2h)
- [ ] Verify Task FK migration (0.5h)
- [ ] Run all validation queries (0.3h)

#### Task 2.3: Update Application Layer (3h)

- [ ] Search codebase for `Task.ProjectId` references (0.5h)
- [ ] Update queries to use `TaskListId` (1.5h)
- [ ] Update CQRS commands/queries (0.5h)
- [ ] Update DTOs and API endpoints (0.5h)

#### Task 2.4: Update RLS Policies (2h)

- [ ] Review existing RLS policies (0.5h)
- [ ] Add SpaceId-based policies (0.5h)
- [ ] Test policies with multiple workspaces (0.5h)
- [ ] Document RLS policy changes (0.5h)

---

## 8. Next Steps (Action Items)

### Immediate Actions (Today)

1. **Create scripts directory:**

   ```bash
   mkdir -p /apps/backend/scripts
   ```

2. **Gather migration metrics:**

   ```sql
   -- Run on production database
   SELECT COUNT(*) as projects_count FROM "Projects";
   SELECT COUNT(*) as tasks_count FROM "Tasks";
   SELECT COUNT(*) as workspaces_count FROM "Workspaces";
   ```

3. **Verify staging environment:**
   - Check if staging database exists
   - Verify can connect to staging
   - Confirm latest backup available

### This Week

4. **Write migration scripts** (Task 2.1)
5. **Test in staging** (Task 2.2)
6. **Update application layer** (Task 2.3)
7. **Schedule migration window** with team

### Before Production Migration

8. **Create maintenance page** for downtime
9. **Notify users** 24h in advance
10. **Prepare rollback plan** with team
11. **Final validation** in staging

---

## 9. Success Criteria

### Phase 2 Complete When:

- [ ] All 4 migration scripts written and tested
- [ ] Migration validated in staging environment
- [ ] Zero orphaned tasks after migration
- [ ] All RLS policies updated and tested
- [ ] Application layer updated to use TaskListId
- [ ] Frontend backward compatibility maintained
- [ ] Migration documentation complete
- [ ] Rollback procedures tested
- [ ] Team briefed on migration schedule
- [ ] Maintenance window scheduled

### Validation Gates:

**Gate 1: Pre-Migration** ✅

- [ ] Staging environment ready
- [ ] Backup procedures tested
- [ ] Scripts reviewed by senior dev

**Gate 2: Staging Migration** ✅

- [ ] All validation queries pass
- [ ] Application works with new schema
- [ ] Rollback tested successfully
- [ ] Performance tests pass

**Gate 3: Production Readiness** ✅

- [ ] Maintenance window approved
- [ ] Users notified
- [ ] On-call engineer scheduled
- [ ] Monitoring dashboards ready

---

## 10. Unresolved Questions

1. **When is the migration scheduled?**
   - Need team coordination for maintenance window

2. **Is staging environment available?**
   - Must verify before testing scripts

3. **What is the current Projects/Tasks count?**
   - Affects migration time estimation

4. **Should we create Folders for migrated Projects?**
   - RESOLVED: No, use flat structure initially (YAGNI)

5. **When can we deprecate /api/projects endpoints?**
   - Answer: After 6-month transition period

6. **Who is the on-call engineer during migration?**
   - Must assign before production deployment

7. **What monitoring alerts should be configured?**
   - Need ops team input

8. **Should we back up Tasks table before migration?**
   - YES - already in script (Step 1 of MigrateTasksToTaskLists.sql)

---

## Appendix A: File Status Summary

### Entities (Domain Layer)

| Entity          | File                                                  | Status               | Notes                             |
| --------------- | ----------------------------------------------------- | -------------------- | --------------------------------- |
| Space           | `/src/Nexora.Management.Domain/Entities/Space.cs`     | ✅ EXISTS            | 65 lines, documented              |
| Folder          | `/src/Nexora.Management.Domain/Entities/Folder.cs`    | ✅ EXISTS            | 61 lines, documented              |
| TaskList (List) | `/src/Nexora.Management.Domain/Entities/TaskList.cs`  | ✅ EXISTS            | 102 lines, documented             |
| Task            | `/src/Nexora.Management.Domain/Entities/Task.cs`      | ⚠️ NEEDS UPDATE      | Has both ProjectId and TaskListId |
| Workspace       | `/src/Nexora.Management.Domain/Entities/Workspace.cs` | ✅ EXISTS            | Has Spaces collection             |
| Project         | `/src/Nexora.Management.Domain/Entities/Project.cs`   | ⚠️ NEEDS DEPRECATION | Add [Obsolete] attribute          |

### Configurations (Infrastructure Layer)

| Configuration          | File                                                                                         | Status    | Notes                          |
| ---------------------- | -------------------------------------------------------------------------------------------- | --------- | ------------------------------ |
| SpaceConfiguration     | `/src/Nexora.Management.Infrastructure/Persistence/Configurations/SpaceConfiguration.cs`     | ✅ EXISTS | -                              |
| FolderConfiguration    | `/src/Nexora.Management.Infrastructure/Persistence/Configurations/FolderConfiguration.cs`    | ✅ EXISTS | Has unique PositionOrder index |
| TaskListConfiguration  | `/src/Nexora.Management.Infrastructure/Persistence/Configurations/TaskListConfiguration.cs`  | ✅ EXISTS | Has ListType, Status defaults  |
| TaskConfiguration      | `/src/Nexora.Management.Infrastructure/Persistence/Configurations/TaskConfiguration.cs`      | ✅ EXISTS | Has Project and TaskList FKs   |
| WorkspaceConfiguration | `/src/Nexora.Management.Infrastructure/Persistence/Configurations/WorkspaceConfiguration.cs` | ✅ EXISTS | Has Spaces collection          |

### Migrations (API Layer)

| Migration                 | File                                                                                            | Status    | Notes                              |
| ------------------------- | ----------------------------------------------------------------------------------------------- | --------- | ---------------------------------- |
| AddClickUpHierarchyTables | `/src/Nexora.Management.API/Persistence/Migrations/20260106184122_AddClickUpHierarchyTables.cs` | ✅ EXISTS | Creates Spaces, Folders, TaskLists |

### Scripts (Missing)

| Script                     | File                                                   | Status     | Priority  |
| -------------------------- | ------------------------------------------------------ | ---------- | --------- |
| MigrateProjectsToTaskLists | `/apps/backend/scripts/MigrateProjectsToTaskLists.sql` | ❌ MISSING | HIGH 🔴   |
| MigrateTasksToTaskLists    | `/apps/backend/scripts/MigrateTasksToTaskLists.sql`    | ❌ MISSING | HIGH 🔴   |
| ValidateMigration          | `/apps/backend/scripts/ValidateMigration.sql`          | ❌ MISSING | HIGH 🔴   |
| RollbackMigration          | `/apps/backend/scripts/RollbackMigration.sql`          | ❌ MISSING | MEDIUM 🟡 |

---

## Appendix B: Critical Code Snippets

### B.1 Task Entity Current State

**File:** `/apps/backend/src/Nexora.Management.Domain/Entities/Task.cs`

```csharp
public class Task : BaseEntity
{
    // TODO: Migrate to TaskListId, keep ProjectId for backward compatibility during migration
    public Guid ProjectId { get; set; } // DEPRECATED: Use TaskListId after migration
    public Guid TaskListId { get; set; } // NEW: References TaskList in ClickUp hierarchy

    // Navigation properties
    public Project Project { get; set; } = null!; // DEPRECATED: Remove after migration
    public TaskList TaskList { get; set; } = null!; // NEW: TaskList navigation
}
```

**Required Update:**

```csharp
[Obsolete("Use TaskListId instead. This property will be removed after migration.")]
public Guid ProjectId { get; set; }
```

### B.2 Migration Script Structure

**File:** `/apps/backend/scripts/MigrateProjectsToTaskLists.sql` (TO BE CREATED)

```sql
-- ⚠️ CRITICAL: Run this migration in a TRANSACTION with ROLLBACK available
-- BEGIN;

-- Step 1: Create backup schema
CREATE SCHEMA IF NOT EXISTS _backup_projects;

-- Step 2: Backup Projects table
CREATE TABLE _backup_projects."Projects" AS SELECT * FROM "Projects";

-- Step 3: Create default Space for each Workspace
INSERT INTO "Spaces" ("Id", "WorkspaceId", "Name", "Description", "IsPrivate", "CreatedAt", "UpdatedAt")
SELECT uuid_generate_v4(), "Id", 'General', 'Default space migrated from Projects', false, "CreatedAt", "UpdatedAt"
FROM "Workspaces"
ON CONFLICT DO NOTHING;

-- Step 4: Migrate Projects → TaskLists (copy all data)
INSERT INTO "TaskLists" ("Id", "SpaceId", "FolderId", "Name", "Description", "Color", "Icon", "ListType", "Status", "OwnerId", "PositionOrder", "SettingsJsonb", "CreatedAt", "UpdatedAt")
SELECT "Id", (SELECT "Id" FROM "Spaces" WHERE "Spaces"."WorkspaceId" = "Projects"."WorkspaceId" LIMIT 1), NULL, "Name", "Description", "Color", "Icon", 'task', "Status", "OwnerId", 0, "SettingsJsonb"::jsonb, "CreatedAt", "UpdatedAt"
FROM "Projects"
ON CONFLICT ("Id") DO NOTHING;

-- Step 5: Verify migration (should return 0)
SELECT COUNT(*) as total_projects,
       (SELECT COUNT(*) FROM "TaskLists") as migrated_tasklists,
       (SELECT COUNT(*) FROM "Projects") as remaining_projects
FROM "Projects";

-- COMMIT; -- Uncomment after verification
```

---

**End of Report**

**Generated by:** Project Manager Agent (adeb80f)
**Report ID:** project-manager-260107-1555-phase02-database-migration-analysis
**Next Review:** After Task 2.1 completion (migration scripts written)
