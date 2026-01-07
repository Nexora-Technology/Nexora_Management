-- ⚠️ CRITICAL: Data Migration Script - Tasks.ProjectId → Tasks.TaskListId
-- ⚠️ Run ONLY after MigrateProjectsToTaskLists.sql completes successfully
-- ⚠️ Run this in a TRANSACTION with ROLLBACK available
-- ⚠️ Test in STAGING first before production
--
-- Purpose: Migrate Tasks from referencing Projects to referencing TaskLists
-- Hierarchy Change: Task.ProjectId → Task.TaskListId
--
-- Prerequisites:
--   1. MigrateProjectsToTaskLists.sql completed successfully
--   2. All TaskLists created and verified
--   3. Task.ProjectId still exists in Tasks table
--
-- Expected Outcome:
--   - Tasks.TaskListId replaces Tasks.ProjectId
--   - All Tasks preserve their List references
--   - Foreign key updated to reference TaskLists table
--   - Original Tasks table backed up
--
-- Estimated Time: 10-30 minutes (depending on data volume)
-- Risk Level: 🔴 HIGH (modifies Tasks table structure)
--
-- ================================================================

BEGIN;

-- Step 0.5: Lock tables to prevent concurrent writes during migration
-- This prevents race conditions and data corruption
LOCK TABLE "Tasks", "TaskLists", "Projects" IN ACCESS EXCLUSIVE MODE;

-- Step 1: Verify prerequisite - TaskLists must exist
DO $$
DECLARE
    v_tasklists_count INT;
BEGIN
    SELECT COUNT(*) INTO v_tasklists_count FROM "TaskLists";

    IF v_tasklists_count = 0 THEN
        RAISE EXCEPTION '❌ PREREQUISITE FAILED: No TaskLists found. Run MigrateProjectsToTaskLists.sql first!';
    ELSE
        RAISE NOTICE '✓ Prerequisite check passed: % TaskLists found', v_tasklists_count;
    END IF;
END $$;

-- Step 2: Backup Tasks table
CREATE TABLE IF NOT EXISTS _backup_projects."Tasks" AS
SELECT * FROM "Tasks";

RAISE NOTICE 'Backup created: _backup_projects."Tasks" (%) rows',
    (SELECT COUNT(*) FROM _backup_projects."Tasks");

-- Step 3: Verify all Tasks have valid ProjectId (should be 0)
DO $$
DECLARE
    v_invalid_count INT;
BEGIN
    SELECT COUNT(*) INTO v_invalid_count
    FROM "Tasks" t
    LEFT JOIN "Projects" p ON t."ProjectId" = p."Id"
    WHERE t."ProjectId" IS NOT NULL AND p."Id" IS NULL;

    IF v_invalid_count > 0 THEN
        RAISE WARNING '⚠️ Found % Tasks with invalid ProjectId (will be orphaned)', v_invalid_count;
    ELSE
        RAISE NOTICE '✓ All Tasks have valid ProjectId references';
    END IF;
END $$;

-- Step 4: Add temporary column for new foreign key
ALTER TABLE "Tasks"
ADD COLUMN IF NOT EXISTS "TaskListId_New" UUID NULL;

RAISE NOTICE 'Added temporary column: TaskListId_New';

-- Step 5: Copy ProjectId → TaskListId_New (direct mapping since IDs preserved)
UPDATE "Tasks"
SET "TaskListId_New" = "ProjectId"
WHERE "ProjectId" IS NOT NULL;

RAISE NOTICE 'Copied ProjectId → TaskListId_New: % rows updated',
    (SELECT COUNT(*) FROM "Tasks" WHERE "TaskListId_New" IS NOT NULL);

-- Step 6: Verify all Tasks have valid TaskListId_New (should be 0 orphaned)
DO $$
DECLARE
    v_orphaned_count INT;
BEGIN
    SELECT COUNT(*) INTO v_orphaned_count
    FROM "Tasks" t
    LEFT JOIN "TaskLists" tl ON t."TaskListId_New" = tl."Id"
    WHERE t."TaskListId_New" IS NOT NULL AND tl."Id" IS NULL;

    IF v_orphaned_count > 0 THEN
        RAISE EXCEPTION '❌ MIGRATION FAILED: % Tasks would be orphaned after migration',
            v_orphaned_count;
    ELSE
        RAISE NOTICE '✓ Verification PASSED: All Tasks have valid TaskListId references';
    END IF;
END $$;

-- Step 7: Drop old foreign key constraint (if exists)
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.table_constraints
        WHERE table_name = 'Tasks'
        AND constraint_name = 'FK_Tasks_Projects_ProjectId'
    ) THEN
        ALTER TABLE "Tasks"
        DROP CONSTRAINT "FK_Tasks_Projects_ProjectId";
        RAISE NOTICE 'Dropped old FK: FK_Tasks_Projects_ProjectId';
    ELSE
        RAISE NOTICE 'Old FK constraint not found (may have different name)';
    END IF;
END $$;

-- Step 8: Drop old ProjectId column
ALTER TABLE "Tasks"
DROP COLUMN IF EXISTS "ProjectId";

RAISE NOTICE 'Dropped old column: ProjectId';

-- Step 9: Rename TaskListId_New → TaskListId
ALTER TABLE "Tasks"
RENAME COLUMN "TaskListId_New" TO "TaskListId";

RAISE NOTICE 'Renamed TaskListId_New → TaskListId';

-- Step 10: Add NOT NULL constraint
ALTER TABLE "Tasks"
ALTER COLUMN "TaskListId" SET NOT NULL;

RAISE NOTICE 'Added NOT NULL constraint to TaskListId';

-- Step 11: Create new foreign key constraint to TaskLists
ALTER TABLE "Tasks"
ADD CONSTRAINT "FK_Tasks_TaskLists_TaskListId"
FOREIGN KEY ("TaskListId")
REFERENCES "TaskLists"("Id")
ON DELETE CASCADE;

RAISE NOTICE 'Created new FK: FK_Tasks_TaskLists_TaskListId';

-- Step 12: Create index for performance
CREATE INDEX IF NOT EXISTS "IX_Tasks_TaskListId"
ON "Tasks"("TaskListId");

RAISE NOTICE 'Created index: IX_Tasks_TaskListId';

-- Step 13: Final verification queries

-- Verify 1: Tasks count unchanged
DO $$
DECLARE
    v_original_count INT;
    v_current_count INT;
BEGIN
    SELECT COUNT(*) INTO v_original_count FROM _backup_projects."Tasks";
    SELECT COUNT(*) INTO v_current_count FROM "Tasks";

    IF v_current_count <> v_original_count THEN
        RAISE EXCEPTION '❌ VERIFICATION FAILED: Tasks count changed! Original: %, Current: %',
            v_original_count, v_current_count;
    ELSE
        RAISE NOTICE '✓ Verification 1 PASSED: Tasks count unchanged (%)', v_current_count;
    END IF;
END $$;

-- Verify 2: No orphaned Tasks
DO $$
DECLARE
    v_orphaned_count INT;
BEGIN
    SELECT COUNT(*) INTO v_orphaned_count
    FROM "Tasks" t
    LEFT JOIN "TaskLists" tl ON t."TaskListId" = tl."Id"
    WHERE tl."Id" IS NULL;

    IF v_orphaned_count > 0 THEN
        RAISE EXCEPTION '❌ VERIFICATION FAILED: % Tasks have invalid TaskListId', v_orphaned_count;
    ELSE
        RAISE NOTICE '✓ Verification 2 PASSED: All Tasks have valid TaskListId';
    END IF;
END $$;

-- Verify 3: FK constraint exists
DO $$
DECLARE
    v_fk_exists BOOLEAN;
BEGIN
    SELECT EXISTS (
        SELECT 1 FROM information_schema.table_constraints
        WHERE table_name = 'Tasks'
        AND constraint_name = 'FK_Tasks_TaskLists_TaskListId'
        AND constraint_type = 'FOREIGN KEY'
    ) INTO v_fk_exists;

    IF NOT v_fk_exists THEN
        RAISE EXCEPTION '❌ VERIFICATION FAILED: FK constraint not found';
    ELSE
        RAISE NOTICE '✓ Verification 3 PASSED: FK constraint exists';
    END IF;
END $$;

-- Verify 4: Index exists
DO $$
DECLARE
    v_index_exists BOOLEAN;
BEGIN
    SELECT EXISTS (
        SELECT 1 FROM pg_indexes
        WHERE tablename = 'Tasks'
        AND indexname = 'IX_Tasks_TaskListId'
    ) INTO v_index_exists;

    IF NOT v_index_exists THEN
        RAISE WARNING '⚠️ Verification 4 WARNING: Index not found (performance may suffer)';
    ELSE
        RAISE NOTICE '✓ Verification 4 PASSED: Index created for performance';
    END IF;
END $$;

-- Step 14: Summary Report
SELECT
    'Tasks.ProjectId → TaskListId Migration' as operation,
    (SELECT COUNT(*) FROM _backup_projects."Tasks") as original_tasks_count,
    (SELECT COUNT(*) FROM "Tasks") as current_tasks_count,
    (SELECT COUNT(*) FROM "TaskLists") as tasklists_count,
    (SELECT COUNT(*) FROM "Tasks" WHERE "TaskListId" IS NULL) as orphaned_tasks,
    (SELECT COUNT(*) FROM _backup_projects."Tasks") as backup_rows;

-- ================================================================
-- ⚠️ IMPORTANT: Do NOT COMMIT until:
--   1. All verifications pass (✓ marks above)
--   2. Tasks count matches original count
--   3. No orphaned Tasks (orphaned_tasks = 0)
--   4. You review the summary report
--
-- To rollback: ROLLBACK;
-- To commit: UNCOMMENT the line below and run:
-- COMMIT;
-- ================================================================
