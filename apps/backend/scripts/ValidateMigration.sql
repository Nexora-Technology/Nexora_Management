-- ================================================================
-- Migration Validation Queries
-- ================================================================
-- Purpose: Comprehensive post-migration validation
-- Run AFTER both migration scripts complete successfully
-- Risk Level: 🟢 LOW (read-only queries)
--
-- Expected Results:
--   - All queries should show 0 orphaned/invalid records
--   - Counts should match baseline
--   - Performance should be acceptable
-- ================================================================

-- Set output format for better readability
\x off

-- ================================================================
-- SECTION 1: Data Integrity Checks
-- ================================================================

\echo '=========================================='
\echo 'SECTION 1: DATA INTEGRITY CHECKS'
\echo '=========================================='

-- Check 1: Spaces created per Workspace
\echo ''
\echo '✓ Check 1: Spaces created per Workspace'
\echo 'Expected: At least 1 Space per Workspace'
\echo ''
SELECT
    w."Name" as workspace_name,
    COUNT(s."Id") as spaces_count,
    STRING_AGG(s."Name", ', ' ORDER BY s."Name") as space_names
FROM "Workspaces" w
LEFT JOIN "Spaces" s ON s."WorkspaceId" = w."Id"
GROUP BY w."Id", w."Name"
ORDER BY spaces_count DESC, w."Name";

-- Check 2: TaskLists count equals Projects count
\echo ''
\echo '✓ Check 2: TaskLists migrated from Projects'
\echo 'Expected: tasklists_count = original_projects_count, difference = 0'
\echo ''
SELECT
    (SELECT COUNT(*) FROM "Projects") as original_projects_count,
    (SELECT COUNT(*) FROM "TaskLists") as tasklists_count,
    (SELECT COUNT(*) FROM "Projects") -
        (SELECT COUNT(*) FROM "TaskLists") as difference;

-- Check 3: Tasks count unchanged
\echo ''
\echo '✓ Check 3: Tasks count after migration'
\echo 'Expected: current_tasks = original_tasks (no data loss)'
\echo ''
SELECT
    (SELECT COUNT(*) FROM _backup_projects."Tasks") as original_tasks_count,
    (SELECT COUNT(*) FROM "Tasks") as current_tasks_count,
    (SELECT COUNT(*) FROM _backup_projects."Tasks") -
        (SELECT COUNT(*) FROM "Tasks") as difference;

-- Check 4: No orphaned Tasks
\echo ''
\echo '✓ Check 4: Orphaned Tasks check'
\echo 'Expected: orphaned_tasks = 0'
\echo ''
SELECT COUNT(*) as orphaned_tasks
FROM "Tasks" t
LEFT JOIN "TaskLists" tl ON t."TaskListId" = tl."Id"
WHERE tl."Id" IS NULL;

-- Check 5: List-Space relationships
\echo ''
\echo '✓ Check 5: TaskList distribution by Space'
\echo 'Shows: How many TaskLists per Space'
\echo ''
SELECT
    s."Name" as space_name,
    COUNT(tl."Id") as tasklists_count
FROM "Spaces" s
LEFT JOIN "TaskLists" tl ON tl."SpaceId" = s."Id"
GROUP BY s."Id", s."Name"
ORDER BY tasklists_count DESC;

-- Check 6: No TaskLists with invalid Space
\echo ''
\echo '✓ Check 6: TaskLists with invalid SpaceId'
\echo 'Expected: count = 0'
\echo ''
SELECT COUNT(*) as tasklists_with_invalid_space
FROM "TaskLists" tl
LEFT JOIN "Spaces" s ON tl."SpaceId" = s."Id"
WHERE s."Id" IS NULL;

-- ================================================================
-- SECTION 2: Foreign Key & Index Validation
-- ================================================================

\echo ''
\echo '=========================================='
\echo 'SECTION 2: FOREIGN KEY & INDEX VALIDATION'
\echo '=========================================='

-- Check 7: FK constraints on Tasks table
\echo ''
\echo '✓ Check 7: Foreign Key constraints on Tasks'
\echo 'Expected: FK_Tasks_TaskLists_TaskListId exists'
\echo ''
SELECT
    constraint_name,
    'FOREIGN KEY' as constraint_type
FROM information_schema.table_constraints
WHERE table_name = 'Tasks'
AND constraint_type = 'FOREIGN KEY'
ORDER BY constraint_name;

-- Check 8: Indexes on Tasks table
\echo ''
\echo '✓ Check 8: Indexes on Tasks table'
\echo 'Expected: IX_Tasks_TaskListId exists'
\echo ''
SELECT
    indexname,
    indexdef
FROM pg_indexes
WHERE tablename = 'Tasks'
AND indexname LIKE '%TaskListId%'
ORDER BY indexname;

-- Check 9: FK constraints on TaskLists table
\echo ''
\echo '✓ Check 9: Foreign Key constraints on TaskLists'
\echo 'Expected: FKs to Spaces and Folders exist'
\echo ''
SELECT
    tc.constraint_name,
    ccu.table_name AS references_table,
    'FOREIGN KEY' as constraint_type
FROM information_schema.table_constraints tc
JOIN information_schema.constraint_column_usage ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.table_name = 'TaskLists'
AND tc.constraint_type = 'FOREIGN KEY'
ORDER BY tc.constraint_name;

-- ================================================================
-- SECTION 3: Schema Validation
-- ================================================================

\echo ''
\echo '=========================================='
\echo 'SECTION 3: SCHEMA VALIDATION'
\echo '=========================================='

-- Check 10: New tables exist
\echo ''
\echo '✓ Check 10: New ClickUp hierarchy tables'
\echo 'Expected: All tables exist'
\echo ''
SELECT
    table_name,
    CASE
        WHEN table_name IN ('Spaces', 'Folders', 'TaskLists') THEN '✓ NEW'
        ELSE 'EXISTING'
    END as status
FROM information_schema.tables
WHERE table_schema = 'public'
AND table_name IN ('Spaces', 'Folders', 'TaskLists', 'Projects', 'Tasks')
ORDER BY status DESC, table_name;

-- Check 11: Column structure validation
\echo ''
\echo '✓ Check 11: Tasks table columns'
\echo 'Expected: TaskListId exists, ProjectId does NOT exist'
\echo ''
SELECT
    column_name,
    data_type,
    is_nullable
FROM information_schema.columns
WHERE table_name = 'Tasks'
AND column_name IN ('TaskListId', 'ProjectId')
ORDER BY column_name;

-- ================================================================
-- SECTION 4: Data Quality Checks
-- ================================================================

\echo ''
\echo '=========================================='
\echo 'SECTION 4: DATA QUALITY CHECKS'
\echo '=========================================='

-- Check 12: TaskList types distribution
\echo ''
\echo '✓ Check 12: TaskList distribution by type'
\echo 'Shows: Breakdown of TaskList types'
\echo ''
SELECT
    "ListType",
    COUNT(*) as count,
    ROUND(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER (), 2) as percentage
FROM "TaskLists"
GROUP BY "ListType"
ORDER BY count DESC;

-- Check 13: TaskList status distribution
\echo ''
\echo '✓ Check 13: TaskList distribution by status'
\echo 'Shows: Breakdown of TaskList statuses'
\echo ''
SELECT
    "Status",
    COUNT(*) as count
FROM "TaskLists"
GROUP BY "Status"
ORDER BY count DESC;

-- Check 14: Tasks per TaskList
\echo ''
\echo '✓ Check 14: Tasks distribution by TaskList (Top 10)'
\echo 'Shows: Top 10 TaskLists by task count'
\echo ''
SELECT
    tl."Name" as tasklist_name,
    s."Name" as space_name,
    COUNT(t."Id") as tasks_count
FROM "TaskLists" tl
LEFT JOIN "Spaces" s ON tl."SpaceId" = s."Id"
LEFT JOIN "Tasks" t ON t."TaskListId" = tl."Id"
GROUP BY tl."Id", tl."Name", s."Name"
ORDER BY tasks_count DESC
LIMIT 10;

-- ================================================================
-- SECTION 5: Performance Checks
-- ================================================================

\echo ''
\echo '=========================================='
\echo 'SECTION 5: PERFORMANCE CHECKS'
\echo '=========================================='

-- Check 15: Query performance test
\echo ''
\echo '✓ Check 15: Query execution time (EXPLAIN ANALYZE)'
\echo 'Shows: Performance of common Tasks query'
\echo ''
EXPLAIN (ANALYZE, BUFFERS, FORMAT TEXT)
SELECT t.*, tl."Name" as "TaskListName", s."Name" as "SpaceName"
FROM "Tasks" t
JOIN "TaskLists" tl ON t."TaskListId" = tl."Id"
JOIN "Spaces" s ON tl."SpaceId" = s."Id"
LIMIT 100;

-- ================================================================
-- SECTION 6: Migration Summary Report
-- ================================================================

\echo ''
\echo '=========================================='
\echo 'MIGRATION SUMMARY REPORT'
\echo '=========================================='

\echo ''
SELECT 'MIGRATION SUMMARY' as report_section,
    jsonb_build_object(
        'workspaces_count', (SELECT COUNT(*) FROM "Workspaces"),
        'spaces_count', (SELECT COUNT(*) FROM "Spaces"),
        'original_projects_count', (SELECT COUNT(*) FROM "Projects"),
        'migrated_tasklists_count', (SELECT COUNT(*) FROM "TaskLists"),
        'original_tasks_count', (SELECT COUNT(*) FROM _backup_projects."Tasks"),
        'current_tasks_count', (SELECT COUNT(*) FROM "Tasks"),
        'orphaned_tasks_count', (SELECT COUNT(*) FROM "Tasks" t LEFT JOIN "TaskLists" tl ON t."TaskListId" = tl."Id" WHERE tl."Id" IS NULL),
        'backup_projects_rows', (SELECT COUNT(*) FROM _backup_projects."Projects"),
        'backup_tasks_rows', (SELECT COUNT(*) FROM _backup_projects."Tasks")
    ) as metrics;

\echo ''
\echo '=========================================='
\echo 'VALIDATION COMPLETE'
\echo '=========================================='
\echo ''
\echo 'Review results above:'
\echo '  ✓ All orphaned counts should be 0'
\echo '  ✓ All counts should match baseline'
\echo '  ✓ Execution time should be < 100ms for queries'
\echo ''
\echo 'If all checks pass, migration is successful!'
\echo ''
