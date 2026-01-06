# ClickUp Hierarchy Implementation - Planning Report

**Date:** 2026-01-07
**Plan ID:** 260107-0051-clickup-hierarchy-implementation
**Reporter:** Planner Agent
**Status:** Planning Complete

---

## Executive Summary

Comprehensive implementation plan created for restructuring Nexora Management from 3-level hierarchy (Workspace → Project → Task) to ClickUp's 4-level hierarchy (Workspace → Space → Folder → List → Task).

**Key Decision:** **REUSE Project entity as List** (Approach A) to preserve data and minimize disruption.

**Total Effort:** 50 hours across 7 phases

**Recommendation:** **PROCEED** with Approach A

---

## Current State Analysis

**Existing Hierarchy:**
- Workspace → Project → Task (3 levels)
- Project entity: Name, Description, Color, Icon, Status, SettingsJsonb
- Task entity references Project (ProjectId foreign key)
- Navigation: "Tasks" item in sidebar

**24 Domain Entities:**
- User, Role, Permission, UserRole, RolePermission, RefreshToken
- Workspace, WorkspaceMember
- **Project**, **Task**, TaskStatus, Comment, Attachment, ActivityLog
- UserPresence, Notification, NotificationPreference
- Page, PageVersion, PageCollaborator, PageComment
- GoalPeriod, Objective, KeyResult

---

## Implementation Strategy

### Approach A: Reuse Project as List ✅ RECOMMENDED

**Benefits:**
1. **Data Preservation** - No migration needed for Project → List conversion
2. **Minimal Code Changes** - Task entity already has ProjectId foreign key
3. **YAGNI Compliance** - Avoid redundant entity creation
4. **Backward Compatibility** - Existing Project features continue working
5. **Faster Delivery** - 40h vs 60h (new entity approach)

**Trade-offs:**
- Naming inconsistency (Project vs List) → **Mitigated** via display names and code comments
- Conceptual mismatch → **Mitigated** via documentation

### Approach B: Create New List Entity ❌ NOT RECOMMENDED

**Drawbacks:**
- Complex migration with data loss risk
- Dual entity management (deprecation period)
- Longer timeline (60h vs 40h)

**Decision:** Proceed with Approach A

---

## Implementation Phases

### Phase 1: Backend Entity Design (8h)

**New Entities:**
1. **Space** - Workspace container
   - Properties: WorkspaceId, Name, Description, Color, Icon, IsPrivate, SettingsJsonb
   - Relationships: Workspace (Many-to-One), Folders (One-to-Many), Lists/Projects (One-to-Many)

2. **Folder** - Optional grouping
   - Properties: SpaceId, Name, Description, Color, Icon, PositionOrder, SettingsJsonb
   - Relationships: Space (Many-to-One), Lists/Projects (One-to-Many)
   - Constraint: NO sub-folders (single level deep)

**Modified Entity:**
3. **Project → List** (display name)
   - Remove: WorkspaceId
   - Add: SpaceId (NOT NULL), FolderId (NULL, optional), PositionOrder
   - Keep: All other properties unchanged

**No Changes:**
4. **Task** - Already references Project (becomes List in practice)

### Phase 2: Database Migration (10h)

**New Tables:**
- `Spaces` - Space entities
- `Folders` - Folder entities with unique index on (SpaceId, PositionOrder)

**Modified Tables:**
- `Projects` - Add SpaceId, FolderId, PositionOrder; Remove WorkspaceId

**Migration Steps:**
1. Create Spaces table
2. Create Folders table
3. Modify Projects table (remove WorkspaceId, add SpaceId/FolderId/PositionOrder)
4. Create default Space for each Workspace (name: "General")
5. Migrate existing Projects to default Space
6. Verify orphaned projects count = 0
7. Create indexes for performance

**Data Preservation:**
- Zero data loss (existing Projects → default Space)
- Backward compatible (keep Projects table name, entity is Project)

### Phase 3: API Endpoints (10h)

**Space Endpoints (5):**
- POST /api/spaces - Create space
- GET /api/spaces/{id} - Get by ID
- GET /api/spaces?workspaceId={id} - Get by workspace
- PUT /api/spaces/{id} - Update
- DELETE /api/spaces/{id} - Delete
- GET /api/spaces/{id}/lists - Get lists in space
- GET /api/spaces/{id}/folders - Get folders in space

**Folder Endpoints (5):**
- POST /api/folders - Create folder
- GET /api/folders/{id} - Get by ID
- PUT /api/folders/{id} - Update
- DELETE /api/folders/{id} - Delete
- PATCH /api/folders/{id}/position - Reorder
- GET /api/folders/{id}/lists - Get lists in folder

**List Endpoints (5, modified from Projects):**
- POST /api/lists - Create list (entity is Project)
- GET /api/lists/{id} - Get by ID
- GET /api/lists?spaceId={id}&folderId={id} - Get lists
- PUT /api/lists/{id} - Update
- DELETE /api/lists/{id} - Delete
- GET /api/lists/{id}/tasks - Get tasks in list

**Note:** Keep `/api/projects` as alias for 6-month transition period

### Phase 4: CQRS Commands and Queries (8h)

**Space Commands:**
- CreateSpaceCommand
- UpdateSpaceCommand
- DeleteSpaceCommand

**Folder Commands:**
- CreateFolderCommand
- UpdateFolderCommand
- DeleteFolderCommand
- UpdateFolderPositionCommand

**List Commands (modified from Project):**
- CreateListCommand (was CreateProjectCommand)
- UpdateListCommand (was UpdateProjectCommand)
- DeleteListCommand (was DeleteProjectCommand)
- MoveListCommand (NEW - move between folders/spaces)

**Queries:**
- GetSpaceByIdQuery
- GetSpacesByWorkspaceQuery
- GetFolderByIdQuery
- GetFoldersBySpaceQuery
- GetListByIdQuery
- GetListsQuery (filter by spaceId, folderId)

### Phase 5: Frontend Types and Components (4h)

**TypeScript Types:**
```typescript
interface Space {
  id: string;
  workspaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  isPrivate: boolean;
}

interface Folder {
  id: string;
  spaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  positionOrder: number;
}

interface List {  // Entity is Project
  id: string;
  spaceId: string;
  folderId?: string;
  name: string;
  // ... other properties
}
```

**Components:**
- `SpaceTreeNav` - Hierarchical navigation tree
- `SpaceCard` - Space display card
- `FolderCard` - Folder display card
- `ListCard` - List display card

**API Client:**
- `spacesApi.createSpace()`
- `spacesApi.createFolder()`
- `spacesApi.createList()`
- `spacesApi.getSpacesByWorkspace()`
- `spacesApi.getFoldersBySpace()`
- `spacesApi.getLists()`

### Phase 6: Frontend Pages and Routes (6h)

**Navigation Changes:**
- OLD: "Tasks" → `/tasks`
- NEW: "Spaces" → `/spaces`

**New Pages:**
1. `/spaces` - Space tree navigation
2. `/spaces/[id]` - Space detail view
3. `/folders/[id]` - Folder detail view
4. `/lists/[id]` - List detail view (tasks)

**Modified Pages:**
1. `/tasks/[id]` - Update breadcrumb to show Space → Folder → List → Task

**Breadcrumb Example:**
```
Home > Spaces > Engineering > Backend > API Tasks > Fix Authentication Bug
```

### Phase 7: Testing and Validation (4h)

**Backend Unit Tests:**
- Space CRUD operations
- Folder CRUD + reordering
- List CRUD with Space/Folder association
- Migration verification (orphaned projects = 0)

**Frontend Integration Tests:**
- Space tree navigation renders
- Create space/folder/list
- Expand/collapse functionality
- Active state highlighting

**E2E Tests (Playwright):**
- Create space → appears in tree
- Create folder → nested under space
- Create list → nested under folder or space
- Drag-and-drop reordering
- Task detail breadcrumb correctness

---

## Risk Assessment

### High Risks 🔴

1. **Data Loss During Migration**
   - **Impact:** Critical
   - **Probability:** Low
   - **Mitigation:**
     - Create database backup before migration
     - Test migration in staging environment
     - Verify orphaned projects count after migration (should be 0)
     - Create rollback script

2. **Performance Regression**
   - **Impact:** High
   - **Probability:** Medium
   - **Mitigation:**
     - Add indexes on SpaceId, FolderId
     - Use AsNoTracking() for read-only queries
     - Load test before and after migration
     - Monitor query execution plans

### Medium Risks 🟡

3. **Naming Confusion (Project vs List)**
   - **Impact:** Medium (developer confusion)
   - **Probability:** High
   - **Mitigation:**
     - Code comments: `// Entity is Project, display name is List`
     - Update documentation with terminology mapping
     - Use `ListDto` consistently in frontend
     - Team training session

4. **RLS Policy Updates**
   - **Impact:** Medium (security)
   - **Probability:** Medium
   - **Mitigation:**
     - Update RLS policies to use SpaceId
     - Test with multiple workspaces
     - Security audit before deployment

### Low Risks 🟢

5. **Backward Compatibility**
   - **Impact:** Low (API clients)
   - **Probability:** Low
   - **Mitigation:**
     - Keep `/api/projects` endpoint as alias
     - Support both `ProjectDto` and `ListDto` for transition
     - Deprecation notice in API documentation

---

## Success Criteria

### Phase 1: Backend Entity Design
- ✅ Space entity created with all properties
- ✅ Folder entity created with PositionOrder
- ✅ Project entity modified (SpaceId, FolderId, PositionOrder)
- ✅ Navigation properties configured

### Phase 2: Database Migration
- ✅ Migration SQL compiles without errors
- ✅ Default Space created per Workspace
- ✅ All Projects migrated to default Space
- ✅ Orphaned projects = 0
- ✅ Indexes created
- ✅ Foreign keys valid

### Phase 3: API Endpoints
- ✅ Space CRUD endpoints (5)
- ✅ Folder CRUD endpoints (5)
- ✅ List CRUD endpoints (5)
- ✅ Swagger documentation updated
- ✅ Authorization applied

### Phase 4: CQRS Commands/Queries
- ✅ Space commands (3)
- ✅ Folder commands (4)
- ✅ List commands (3)
- ✅ Queries implemented (6)
- ✅ Validation enforced

### Phase 5: Frontend Types/Components
- ✅ TypeScript types match backend DTOs
- ✅ API client working
- ✅ SpaceTreeNav renders correctly
- ✅ Expansion/collapse working

### Phase 6: Frontend Pages/Routes
- ✅ `/spaces` page renders tree
- ✅ `/lists/[id]` shows tasks
- ✅ Navigation updated
- ✅ Breadcrumb shows full path

### Phase 7: Testing/Validation
- ✅ Unit tests pass (backend)
- ✅ Integration tests pass (frontend)
- ✅ E2E tests pass (Playwright)
- ✅ Load tests pass (performance baseline)

---

## Timeline and Effort

| Phase | Duration | Dependencies | Critical Path |
|-------|----------|--------------|---------------|
| Phase 1: Backend Entity Design | 8h | None | ✅ YES |
| Phase 2: Database Migration | 10h | Phase 1 | ✅ YES |
| Phase 3: API Endpoints | 10h | Phase 2, 4 | ✅ YES |
| Phase 4: CQRS Commands/Queries | 8h | Phase 1 | ✅ YES |
| Phase 5: Frontend Types/Components | 4h | Phase 3 | |
| Phase 6: Frontend Pages/Routes | 6h | Phase 5 | |
| Phase 7: Testing/Validation | 4h | All phases | ✅ YES |
| **Total** | **50h** | - | - |

**Parallel Work:**
- Phase 4 can run parallel with Phase 2 and 3
- Phase 5 can start once Phase 3 is partially complete

**Critical Path:** Phase 1 → Phase 2 → Phase 4 → Phase 3 → Phase 5 → Phase 6 → Phase 7

---

## Open Questions

1. **Should we deprecate `/api/projects` endpoint immediately?**
   - **Recommendation:** No, keep as alias for 6-month transition period
   - **Timeline:** Deprecation notice in Phase 09

2. **Should we enforce Folder structure at database level?**
   - **Recommendation:** No, keep at application layer for flexibility
   - **Reason:** ClickUp allows Lists directly under Spaces

3. **Should we add Workspace Settings for "Default Space"?**
   - **Recommendation:** Yes, in Phase 09 (Workspace Settings)
   - **Benefit:** Users can choose where new lists go by default

4. **Should we support moving Lists between Folders/Spaces?**
   - **Recommendation:** Yes, implement `MoveListCommand` in Phase 09
   - **Reason:** ClickUp supports drag-and-drop reorganization

5. **Should we add permission granularity at Space/Folder level?**
   - **Recommendation:** No, keep Workspace-level permissions for now (YAGNI)
   - **Future:** Consider in Phase 11 (Advanced Security)

---

## Next Steps

1. **Review plan with team** - Get buy-in on Approach A (Reuse Project as List)
2. **Create feature branch** - `feat/clickup-hierarchy`
3. **Start Phase 1** - Create Space and Folder entities
4. **Setup staging database** - For migration testing
5. **Schedule migration window** - Coordinate with team for deployment

---

## Files Modified

**Backend:**
- `/apps/backend/src/Nexora.Management.Domain/Entities/Space.cs` (NEW)
- `/apps/backend/src/Nexora.Management.Domain/Entities/Folder.cs` (NEW)
- `/apps/backend/src/Nexora.Management.Domain/Entities/Project.cs` (MODIFIED)
- `/apps/backend/src/Nexora.Management.Domain/Entities/Workspace.cs` (MODIFIED)
- `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/Configurations/SpaceConfiguration.cs` (NEW)
- `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/Configurations/FolderConfiguration.cs` (NEW)
- `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/Configurations/ProjectConfiguration.cs` (MODIFIED)
- `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/Configurations/WorkspaceConfiguration.cs` (MODIFIED)
- `/apps/backend/src/Nexora.Management.Infrastructure/Persistence/AppDbContext.cs` (MODIFIED)
- `/apps/backend/src/Nexora.Management.API/Persistence/Migrations/20260107XXXXXX_AddClickUpHierarchyTables.cs` (NEW)
- `/apps/backend/scripts/MigrateToClickUpHierarchy.sql` (NEW)
- `/apps/backend/src/Nexora.Management.API/Endpoints/SpaceEndpoints.cs` (NEW)
- `/apps/backend/src/Nexora.Management.API/Endpoints/FolderEndpoints.cs` (NEW)
- `/apps/backend/src/Nexora.Management.API/Endpoints/ListEndpoints.cs` (RENAMED from ProjectEndpoints.cs)
- `/apps/backend/src/Nexora.Management.Application/Spaces/*` (NEW - CQRS)
- `/apps/backend/src/Nexora.Management.Application/Folders/*` (NEW - CQRS)

**Frontend:**
- `/apps/frontend/src/features/spaces/types.ts` (NEW)
- `/apps/frontend/src/features/spaces/api.ts` (NEW)
- `/apps/frontend/src/components/spaces/space-tree-nav.tsx` (NEW)
- `/apps/frontend/src/app/spaces/page.tsx` (NEW)
- `/apps/frontend/src/app/spaces/[id]/page.tsx` (NEW)
- `/apps/frontend/src/app/lists/[id]/page.tsx` (NEW)
- `/apps/frontend/src/components/layout/sidebar-nav.tsx` (MODIFIED - Tasks → Spaces)
- `/apps/frontend/src/components/tasks/task-card.tsx` (MODIFIED - List instead of Project)
- `/apps/frontend/src/components/tasks/task-modal.tsx` (MODIFIED - List selector)

---

## Recommendations

1. **PROCEED with Approach A** (Reuse Project as List)
   - Minimizes risk of data loss
   - Faster delivery (50h vs 60h)
   - Backward compatible

2. **Implement in phases**
   - Start with Phase 1 (Backend Entity Design)
   - Test migration thoroughly in staging
   - Deploy to production after Phase 7 completion

3. **Monitor performance**
   - Run load tests before and after migration
   - Track query execution times
   - Add monitoring for Space/Folder/List queries

4. **Document terminology mapping**
   - Create glossary: Project entity = List display name
   - Update onboarding materials
   - Conduct team training session

5. **Plan for deprecation**
   - Keep `/api/projects` alias for 6 months
   - Add deprecation notice to API docs
   - Communicate with API consumers

---

## ClickUp Hierarchy Reference

**Target Structure:**
```
Workspace
  └─ Space (Required)
       ├─ Folder (Optional)
       │    └─ List (Required)
       │         └─ Task
       └─ List (Can exist directly under Space)
            └─ Task
```

**Key Features:**
- ✅ Folders are OPTIONAL
- ✅ No sub-folders (single level)
- ✅ Tasks MUST be in Lists
- ✅ Mixed structures allowed
- ✅ Lists track diverse content (projects, teams, campaigns)

**Terminology Mapping:**
| ClickUp | Nexora Entity | Display Name |
|---------|---------------|--------------|
| Workspace | Workspace | Workspace |
| Space | Space | Space |
| Folder | Folder | Folder |
| List | Project | List |
| Task | Task | Task |

---

**Report Version:** 1.0
**Date:** 2026-01-07
**Status:** Planning Complete
**Next Action:** Team Review and Approval
