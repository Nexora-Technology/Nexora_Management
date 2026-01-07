-- ⚠️ CRITICAL: Data Migration Script - Projects → TaskLists
-- ⚠️ Run this in a TRANSACTION with ROLLBACK available
-- ⚠️ Test in STAGING first before production
--
-- Purpose: Migrate Projects table to TaskLists table as part of ClickUp hierarchy restructure
-- Hierarchy: Workspace → Space → Folder (optional) → TaskList → Task
--
-- Prerequisites:
--   1. Spaces, Folders, TaskLists tables must exist (run EF Core migration first)
--   2. Database backup completed
--   3. Maintenance window announced to users
--
-- Expected Outcome:
--   - One "General" Space created per Workspace
--   - All Projects copied to TaskLists (preserving IDs)
--   - Original Projects table kept for rollback
--
-- Estimated Time: 5-15 minutes (depending on data volume)
-- Risk Level: 🔴 HIGH (data migration operation)
--
-- ================================================================

BEGIN;

-- Step 0.5: Lock tables to prevent concurrent writes during migration
-- This prevents race conditions and data corruption
LOCK TABLE "Projects", "Workspaces", "Spaces" IN ACCESS EXCLUSIVE MODE;

-- Step 1: Create backup schema for emergency rollback
CREATE SCHEMA IF NOT EXISTS _backup_projects;

-- Step 2: Backup Projects table (full copy)
CREATE TABLE IF NOT EXISTS _backup_projects."Projects" AS
SELECT * FROM "Projects";

RAISE NOTICE 'Backup created: _backup_projects."Projects" (%) rows', (SELECT COUNT(*) FROM _backup_projects."Projects");

-- Step 3: Create default "General" Space for each Workspace
-- This ensures TaskLists have a parent Space
INSERT INTO "Spaces" (
    "Id",
    "WorkspaceId",
    "Name",
    "Description",
    "IsPrivate",
    "SettingsJsonb",
    "CreatedAt",
    "UpdatedAt"
)
SELECT
    uuid_generate_v4() as "Id",
    "Id" as "WorkspaceId",
    'General' as "Name",
    'Default space for migrated projects' as "Description",
    false as "IsPrivate",
    '{}'::jsonb as "SettingsJsonb",
    "CreatedAt" as "CreatedAt",
    "UpdatedAt" as "UpdatedAt"
FROM "Workspaces"
ON CONFLICT DO NOTHING; -- Skip if Space already exists

RAISE NOTICE 'Default Spaces created per Workspace';

-- Step 4: Migrate Projects → TaskLists (copy all data, preserve IDs)
INSERT INTO "TaskLists" (
    "Id",              -- Preserve ID for Task references
    "SpaceId",         -- Map to default Space
    "FolderId",        -- NULL initially (no Folders)
    "Name",
    "Description",
    "Color",
    "Icon",
    "ListType",        -- Default to "task"
    "Status",
    "OwnerId",
    "PositionOrder",   -- Default to 0
    "SettingsJsonb",
    "CreatedAt",
    "UpdatedAt"
)
SELECT
    p."Id",                                                    -- Preserve same ID
    (SELECT s."Id"
     FROM "Spaces" s
     WHERE s."WorkspaceId" = p."WorkspaceId"
     LIMIT 1) as "SpaceId",                                   -- Get default Space for Workspace
    NULL as "FolderId",                                       -- No Folders initially
    p."Name",
    p."Description",
    p."Color",
    p."Icon",
    'task' as "ListType",                                     -- Default list type
    p."Status",
    p."OwnerId",
    0 as "PositionOrder",                                     -- Default position
    p."SettingsJsonb",
    p."CreatedAt",
    p."UpdatedAt"
FROM "Projects" p
ON CONFLICT ("Id") DO NOTHING;                               -- Skip if already migrated

RAISE NOTICE 'Projects → TaskLists migrated: % rows', (SELECT COUNT(*) FROM "TaskLists");

-- Step 5: Verification queries (should all return TRUE)

-- Verify 1: TaskLists count should match Projects count
DO $$
DECLARE
    v_projects_count INT;
    v_tasklists_count INT;
BEGIN
    SELECT COUNT(*) INTO v_projects_count FROM "Projects";
    SELECT COUNT(*) INTO v_tasklists_count FROM "TaskLists";

    IF v_tasklists_count <> v_projects_count THEN
        RAISE EXCEPTION '❌ MIGRATION FAILED: TaskLists (%) != Projects (%)',
            v_tasklists_count, v_projects_count;
    ELSE
        RAISE NOTICE '✓ Verification 1 PASSED: TaskLists count = Projects count (%)',
            v_tasklists_count;
    END IF;
END $$;

-- Verify 2: All TaskLists should have valid SpaceId
DO $$
DECLARE
    v_orphaned_count INT;
BEGIN
    SELECT COUNT(*) INTO v_orphaned_count
    FROM "TaskLists" tl
    LEFT JOIN "Spaces" s ON tl."SpaceId" = s."Id"
    WHERE s."Id" IS NULL;

    IF v_orphaned_count > 0 THEN
        RAISE EXCEPTION '❌ MIGRATION FAILED: % TaskLists have invalid SpaceId', v_orphaned_count;
    ELSE
        RAISE NOTICE '✓ Verification 2 PASSED: All TaskLists have valid SpaceId';
    END IF;
END $$;

-- Verify 3: Preserved IDs should match
DO $$
DECLARE
    v_mismatched_ids INT;
BEGIN
    SELECT COUNT(*) INTO v_mismatched_ids
    FROM "Projects" p
    LEFT JOIN "TaskLists" tl ON p."Id" = tl."Id"
    WHERE tl."Id" IS NULL;

    IF v_mismatched_ids > 0 THEN
        RAISE EXCEPTION '❌ MIGRATION FAILED: % Project IDs not found in TaskLists',
            v_mismatched_ids;
    ELSE
        RAISE NOTICE '✓ Verification 3 PASSED: All Project IDs preserved in TaskLists';
    END IF;
END $$;

-- Summary Report
SELECT
    'Projects → TaskLists Migration' as operation,
    (SELECT COUNT(*) FROM "Projects") as original_projects,
    (SELECT COUNT(*) FROM "TaskLists") as migrated_tasklists,
    (SELECT COUNT(*) FROM "Spaces") as spaces_created,
    (SELECT COUNT(*) FROM _backup_projects."Projects") as backup_rows;

-- ================================================================
-- ⚠️ IMPORTANT: Do NOT COMMIT until:
--   1. All verifications pass (✓ marks above)
--   2. You review the summary report
--   3. You're ready to proceed to Step 2 (MigrateTasksToTaskLists.sql)
--
-- To rollback: ROLLBACK;
-- To commit: UNCOMMENT the line below and run:
-- COMMIT;
-- ================================================================
