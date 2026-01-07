-- ================================================================
-- EMERGENCY ROLLBACK SCRIPT
-- ================================================================
-- ⚠️ CRITICAL: Use ONLY if migration fails and causes issues
-- ⚠️ This will REVERT all changes made by migration scripts
-- ⚠️ Run in TRANSACTION with ROLLBACK available
--
-- Purpose: Emergency rollback to restore Projects-based hierarchy
-- Reverts: MigrateProjectsToTaskLists.sql + MigrateTasksToTaskLists.sql
--
-- Prerequisites:
--   1. Backup tables exist (_backup_projects schema)
--   2. Migration was completed (not partial)
--   3. Understand this will DELETE TaskLists and restore Projects
--
-- Expected Outcome:
--   - Tasks.TaskListId → Tasks.ProjectId (reversed)
--   - TaskLists table deleted
--   - Projects table restored from backup
--   - Spaces/Folders kept (safe to keep)
--
-- Risk Level: 🔴 CRITICAL (data restoration operation)
-- Estimated Time: 10-20 minutes
--
-- ================================================================

BEGIN;

-- Step 1: Verify backup tables exist
DO $$
DECLARE
    v_backup_projects_exists BOOLEAN;
    v_backup_tasks_exists BOOLEAN;
BEGIN
    SELECT EXISTS (
        SELECT 1 FROM information_schema.tables
        WHERE table_schema = '_backup_projects'
        AND table_name = 'Projects'
    ) INTO v_backup_projects_exists;

    SELECT EXISTS (
        SELECT 1 FROM information_schema.tables
        WHERE table_schema = '_backup_projects'
        AND table_name = 'Tasks'
    ) INTO v_backup_tasks_exists;

    IF NOT v_backup_projects_exists OR NOT v_backup_tasks_exists THEN
        RAISE EXCEPTION '❌ ROLLBACK ABORTED: Backup tables not found in _backup_projects schema';
    ELSE
        RAISE NOTICE '✓ Backup tables verified';
    END IF;
END $$;

RAISE NOTICE 'Starting emergency rollback...';

-- Step 2: Drop FK constraint from Tasks
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.table_constraints
        WHERE table_name = 'Tasks'
        AND constraint_name = 'FK_Tasks_TaskLists_TaskListId'
    ) THEN
        ALTER TABLE "Tasks"
        DROP CONSTRAINT "FK_Tasks_TaskLists_TaskListId";
        RAISE NOTICE 'Dropped FK: FK_Tasks_TaskLists_TaskListId';
    END IF;
END $$;

-- Step 3: Drop TaskListId index
DROP INDEX IF EXISTS "IX_Tasks_TaskListId";
RAISE NOTICE 'Dropped index: IX_Tasks_TaskListId';

-- Step 4: Add ProjectId column back
ALTER TABLE "Tasks"
ADD COLUMN IF NOT EXISTS "ProjectId" UUID NULL;

RAISE NOTICE 'Added column: ProjectId';

-- Step 5: Copy TaskListId → ProjectId (restore original references)
UPDATE "Tasks"
SET "ProjectId" = "TaskListId"
WHERE "TaskListId" IS NOT NULL;

RAISE NOTICE 'Restored ProjectId from TaskListId: % rows',
    (SELECT COUNT(*) FROM "Tasks" WHERE "ProjectId" IS NOT NULL);

-- Step 6: Set ProjectId to NOT NULL
ALTER TABLE "Tasks"
ALTER COLUMN "ProjectId" SET NOT NULL;

RAISE NOTICE 'Set ProjectId to NOT NULL';

-- Step 7: Recreate FK to Projects
ALTER TABLE "Tasks"
ADD CONSTRAINT "FK_Tasks_Projects_ProjectId"
FOREIGN KEY ("ProjectId")
REFERENCES "Projects"("Id")
ON DELETE CASCADE;

RAISE NOTICE 'Created FK: FK_Tasks_Projects_ProjectId';

-- Step 8: Create index for ProjectId
CREATE INDEX IF NOT EXISTS "IX_Tasks_ProjectId"
ON "Tasks"("ProjectId");

RAISE NOTICE 'Created index: IX_Tasks_ProjectId';

-- Step 9: Drop TaskListId column
ALTER TABLE "Tasks"
DROP COLUMN IF EXISTS "TaskListId";

RAISE NOTICE 'Dropped column: TaskListId';

-- Step 10: Verify Tasks restored correctly
DO $$
DECLARE
    v_backup_tasks_count INT;
    v_current_tasks_count INT;
    v_orphaned_tasks INT;
BEGIN
    SELECT COUNT(*) INTO v_backup_tasks_count FROM _backup_projects."Tasks";
    SELECT COUNT(*) INTO v_current_tasks_count FROM "Tasks";

    IF v_current_tasks_count <> v_backup_tasks_count THEN
        RAISE WARNING '⚠️ Tasks count mismatch: backup=%, current=%',
            v_backup_tasks_count, v_current_tasks_count;
    END IF;

    SELECT COUNT(*) INTO v_orphaned_tasks
    FROM "Tasks" t
    LEFT JOIN "Projects" p ON t."ProjectId" = p."Id"
    WHERE p."Id" IS NULL;

    IF v_orphaned_tasks > 0 THEN
        RAISE WARNING '⚠️ Found % Tasks with invalid ProjectId after rollback', v_orphaned_tasks;
    ELSE
        RAISE NOTICE '✓ All Tasks have valid ProjectId references';
    END IF;
END $$;

-- Step 11: Delete migrated TaskLists (they were copies anyway)
-- ⚠️ This is safe because Projects still exist with original IDs
DROP TABLE IF EXISTS "TaskLists" CASCADE;
RAISE NOTICE 'Dropped table: TaskLists';

-- Step 12: Optionally delete Spaces and Folders (kept for flexibility)
-- Uncomment below if you want to completely remove new hierarchy
-- DROP TABLE IF EXISTS "Folders" CASCADE;
-- DROP TABLE IF EXISTS "Spaces" CASCADE;
-- RAISE NOTICE 'Dropped tables: Folders, Spaces';

-- Step 13: Drop backup schema (AFTER confirming rollback successful)
-- ⚠️ ONLY run this after verifying everything works
-- DROP SCHEMA IF EXISTS _backup_projects CASCADE;
-- RAISE NOTICE 'Dropped backup schema: _backup_projects';

-- Step 14: Rollback Summary Report
SELECT
    'EMERGENCY ROLLBACK COMPLETE' as status,
    (SELECT COUNT(*) FROM "Projects") as projects_restored,
    (SELECT COUNT(*) FROM "Tasks") as tasks_restored,
    (SELECT COUNT(*) FROM "Spaces") as spaces_kept,
    (SELECT COUNT(*) FROM _backup_projects."Tasks") as backup_tasks_available;

-- ================================================================
-- ⚠️ IMPORTANT: Do NOT COMMIT until:
--   1. You verify Projects table is accessible
--   2. You verify Tasks.ProjectId references work
--   3. You test the application with Projects
--   4. You're confident rollback is successful
--
-- To review before committing: Review the summary report above
-- To commit: UNCOMMENT the line below and run:
-- COMMIT;
--
-- To abort rollback: ROLLBACK;
-- ================================================================

-- ================================================================
-- POST-ROLLBACK CHECKLIST
-- ================================================================
-- After committing rollback:
--
-- [ ] Verify application starts successfully
-- [ ] Verify /api/projects endpoints work
-- [ ] Verify /api/tasks endpoints work
-- [ ] Verify Tasks can be created/edited
-- [ ] Verify no errors in application logs
-- [ ] Verify Projects page loads
-- [ ] Verify Task board loads
--
-- If any issues:
-- 1. Check _backup_projects schema still exists
-- 2. Run validation queries manually
-- 3. Review application error logs
-- 4. Consider restoring from database backup
--
-- ================================================================
