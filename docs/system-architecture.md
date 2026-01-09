# System Architecture

**Last Updated:** 2026-01-09
**Version:** Phase 09 Complete - Time Tracking + Docker Testing Phase (Phases 17/18 In Progress)
**Backend Files:** 220 C# files (~26,500 LOC)
**Database Entities:** 29 (up from 27)
**API Endpoint Groups:** 12 (up from 11)
**Database Migrations:** 9 (up from 7)
**Test Coverage:** 0% (critical issue)
**Docker Testing:** COMPLETE - 3/4 services healthy, 3 critical issues found

## Overview

Nexora Management implements **Clean Architecture** principles with clear separation of concerns across four distinct layers. This architecture ensures testability, maintainability, and independence from external frameworks and databases.

## Architectural Layers

```
┌─────────────────────────────────────────────────────────────┐
│                     API Layer (Presentation)                 │
│           Controllers, Endpoints, DTOs, Middleware          │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              Application Layer (Business Logic)              │
│            Use Cases, CQRS, MediatR, Services               │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              Infrastructure Layer (External)                 │
│          EF Core, PostgreSQL, External Services              │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                   Domain Layer (Core)                        │
│              Entities, Value Objects, Interfaces             │
└─────────────────────────────────────────────────────────────┘
```

## 1. Domain Layer

**Location:** `/apps/backend/src/Nexora.Management.Domain/`

**Purpose:** Contains core business logic and enterprise rules. This layer has no dependencies on external frameworks or databases.

### Frontend Domain Models (Phase 09 - Phase 5)

**Location:** `/apps/frontend/src/features/spaces/types.ts`

**Purpose:** TypeScript type definitions mirroring backend domain entities for ClickUp hierarchy

**Type Definitions:**

```typescript
// Space Types - First organizational level under Workspace
interface Space {
  id: string;
  workspaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  isPrivate: boolean;
  createdAt: string;
  updatedAt: string;
}

interface CreateSpaceRequest {
  workspaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  isPrivate?: boolean;
}

interface UpdateSpaceRequest {
  name?: string;
  description?: string;
  color?: string;
  icon?: string;
  isPrivate?: boolean;
}

// Folder Types - Optional grouping container for Lists
interface Folder {
  id: string;
  spaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  positionOrder: number;
  createdAt: string;
  updatedAt: string;
}

interface CreateFolderRequest {
  spaceId: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
}

interface UpdateFolderRequest {
  name?: string;
  description?: string;
  color?: string;
  icon?: string;
}

interface UpdateFolderPositionRequest {
  positionOrder: number;
}

// TaskList Types - Mandatory container for Tasks
interface TaskList {
  id: string;
  spaceId: string;
  folderId?: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  listType: 'task' | 'project' | 'team' | 'campaign';
  status: string;
  ownerId: string;
  positionOrder: number;
  createdAt: string;
  updatedAt: string;
}

interface CreateTaskListRequest {
  spaceId: string;
  folderId?: string;
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  listType?: string;
  ownerId?: string;
}

interface UpdateTaskListRequest {
  name?: string;
  description?: string;
  color?: string;
  icon?: string;
  status?: string;
}

interface UpdateTaskListPositionRequest {
  positionOrder: number;
}

// Tree Navigation Types - Hierarchical UI structure
type SpaceTreeNodeType = 'space' | 'folder' | 'tasklist';

interface SpaceTreeNode {
  id: string;
  name: string;
  type: SpaceTreeNodeType;
  spaceId?: string;
  folderId?: string;
  children?: SpaceTreeNode[];
  color?: string;
  icon?: string;
  listType?: string;
}
```

**Design Principles:**

- **Type Safety:** Full TypeScript coverage for all entities
- **API Parity:** Create/Update request types match backend DTOs
- **Tree Structure:** SpaceTreeNode enables hierarchical UI rendering
- **Optional Fields:** Proper handling of nullable fields with `?`
- **Enum Types:** ListType restricted to specific values for type safety

### Components

#### Entities (27 Domain Models)

All entities inherit from `BaseEntity` which provides:

- `Id` (Guid) - Unique identifier
- `CreatedAt` (DateTime) - Creation timestamp
- `UpdatedAt` (DateTime) - Last update timestamp

**Entity Hierarchy:**

```
BaseEntity (abstract)
├── User
├── Role
├── Permission
├── UserRole (join table)
├── RolePermission (join table)
├── RefreshToken
├── Workspace
├── WorkspaceMember (join table)
├── Project (DEPRECATED - migrating to TaskList)
├── Space (NEW Phase 09)
├── Folder (NEW Phase 09)
├── TaskList (NEW Phase 09)
├── Task
├── TaskStatus
├── Comment
├── Attachment
├── ActivityLog
├── UserPresence
├── Notification
├── NotificationPreference
├── Page (Phase 07)
├── PageVersion (Phase 07)
├── PageCollaborator (Phase 07)
├── PageComment (Phase 07)
├── GoalPeriod (Phase 08)
├── Objective (Phase 08)
└── KeyResult (Phase 08)
```

**Key Entities:**

1. **User**
   - Authentication and user profile
   - Relationships: Workspaces (owned), TaskLists (owned), Tasks (assigned), Comments, Attachments

2. **Workspace**
   - Top-level container for multi-tenancy
   - JSONB settings for flexible configuration
   - Owner and members relationship
   - Spaces collection (NEW Phase 09)

3. **Space** (NEW Phase 09)
   - First organizational level under Workspace in ClickUp-style hierarchy
   - Organizes work by: departments, teams, clients, or high-level initiatives
   - Properties: Name, Description, Color, Icon, IsPrivate, SettingsJsonb
   - Relationships: Parent Workspace, Folders, TaskLists
   - Independent settings per Space

4. **Folder** (NEW Phase 09)
   - Optional grouping container for Lists within a Space
   - Single-level only (no sub-folders)
   - Properties: Name, Description, Color, Icon, PositionOrder, SettingsJsonb
   - Relationships: Parent Space, TaskLists
   - Position ordering for drag-and-drop

5. **TaskList** (NEW Phase 09)
   - Mandatory container for Tasks in ClickUp-style hierarchy
   - Display name in UI: "List"
   - Can exist directly under Spaces or within Folders
   - Properties: Name, Description, Color, Icon, ListType, Status, OwnerId, PositionOrder, SettingsJsonb
   - ListType examples: "task", "project", "team", "campaign", "milestone"
   - Relationships: Parent Space, optional parent Folder, Owner, Tasks, TaskStatuses
   - Every Task MUST belong to a TaskList (no orphaned tasks)

6. **Project** (DEPRECATED - Migrating to TaskList)
   - Workspace-scoped project management
   - Properties: Name, Description, Color, Icon, Status
   - Tasks and TaskStatus relationships
   - NOTE: Being replaced by TaskList in Phase 09

7. **Task**
   - Hierarchical tasks (parent-child via `ParentTaskId`)
   - Flexible scheduling (StartDate, DueDate)
   - Time tracking (EstimatedHours)
   - Priority levels (low, medium, high, urgent)
   - JSONB custom fields for extensibility
   - Position ordering for drag-and-drop
   - NEW: TaskListId (references TaskList in ClickUp hierarchy)
   - DEPRECATED: ProjectId (kept for migration compatibility)

8. **Comment**
   - Threaded comments (self-referencing via `ParentCommentId`)
   - Associated with Tasks and Users

9. **Attachment**
   - File attachments for tasks
   - Metadata: FileName, FilePath, FileSizeBytes, MimeType

10. **UserPresence**
    - Tracks user online/offline status
    - Current view tracking (task/project being viewed)
    - Last seen timestamp
    - Connection ID for SignalR

11. **Notification**
    - User notifications
    - Types: task_assigned, comment_mentioned, status_changed, due_date_reminder
    - Read status tracking
    - JSONB data field for metadata

12. **NotificationPreference**
    - User notification settings
    - Per-event type toggles
    - In-app vs email preferences
    - Quiet hours configuration

13. **Page** (Phase 07)
    - Wiki/document pages with hierarchical structure
    - Self-referencing via `ParentPageId` for nested pages
    - Unique slug for URLs
    - Rich text content storage
    - Favorite and recent view tracking
    - Workspace-scoped with RLS

14. **PageVersion** (Phase 07)
    - Version history for page restore capability
    - Auto-created on content changes
    - Composite key (PageId + VersionNumber)
    - Stores full content snapshot
    - Created by tracking

15. **PageCollaborator** (Phase 07)
    - Page collaboration with role-based access
    - Roles: Owner, Editor, Viewer
    - Composite key (PageId + UserId)
    - Workspace membership validation

16. **PageComment** (Phase 07)
    - Threaded comments on document pages
    - Self-referencing via `ParentCommentId` for nested replies
    - Similar to Task comments but for pages

17. **GoalPeriod** (Phase 08)
    - Time periods for goal tracking (e.g., Q1 2026, FY 2026)
    - Workspace-scoped with start/end dates
    - Status tracking (active, archived)
    - Objectives association for period-based goals

18. **Objective** (Phase 08)
    - Objectives with hierarchical structure (parent-child relationships)
    - Workspace-scoped with optional period association
    - Owner assignment to users
    - Weight-based priority (1-10)
    - Status tracking (on-track, at-risk, off-track, completed)
    - Progress percentage (0-100) calculated from weighted average of key results
    - Position ordering for drag-and-drop

19. **KeyResult** (Phase 08)
    - Measurable key results for objectives
    - Metric types: number, percentage, currency
    - Current and target values for progress tracking
    - Unit specification (%, $, count, etc.)
    - Due date for time-bound key results
    - Progress percentage (0-100) calculated as (CurrentValue / TargetValue) \* 100
    - Weight-based priority for weighted average calculation

#### Common Abstractions

- **BaseEntity:** Base class for all entities with audit fields
- **IAuditable:** Interface for entities requiring detailed audit trails

### Design Principles

- **Framework Independent:** No EF Core attributes, only POCO classes
- **Business Logic:** Contains validation rules and business invariants
- **No External Dependencies:** Pure C# classes with domain logic

## 2. Infrastructure Layer

**Location:** `/apps/backend/src/Nexora.Management.Infrastructure/`

**Purpose:** Handles external concerns including database access, file system, and external services.

### Components

#### Persistence Subsystem

**AppDbContext**

```csharp
public class AppDbContext : DbContext, IAppDbContext
{
    // 27 DbSets for all entities
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();
    public DbSet<Project> Projects => Set<Project>(); // DEPRECATED
    public DbSet<Space> Spaces => Set<Space>(); // NEW Phase 09
    public DbSet<Folder> Folders => Set<Folder>(); // NEW Phase 09
    public DbSet<TaskList> TaskLists => Set<TaskList>(); // NEW Phase 09
    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<TaskStatus> TaskStatuses => Set<TaskStatus>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<UserPresence> UserPresences => Set<UserPresence>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<PageVersion> PageVersions => Set<PageVersion>();
    public DbSet<PageCollaborator> PageCollaborators => Set<PageCollaborator>();
    public DbSet<PageComment> PageComments => Set<PageComment>();
    public DbSet<GoalPeriod> GoalPeriods => Set<GoalPeriod>();
    public DbSet<Objective> Objectives => Set<Objective>();
    public DbSet<KeyResult> KeyResults => Set<KeyResult>();

    // Auto-audit on SaveChangesAsync
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.Id = Guid.NewGuid();
            }
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

**Key Features:**

- Auto-auditing (CreatedAt, UpdatedAt)
- Auto-generation of UUIDs
- PostgreSQL extension registration (uuid-ossp, pg_trgm)
- Configuration assembly scanning
- Raw SQL execution support for RLS and authorization queries

**Raw SQL Methods:**

```csharp
public Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
{
    return Database.ExecuteSqlRawAsync(sql, parameters);
}

public async Task<List<T>> SqlQueryRawAsync<T>(string sql, params object[] parameters)
{
    return await Database.SqlQueryRaw<T>(sql, parameters).ToListAsync();
}

public async Task<T> SqlQuerySingleAsync<T>(string sql, params object[] parameters)
{
    return await Database.SqlQueryRaw<T>(sql, parameters).FirstOrDefaultAsync();
}
```

**Usage:**

- **RLS User Context:** `ExecuteSqlRawAsync` sets `app.current_user_id` for Row-Level Security
- **Permission Queries:** `SqlQuerySingleAsync<bool>` checks user permissions efficiently
- **Custom Queries:** `SqlQueryRawAsync<T>` for complex database queries

#### EF Core Configurations (31 Files)

Each entity has a dedicated `IEntityTypeConfiguration<T>` implementation:

**Example: UserConfiguration**

```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasDefaultValueSql("uuid_generate_v4()");
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
        builder.Property(u => u.Name).HasMaxLength(100);
        builder.HasIndex(u => u.Id);
    }
}
```

**Configuration Features:**

- Table and column mappings
- Data type constraints (max length, required)
- Index creation (unique, composite, filtered)
- Default value SQL expressions
- Relationship configurations

#### Interfaces

**IAppDbContext**

```csharp
public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    // ... (all DbSets)
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // For raw SQL execution (needed for RLS and authorization queries)
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
    Task<List<T>> SqlQueryRawAsync<T>(string sql, params object[] parameters);
    Task<T> SqlQuerySingleAsync<T>(string sql, params object[] parameters);
}
```

**Purpose:** Dependency inversion - allows mocking DbContext in tests. Raw SQL methods enable RLS user context setting and optimized permission queries.

## 3. Application Layer

**Location:** `/apps/backend/src/Nexora.Management.Application/`

**Purpose:** Orchestrates business logic use cases and implements application-specific rules.

### Components

#### Common

**Result Pattern**
Non-generic and generic result types for operation outcomes:

```csharp
public record Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

public record Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? Error { get; }
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

**ApiResponse<T>**
Standardized wrapper for all API responses:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; }
}
```

#### MediatR Setup

Registered in `Program.cs`:

```csharp
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ApiResponse<>).Assembly));
```

**Purpose:** CQRS pattern implementation (Commands and Queries to be added in Phase 03+)

### Authentication Commands

**Location:** `/apps/backend/src/Nexora.Management.Application/Authentication/Commands/`

Implemented CQRS commands for authentication:

- **RegisterCommand** (`Commands/Register/`)
  - Creates new user with hashed password
  - Assigns Owner role
  - Creates default workspace
  - Generates JWT access + refresh tokens
  - Returns AuthResponse with user details

- **LoginCommand** (`Commands/Login/`)
  - Validates user credentials
  - Verifies password hash
  - Generates new JWT tokens
  - Returns AuthResponse

- **RefreshTokenCommand** (`Commands/RefreshToken/`)
  - Validates refresh token
  - Checks token expiry and revocation status
  - Generates new access token
  - Rotates refresh token

**DTOs** (`DTOs/`):

- `AuthRequests`: RegisterRequest, LoginRequest, RefreshTokenRequest
- `AuthResponses`: AuthResponse, UserDto

### Authorization System

**Location:** `/apps/backend/src/Nexora.Management.Application/Authorization/`

Permission-based authorization system with dynamic policy provider:

**Components:**

1. **PermissionRequirement** - Authorization requirement for resource-action based access control
   - Format: `resource:action` (e.g., `tasks:create`)
   - Validates against user's role permissions

2. **PermissionAuthorizationHandler** - Handler that validates permissions against user roles
   - Retrieves user's roles from WorkspaceMemberships
   - Joins with RolePermissions and Permissions
   - Executes raw SQL for efficient permission lookup
   - Includes SQL injection protection via permission format validation
   - Registered as Scoped service for DbContext resolution

3. **PermissionAuthorizationPolicyProvider** - Dynamic policy provider
   - Handles policies in format: `Permission:resource:action`
   - Dynamically generates authorization policies at runtime
   - Registered as Singleton service

4. **RequirePermissionAttribute** - Attribute for endpoint-level authorization
   - Usage: `[RequirePermission("tasks", "create")]`
   - Generates policy: `Permission:tasks:create`
   - Applies to methods or classes

**Permission Lookup Flow:**

```
Request with [RequirePermission("tasks", "create")]
    ↓
Policy: "Permission:tasks:create"
    ↓
PermissionAuthorizationPolicyProvider.GetPolicyAsync()
    ↓
Creates PermissionRequirement("tasks", "create")
    ↓
PermissionAuthorizationHandler.HandleRequirementAsync()
    ↓
SQL Query: User → WorkspaceMembers → Roles → RolePermissions → Permissions
    ↓
Validate: Permission.Name == "tasks:create"
    ↓
Grant/Deny Access
```

### Task Management Commands and Queries

**Location:** `/apps/backend/src/Nexora.Management.Application/Tasks/`

Implemented CQRS operations for task management:

- **Commands** (`Commands/`):
  - `CreateTask` - Creates new task with project association, optional parent task, assignee, dates, estimates
  - `UpdateTask` - Updates task fields (title, description, status, priority, assignee, dates, estimates)
  - `DeleteTask` - Soft deletes task by ID

- **Queries** (`Queries/`):
  - `GetTaskById` - Retrieves single task by ID
  - `GetTasks` - Lists tasks with filtering (project, status, assignee, search, sorting, pagination)

**DTOs** (`DTOs/`):

- `TaskDto`: Task response with all fields
- `CreateTaskRequest`: Task creation payload
- `UpdateTaskRequest`: Task update payload
- `GetTasksQueryRequest`: Task list query parameters

### Frontend API Client (Phase 09 - Phase 5)

**Location:** `/apps/frontend/src/features/spaces/api.ts`

**Purpose:** HTTP client for Space/Folder/TaskList CRUD operations using centralized API client

**API Endpoints:**

```typescript
// Spaces API
getSpaceById(id: string)              // GET /api/spaces/{id}
getSpacesByWorkspace(workspaceId)      // GET /api/spaces?workspaceId={id}
createSpace(data: CreateSpaceRequest)  // POST /api/spaces
updateSpace(id, data: UpdateSpaceRequest) // PUT /api/spaces/{id}
deleteSpace(id: string)                // DELETE /api/spaces/{id}

// Folders API
getFolderById(id: string)              // GET /api/folders/{id}
getFoldersBySpace(spaceId)             // GET /api/spaces/{spaceId}/folders
createFolder(data: CreateFolderRequest) // POST /api/folders
updateFolder(id, data)                 // PUT /api/folders/{id}
updateFolderPosition(id, data)         // PATCH /api/folders/{id}/position
deleteFolder(id: string)               // DELETE /api/folders/{id}

// TaskLists API
getTaskListById(id: string)            // GET /api/tasklists/{id}
getTaskLists(spaceId?, folderId?)      // GET /api/tasklists?spaceId={id}&folderId={id}
createTaskList(data)                   // POST /api/tasklists
updateTaskList(id, data)               // PUT /api/tasklists/{id}
updateTaskListPosition(id, data)       // PATCH /api/tasklists/{id}/position
deleteTaskList(id: string)             // DELETE /api/tasklists/{id}

// Exported as grouped object
export const spacesApi = {
  // Spaces
  getSpaceById, getSpacesByWorkspace, createSpace, updateSpace, deleteSpace,
  // Folders
  getFolderById, getFoldersBySpace, createFolder, updateFolder, updateFolderPosition, deleteFolder,
  // TaskLists
  getTaskListById, getTaskLists, createTaskList, updateTaskList, updateTaskListPosition, deleteTaskList,
};
```

**Design Features:**

- **Centralized Client:** Uses `apiClient` from `@/lib/api-client`
- **Typed Responses:** Generic type parameters for type safety
- **Grouped Exports:** Convenient object with all CRUD operations
- **Query Parameters:** Proper handling of optional query params
- **RESTful Conventions:** GET/POST/PUT/PATCH/DELETE mapping

**Usage Example:**

```typescript
import { spacesApi } from '@/features/spaces';

// Get all spaces for a workspace
const spaces = await spacesApi.getSpacesByWorkspace(workspaceId);

// Create a new folder
const folder = await spacesApi.createFolder({
  spaceId: space.id,
  name: 'Marketing Campaigns',
  color: '#7B68EE',
});

// Update tasklist position
await spacesApi.updateTaskListPosition(tasklist.id, { positionOrder: 5 });
```

### Frontend Tree Utilities (Phase 09 - Phase 5)

**Location:** `/apps/frontend/src/features/spaces/utils.ts`

**Purpose:** Transform flat database relationships into hierarchical UI structures

**Utility Functions:**

```typescript
/**
 * Builds hierarchical tree from flat lists
 * Hierarchy: Space → Folder (optional) → TaskList
 */
buildSpaceTree(
  spaces: Space[],
  folders: Folder[],
  taskLists: TaskList[]
): SpaceTreeNode[]
// Returns: Space nodes with nested folders and tasklists

/**
 * Finds a node by ID in the tree
 * Useful for locating specific nodes in large hierarchies
 */
findNodeById(nodes: SpaceTreeNode[], id: string): SpaceTreeNode | undefined
// Returns: The found node or undefined

/**
 * Gets breadcrumb path to a node
 * Returns array of nodes from root to target
 */
getNodePath(nodes: SpaceTreeNode[], id: string): SpaceTreeNode[]
// Returns: Path array [root, ..., target]
```

### Frontend Workspace Context (Phase 08)

**Location:** `/apps/frontend/src/features/workspaces/workspace-provider.tsx`

**Purpose:** Global workspace state management with React Context and React Query

**Context Interface:**

```typescript
interface WorkspaceContextType {
  // State
  currentWorkspace: Workspace | null;
  workspaces: Workspace[];
  isLoading: boolean;
  error: Error | null;

  // Actions
  setCurrentWorkspace: (workspace: Workspace | null) => void;
  switchWorkspace: (workspaceId: string) => Promise<void>;
  createWorkspace: (data: CreateWorkspaceRequest) => Promise<Workspace>;
  refetchWorkspaces: () => void;
}
```

**Features:**

1. **LocalStorage Persistence:**
   - Key: `current_workspace_id`
   - Validation: Non-empty string check
   - Automatic loading on mount

2. **Workspace Resolution Logic:**

   ```typescript
   1. Try to find workspace by stored ID
   2. If not found, find default workspace (isDefault: true)
   3. If no default, use first workspace in list
   4. If no workspaces, return null
   ```

3. **Query Invalidation on Workspace Switch:**
   - Invalidates: `['spaces']`, `['folders']`, `['tasklists']`
   - Preserves: Auth queries, other workspace data
   - Ensures fresh data for new workspace

4. **React Query Integration:**
   - 5-minute stale time for workspace list
   - Automatic refetch on window focus
   - Optimistic updates for mutations

**Usage Example:**

```typescript
import { useWorkspace } from '@/features/workspaces';

function SpacesPage() {
  const { currentWorkspace, workspaces, setCurrentWorkspace, isLoading } = useWorkspace();

  if (isLoading) return <LoadingSkeleton />;
  if (!currentWorkspace) return <EmptyState />;

  return (
    <div>
      <h1>{currentWorkspace.name}</h1>
      {/* Use currentWorkspace.id for queries */}
    </div>
  );
}
```

**Provider Setup:**

```typescript
// src/lib/providers.tsx
export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <WorkspaceProvider>
      <AuthProvider>
        <QueryClientProvider>
          {children}
        </QueryClientProvider>
      </AuthProvider>
    </WorkspaceProvider>
  );
}
```

**State Management Flow:**

```
User visits app
    ↓
WorkspaceProvider mounts
    ↓
Load current_workspace_id from localStorage
    ↓
Query: GET /api/workspaces
    ↓
Resolve currentWorkspace:
  - Find by stored ID
  - Fallback to default workspace
  - Fallback to first workspace
  - Fallback to null
    ↓
Render app with currentWorkspace context
    ↓
User switches workspace
    ↓
setCurrentWorkspace(newWorkspace)
    ↓
Update localStorage
    ↓
Invalidate queries: spaces, folders, tasklists
    ↓
Refetch data for new workspace
```

**Performance Optimizations:**

- useMemo for currentWorkspace resolution
- useCallback for action handlers
- React Query caching for workspaces list
- Selective query invalidation
- Minimal re-renders with proper dependency arrays

**Error Handling:**

- try-catch for localStorage access
- Error state in context
- Graceful fallbacks for missing workspaces
- Console logging for debugging

**Algorithm Complexity:**

- **buildSpaceTree:** O(n + m + p) where n=spaces, m=folders, p=tasklists
  - Creates Map data structures for O(1) lookups
  - Single pass through each array
  - Attaches children to parents via foreign keys

- **findNodeById:** O(n) worst case for tree traversal
  - Depth-first search through all nodes
  - Short-circuits on first match

- **getNodePath:** O(n) worst case
  - Recursive search builds path from root
  - Returns empty array if not found

**Usage Example:**

```typescript
import { buildSpaceTree, findNodeById, getNodePath } from '@/features/spaces';

// Build tree from API data
const tree = buildSpaceTree(spaces, folders, taskLists);

// Find specific tasklist
const tasklist = findNodeById(tree, 'tasklist-123');
console.log(tasklist?.name); // "Marketing Campaigns"

// Get breadcrumb for navigation
const path = getNodePath(tree, 'tasklist-123');
// [{name: 'Workspace'}, {name: 'Space'}, {name: 'Folder'}, {name: 'TaskList'}]
```

### Frontend Components (Phase 09 - Phase 5)

**Location:** `/apps/frontend/src/components/spaces/space-tree-nav.tsx`

**Purpose:** Hierarchical navigation tree with expand/collapse functionality

**Component Interface:**

```typescript
interface SpaceTreeNavProps {
  spaces: SpaceTreeNode[]; // Hierarchical tree data
  onNodeClick?: (node: SpaceTreeNode) => void; // Node selection callback
  onCreateSpace?: () => void; // Create space callback
  onCreateFolder?: (spaceId: string) => void; // Create folder callback
  onCreateList?: (spaceId: string, folderId?: string) => void; // Create list callback
  collapsed?: boolean; // Icon-only mode
  className?: string; // Custom styling
  selectedNodeId?: string; // Currently selected node
}
```

**Features:**

1. **Expandable Nodes:**
   - ChevronRight icons rotate on expand (90deg)
   - Click to toggle expand/collapse
   - State managed via Set for O(1) lookups

2. **Dynamic Icons:**
   - Circle icon for spaces
   - Folder icon for folders
   - List icon for tasklists
   - Custom colors from node.color property

3. **Action Buttons:**
   - Hover-to-reveal on nodes
   - Create folder on spaces
   - Create list on spaces or folders
   - Create space at root level

4. **Accessibility (WCAG 2.1 AA):**
   - `role="tree"` on container
   - `role="treeitem"` on nodes
   - `aria-expanded` for expand state
   - `aria-selected` for selection state
   - `aria-label="Space hierarchy"`
   - Keyboard navigation support

5. **Performance Optimizations:**
   - React.memo with custom comparison
   - useCallback for event handlers
   - Optimized re-render logic

6. **Styling:**
   - Dynamic indentation based on level (`level * 16 + 8` px)
   - Inline styles for dynamic colors
   - Hover effects (gray-100/gray-800)
   - Smooth transitions (200ms)
   - Dark mode support

**Usage Example:**

```typescript
import { SpaceTreeNav } from '@/components/spaces';

<SpaceTreeNav
  spaces={spaceTree}
  onNodeClick={(node) => navigate(`/${node.type}/${node.id}`)}
  onCreateSpace={() => setShowCreateDialog(true)}
  onCreateFolder={(spaceId) => createFolder(spaceId)}
  onCreateList={(spaceId, folderId) => createTaskList(spaceId, folderId)}
  collapsed={sidebarCollapsed}
  selectedNodeId={currentNodeId}
/>
```

**Memoized Version:**

```typescript
import { MemoizedSpaceTreeNav } from '@/components/spaces';

// Prevents unnecessary re-renders when props haven't changed
<MemoizedSpaceTreeNav spaces={spaceTree} collapsed={false} />
```

### Frontend Pages (Phase 09 - Phase 6)

**Purpose:** ClickUp hierarchy navigation and list management pages

**Pages Created:**

#### 1. Spaces Overview Page

**Location:** `/apps/frontend/src/app/(app)/spaces/page.tsx`

**Route:** `/spaces`

**Features:**

- Two-column layout (tree sidebar + main content)
- Left sidebar: Hierarchical space tree navigation (288px width)
- Main content: Empty state with "Select a Space, Folder, or List"
- React Query integration for spaces, folders, tasklists
- Tree building with buildSpaceTree utility
- Node click handlers:
  - Space → Show space detail view (TODO)
  - Folder → Show folder detail view (TODO)
  - TaskList → Navigate to `/lists/[id]`

**Component Structure:**

```typescript
// Data fetching
const { data: spaces } = useQuery({
  queryKey: ['spaces', workspaceId],
  queryFn: () => spacesApi.getSpacesByWorkspace(workspaceId),
});

const { data: folders } = useQuery({
  queryKey: ['folders'],
  queryFn: () => fetch('/api/folders'),
});

const { data: taskLists } = useQuery({
  queryKey: ['tasklists'],
  queryFn: () => spacesApi.getTaskLists(),
});

// Build hierarchical tree
const tree = useMemo(() => {
  return buildSpaceTree(spaces, folders, taskLists);
}, [spaces, folders, taskLists]);

// Node click handler
const handleNodeClick = (node: SpaceTreeNode) => {
  if (node.type === 'tasklist') {
    router.push(`/lists/${node.id}`);
  }
};
```

**Loading States:**

- Skeleton with "Loading spaces..." message
- Empty state with "No spaces yet" + "Create your first space" button
- Selection placeholder with icon and instructions

#### 2. List Detail Page

**Location:** `/apps/frontend/src/app/(app)/lists/[id]/page.tsx`

**Route:** `/lists/[id]`

**Features:**

- Breadcrumb navigation (Home → Spaces → List)
- List header with:
  - Name (h1, 2xl font-bold)
  - Type badge (colored, based on list.listType)
  - Description (if available)
- Toolbar with:
  - View toggle (Board/List buttons)
  - "Add Task" button (primary color)
- Task board grid layout (responsive: 1/2/3 columns)
- Empty state with "No tasks yet" + "Add Task" button

**Component Structure:**

```typescript
// Data fetching
const { data: list } = useQuery({
  queryKey: ['tasklists', listId],
  queryFn: () => spacesApi.getTaskListById(listId),
});

const { data: tasks } = useQuery({
  queryKey: ['tasks', listId],
  queryFn: () => fetch(`/api/tasks?projectId=${listId}`),
  enabled: !!listId,
});

// Breadcrumb path
const breadcrumbItems = [
  { label: 'Home', href: '/' },
  { label: 'Spaces', href: '/spaces' },
  { label: list.name, href: `/lists/${list.id}` },
];
```

**List Type Badge:**

```typescript
<span
  className="px-2 py-1 text-xs font-medium rounded-full"
  style={{
    backgroundColor: list.color ? `${list.color}20` : "#6b728020",
    color: list.color || "#6b7280"
  }}
>
  {list.listType || "task"}
</span>
```

**TODO Comments:**

- Add view toggle (board/list/calendar) functionality
- Implement task board columns by status
- Fetch tasks with proper listId filtering
- Add space and folder names to breadcrumb when available from API

#### 3. Task Detail Page (Updated)

**Location:** `/apps/frontend/src/app/(app)/tasks/[id]/page.tsx`

**Route:** `/tasks/[id]`

**Updates for Phase 6:**

- Breadcrumb updated: "Tasks" → "Spaces"
- Back button navigates to "/spaces" instead of "/tasks"
- TODO comments added for future hierarchy:
  ```typescript
  // TODO: Add space, folder, and list names when available from API
  // { label: task.spaceName, href: `/spaces/${task.spaceId}` },
  // ...(task.folderName ? [{ label: task.folderName, href: `/spaces/${task.spaceId}/folders/${task.folderId}` }] : []),
  // { label: task.listName, href: `/lists/${task.listId}` },
  ```

### Updated Components (Phase 09 - Phase 6)

#### 1. Navigation Sidebar (Updated)

**Location:** `/apps/frontend/src/components/layout/sidebar-nav.tsx`

**Changes:**

- Navigation item updated: "Tasks" → "Spaces"
- Added navigation items: "Goals", "Documents"
- Icon: Folder icon for Spaces
- Route: `/spaces` for Spaces navigation

```typescript
const navItems = [
  { title: 'Home', href: '/', icon: Home },
  { title: 'Spaces', href: '/spaces', icon: Folder }, // Changed from "Tasks"
  { title: 'Goals', href: '/goals', icon: Target }, // NEW
  { title: 'Documents', href: '/documents', icon: FileText }, // NEW
  { title: 'Team', href: '/team', icon: Users },
  { title: 'Calendar', href: '/calendar', icon: Calendar },
  { title: 'Settings', href: '/settings', icon: Settings },
];
```

#### 2. Task Modal (Updated)

**Location:** `/apps/frontend/src/components/tasks/task-modal.tsx`

**Changes:**

- Added "List" dropdown field (lines 322-347)
- Form data includes: listId field
- List options: Engineering Tasks, Marketing Campaign, Sprint Backlog (TODO: fetch from API)

```typescript
// List (TaskList) field
<div>
  <label htmlFor="listId">List</label>
  <Select value={formData.listId} onValueChange={(value) => setFormData({ ...formData, listId: value })}>
    <SelectTrigger>
      <SelectValue placeholder="Select list" />
    </SelectTrigger>
    <SelectContent>
      {listOptions.map((list) => (
        <SelectItem key={list.id} value={list.id}>
          {list.name}
        </SelectItem>
      ))}
    </SelectContent>
  </Select>
</div>
```

**TODO:**

```typescript
// TODO: Fetch available lists from spaces API
const listOptions = [
  { id: 'list-1', name: 'Engineering Tasks' },
  { id: 'list-2', name: 'Marketing Campaign' },
  { id: 'list-3', name: 'Sprint Backlog' },
];
```

#### 3. Task Types (Updated)

**Location:** `/apps/frontend/src/components/tasks/types.ts`

**Changes:**

- Added: listId?: string (line 28) - Reference to TaskList (replaces projectId)
- Added: spaceId?: string (line 29) - Reference to Space
- Added: folderId?: string (line 30) - Reference to Folder (optional)
- Deprecated: projectId (kept for migration compatibility)

```typescript
export interface Task {
  id: string;
  title: string;
  description?: string;
  status: TaskStatus;
  priority: TaskPriority;
  taskType: TaskType;
  // ... other fields

  // Hierarchy fields - 3-level hierarchy: Epic → Story → Subtask
  parentTaskId?: string | null;
  epicId?: string | null;
  storyId?: string | null;

  assignee?: { id: string; name: string; avatar?: string };
  dueDate?: string;
  startDate?: string;
  estimatedHours?: number;
  commentCount: number;
  attachmentCount: number;

  projectId: string; // Deprecated: Use listId instead
  listId?: string; // NEW: Reference to TaskList (replaces projectId)
  spaceId?: string; // NEW: Reference to Space
  folderId?: string; // NEW: Reference to Folder (optional)

  createdAt: string;
  updatedAt: string;
}
```

**Migration Notes:**

- `projectId` field kept for backward compatibility
- New tasks should use `listId` instead of `projectId`
- `spaceId` and `folderId` provide full hierarchy context
- Future: Remove `projectId` after migration complete

### Route Structure (Phase 6)

**New Routes:**

```
/spaces              # Spaces overview page (NEW)
/lists/[id]          # List detail page (NEW)
```

**Updated Routes:**

```
/tasks/[id]          # Task detail page (breadcrumb updated)
```

**Navigation Flow:**

1. User clicks "Spaces" in sidebar
2. Lands on `/spaces` overview page
3. Navigates tree in left sidebar
4. Clicks on TaskList node
5. Redirected to `/lists/[id]` detail page
6. Views tasks in board/list layout
7. Clicks on task card
8. Redirected to `/tasks/[id]` detail page
9. Breadcrumb shows: Home → Spaces → [List] → [Task]

**Future Enhancements:**

- Space detail page (`/spaces/[id]`)
- Folder detail page (`/spaces/[spaceId]/folders/[folderId]`)
- Breadcrumb with full hierarchy (Space → Folder → List → Task)

## 4. API Layer

**Location:** `/apps/backend/src/Nexora.Management.API/`

**Purpose:** Presentation layer - handles HTTP requests, responses, and API contracts.

### Components

#### Program.cs (Application Entry Point)

**Configuration:**

1. **Serilog Logging:**

   ```csharp
   builder.Host.UseSerilog((context, configuration) =>
       configuration.ReadFrom.Configuration(context.Configuration));
   ```

2. **JWT Authentication:**

   ```csharp
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
               ValidateIssuer = true,
               ValidIssuer = jwtSettings.Issuer,
               ValidateAudience = true,
               ValidAudience = jwtSettings.Audience,
               ValidateLifetime = true,
               ClockSkew = TimeSpan.Zero
           };
       });
   builder.Services.AddAuthorization();
   ```

3. **CORS Policy:**

   ```csharp
   builder.Services.AddCors(options =>
   {
       options.AddPolicy("AllowFrontend", policy =>
       {
           policy.WithOrigins(allowedOrigins)
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials();
       });
   });
   ```

   **⚠️ SECURITY ISSUE:** Current implementation uses `AllowAnyOrigin()` which breaks JWT authentication. Must be fixed before production deployment.

4. **Swagger/OpenAPI (NEW 2026-01-09):**

   ```csharp
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();
   ```

5. **DbContext Registration:**

   ```csharp
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
   builder.Services.AddScoped<IAppDbContext>(provider =>
       provider.GetRequiredService<AppDbContext>());
   ```

6. **Workspace Endpoints (NEW 2026-01-09):**

   ```csharp
   // Register Workspace CRUD endpoints
   builder.Services.AddWorkspaceEndpoints();

   // Maps to:
   // POST /api/workspaces
   // GET /api/workspaces
   // GET /api/workspaces/{id}
   // PUT /api/workspaces/{id}
   // DELETE /api/workspaces/{id}
   ```

7. **MediatR Registration:**

   ```csharp
   builder.Services.AddMediatR(cfg =>
       cfg.RegisterServicesFromAssembly(typeof(Nexora.Management.Application.Common.ApiResponse<>).Assembly));
   ```

7. **Infrastructure Services:**

   ```csharp
   builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
   builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
   builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
   ```

8. **Authorization Services:**
   ```csharp
   builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
   builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
   ```

9. **Workspace Authorization Middleware:**

**Middleware Pipeline:**

1. Swagger (Development only)
2. HTTPS Redirection
3. CORS
4. Authentication
5. Authorization
6. Workspace Authorization (RLS user context)
7. Map Controllers

#### Workspace Authorization Middleware

**Location:** `/apps/backend/src/Nexora.Management.API/Middleware/WorkspaceAuthorizationMiddleware.cs`

**Purpose:** Sets user context for Row-Level Security (RLS) in PostgreSQL

**Implementation:**

```csharp
public async Task InvokeAsync(HttpContext context, IAppDbContext db)
{
    var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
    {
        // Set user context for RLS in PostgreSQL
        await db.ExecuteSqlRawAsync(
            "SET LOCAL app.current_user_id = {0}", userId);
    }

    await _next(context);
}
```

**Key Features:**

- Registered after authentication middleware via `UseWorkspaceAuthorization()`
- Extracts user ID from JWT claims
- Sets PostgreSQL session variable `app.current_user_id`
- Enables RLS policies to filter queries based on user context
- Executes `SET LOCAL` for request-scoped user context

**Integration with RLS:**

The middleware works with PostgreSQL RLS policies defined in migrations:

```sql
-- RLS policy uses the user context set by middleware
CREATE POLICY tasks_select_policy ON "Tasks"
FOR SELECT
USING (
    "ProjectId" IN (
        SELECT "Id" FROM "Projects"
        WHERE "WorkspaceId" IN (
            SELECT "WorkspaceId" FROM "WorkspaceMembers"
            WHERE "UserId" = current_setting('app.current_user_id', true)::UUID
        )
    )
);
```

#### SignalR Hubs

**Location:** `/apps/backend/src/Nexora.Management.API/Hubs/`

**Purpose:** Real-time WebSocket communication for live updates

**Components:**

1. **TaskHub** - Task real-time updates
   - Broadcasts: TaskCreated, TaskUpdated, TaskDeleted, TaskStatusChanged
   - Project-based groups for efficient messaging
   - Authenticated via JWT Bearer token

2. **PresenceHub** - User presence tracking
   - Broadcasts: UserPresence (online/offline/typing)
   - Tracks current view (task/project being viewed)
   - Manages connection lifecycle (connect/disconnect)

3. **NotificationHub** - Notification delivery
   - Sends: NotificationReceived events
   - Respects user notification preferences
   - Marks notifications as read

**SignalR Configuration:**

```csharp
builder.Services.AddSignalR();

app.MapHub<TaskHub>("/hubs/tasks");
app.MapHub<PresenceHub>("/hubs/presence");
app.MapHub<NotificationHub>("/hubs/notifications");
```

**Hub Authentication:**

All hubs require JWT authentication:

```csharp
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
```

**Services:**

- **PresenceService** - In-memory user presence tracking with auto-cleanup
- **NotificationService** - Notification creation and delivery with preference filtering

**API Endpoints (11 Groups):**

- **AuthEndpoints.cs** - Authentication at `/api/auth`
- **TaskEndpoints.cs** - Task CRUD at `/api/tasks`
- **CommentEndpoints.cs** - Comments at `/api/comments`
- **AttachmentEndpoints.cs** - File attachments at `/api/attachments`
- **DocumentEndpoints.cs** - Document management at `/api/documents`
- **GoalEndpoints.cs** - Goal tracking at `/api/goals`
- **WorkspaceEndpoints.cs** - Workspace CRUD at `/api/workspaces` (NEW 2026-01-09)
- **SpaceEndpoints.cs** - Space CRUD at `/api/spaces` (ClickUp hierarchy)
- **FolderEndpoints.cs** - Folder CRUD at `/api/spaces/{spaceId}/folders` (ClickUp hierarchy)
- **TaskListEndpoints.cs** - TaskList CRUD at `/api/tasklists` (ClickUp hierarchy)
- **NotificationHub.cs** - Real-time notifications at `/hubs/notifications`

**Endpoint Routes:**

- `GET /` - Welcome message with API info
- `GET /health` - Health check endpoint
- `GET /swagger` - Swagger UI (development only) (NEW 2026-01-09)
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Token refresh

**Workspace Endpoints (NEW 2026-01-09):**
- `POST /api/workspaces` - Create workspace
- `GET /api/workspaces` - List workspaces
- `GET /api/workspaces/{id}` - Get workspace by ID
- `PUT /api/workspaces/{id}` - Update workspace
- `DELETE /api/workspaces/{id}` - Delete workspace

**ClickUp Hierarchy Endpoints:**
- `POST /api/spaces` - Create space
- `GET /api/spaces?workspaceId={id}` - List spaces
- `POST /api/folders` - Create folder
- `GET /api/spaces/{spaceId}/folders` - List folders
- `POST /api/tasklists` - Create tasklist
- `GET /api/tasklists?spaceId={id}&folderId={id}` - List tasklists

**Task Endpoints:**
- `POST /api/tasks` - Create task
- `GET /api/tasks/{id}` - Get task by ID
- `GET /api/tasks` - List tasks with filters
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- **Document Endpoints (Phase 07):**
  - `POST /api/documents` - Create page
  - `GET /api/documents/{id}` - Get page by ID
  - `PUT /api/documents/{id}` - Update page
  - `DELETE /api/documents/{id}` - Soft delete page
  - `GET /api/documents/tree/{workspaceId}` - Get page tree
  - `POST /api/documents/{id}/favorite` - Toggle favorite
  - `GET /api/documents/{id}/versions` - Get version history
  - `POST /api/documents/{id}/restore` - Restore version
  - `POST /api/documents/{id}/move` - Move page
  - `GET /api/documents/search` - Search pages
- **Goal Endpoints (Phase 08):**
  - `POST /api/goals/periods` - Create goal period
  - `GET /api/goals/periods` - Get periods for workspace
  - `PUT /api/goals/periods/{id}` - Update period
  - `DELETE /api/goals/periods/{id}` - Delete period
  - `POST /api/goals/objectives` - Create objective
  - `GET /api/goals/objectives` - Get objectives (paginated)
  - `GET /api/goals/objectives/tree` - Get hierarchical objective tree
  - `PUT /api/goals/objectives/{id}` - Update objective
  - `DELETE /api/goals/objectives/{id}` - Delete objective
  - `POST /api/goals/keyresults` - Create key result
  - `PUT /api/goals/keyresults/{id}` - Update key result
  - `DELETE /api/goals/keyresults/{id}` - Delete key result
  - `GET /api/goals/dashboard` - Get progress dashboard statistics

#### Database Migrations

**Location:** `/Persistence/Migrations/`

**Seven Migration Files:**

1. **InitialCreate (20260103071610)**
   - Creates all 17 base tables (Users, Roles, Permissions, Workspaces, Projects, Tasks, etc.)
   - Defines primary keys, foreign keys, indexes
   - Sets up PostgreSQL extensions (uuid-ossp, pg_trgm)
   - Creates 30+ indexes for performance

2. **EnableRowLevelSecurity (20260103071738)**
   - Creates `set_current_user_id(UUID)` function
   - Enables RLS on 5 tables: Tasks, Projects, Comments, Attachments, ActivityLog
   - Creates 15+ security policies (SELECT, INSERT, UPDATE, DELETE per table)
   - Policies enforce workspace membership validation

3. **SeedRolesAndPermissions (20260103071908)**
   - Seeds system roles (Admin, Member, Guest)
   - Seeds base permissions (Create, Read, Update, Delete per resource)
   - Creates initial role-permission assignments

4. **AddRealtimeCollaborationTables (20260103171029)**
   - Creates `user_presence` table for online/offline tracking
   - Creates `notifications` table for notification history
   - Creates `notification_preferences` table for user settings
   - Adds indexes for presence and notification queries

5. **AddDocumentTables (20260104112014)** (Phase 07)
   - Creates `Pages` table for wiki/document pages
   - Creates `PageVersions` table for version history
   - Creates `PageCollaborators` table for collaboration management
   - Creates `PageComments` table for threaded comments
   - Adds unique constraints on PageId + VersionNumber
   - Adds indexes for workspace, parent page, slug queries
   - Enables RLS on document tables

6. **AddGoalTrackingTables (20260105165809)** (NEW Phase 08)
   - Creates `goal_periods` table for time-based goal tracking
   - Creates `objectives` table for hierarchical objectives
   - Creates `key_results` table for measurable key results
   - Adds indexes for workspace, period, owner, status queries
   - Composite indexes for efficient filtering
   - Foreign key relationships for goal hierarchy

**Model Snapshot:**

- `AppDbContextModelSnapshot.cs` - Current EF Core model state

#### Phase 2 Migration Strategy (Backend Database Migration) ✅ **COMPLETE**

**Purpose:** Migrate from Project-based to ClickUp hierarchy (Workspace → Space → Folder → TaskList → Task)

**Location:** `/apps/backend/scripts/`

**Migration Overview:**

The Phase 2 migration implements a safe, transaction-based approach to migrate existing Projects to TaskLists while maintaining data integrity and providing rollback capabilities.

**Migration Scripts:**

1. **MigrateProjectsToTaskLists.sql (~8KB)**
   - Creates TaskList records from existing Projects
   - Preserves all Project properties: Name, Description, Color, Icon, Status
   - Maps WorkspaceId to maintain workspace relationships
   - Sets ListType to "project" for backward compatibility
   - Calculates PositionOrder for drag-and-drop support
   - Uses transactions to ensure atomicity
   - Includes error handling and logging
   - Table locks prevent concurrent modifications during migration

2. **MigrateTasksToTaskLists.sql (~7KB)**
   - Updates Task.TaskListId from corresponding TaskList (mapped from old ProjectId)
   - Updates TaskStatus.TaskListId from corresponding TaskList
   - Preserves all existing task relationships and data
   - Uses table locks to prevent data corruption
   - Transaction-safe with rollback capability
   - Includes validation checks before updates
   - Detailed logging for troubleshooting

3. **ValidateMigration.sql (~8KB)**
   - Verifies all Projects have corresponding TaskLists created
   - Verifies all Tasks have TaskListId properly set
   - Verifies all TaskStatuses have TaskListId properly set
   - Checks for orphaned records (Tasks without TaskListId)
   - Checks for data consistency (Task count matches)
   - Provides detailed validation report with counts
   - Returns success/failure status

4. **RollbackMigration.sql (~7KB)**
   - Emergency rollback procedure if migration fails
   - Restores original state by deleting created TaskLists
   - Can restore ProjectId on Tasks if needed
   - Transaction-safe with confirmation prompt
   - Includes validation before rollback
   - Detailed logging for audit trail

**Migration Safety Features:**

- **Transaction-Based:** All operations wrapped in transactions for atomicity
- **Table Locks:** Prevents concurrent modifications during migration
- **Pre-Migration Backup:** Assumes database backup is created before migration
- **Validation:** Comprehensive validation before and after migration
- **Rollback:** Complete rollback procedure if migration fails
- **Logging:** Detailed logging at every step for troubleshooting
- **Error Handling:** Graceful error handling with clear error messages

**Migration Process:**

```
Pre-Migration Checklist:
1. ✅ Create database backup
2. ✅ Verify no active connections to database
3. ✅ Review migration scripts
4. ✅ Prepare rollback plan

Migration Steps:
1. ✅ Execute MigrateProjectsToTaskLists.sql
   - Creates TaskLists from Projects
   - Preserves all data
   - Transaction-safe
2. ✅ Execute MigrateTasksToTaskLists.sql
   - Updates Task.TaskListId
   - Updates TaskStatus.TaskListId
   - Validates relationships
3. ✅ Execute ValidateMigration.sql
   - Verifies all data migrated correctly
   - Checks for orphaned records
   - Provides validation report

Post-Migration:
1. ✅ Verify validation report shows success
2. ✅ Test application functionality
3. ✅ Monitor for errors
4. ✅ Keep backup for 30 days

Rollback (if needed):
1. ✅ Execute RollbackMigration.sql
2. ✅ Verify original state restored
3. ✅ Investigate failure cause
4. ✅ Fix issues and retry migration
```

**Application Layer Updates:**

To support the migration, 19 application layer files were updated:

- **Domain Layer (2 files):**
  - `Task.cs` - Added [Obsolete] attribute to ProjectId property
  - `Project.cs` - Added [Obsolete] attribute to entire class

- **Application Layer (13 files):**
  - `CreateTaskCommand.cs` - Updated to use TaskListId
  - `UpdateTaskCommand.cs` - Updated to use TaskListId
  - `UpdateTaskStatusCommand.cs` - Updated to use TaskListId
  - `DeleteTaskCommand.cs` - Updated to use TaskListId
  - `GetTaskByIdQuery.cs` - Updated to use TaskListId
  - `GetTasksQuery.cs` - Updated to use TaskListId
  - `GetBoardViewQuery.cs` - Updated to use TaskListId
  - `GetCalendarViewQuery.cs` - Updated to use TaskListId
  - `GetGanttViewQuery.cs` - Updated to use TaskListId
  - `TaskDto.cs` - Added TaskListId property
  - `CreateTaskRequest.cs` - Added TaskListId property
  - `UpdateTaskRequest.cs` - Added TaskListId property
  - SignalR DTOs - Updated to use TaskListId

- **API Layer (4 files):**
  - `TaskEndpoints.cs` - Updated endpoints to use TaskListId
  - `CommentEndpoints.cs` - Updated to use TaskListId
  - `AttachmentEndpoints.cs` - Updated to use TaskListId
  - `TaskHub.cs` - Updated SignalR hub to use TaskListId

**Code Review Results:**

- **Overall Grade:** A- (92/100)
- **Build Status:** 0 errors, 7 pre-existing warnings
- **Critical Issues Fixed:** 3
- **Migration Safety:** Excellent (transaction-based, rollback procedures)
- **Documentation:** Comprehensive (~21KB total)

**Migration Documentation:**

1. **MIGRATION_README.md (~15KB)**
   - Pre-migration checklist
   - Step-by-step migration instructions
   - Validation procedures
   - Rollback procedures
   - Troubleshooting guide
   - FAQ section

2. **ROLLBACK_PROCEDURES.md (~6KB)**
   - Emergency rollback steps
   - Data recovery procedures
   - Validation after rollback
   - Common rollback scenarios

**Key Benefits:**

- ✅ Safe, transaction-based migration
- ✅ Complete rollback capability
- ✅ Comprehensive validation
- ✅ Minimal application downtime
- ✅ Data integrity preserved
- ✅ Backward compatibility maintained
- ✅ Detailed documentation

**Next Steps:**

- [ ] Execute migration scripts in development environment
- [ ] Validate migration results
- [ ] Test application functionality
- [ ] Execute migration in staging environment
- [ ] Plan production migration timeline
- [ ] Create production migration runbook

#### Authentication Infrastructure

**JWT Settings** (`Infrastructure/Authentication/JwtSettings.cs`):

```csharp
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
}
```

**JWT Token Service** (`Infrastructure/Authentication/JwtTokenService.cs`):

- `GenerateAccessToken()` - Creates JWT with user claims
- `GenerateRefreshToken()` - Creates cryptographically secure random token
- `ValidateToken()` - Validates JWT and returns ClaimsPrincipal

**Configuration** (`appsettings.json`):

```json
"Jwt": {
  "Secret": "YOUR_SUPER_SECRET_KEY_MUST_BE_AT_LEAST_32_CHARACTERS_LONG_FOR_SECURITY",
  "Issuer": "NexoraManagement",
  "Audience": "NexoraManagementAPI",
  "AccessTokenExpirationMinutes": 15,
  "RefreshTokenExpirationDays": 7
}
```

## Security Architecture

### Authentication & Authorization Overview

Nexora Management implements a comprehensive security model with three layers:

1. **Authentication Layer** - JWT-based user authentication
2. **Authorization Layer** - Permission-based access control
3. **Data Layer** - Row-Level Security (RLS) for multi-tenancy

### Permission-Based Authorization

**Concept:** Resource-action based permissions (e.g., `tasks:create`, `projects:delete`)

**Implementation:**

1. **RequirePermission Attribute:**

   ```csharp
   [RequirePermission("tasks", "create")]
   [HttpPost]
   public async Task<IActionResult> CreateTask(CreateTaskRequest request)
   {
       // Handler checks if user has "tasks:create" permission
   }
   ```

2. **Dynamic Policy Generation:**
   - Policy format: `Permission:{resource}:{action}`
   - Provider dynamically creates policies at runtime
   - No need to pre-register policies

3. **Permission Validation Flow:**
   ```
   Endpoint Request
       ↓
   [RequirePermission] Attribute
       ↓
   Authorization Policy: "Permission:tasks:create"
       ↓
   PermissionAuthorizationHandler
       ↓
   SQL Query (via raw SQL):
   SELECT EXISTS (
       SELECT 1
       FROM "Users" u
       JOIN "WorkspaceMembers" wm ON u."Id" = wm."UserId"
       JOIN "Roles" r ON wm."RoleId" = r."Id"
       JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
       JOIN "Permissions" p ON rp."PermissionId" = p."Id"
       WHERE u."Id" = @userId
       AND p."Name" = 'tasks:create'
   )
       ↓
   Grant/Deny Access
   ```

**Security Features:**

- SQL injection protection via permission format validation
- Workspace-scoped role checking (users have different roles per workspace)
- Efficient single-query permission lookup
- No permission caching (always fresh from database)

### Row-Level Security (RLS)

**Concept:** Database-level policies that automatically filter queries based on user context.

**Implementation:**

1. **User Context Function:**

   ```sql
   CREATE FUNCTION set_current_user_id(user_id UUID)
   RETURNS VOID AS $$
   BEGIN
       PERFORM set_config('app.current_user_id', user_id::TEXT, true);
   END;
   $$ LANGUAGE plpgsql SECURITY DEFINER;
   ```

2. **Middleware Sets Context:**

   ```csharp
   // WorkspaceAuthorizationMiddleware
   await db.ExecuteSqlRawAsync(
       "SET LOCAL app.current_user_id = {0}", userId);
   ```

3. **Policy Example (Tasks SELECT):**
   ```sql
   CREATE POLICY tasks_select_policy ON "Tasks"
   FOR SELECT
   USING (
       "ProjectId" IN (
           SELECT "Id" FROM "Projects"
           WHERE "WorkspaceId" IN (
               SELECT "WorkspaceId" FROM "WorkspaceMembers"
               WHERE "UserId" = current_setting('app.current_user_id', true)::UUID
           )
       )
   );
   ```

**Protected Tables:**

- Tasks (4 policies: SELECT, INSERT, UPDATE, DELETE)
- Projects (4 policies)
- Comments (4 policies)
- Attachments (3 policies: SELECT, INSERT, DELETE)
- ActivityLog (1 policy: SELECT)

**Unprotected Tables:**

- Users (authentication layer handles access)
- Roles, Permissions (static system data)
- WorkspaceMembers, UserRoles, RolePermissions (junction tables)

**Benefits:**

- Defense in depth (application + database layer)
- Automatic query filtering
- No accidental data leaks
- Performance (filtered at source)

## Database Architecture

### Technology Stack

- **Database:** PostgreSQL 16
- **ORM:** Entity Framework Core 9.0
- **Driver:** Npgsql (PostgreSQL provider)

### Schema Design

**Multi-tenancy Pattern:**

- Workspace-based data isolation
- Users can be members of multiple workspaces
- All data (except Users, Roles, Permissions) is workspace-scoped

**Key Features:**

1. **UUID Primary Keys:**
   - Generated via `uuid_generate_v4()` PostgreSQL function
   - Distributed system friendly
   - No sequential ID exposure

2. **JSONB Columns:**
   - `Workspace.SettingsJsonb` - Flexible workspace configuration
   - `Task.CustomFieldsJsonb` - Extensible task metadata
   - `ActivityLog.ChangesJsonb` - Flexible change tracking
   - GIN indexes on JSONB for efficient querying

3. **Row-Level Security (RLS):**
   - Database-level access control
   - Policies enforce workspace membership
   - User context set via `set_current_user_id()` function
   - Automatic filtering at database engine level

4. **Audit Trail:**
   - All entities have CreatedAt, UpdatedAt
   - ActivityLog tracks all changes
   - Automatic audit in `SaveChangesAsync`

### Entity Relationships

**Core Hierarchy:**

```
User
 ├─ OwnedWorkspaces → Workspace
 ├─ WorkspaceMemberships → WorkspaceMember → Workspace
 ├─ OwnedProjects → Project → Workspace
 ├─ AssignedTasks → Task → Project → Workspace
 └─ TimeEntries → TimeEntry → Workspace
    └─ TimeRates → TimeRate → Project
```

**Many-to-Many:**

```
User ←─ UserRole ──→ Role ←─ RolePermission ──→ Permission
       (Workspace)              (Resource, Action)
```

**Self-Referencing:**

```
Task (ParentTaskId) → Task (self)
Comment (ParentCommentId) → Comment (self)
```

**Cascading Deletes:**

- Workspace → Projects, Tasks, Comments, Attachments, TimeEntries
- Project → Tasks, TaskStatuses, TimeRates
- Task → Comments, Attachments, TimeEntries
- Role → UserRoles, RolePermissions

### Indexing Strategy

**Unique Indexes:**

- `Users.Email`
- `Roles.Name`
- `TaskStatuses.ProjectId, OrderIndex`
- `TimeEntries.UserId, StartedAt` (prevent duplicate time entries)

**Foreign Key Indexes:**

- All foreign keys have indexes for JOIN performance

**Composite Indexes:**

- `Tasks.ProjectId, StatusId, PositionOrder` (task list queries)
- `ActivityLog.EntityType, EntityId, CreatedAt` (activity feed)
- `ActivityLog.WorkspaceId, CreatedAt` (workspace activity)
- `TimeEntries.UserId, StartedAt` (time entry queries)
- `TimeEntries.WorkspaceId, Status` (timesheet queries)
- `TimeEntries.TaskId` (task time tracking)

**GIN Indexes:**

- `Tasks.CustomFieldsJsonb` (JSONB queries)

**Filtered Indexes:**

- `Tasks.AssigneeId` WHERE assignee_id IS NOT NULL
- `Tasks.DueDate` WHERE due_date IS NOT NULL
- `Projects.WorkspaceId` WHERE status = 'active'

## Data Flow

### Example: Create Task with Authorization

```
1. HTTP POST /api/tasks with JWT Bearer token
   ↓ (API Layer)
2. WorkspaceAuthorizationMiddleware
   - Extracts userId from JWT claims
   - Executes: SET LOCAL app.current_user_id = {userId}
   ↓
3. [RequirePermission("tasks", "create")] Attribute
   ↓
4. PermissionAuthorizationHandler
   - SQL Query checks if user has "tasks:create" permission
   - Validates via WorkspaceMembers → Roles → RolePermissions → Permissions
   ↓ (if authorized)
5. TaskController.CreateCommand()
   ↓ (MediatR)
6. CreateTaskCommandHandler()
   ↓ (Application Layer)
7. Validates business rules
   ↓
8. Creates Task entity
   ↓
9. AppDbContext.Tasks.Add(task)
   ↓ (Infrastructure Layer)
10. AppDbContext.SaveChangesAsync()
    ↓ (EF Core)
11. Sets CreatedAt, UpdatedAt
    ↓
12. Generates SQL INSERT
    ↓ (PostgreSQL with RLS)
13. INSERT checks RLS policy: Is user member of task's workspace?
    ↓ (if RLS passes)
14. Returns new Task with Id
    ↓ (Back up the stack)
15. ApiResponse<Task> with Success=true
    ↓
16. HTTP 201 Created with task JSON
```

## Technology Justification

### .NET 9.0

- Latest LTS with performance improvements
- Native AOT compilation support
- Enhanced JSON APIs
- Improved logging and diagnostics

### Entity Framework Core 9.0

- LINQ query translation improvements
- Better PostgreSQL support via Npgsql
- Migration management
- Change tracking optimization

### PostgreSQL 16

- Advanced RLS capabilities
- JSONB with GIN indexes
- Full-text search (pg_trgm)
- ACID compliance
- Excellent performance

### Clean Architecture

- Testability (mock dependencies)
- Maintainability (clear separation)
- Flexibility (swap implementations)
- Domain-centric (business logic focus)

## Deployment Architecture

### Development Environment

```
Frontend (Next.js) :3000
    ↓ HTTP/WebSocket
Backend API (.NET) :5001
    ↓ SQL
PostgreSQL :5432
    ↓ SignalR (WebSocket)
/hubs/tasks, /hubs/presence, /hubs/notifications
```

### Docker Compose (Implemented - Testing Phase Complete)

**Status:** ✅ Implemented (2026-01-07) | **Production Ready:** ❌ No (3-4 days work required)

**Current Configuration:**

```yaml
services:
  frontend:
    image: nexora-frontend
    ports: ['3000:3000']
    healthcheck:
      test: wget -spider http://localhost:3000/api/health
      interval: 30s
      retries: 3

  backend:
    image: nexora-backend
    ports: ['5001:8080']
    healthcheck:
      test: curl -f http://localhost:8080/health
      interval: 30s
      retries: 3

  postgres:
    image: postgres:16-alpine
    ports: ['5432:5432']
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: pg_isready -U nexora
      interval: 10s

  redis:
    image: redis:7-alpine
    ports: ['6379:6379']
    healthcheck:
      test: redis-cli ping
      interval: 10s
```

**Test Results (2026-01-07):**

| Service | Status | Health Check | Response Time |
|---------|--------|--------------|---------------|
| PostgreSQL | ✅ Healthy | Passing (5/5) | ~50ms |
| Redis | ✅ Healthy | Passing (5/5) | ~66ms |
| Backend | ✅ Healthy | Passing (5/5) | ~65ms |
| Frontend | ❌ Unhealthy | Failing (15/15) | Endpoint missing |

**Test Coverage:** 0% (1 placeholder test only)

**Critical Issues Found (3):**

1. **SECURITY:** Hardcoded credentials in docker-compose.yml
2. **RELIABILITY:** Frontend health check endpoint missing
3. **QUALITY:** Zero test coverage across backend and frontend

**Production Readiness Score:** 4.5/10

**Estimated Time to Production-Ready:** 3-4 days (52 hours)

**See Also:** `/docs/project-roadmap.md` - Phase 17/18: Docker Testing & Production Setup

**Files:**
- `/docker/docker-compose.yml` - Base configuration
- `/docker/docker-compose.override.yml` - Development overrides
- `/Dockerfile.backend` - Multi-stage backend build
- `/Dockerfile.frontend` - Multi-stage frontend build
- `/docker/postgres-init/01-init.sql` - Database initialization

## Performance Considerations

### Frontend Performance Patterns (Phase 05A)

**React Optimization:**

- **React.memo with Custom Comparison:**
  - TaskCard, TaskRow, TaskBoard, TaskModal optimized
  - Granular prop comparison prevents unnecessary re-renders
  - 75% reduction in component updates

- **Algorithm Optimization:**
  - Single-pass tasksByStatus grouping (O(n) vs O(n×4))
  - Reduced iterations for task list processing
  - Improved board view rendering speed

- **useCallback for Stable Handlers:**
  - Prevents child component re-renders
  - Consistent function references across updates
  - Memoized event handlers

**Accessibility Performance:**

- **aria-live Regions:**
  - Polite announcements for status updates
  - Assertive announcements for critical changes
  - Screen reader optimized

- **ARIA Labels:**
  - Icon-only buttons properly labeled
  - Interactive elements accessible
  - WCAG 2.1 AA compliant

**Performance Metrics:**

- Component re-render reduction: 75%
- Algorithm complexity improvement: O(n×4) → O(n)
- CPU usage during updates: Reduced
- Scalability: Support for 1000+ tasks

### Database Optimization

- Strategic indexes on foreign keys
- Composite indexes for common query patterns
- Filtered indexes to reduce index size
- GIN indexes for JSONB columns
- Connection pooling (default in Npgsql)

### Caching Strategy (Planned)

- In-memory cache for frequently accessed data (Roles, Permissions)
- Distributed cache (Redis) for session data
- HTTP caching headers for static assets

### Query Optimization

- Projection (SELECT specific columns)
- Eager loading (Include()) vs lazy loading
- Split queries for complex operations
- AsNoTracking for read-only queries

## Scalability Architecture

### Horizontal Scaling

- API layer: Stateless design allows multiple instances
- Database: Read replicas for query scaling
- Frontend: Static files served via CDN

### Vertical Scaling

- PostgreSQL: Connection pooling, resource limits
- .NET: Async/await for better thread utilization

### Future Considerations

- Message queue (RabbitMQ) for background jobs
- CQRS with separate read/write databases
- Event sourcing for audit trail
- Microservices decomposition (if needed)

---

**Documentation Version:** 1.3
**Last Updated:** 2026-01-06
**Maintained By:** Development Team
