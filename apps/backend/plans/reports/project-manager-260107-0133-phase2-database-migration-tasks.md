# Phase 2: Database Migration - Detailed Task Breakdown

**Plan:** ClickUp Hierarchy Implementation
**Phase:** 2 - Database Migration (14h estimated)
**Approach:** Create New List Entity (Approach B)
**Date:** 2026-01-07
**Risk Level:** HIGH - Data migration with rollback requirements

---

## Executive Summary

Phase 2 involves creating new database tables (Spaces, Folders, Lists) and migrating existing Project/Task data to the new hierarchy structure. This is the **highest-risk phase** of the implementation plan.

**Critical Path:**
1. EF Core configurations (3h)
2. Migration file creation (6h)
3. Data migration scripts (4h)
4. Validation and rollback procedures (1h)

**Key Risks:**
- Data loss during Projects → Lists migration
- Task.ProjectId → Task.ListId foreign key corruption
- Application downtime during migration
- Rollback complexity

---

## Task Category 1: EF Core Configurations (3h)

### Task 2.1.1: Create SpaceConfiguration.cs
- **File:** `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/Configurations/SpaceConfiguration.cs`
- **Effort:** 45m
- **Dependencies:** None (can start immediately after Phase 1)
- **Description:** Implement IEntityTypeConfiguration<Space> with table mapping, column constraints, and relationships
- **Validation:**
  - ToTable("Spaces")
  - Primary key with uuid_generate_v4() default
  - Name required, max 100 chars
  - Workspace FK with cascade delete
  - Index on WorkspaceId

### Task 2.1.2: Create FolderConfiguration.cs
- **File:** `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/Configurations/FolderConfiguration.cs`
- **Effort:** 45m
- **Dependencies:** None
- **Description:** Implement IEntityTypeConfiguration<Folder> with PositionOrder support
- **Validation:**
  - ToTable("Folders")
  - Space FK with cascade delete
  - Unique index on (SpaceId, PositionOrder) for drag-drop ordering
  - NO parent FolderId (single-level only)

### Task 2.1.3: Create ListConfiguration.cs 🆕
- **File:** `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/Configurations/ListConfiguration.cs`
- **Effort:** 45m
- **Dependencies:** None
- **Description:** Implement IEntityTypeConfiguration<List> with ListType property and optional FolderId
- **Validation:**
  - ToTable("Lists")
  - SpaceId required FK
  - FolderId optional FK (nullable)
  - ListType default "task"
  - Status default "active"
  - OwnerId FK with restrict delete
  - Index on (SpaceId, FolderId, PositionOrder)
  - Index on OwnerId

### Task 2.1.4: Modify TaskConfiguration.cs
- **File:** `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/Configurations/TaskConfiguration.cs`
- **Effort:** 30m
- **Dependencies:** Task 2.1.3 (ListConfiguration)
- **Description:** Update Task configuration to reference List instead of Project
- **Changes:**
  - Remove: `HasOne(t => t.Project).WithMany(p => p.Tasks)...`
  - Add: `HasOne(t => t.List).WithMany(l => l.Tasks).HasForeignKey(t => t.ListId).OnDelete(DeleteBehavior.Cascade)`

### Task 2.1.5: Modify WorkspaceConfiguration.cs
- **File:** `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/Configurations/WorkspaceConfiguration.cs`
- **Effort:** 15m
- **Dependencies:** Task 2.1.1 (SpaceConfiguration)
- **Description:** Add Spaces collection navigation property
- **Changes:**
  - Add: `HasMany(w => w.Spaces).WithOne(s => s.Workspace).HasForeignKey(s => s.WorkspaceId).OnDelete(DeleteBehavior.Cascade)`
  - Comment out Projects collection (deprecated)

---

## Task Category 2: DbContext Updates (1h)

### Task 2.2.1: Update AppDbContext.cs
- **File:** `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/AppDbContext.cs`
- **Effort:** 1h
- **Dependencies:** Tasks 2.1.1 - 2.1.5
- **Description:** Add new DbSets and register configurations
- **Changes:**
  - Add: `DbSet<Space> Spaces => Set<Space>();`
  - Add: `DbSet<Folder> Folders => Set<Folder>();`
  - Add: `DbSet<List> Lists => Set<List>();`
  - Keep: `DbSet<Project> Projects => Set<Project>();` (with deprecation comment)
  - Register configurations in OnModelCreating:
    - `new SpaceConfiguration().Configure(modelBuilder.Entity<Space>());`
    - `new FolderConfiguration().Configure(modelBuilder.Entity<Folder>());`
    - `new ListConfiguration().Configure(modelBuilder.Entity<List>());`

---

## Task Category 3: EF Core Migration File (6h)

### Task 2.3.1: Generate Initial Migration
- **Command:** `dotnet ef migrations add AddClickUpHierarchyTables --project Nexora.Management.API`
- **Effort:** 1h
- **Dependencies:** Task 2.2.1
- **Description:** Generate base migration file with Up/Down methods
- **Output:** `/apps/backend/src/Nexora.Management.API/Persistence/Migrations/20260107XXXXXX_AddClickUpHierarchyTables.cs`

### Task 2.3.2: Implement Up() Method - Spaces Table
- **File:** Migration file from 2.3.1
- **Effort:** 1h
- **Dependencies:** Task 2.3.1
- **Description:** CreateTable for Spaces with all columns and constraints
- **Columns:**
  - Id (uuid, PK)
  - WorkspaceId (uuid, FK to Workspaces, cascade)
  - Name (varchar 100, required)
  - Description (varchar 500, nullable)
  - Color (varchar 7, nullable)
  - Icon (varchar 50, nullable)
  - IsPrivate (boolean, default false)
  - SettingsJsonb (jsonb)
  - CreatedAt, UpdatedAt (timestamp)

### Task 2.3.3: Implement Up() Method - Folders Table
- **File:** Migration file from 2.3.1
- **Effort:** 1h
- **Dependencies:** Task 2.3.2
- **Description:** CreateTable for Folders with PositionOrder
- **Columns:**
  - Id (uuid, PK)
  - SpaceId (uuid, FK to Spaces, cascade)
  - Name, Description, Color, Icon
  - PositionOrder (integer, default 0)
  - SettingsJsonb (jsonb)
  - CreatedAt, UpdatedAt
- **Indexes:**
  - Unique index on (SpaceId, PositionOrder)

### Task 2.3.4: Implement Up() Method - Lists Table 🆕
- **File:** Migration file from 2.3.1
- **Effort:** 1.5h
- **Dependencies:** Task 2.3.3
- **Description:** CreateTable for Lists with ListType and optional FolderId
- **Columns:**
  - Id (uuid, PK)
  - SpaceId (uuid, FK to Spaces, cascade)
  - FolderId (uuid, FK to Folders, cascade, nullable)
  - Name, Description, Color, Icon
  - ListType (varchar 50, default "task")
  - Status (varchar 50, default "active")
  - OwnerId (uuid, FK to Users, restrict)
  - PositionOrder (integer, default 0)
  - SettingsJsonb (jsonb)
  - CreatedAt, UpdatedAt
- **Indexes:**
  - IX_Lists_SpaceId
  - IX_Lists_FolderId
  - IX_Lists_SpaceId_FolderId_PositionOrder
  - IX_Lists_OwnerId

### Task 2.3.5: Implement Down() Method
- **File:** Migration file from 2.3.1
- **Effort:** 1h
- **Dependencies:** Tasks 2.3.2 - 2.3.4
- **Description:** Implement rollback logic (drop FKs, drop tables in reverse order)
- **Order:**
  1. Drop FK_Lists_Users_OwnerId
  2. DropTable("Lists")
  3. DropTable("Folders")
  4. DropTable("Spaces")

### Task 2.3.6: Test Migration Compilation
- **Command:** `dotnet ef migrations script --project Nexora.Management.API`
- **Effort:** 0.5h
- **Dependencies:** Task 2.3.5
- **Description:** Verify migration SQL compiles without errors
- **Validation:**
  - No SQL syntax errors
  - All FKs valid
  - All indexes valid
  - Down() method reverses Up() completely

---

## Task Category 4: Data Migration Scripts (4h)

### Task 2.4.1: Create MigrateProjectsToLists.sql
- **File:** `/apps/backend/scripts/MigrateProjectsToLists.sql`
- **Effort:** 2h
- **Dependencies:** Task 2.3.6 (migration compiles)
- **Description:** Script to copy Projects → Lists and create default Spaces
- **Steps:**
  1. BEGIN TRANSACTION
  2. CREATE SCHEMA _backup_projects
  3. CREATE TABLE _backup_projects."Projects" AS SELECT * FROM "Projects"
  4. INSERT INTO "Spaces" (default "General" space per Workspace)
  5. INSERT INTO "Lists" (copy all Projects with same IDs)
  6. Verify counts match
  7. COMMIT (after verification)

**Key Logic:**
```sql
INSERT INTO "Lists" (
    "Id", -- Keep same ID for Task references
    "SpaceId",
    "FolderId", -- NULL initially
    "Name", "Description", "Color", "Icon",
    "ListType", -- 'task'
    "Status",
    "OwnerId",
    "PositionOrder", -- 0
    "SettingsJsonb",
    "CreatedAt", "UpdatedAt"
)
SELECT
    "Id",
    (SELECT "Id" FROM "Spaces" WHERE "Spaces"."WorkspaceId" = "Projects"."WorkspaceId" LIMIT 1),
    NULL,
    "Name", "Description", "Color", "Icon",
    'task',
    "Status",
    "OwnerId",
    0,
    "SettingsJsonb"::jsonb,
    "CreatedAt", "UpdatedAt"
FROM "Projects"
ON CONFLICT ("Id") DO NOTHING;
```

### Task 2.4.2: Create MigrateTasksToLists.sql
- **File:** `/apps/backend/scripts/MigrateTasksToLists.sql`
- **Effort:** 2h
- **Dependencies:** Task 2.4.1 (Projects → Lists verified)
- **Description:** Script to rename Task.ProjectId → Task.ListId
- **Steps:**
  1. BEGIN TRANSACTION
  2. CREATE TABLE _backup_projects."Tasks" AS SELECT * FROM "Tasks"
  3. ALTER TABLE "Tasks" ADD COLUMN "ListId_New" uuid NULL
  4. UPDATE "Tasks" SET "ListId_New" = "ProjectId"
  5. Verify orphaned tasks count = 0
  6. DROP CONSTRAINT FK_Tasks_Projects_ProjectId
  7. DROP COLUMN "ProjectId"
  8. RENAME COLUMN "ListId_New" TO "ListId"
  9. ALTER COLUMN "ListId" SET NOT NULL
  10. ADD CONSTRAINT FK_Tasks_Lists_ListId FOREIGN KEY ("ListId") REFERENCES "Lists"("Id") ON DELETE CASCADE
  11. CREATE INDEX IX_Tasks_ListId
  12. Verify tasks count matches
  13. COMMIT

**Orphaned Check:**
```sql
SELECT COUNT(*) as orphaned_tasks
FROM "Tasks" t
LEFT JOIN "Lists" l ON t."ListId_New" = l."Id"
WHERE l."Id" IS NULL;
-- Must return 0 before proceeding
```

---

## Task Category 5: Validation Queries (1h)

### Task 2.5.1: Create ValidateMigration.sql
- **File:** `/apps/backend/scripts/ValidateMigration.sql`
- **Effort:** 1h
- **Dependencies:** Tasks 2.4.1 and 2.4.2
- **Description:** Comprehensive validation queries to run after migration
- **Query Sets:**

1. **Space Creation Validation:**
   - Count Spaces per Workspace (should be ≥1)
   - Verify default "General" space exists

2. **List Migration Validation:**
   - Compare original Projects count vs migrated Lists count
   - Difference should be 0

3. **Task Migration Validation:**
   - Compare original Tasks count vs current Tasks count
   - Check for orphaned tasks (ListId IS NULL)
   - Should return 0

4. **Relationship Integrity:**
   - Lists with invalid SpaceId (should be 0)
   - Tasks with invalid ListId (should be 0)

5. **Performance Check:**
   - EXPLAIN ANALYZE query joining Tasks → Lists → Spaces
   - Verify index usage

6. **Data Completeness:**
   - Spot-check random Projects → Lists mapping
   - Verify SettingsJsonb preserved
   - Verify CreatedAt/UpdatedAt preserved

---

## Task Category 6: Rollback Procedures

### Task 2.6.1: Document Rollback Procedure
- **File:** `/apps/backend/docs/ROLLBACK_PROCEDURE.md`
- **Effort:** 0.5h
- **Dependencies:** All migration scripts created
- **Description:** Step-by-step rollback instructions
- **Scenarios:**

**Scenario A: Projects → Lists Migration Failed**
```sql
ROLLBACK;
DROP TABLE "Lists" CASCADE;
DROP TABLE "Folders" CASCADE;
DROP TABLE "Spaces" CASCADE;
DROP SCHEMA _backup_projects CASCADE;
```

**Scenario B: Tasks Migration Failed (Lists OK)**
```sql
ROLLBACK;
-- Restore Tasks from backup
DROP TABLE "Tasks" CASCADE;
CREATE TABLE "Tasks" AS SELECT * FROM _backup_projects."Tasks";
-- Recreate FK to Projects (temporary)
ALTER TABLE "Tasks" ADD CONSTRAINT "FK_Tasks_Projects_ProjectId"
FOREIGN KEY ("ProjectId") REFERENCES "Projects"("Id") ON DELETE CASCADE;
```

**Scenario C: Need to Restore After 30 Days**
```sql
-- Drop new tables
DROP TABLE "Lists" CASCADE;
DROP TABLE "Folders" CASCADE;
DROP TABLE "Spaces" CASCADE;
-- Restore Projects (kept in main schema)
-- No action needed, Projects table still exists
```

### Task 2.6.2: Create Rollback Scripts
- **Files:**
  - `/apps/backend/scripts/RollbackToLists.sql` (restore Tasks to Lists)
  - `/apps/backend/scripts/RollbackToProjects.sql` (full revert to Projects)
- **Effort:** 0.5h
- **Dependencies:** Task 2.6.1
- **Description:** Automated rollback scripts for emergency use

---

## Execution Checklist

### Pre-Migration (Pre-flight)
- [ ] Full database backup taken (pg_dump)
- [ ] Staging environment tested
- [ ] Migration scripts reviewed by 2 developers
- [ ] Rollback procedure documented and tested
- [ ] Team notified of maintenance window
- [ ] Current Projects/Tasks counts recorded

### During Migration
- [ ] Run in transaction (BEGIN)
- [ ] Create backup schema
- [ ] Backup Projects table
- [ ] Create default Spaces
- [ ] Migrate Projects → Lists
- [ ] Verify Lists count = Projects count
- [ ] Backup Tasks table
- [ ] Add ListId_New column
- [ ] Copy ProjectId → ListId_New
- [ ] Verify orphaned tasks = 0
- [ ] Drop ProjectId FK
- [ ] Drop ProjectId column
- [ ] Rename ListId_New → ListId
- [ ] Add ListId FK constraint
- [ ] Create IX_Tasks_ListId index
- [ ] Verify Tasks count matches
- [ ] COMMIT transaction

### Post-Migration (Validation)
- [ ] Run ValidateMigration.sql
- [ ] All validation queries pass
- [ ] Spot-check 10 random tasks
- [ ] Verify task detail pages load
- [ ] Verify task creation works
- [ ] Performance benchmarks pass
- [ ] Application starts without errors

---

## Critical Success Metrics

| Metric | Target | Actual |
|--------|--------|--------|
| Migration downtime | <2 hours | ___ |
- | Data loss | 0 records | ___ |
- | Orphaned tasks | 0 tasks | ___ |
- | Lists count | = Projects count | ___ |
- | Tasks count | = original count | ___ |
- | Query performance (p95) | <200ms | ___ |
- | Rollback time | <30 min | ___ |

---

## Risk Mitigation

1. **Data Loss Prevention:**
   - Backup schema created before any changes
   - All changes in single transaction
   - Keep Projects table for 30 days
   - Verify counts at each step

2. **Minimize Downtime:**
   - Use temporary column approach (ListId_New)
   - Can be done live without full table lock
   - Schedule during low-traffic hours

3. **Rollback Readiness:**
   - Test rollback in staging
   - Scripts automated and tested
   - Team trained on rollback procedure

4. **Performance Safety:**
   - Indexes created before migration
   - EXPLAIN ANALYZE validation
   - Load test before and after

---

## Dependencies

**Requires:**
- Phase 1 complete (entities created)
- Staging database available
- Backup infrastructure ready

**Blocks:**
- Phase 3 (API Endpoints) - cannot expose Lists until DB ready
- Phase 4 (CQRS) - needs tables for queries
- Phase 5+ (Frontend) - needs API endpoints

**Parallel Work:**
- Phase 4 (CQRS) can start once migration file compiles (Task 2.3.6)

---

## Unresolved Questions

1. Should we create default Spaces per Workspace before Lists migration, or as part of migration script?
   - Recommendation: Include in MigrateProjectsToLists.sql (simpler transaction)

2. Should we keep Projects table indefinitely or set hard 30-day deadline?
   - Recommendation: 30-day deadline with monitoring, extend only if issues found

3. Should we add database triggers to keep Projects/Lists in sync during transition?
   - Recommendation: No, adds complexity. Use API-level alias instead

4. What is the acceptable query performance degradation for the new hierarchy (3 joins vs 2)?
   - Recommendation: <20% slowdown, monitor with EXPLAIN ANALYZE

5. Should we use pg_dump or pg_backup for backups?
   - Recommendation: pg_dump (text format) for portability

---

## Next Steps After Phase 2

1. Run migration in staging environment
2. Execute all validation queries
3. Load test with production-like data
4. Document any deviations from plan
5. Get sign-off from tech lead
6. Schedule production migration window
7. Start Phase 3 (API Endpoints)

---

**Report Generated:** 2026-01-07
**Maintained By:** Project Manager
**Status:** Ready for Implementation
**Priority:** CRITICAL - Blocks all downstream phases
