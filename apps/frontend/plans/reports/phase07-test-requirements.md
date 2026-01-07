# Phase 7: Testing and Validation - Requirements & Setup Guide

## Status: Deferred Pending Test Infrastructure Setup

**Date:** 2025-01-07
**Phase:** ClickUp Hierarchy Implementation - Phase 7
**Issue:** No test infrastructure currently configured

---

## Why Phase 7 is Deferred

### Current State
- **No test runner configured** (vitest/jest not in package.json)
- **No testing library** (@testing-library/react not installed)
- **No E2E framework** (Playwright not configured)
- **Backend tests** require separate backend workspace execution

### Work Required Before Implementation
1. Install and configure vitest
2. Install @testing-library/react and utilities
3. Install and configure Playwright
4. Create test setup files (vitest.config.ts, playwright.config.ts)
5. Add test scripts to package.json
6. Create test utilities and mocks

**Estimated Setup Time:** 2-3 hours

---

## Test Requirements (When Infrastructure is Ready)

### Frontend Unit Tests

#### 1. SpaceTreeNav Component Tests
**File:** `src/components/spaces/space-tree-nav.test.tsx`

```typescript
describe('SpaceTreeNav', () => {
  it('renders empty state when no spaces provided')
  it('renders spaces with folders and tasklists in hierarchy')
  it('expands/collapses nodes on click')
  it('calls onNodeClick with correct node data')
  it('calls onCreateSpace when New Space button clicked')
  it('displays selected state for selectedNodeId')
  it('respects collapsed prop')
  it('has proper ARIA attributes (role, aria-expanded, aria-selected)')
})
```

#### 2. Spaces Page Tests
**File:** `src/app/(app)/spaces/page.test.tsx`

```typescript
describe('SpacesPage', () => {
  it('renders loading state initially')
  it('renders empty state when no spaces exist')
  it('renders space tree when data loaded')
  it('navigates to list detail on tasklist click')
  it('fetches spaces, folders, and tasklists in parallel')
  it('builds hierarchical tree structure correctly')
})
```

#### 3. List Detail Page Tests
**File:** `src/app/(app)/lists/[id]/page.test.tsx`

```typescript
describe('ListDetailPage', () => {
  it('renders loading state')
  it('renders not found state when list invalid')
  it('renders list details with breadcrumb')
  it('displays tasks when loaded')
  it('shows empty state when no tasks')
  it('breadcrumb includes: Home > Spaces > [List Name]')
})
```

#### 4. Task Modal Tests
**File:** `src/components/tasks/task-modal.test.tsx`

```typescript
describe('TaskModal', () => {
  it('renders form fields correctly')
  it('includes list selector dropdown')
  it('validates title is required')
  it('calls onSubmit with correct data')
  it('resets form after create')
  it('pre-fills data in edit mode')
})
```

---

### Frontend Integration Tests

**File:** `src/features/spaces/utils.test.ts`

```typescript
describe('buildSpaceTree', () => {
  it('builds empty tree from empty inputs')
  it('builds tree with only spaces')
  it('nests folders under spaces')
  it('nests tasklists under folders')
  it('nests tasklists under spaces when no folder')
  it('preserves order with positionOrder')
})

describe('findNodeById', () => {
  it('returns undefined for empty tree')
  it('finds space node by id')
  it('finds folder node nested in space')
  it('finds tasklist nested in folder')
})

describe('getNodePath', () => {
  it('returns empty array for node not found')
  it('returns path for space node (1 element)')
  it('returns path for folder (space > folder)')
  it('returns path for tasklist (space > folder > tasklist)')
})
```

---

### E2E Tests with Playwright

**File:** `e2e/spaces.spec.ts`

```typescript
test.describe('Spaces Hierarchy', () => {
  test('user can view spaces page', async ({ page }) => {
    await page.goto('/spaces')
    await expect(page.locator('h2')).toContainText('Spaces')
  })

  test('user sees space tree with folders and lists', async ({ page }) => {
    await page.goto('/spaces')
    // TODO: Add test data seeding
    await expect(page.locator('[role="tree"]')).toBeVisible()
  })

  test('user can navigate to list detail', async ({ page }) => {
    await page.goto('/spaces')
    // TODO: Click on tasklist node
    // await page.click('[data-node-type="tasklist"]')
    // await expect(page).toHaveURL(/\/lists\/.+/)
  })

  test('breadcrumb shows full hierarchy path', async ({ page }) => {
    await page.goto('/lists/test-list-id')
    const breadcrumb = page.locator('nav[aria-label="Breadcrumb"]')
    await expect(breadcrumb).toContainText('Home')
    await expect(breadcrumb).toContainText('Spaces')
  })
})
```

---

### Backend Tests (To be run in backend workspace)

#### 1. SpaceTests.cs
**Location:** `apps/backend/tests/SpaceTests.cs`

- CreateSpace_WithValidData_ReturnsSpaceDto
- CreateSpace_WithInvalidWorkspaceId_ReturnsError
- UpdateSpace_WithValidData_ReturnsUpdatedDto
- DeleteSpace_WithValidId_RemovesSpace
- GetSpacesByWorkspace_ReturnsOnlyWorkspaceSpaces

#### 2. FolderTests.cs
**Location:** `apps/backend/tests/FolderTests.cs`

- CreateFolder_InSpace_SetsSpaceId
- CreateFolder_WithInvalidSpaceId_ReturnsError
- UpdateFolderPosition_WithValidOrder_UpdatesPosition
- DeleteFolder_WithLists_CascadeDeletesLists

#### 3. ListTests.cs (modified ProjectTests.cs)
**Location:** `apps/backend/tests/ListTests.cs`

- CreateList_InSpace_SetsSpaceId
- CreateList_InFolder_SetsFolderId
- CreateList_WithInvalidSpaceId_ReturnsError
- UpdateListPosition_WithinSpace_UpdatesPosition
- GetListsBySpace_ReturnsSpaceLists

#### 4. MigrationTests.cs
**Location:** `apps/backend/tests/MigrationTests.cs`

- MigrateProjectsToLists_AllProjectsCopied_CountsMatch
- MigrateTasksToLists_AllTasksUpdated_NoOrphanedTasks
- ValidateMigration_DataIntegrityChecksPass

---

## Manual Testing Checklist (Can be performed now)

### Spaces Page
- [ ] Navigate to `/spaces` - page loads without errors
- [ ] See empty state when no spaces exist
- [ ] See space tree navigation when spaces exist
- [ ] Click on tasklist node - navigates to `/lists/[id]`
- [ ] See "New Space" button

### List Detail Page
- [ ] Navigate to `/lists/[valid-id]` - shows list details
- [ ] Navigate to `/lists/[invalid-id]` - shows not found state
- [ ] Breadcrumb shows: Home > Spaces > [List Name]
- [ ] See task board area
- [ ] "Add Task" button visible

### Navigation
- [ ] Sidebar shows "Spaces" instead of "Tasks"/"Projects"
- [ ] Clicking "Spaces" navigates to `/spaces`
- [ ] Task detail page breadcrumb shows "Spaces" not "Tasks"

### Task Modal
- [ ] "List" selector dropdown appears
- [ ] List options are selectable
- [ ] Form validation works (title required)

---

## Next Steps

1. **Option A:** Set up test infrastructure now (2-3 hours)
   - Run: `npm install -D vitest @testing-library/react @testing-library/jest-dom @vitejs/plugin-react`
   - Run: `npm install -D @playwright/test`
   - Create config files
   - Implement tests above

2. **Option B:** Proceed to Phase 8 without tests
   - Mark Phase 7 as deferred
   - Continue with Workspace Context implementation
   - Return to tests when infrastructure is ready

3. **Option C:** Manual validation only
   - Complete manual testing checklist
   - Document results
   - Mark Phase 7 as partially complete

---

## Unresolved Questions

1. Should test infrastructure be set up now or deferred?
2. Should backend tests be executed in backend workspace separately?
3. What is the priority: test coverage vs. feature completion?

---

**Recommendation:** Proceed to Phase 8 (Workspace Context) and return to Phase 7 testing when test infrastructure is properly configured. Manual testing checklist can be used for immediate validation.
