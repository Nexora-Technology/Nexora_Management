# Codebase Summary

**Last Updated:** 2026-01-07
**Version:** Phase 09 Complete (ClickUp Hierarchy - Phases 6, 7, 8) + Phase 2 Backend Migration Complete
**Backend Files:** 181 files (4 migration scripts added)
**Frontend Files:** 117 TypeScript files (~13,029 lines)
**Phase 07 Status:** DEFERRED - Test requirements documented, code quality fixes complete
**Phase 2 Status:** ✅ COMPLETE - Backend Database Migration (4 scripts, 19 files updated)

## Documentation Section

### Component Documentation (Phase 05B)

- **JSDoc Coverage:** 5 public components with comprehensive documentation
  - `Button` - UI primitive with 6 variants, 4 sizes
  - `Input` - Form input with error state support
  - `Card` - Container component with header, content, footer
  - `TaskCard` - Board view task card with drag handle
  - `TaskModal` - Create/edit task dialog

- **Component Usage Guide:** `docs/component-usage.md`
  - Usage examples for all major components
  - Best practices and patterns
  - Accessibility guidelines
  - TypeScript type definitions

- **Route Group Layout:** `src/app/(app)/layout.tsx`
  - AppLayout wrapper for authenticated routes
  - Consistent sidebar across all application pages
  - Improved navigation structure

## Project Overview

Nexora Management is a ClickUp-inspired project management platform built with .NET 9.0 backend and Next.js 15 frontend. The platform provides comprehensive task management, collaboration, document management, and productivity tracking features.

## Technology Stack

### Backend

- **Framework:** .NET 9.0 / ASP.NET Core Web API
- **ORM:** Entity Framework Core 9.0
- **Database:** PostgreSQL 16
- **Language:** C# 12
- **Architecture:** Clean Architecture (Domain, Infrastructure, Application, API layers)
- **Real-time:** SignalR

### Frontend

- **Framework:** Next.js 15 (App Router)
- **Language:** TypeScript
- **Styling:** Tailwind CSS
- **Components:** shadcn/ui (16 components)
- **Rich Text:** TipTap (for document editor)
- **State Management:** Zustand
- **Data Fetching:** React Query (@tanstack/react-table)
- **Real-time:** @microsoft/signalr
- **Drag-Drop:** @dnd-kit

## Architecture

### Clean Architecture Layers

#### 1. Domain Layer (`/apps/backend/src/Nexora.Management.Domain/`)

**Purpose:** Core business logic and enterprise rules

**Components:**

- **Entities** (27 domain models):
  - `User` - User accounts and authentication
  - `Role` - User roles (Admin, Member, Guest)
  - `Permission` - Granular permissions (Create, Read, Update, Delete)
  - `UserRole` - Many-to-many relationship between Users and Roles
  - `RolePermission` - Many-to-many relationship between Roles and Permissions
  - `RefreshToken` - JWT refresh token storage
  - `Workspace` - Workspaces as top-level containers
  - `WorkspaceMember` - Workspace membership management
  - `Project` - Projects within workspaces (DEPRECATED - migrating to TaskList)
  - **NEW Phase 09:**
    - `Space` - First organizational level under Workspace (ClickUp hierarchy)
    - `Folder` - Optional grouping container for Lists within Spaces
    - `TaskList` - Mandatory container for Tasks (display name: "List")
  - `Task` - Tasks within task lists (TaskListId added, ProjectId deprecated)
  - `TaskStatus` - Custom task statuses per TaskList (TaskListId added, ProjectId deprecated)
  - `Comment` - Threaded comments on tasks
  - `Attachment` - File attachments for tasks
  - `ActivityLog` - Audit trail for all activities
  - `UserPresence` - Real-time user online/offline status
  - `Notification` - User notifications
  - `NotificationPreference` - User notification settings
  - **Phase 07:**
    - `Page` - Wiki/document pages with hierarchical structure
    - `PageVersion` - Page version history for restore capability
    - `PageCollaborator` - Page collaboration with role-based access
    - `PageComment` - Comments on document pages
  - **Phase 08:**
    - `GoalPeriod` - Time periods for goal tracking (e.g., Q1 2026, FY 2026)
    - `Objective` - Objectives with hierarchical structure and progress tracking
    - `KeyResult` - Measurable key results for objectives

- **Common:**
  - `BaseEntity` - Base entity with Id, CreatedAt, UpdatedAt
  - `IAuditable` - Audit interface

#### 2. Infrastructure Layer (`/apps/backend/src/Nexora.Management.Infrastructure/`)

**Purpose:** External concerns and data access

**Components:**

- **Persistence** (`/Persistence/`):
  - `AppDbContext` - EF Core DbContext with 27 DbSets
  - **Configurations** (31 EF Core configurations):
    - `UserConfiguration` - User entity mapping
    - `RoleConfiguration` - Role entity mapping
    - `PermissionConfiguration` - Permission entity mapping
    - `UserRoleConfiguration` - UserRole junction table
    - `RolePermissionConfiguration` - RolePermission junction table
    - `RefreshTokenConfiguration` - Refresh token storage
    - `WorkspaceConfiguration` - Workspace with JSONB settings
    - `WorkspaceMemberConfiguration` - Workspace membership
    - `ProjectConfiguration` - Project with color/icon/status (DEPRECATED)
    - **NEW Phase 09:**
      - `SpaceConfiguration` - Space entity mapping (ClickUp hierarchy)
      - `FolderConfiguration` - Folder entity mapping (optional grouping)
      - `TaskListConfiguration` - TaskList entity mapping (Lists)
    - `TaskConfiguration` - Task with TaskListId (ProjectId deprecated)
    - `TaskStatusConfiguration` - Custom statuses per TaskList (ProjectId deprecated)
    - `CommentConfiguration` - Threaded comments
    - `AttachmentConfiguration` - File attachments
    - `ActivityLogConfiguration` - Audit logging
    - `UserPresenceConfiguration` - User presence tracking
    - `NotificationConfiguration` - Notification storage
    - `NotificationPreferenceConfiguration` - User notification settings
    - **NEW Phase 07:**
      - `PageConfiguration` - Page with self-referencing hierarchy
      - `PageVersionConfiguration` - Version history with unique constraint
      - `PageCollaboratorConfiguration` - Composite key (PageId + UserId)
      - `PageCommentConfiguration` - Threaded comments on pages
    - **NEW Phase 08:**
      - `GoalEntitiesConfiguration` - Goal tracking entities (GoalPeriod, Objective, KeyResult)

- **Interfaces:**
  - `IAppDbContext` - Abstraction for DbContext

#### 3. Application Layer (`/apps/backend/src/Nexora.Management.Application/`)

**Purpose:** Application logic and use cases

**Components:**

- **Common:**
  - `Result` / `Result<T>` - Non-generic and generic result patterns for operation outcomes
  - `ApiResponse<T>` - Standardized API response wrapper
  - MediatR setup for CQRS pattern

**CQRS Modules Summary (78 files across 9 feature modules):**

- **Authentication:** 3 Commands, 3 DTOs (9 files)
- **Authorization:** 4 components (Handler, Provider, Attribute, Requirement) (4 files)
- **Tasks:** 4 Commands, 5 Queries, 5 DTOs (14 files)
- **Comments:** 3 Commands, 2 Queries, 3 DTOs (8 files)
- **Attachments:** 2 Commands, 1 Query, 3 DTOs (6 files)
- **Documents:** 6 Commands, 4 Queries, 7 DTOs (17 files) - Phase 07
- **Goals:** 9 Commands, 4 Queries, 9 DTOs (22 files) - Phase 08
- **Common:** Result patterns, ApiResponse, IUserContext, PagedResult (4 files)
- **SignalR:** Message DTOs for real-time (3 files)

- **Authentication:**
  - **Commands:** RegisterCommand, LoginCommand, RefreshTokenCommand
  - **DTOs:** RegisterRequest, LoginRequest, RefreshTokenRequest, AuthResponse, UserDto

- **Authorization:**
  - `PermissionAuthorizationHandler` - Validates permissions against user roles
  - `PermissionAuthorizationPolicyProvider` - Dynamic policy provider for permission-based authorization
  - `RequirePermissionAttribute` - Attribute for endpoint-level authorization
  - `PermissionRequirement` - Authorization requirement for resource-action based access control

- **Tasks:**
  - **Commands:** CreateTask, UpdateTask, DeleteTask, UpdateTaskStatus
  - **Queries:** GetTaskById, GetTasks (with filtering), GetBoardView, GetCalendarView, GetGanttView
  - **DTOs:** TaskDto, CreateTaskRequest, UpdateTaskRequest, GetTasksQueryRequest, ViewDTOs (BoardColumnDto, CalendarTaskDto, GanttTaskDto)

- **Comments:**
  - **Commands:** AddComment, UpdateComment, DeleteComment
  - **Queries:** GetComments, GetCommentReplies
  - **DTOs:** CommentDto, CreateCommentRequest, UpdateCommentRequest

- **Attachments:**
  - **Commands:** UploadAttachment, DeleteAttachment
  - **Queries:** GetAttachments
  - **DTOs:** AttachmentDto, UploadAttachmentResponse

- **Documents** (Phase 07):
  - **Commands:** CreatePage, UpdatePage, DeletePage, ToggleFavorite, MovePage, RestorePageVersion
  - **Queries:** GetPageById, GetPageTree, GetPageHistory, SearchPages
  - **DTOs:** PageDto, PageDetailDto, PageTreeNodeDto, PageVersionDto, PageCommentDto, CreatePageRequest, UpdatePageRequest, MovePageRequest, SearchPagesRequest

- **Goals** (NEW Phase 08):
  - **Commands:** CreatePeriod, UpdatePeriod, DeletePeriod, CreateObjective, UpdateObjective, DeleteObjective, CreateKeyResult, UpdateKeyResult, DeleteKeyResult
  - **Queries:** GetPeriods, GetObjectives, GetObjectiveTree, GetProgressDashboard
  - **DTOs:** GoalPeriodDto, ObjectiveDto, KeyResultDto, ObjectiveTreeNodeDto, ProgressDashboardDto, StatusBreakdownDto, ObjectiveSummaryDto, PagedResult<T>

#### 4. API Layer (`/apps/backend/src/Nexora.Management.API/`)

**Purpose:** Presentation and external interfaces

**Components:**

- **Endpoints:** (6 Minimal API groups)
  - `AuthEndpoints.cs` - Authentication endpoints at `/api/auth`
  - `TaskEndpoints.cs` - Task CRUD endpoints at `/api/tasks`
  - `CommentEndpoints.cs` - Comment endpoints at `/api/comments`
  - `AttachmentEndpoints.cs` - File attachment endpoints at `/api/attachments`
  - `DocumentEndpoints.cs` - Document endpoints at `/api/documents` (Phase 07)
  - `GoalEndpoints.cs` - Goal tracking endpoints at `/api/goals` (NEW Phase 08)

- **SignalR Hubs:** (3 real-time hubs)
  - `TaskHub` - Task real-time updates (created, updated, deleted, status changed)
  - `NotificationHub` - Notification delivery with preference filtering
  - `PresenceHub` - User presence tracking (online/offline, typing, current view)

- **Middleware:**
  - `WorkspaceAuthorizationMiddleware.cs` - Sets user context for Row-Level Security (RLS)

- **Program.cs** - Application entry point
  - Serilog configuration
  - JWT Authentication setup
  - Authorization with dynamic policy provider
  - CORS setup
  - DbContext registration
  - Swagger/OpenAPI setup
  - Workspace Authorization Middleware registration
  - Health check endpoint
  - Welcome endpoint

- **Persistence/Migrations** (7 migration files):
  - `20260103071610_InitialCreate` - Initial schema creation
  - `20260103071738_EnableRowLevelSecurity` - RLS policies
  - `20260103071908_SeedRolesAndPermissions` - Initial data seeding
  - `20260103171029_AddRealtimeCollaborationTables` - Real-time features (presence, notifications)
  - `20260104112014_AddDocumentTables` (Phase 07) - Document/Wiki system tables
  - `20260105165809_AddGoalTrackingTables` (NEW Phase 08) - Goal tracking tables (GoalPeriod, Objective, KeyResult)
  - `AppDbContextModelSnapshot` - EF Core model snapshot

- **appsettings.json** - Configuration including connection string

- **Migration Scripts** (Phase 2 - Backend Database Migration) ✅ **COMPLETE**:
  - `/scripts/MigrateProjectsToTaskLists.sql` - Projects → TaskLists migration script (~8KB)
    - Creates TaskLists from existing Projects
    - Preserves all Project properties (name, description, color, icon, status)
    - Maps WorkspaceId, sets ListType to "project"
    - Handles PositionOrder for drag-and-drop
  - `/scripts/MigrateTasksToTaskLists.sql` - Tasks.ProjectId → TaskListId migration (~7KB)
    - Updates Task.TaskListId from corresponding TaskList
    - Updates TaskStatus.TaskListId from corresponding TaskList
    - Preserves all existing task relationships
    - Uses table locks to prevent concurrent modifications
  - `/scripts/ValidateMigration.sql` - Post-migration validation script (~8KB)
    - Verifies all Projects have corresponding TaskLists
    - Verifies all Tasks have TaskListId set
    - Verifies all TaskStatuses have TaskListId set
    - Checks for orphaned records
    - Provides detailed validation report
  - `/scripts/RollbackMigration.sql` - Emergency rollback script (~7KB)
    - Restores ProjectId on Tasks (if needed)
    - Deletes created TaskLists
    - Restores original state
    - Transaction-safe with rollback confirmation
  - **Documentation** (Phase 2):
    - `/docs/migration/MIGRATION_README.md` - Comprehensive migration guide (~15KB)
      - Step-by-step migration instructions
      - Pre-migration checklist
      - Validation procedures
      - Rollback procedures
      - Troubleshooting guide
    - `/docs/migration/ROLLBACK_PROCEDURES.md` - Detailed rollback procedures (~6KB)
      - Emergency rollback steps
      - Data recovery procedures
      - Validation after rollback

## Database Schema

### Entity Relationships

```
User (1) ────< (N) UserRole >─── (N) Role
                     │
                     └───── (N) Workspace
                              │
                              ├───── (N) WorkspaceMember ──── (1) Role
                              ├───── (N) Project
                              │         │
                              │         ├───── (N) TaskStatus
                              │         │
                              │         └───── (N) Task
                              │                   │
                              │                   ├───── (1) TaskStatus
                              │                   ├───── (N) Comment
                              │                   └───── (N) Attachment
                              │
                              ├───── (N) Page ──── (1) ParentPage (self-ref)
                              │         │
                              │         ├───── (N) PageVersion
                              │         ├───── (N) PageComment ──── (1) ParentComment (self-ref)
                              │         └───── (N) PageCollaborator ──── (1) User
                              │
                              ├───── (N) GoalPeriod ──── (1) Workspace
                              │         │
                              │         └───── (N) Objective ──── (1) ParentObjective (self-ref)
                              │                   │
                              │                   ├───── (N) KeyResult
                              │                   └───── (N) Owner ──── (1) User
                              │
                              ├───── (N) ActivityLog
                              ├───── (N) Notification ──── (1) User
                              └───── (N) UserPresence ──── (1) User

Role (1) ────< (N) RolePermission >─── (N) Permission
```

### Key Features

1. **Multi-tenancy via Workspaces:**
   - Workspace-centric data isolation
   - User membership in multiple workspaces
   - Role-based access per workspace

2. **ClickUp Hierarchy (NEW Phase 09):**
   - Workspace → Space → Folder (optional) → TaskList → Task
   - Spaces organize work by departments, teams, or clients
   - Folders provide optional grouping for related Lists
   - TaskLists are mandatory containers for Tasks
   - Migrating from Project-based to TaskList-based organization
   - Position ordering for drag-and-drop at all levels

3. **Hierarchical Task Management:**
   - Workspace → Project → Task hierarchy (DEPRECATED - migrating to TaskList)
   - Task nesting (parent-child relationships)
   - Custom statuses per task list
   - Position ordering for drag-and-drop

4. **Goal Tracking & OKRs (Phase 08):**
   - GoalPeriod for time-based goal tracking (Q1, FY, etc.)
   - Objective with hierarchical structure (parent-child relationships)
   - KeyResult with measurable metrics (number, percentage, currency)
   - Weighted progress calculation (0-100%)
   - Status tracking (on-track, at-risk, off-track, completed)
   - Progress dashboard with statistics

5. **Row-Level Security (RLS):**
   - Applied to Tasks, Projects, Comments, Attachments, ActivityLog
   - Policies enforce workspace membership
   - User context set via `set_current_user_id()` function
   - Automatic filtering at database level

6. **Audit Trail:**
   - ActivityLog tracks all entity changes
   - JSONB storage for flexible change tracking
   - Workspace and user association

### PostgreSQL Extensions

1. **uuid-ossp:** UUID generation functions
2. **pg_trgm:** Trigram-based text search (for future search features)

### Database Constraints

- **Primary Keys:** All entities use UUID primary keys
- **Foreign Keys:** Cascade and restrict actions defined
- **Indexes:** Strategic indexes on:
  - Email (unique)
  - Workspace membership
  - Task relationships (project, status, assignee)
  - Activity logs (workspace, entity type/id)
  - Custom fields (GIN index for JSONB)

### Row-Level Security Policies

**Tasks Table:**

- SELECT/INSERT/UPDATE/DELETE policies based on workspace membership
- Validates access through Project → Workspace → WorkspaceMember

**Projects Table:**

- SELECT/INSERT/UPDATE/DELETE policies based on workspace membership

**Comments Table:**

- SELECT/INSERT based on task accessibility (via workspace membership)
- UPDATE/DELETE restricted to comment author

**Attachments Table:**

- SELECT/INSERT based on task accessibility
- DELETE restricted to attachment uploader

**ActivityLog Table:**

- SELECT policy allows access to workspace member activities

## File Structure

```
apps/backend/
├── src/
│   ├── Nexora.Management.Domain/
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs
│   │   │   └── IAuditable.cs
│   │   └── Entities/
│   │       ├── User.cs
│   │       ├── Role.cs
│   │       ├── Permission.cs
│   │       ├── UserRole.cs
│   │       ├── RolePermission.cs
│   │       ├── Workspace.cs
│   │       ├── WorkspaceMember.cs
│   │       ├── Project.cs
│   │       ├── Task.cs
│   │       ├── TaskStatus.cs
│   │       ├── Comment.cs
│   │       ├── Attachment.cs
│   │       └── ActivityLog.cs
│   │
│   ├── Nexora.Management.Infrastructure/
│   │   ├── Interfaces/
│   │   │   └── IAppDbContext.cs
│   │   ├── Services/
│   │   │   ├── IFileStorageService.cs
│   │   │   └── LocalFileStorageService.cs
│   │   └── Persistence/
│   │       ├── AppDbContext.cs
│   │       └── Configurations/
│   │           ├── UserConfiguration.cs
│   │           ├── RoleConfiguration.cs
│   │           ├── PermissionConfiguration.cs
│   │           ├── UserRoleConfiguration.cs
│   │           ├── RolePermissionConfiguration.cs
│   │           ├── WorkspaceConfiguration.cs
│   │           ├── WorkspaceMemberConfiguration.cs
│   │           ├── ProjectConfiguration.cs
│   │           ├── TaskConfiguration.cs
│   │           ├── TaskStatusConfiguration.cs
│   │           ├── CommentConfiguration.cs
│   │           ├── AttachmentConfiguration.cs
│   │           └── ActivityLogConfiguration.cs
│   │
│   ├── Nexora.Management.Application/
│   │   ├── Authentication/
│   │   │   ├── Commands/
│   │   │   │   ├── Register/
│   │   │   │   │   ├── RegisterCommand.cs
│   │   │   │   │   └── RegisterCommandHandler.cs
│   │   │   │   ├── Login/
│   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   └── LoginCommandHandler.cs
│   │   │   │   └── RefreshToken/
│   │   │   │       ├── RefreshTokenCommand.cs
│   │   │   │       └── RefreshTokenCommandHandler.cs
│   │   │   └── DTOs/
│   │   │       ├── AuthRequests.cs
│   │   │       └── AuthResponses.cs
│   │   ├── Authorization/
│   │   │   ├── PermissionAuthorizationHandler.cs
│   │   │   ├── PermissionAuthorizationPolicyProvider.cs
│   │   │   └── RequirePermissionAttribute.cs
│   │   ├── Common/
│   │   │   ├── ApiResponse.cs
│   │   │   ├── Result.cs
│   │   │   └── IUserContext.cs
│   │   ├── Tasks/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateTask/
│   │   │   │   ├── UpdateTask/
│   │   │   │   └── DeleteTask/
│   │   │   ├── Queries/
│   │   │   │   ├── GetTasks/
│   │   │   │   └── GetTaskById/
│   │   │   └── DTOs/
│   │   ├── Comments/
│   │   │   ├── Commands/
│   │   │   │   ├── AddComment/
│   │   │   │   ├── UpdateComment/
│   │   │   │   └── DeleteComment/
│   │   │   ├── Queries/
│   │   │   │   ├── GetComments/
│   │   │   │   └── GetCommentReplies/
│   │   │   └── DTOs/
│   │   └── Attachments/
│   │       ├── Commands/
│   │       │   ├── UploadAttachment/
│   │       │   └── DeleteAttachment/
│   │       ├── Queries/
│   │       │   └── GetAttachments/
│   │       └── DTOs/
│   │
│   └── Nexora.Management.API/
│       ├── Middleware/
│       │   └── WorkspaceAuthorizationMiddleware.cs
│       ├── Middlewares/
│       │   └── UserContext.cs
│       ├── Extensions/
│       │   └── AuthorizationExtensions.cs
│       ├── Endpoints/
│       │   ├── AuthEndpoints.cs
│       │   ├── TaskEndpoints.cs
│       │   ├── CommentEndpoints.cs
│       │   └── AttachmentEndpoints.cs
│       ├── Persistence/
│       │   └── Migrations/
│       │       ├── 20260103071610_InitialCreate.cs
│       │       ├── 20260103071610_InitialCreate.Designer.cs
│       │       ├── 20260103071738_EnableRowLevelSecurity.cs
│       │       ├── 20260103071738_EnableRowLevelSecurity.Designer.cs
│       │       ├── 20260103071908_SeedRolesAndPermissions.cs
│       │       ├── 20260103071908_SeedRolesAndPermissions.Designer.cs
│       │       └── AppDbContextModelSnapshot.cs
│       ├── appsettings.json
│       └── Program.cs
│
└── tests/ (to be implemented)
```

## Frontend Structure (~5,722 lines)

## ClickUp Design System (Phase 01.1 Foundation) ✅

**Status:** Complete (2026-01-04)
**Version:** ClickUp Purple v2.0
**Design Tokens:** 260+ lines in globals.css

### Design Token Architecture

#### Color System

**Primary Brand Colors (ClickUp Purple):**

```css
--primary: 250 73% 68%; /* #7B68EE - ClickUp Purple */
--primary-hover: 250 73% 76%; /* #A78BFA - Light Purple */
--primary-active: 250 62% 55%; /* #5B4BC4 - Dark Purple */
--primary-bg: 250 100% 97%; /* #F5F3FF - Purple Tint */
```

**Semantic Colors:**

- `--success: 158 64% 42%` - Green (#10B981)
- `--warning: 38 92% 50%` - Yellow (#F59E0B)
- `--error: 0 72% 51%` - Red (#EF4444)
- `--info: 217 91% 60%` - Blue (#3B82F6)

**Gray Scale (HSL format for consistent theming):**

```css
--gray-50: 220 20% 98%; /* Lightest */
--gray-100: 220 15% 95%;
--gray-200: 220 10% 90%;
--gray-300: 220 10% 75%;
--gray-400: 220 10% 60%;
--gray-500: 220 10% 45%;
--gray-600: 220 10% 35%;
--gray-700: 220 15% 25%;
--gray-800: 220 20% 15%;
--gray-900: 220 25% 10%; /* Darkest */
```

**Component Colors (shadcn/ui mapping):**

- Background: White (0 0% 100%)
- Foreground: Gray 900 (220 25% 10%)
- Border: Gray 200 (220 10% 90%)
- Muted: Gray 50 (220 20% 98%)
- Ring: Primary purple (250 73% 68%)

#### Typography System

**Font Families:**

- **Primary:** Inter (Google Fonts)
  - Subsets: latin, latin-ext, vietnamese
  - Display: swap
  - Variable: --font-inter
- **Monospace:** JetBrains Mono
  - Subsets: latin
  - Variable: --font-jetbrains-mono

**Type Scale (ClickUp-inspired):**

```css
--text-xs: 0.6875rem; /* 11px - Tiny/Timestamps */
--text-sm: 0.75rem; /* 12px - Small/Captions */
--text-base: 0.875rem; /* 14px - Body text */
--text-md: 1rem; /* 16px - H3/Subsection */
--text-lg: 1.25rem; /* 20px - H2/Card titles */
--text-xl: 1.5rem; /* 24px - H1/Section headers */
--text-2xl: 2rem; /* 32px - Display/Page titles */
```

**Font Weights:**

```css
--font-regular: 400; /* Body text */
--font-medium: 500; /* Emphasized text, labels */
--font-semibold: 600; /* Headings, important UI */
--font-bold: 700; /* Strong headings, CTAs */
```

**Line Heights:**

```css
--leading-tight: 1.25; /* Headings, compact text */
--leading-normal: 1.5; /* Body text */
```

#### Spacing System

**Base Unit:** 4px (powers of 2 scale)

```css
--space-0: 0;
--space-1: 0.25rem; /* 4px - Tight gaps */
--space-2: 0.5rem; /* 8px - Icon padding */
--space-3: 0.75rem; /* 12px - Compact padding */
--space-4: 1rem; /* 16px - Standard spacing */
--space-5: 1.25rem; /* 20px - Component gaps */
--space-6: 1.5rem; /* 24px - Section spacing */
--space-8: 2rem; /* 32px - Large gaps */
--space-10: 2.5rem; /* 40px - XL spacing */
--space-12: 3rem; /* 48px - XXL spacing */
--space-16: 4rem; /* 64px - Huge spacing */
```

#### Border Radius Scale

```css
--radius-sm: 4px; /* Small badges, dots */
--radius-md: 6px; /* Buttons, inputs (default) */
--radius-lg: 8px; /* Cards, panels */
--radius-xl: 12px; /* Modals, large cards */
--radius-2xl: 16px; /* Hero sections */
```

**Component-Specific Usage:**

- Buttons (Primary/Secondary/Icon): 6px (0.375rem)
- Buttons (Ghost): 4px (0.25rem)
- Task Cards: 8px (0.5rem)
- Status Badges: 4px (0.25rem)
- Inputs: 6px (0.375rem)
- Modals: 12px (0.75rem)

#### Shadow System

**Elevation Levels (5 levels):**

```css
--shadow-none: none;
--shadow-sm: 0 1px 2px rgba(0, 0, 0, 0.05);
--shadow-md: 0 1px 3px rgba(0, 0, 0, 0.1), 0 1px 2px rgba(0, 0, 0, 0.06);
--shadow-lg: 0 4px 6px rgba(0, 0, 0, 0.1), 0 2px 4px rgba(0, 0, 0, 0.06);
--shadow-xl: 0 10px 15px rgba(0, 0, 0, 0.1), 0 4px 6px rgba(0, 0, 0, 0.05);
--shadow-2xl: 0 20px 25px rgba(0, 0, 0, 0.15), 0 10px 10px rgba(0, 0, 0, 0.04);
```

**Component Usage:**

- Buttons: sm (subtle lift)
- Cards: sm → md on hover
- Dropdowns: md
- Modals: xl
- Tooltips: md

#### Transition System

**Durations:**

```css
--transition-fast: 150ms; /* Buttons, toggles */
--transition-base: 200ms; /* Dropdowns, sidebar */
--transition-slow: 300ms; /* Modals, page */
```

**Easing Functions:**

```css
--ease-out: cubic-bezier(0, 0, 0.2, 1); /* Deceleration */
--ease-in-out: cubic-bezier(0.4, 0, 0.2, 1); /* Smooth in/out */
```

#### Accessibility Features

**WCAG 2.1 AA Compliance:**

- Color contrast ratio: 4.7:1 (exceeds AA standard)
- Focus-visible states with 2px purple outline
- Reduced motion support via `@media (prefers-reduced-motion: reduce)`
- Semantic HTML structure
- Keyboard navigation support

**Focus Styles:**

```css
*:focus-visible {
  outline: 2px solid hsl(var(--primary));
  outline-offset: 2px;
  border-radius: var(--radius-sm);
}
```

**Reduced Motion:**

```css
@media (prefers-reduced-motion: reduce) {
  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    transition-duration: 0.01ms !important;
    scroll-behavior: auto !important;
  }
}
```

#### Dark Mode Support

**Primary Color Adjustment:**

```css
.dark {
  --primary: 250 73% 70%; /* #A78BFA - Lighter purple for dark mode */
  --background: 220 25% 10%; /* Gray 900 */
  --foreground: 220 20% 98%; /* Gray 50 */
  --border: 220 10% 35%; /* Gray 600 */
}
```

**Dark Mode Benefits:**

- Lighter purple (#A78BFA) provides better contrast on dark backgrounds
- All component colors automatically invert
- Maintains visual hierarchy
- Preserves brand identity

#### Component Utility Classes

**Button Classes:**

```css
.clickup-button-primary    /* Purple gradient with lift effect */
.clickup-button-secondary  /* White with border */
.clickup-button-ghost      /* Transparent with hover */
```

**Input Classes:**

```css
.clickup-input             /* 40px height, purple focus ring */
```

**Card Classes:**

```css
.clickup-card              /* White background, shadow on hover */
```

**Badge Classes:**

```css
.clickup-badge-complete    /* Green for success */
.clickup-badge-progress    /* Amber for in progress */
.clickup-badge-overdue     /* Red for overdue */
```

#### Tailwind Configuration

**Extended Theme:**

```typescript
colors: {
  primary: {
    DEFAULT: "hsl(var(--primary))",
    hover: "hsl(var(--primary-hover))",
    active: "hsl(var(--primary-active))",
  },
  gray: { 50: "hsl(var(--gray-50))", ..., 900: "hsl(var(--gray-900))" },
  success: "hsl(var(--success))",
  warning: "hsl(var(--warning))",
  error: "hsl(var(--error))",
  info: "hsl(var(--info))",
},
borderRadius: {
  DEFAULT: "var(--radius)", /* 6px */
  xl: "var(--radius-xl)",    /* 12px */
  "2xl": "var(--radius-2xl)", /* 16px */
},
fontFamily: {
  sans: ["var(--font-inter)", "sans-serif"],
  mono: ["var(--font-jetbrains-mono)", "monospace"],
},
fontSize: {
  xs: ["var(--text-xs)", { lineHeight: "var(--leading-tight)" }],
  sm: ["var(--text-sm)", { lineHeight: "var(--leading-normal)" }],
  // ... etc
},
boxShadow: {
  sm: "var(--shadow-sm)",
  md: "var(--shadow-md)",
  lg: "var(--shadow-lg)",
  xl: "var(--shadow-xl)",
  "2xl": "var(--shadow-2xl)",
},
transitionDuration: {
  fast: "var(--transition-fast)",   /* 150ms */
  base: "var(--transition-base)",   /* 200ms */
  slow: "var(--transition-slow)",   /* 300ms */
},
```

### Design Token Files

**Implementation Files:**

- `/apps/frontend/src/app/globals.css` - 260+ lines of ClickUp design tokens
- `/apps/frontend/tailwind.config.ts` - Tailwind extensions for ClickUp tokens
- `/apps/frontend/src/app/layout.tsx` - Inter font integration
- `/apps/frontend/next.config.ts` - typedRoutes experiment

**Documentation Files:**

- `/docs/design-guidelines.md` - Complete ClickUp design system reference (v2.0)
- `/docs/codebase-summary.md` - This file

### Design Token Usage

**CSS Variables (recommended for custom components):**

```css
.my-component {
  color: hsl(var(--foreground));
  background: hsl(var(--background));
  padding: var(--space-4);
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-sm);
  transition: all var(--transition-fast) var(--ease-out);
}
```

**Tailwind Classes (recommended for layout):**

```tsx
<div className="bg-white border border-gray-200 rounded-lg p-4 shadow-sm">
  <h2 className="text-xl font-semibold text-gray-900">Title</h2>
  <p className="text-base text-gray-600">Body text</p>
</div>
```

**Component Classes (recommended for consistent UI):**

```tsx
<button className="clickup-button-primary">Create Task</button>
<input className="clickup-input" placeholder="Enter task name..." />
<div className="clickup-card">Card content</div>
```

---

## ClickUp Layouts (Phase 03) ✅

**Status:** Complete (2026-01-05)
**Components:** 7 layout components
**Features:** Responsive design, dark mode support, collapsible sidebar

### Layout Component Library

#### 1. AppLayout Component (`src/components/layout/app-layout.tsx`)

**Purpose:** Main application wrapper providing consistent layout structure

**Props:**

```typescript
interface AppLayoutProps {
  children: React.ReactNode;
}
```

**Features:**

- Full-screen flex container (h-screen)
- Fixed header (56px)
- Collapsible sidebar (240px → 64px)
- Scrollable main content area
- Responsive background colors

**Layout Structure:**

```
AppLayout (flex-col, h-screen)
├── AppHeader (h-14, fixed)
└── div.flex-1 (flex row)
    ├── AppSidebar (w-60 → w-16 collapsed)
    └── main (flex-1, overflow-auto)
```

**Usage:**

```tsx
import { AppLayout } from '@/components/layout/app-layout';

export default function Layout({ children }: { children: React.ReactNode }) {
  return <AppLayout>{children}</AppLayout>;
}
```

#### 2. AppHeader Component (`src/components/layout/app-header.tsx`)

**Purpose:** Top navigation bar with logo, search, notifications, and profile

**Props:**

```typescript
interface AppHeaderProps {
  sidebarCollapsed: boolean;
  onToggleSidebar: () => void;
}
```

**Features:**

- 56px height (h-14)
- Logo with gradient background
- Global search bar (hidden on mobile)
- Notification bell icon
- Settings gear icon
- User avatar with initials
- Sidebar collapse button

**Sections:**

- **Left:** Menu toggle, Logo, Search
- **Right:** Notifications, Settings, Profile

**Responsive Behavior:**

- Search hidden on mobile (< 768px)
- All icons remain visible

**Usage:**

```tsx
<AppHeader sidebarCollapsed={collapsed} onToggleSidebar={() => setCollapsed(!collapsed)} />
```

#### 3. AppSidebar Component (`src/components/layout/app-sidebar.tsx`)

**Purpose:** Collapsible navigation sidebar

**Props:**

```typescript
interface AppSidebarProps {
  collapsed?: boolean;
}
```

**Features:**

- 240px expanded (w-60)
- 64px collapsed (w-16)
- Smooth 200ms transition
- White background with border
- Dark mode support
- Vertical scroll for overflow

**Dimensions:**

- Expanded: 240px
- Collapsed: 64px
- Transition: 200ms ease

**Usage:**

```tsx
<AppSidebar collapsed={collapsed} />
```

#### 4. SidebarNav Component (`src/components/layout/sidebar-nav.tsx`)

**Purpose:** Navigation items with active state highlighting

**Props:**

```typescript
interface SidebarNavProps {
  collapsed?: boolean;
}
```

**Features:**

- 6 navigation items (Home, Tasks, Projects, Team, Calendar, Settings)
- Active route highlighting
- Hover effects
- Chevron indicator for active item
- Icon-only mode when collapsed

**Navigation Items:**

```typescript
const navItems = [
  { title: 'Home', href: '/', icon: Home },
  { title: 'Tasks', href: '/tasks', icon: CheckSquare },
  { title: 'Projects', href: '/projects', icon: Folder },
  { title: 'Team', href: '/team', icon: Users },
  { title: 'Calendar', href: '/calendar', icon: Calendar },
  { title: 'Settings', href: '/settings', icon: Settings },
];
```

**Active State:**

- Purple background (bg-primary/10)
- Purple text (text-primary)
- Chevron right icon

**Usage:**

```tsx
<SidebarNav collapsed={collapsed} />
```

#### 5. Breadcrumb Component (`src/components/layout/breadcrumb.tsx`)

**Purpose:** Navigation path indicator with chevron separators

**Props:**

```typescript
interface BreadcrumbItem {
  label: string;
  href?: string; // Optional link
}

interface BreadcrumbProps {
  items: BreadcrumbItem[];
  className?: string;
}
```

**Features:**

- ChevronRight separators
- Links for clickable items
- Plain text for current page
- Hover effects on links
- ARIA label for accessibility

**Usage:**

```tsx
<Breadcrumb
  items={[
    { label: 'Home', href: '/' },
    { label: 'Tasks', href: '/tasks' },
    { label: 'Task Detail' },
  ]}
/>
```

**Styling:**

- Text color: gray-500
- Hover color: gray-900
- Dark mode: gray-400 → gray-200

#### 6. Container Component (`src/components/layout/container.tsx`)

**Purpose:** Responsive content container with max-width constraints

**Props:**

```typescript
interface ContainerProps {
  children: React.ReactNode;
  size?: 'sm' | 'md' | 'lg' | 'xl' | 'full';
  className?: string;
}
```

**Size Variants:**

```typescript
const sizeClasses = {
  sm: 'max-w-3xl', // 768px
  md: 'max-w-4xl', // 896px
  lg: 'max-w-6xl', // 1152px (ClickUp default)
  xl: 'max-w-7xl', // 1280px
  full: 'max-w-full',
};
```

**Responsive Padding:**

- Mobile: px-4 (16px)
- Tablet: sm:px-6 (24px)
- Desktop: lg:px-8 (32px)

**Usage:**

```tsx
<Container size="lg">
  <h1>Page Title</h1>
  <p>Content</p>
</Container>
```

#### 7. BoardLayout Component (`src/components/layout/board-layout.tsx`)

**Purpose:** Kanban board layout with horizontal scrolling columns

**BoardLayout Props:**

```typescript
interface BoardLayoutProps {
  children: React.ReactNode;
  className?: string;
}
```

**BoardColumn Props:**

```typescript
interface BoardColumnProps {
  title: string;
  count?: number;
  children: React.ReactNode;
  className?: string;
}
```

**Features:**

- Horizontal scroll container
- Snap scrolling (snap-x snap-mandatory)
- Column width: 280px
- Column gap: 24px (gap-6)
- Prevents column shrink
- Column count badge

**Usage:**

```tsx
<BoardLayout>
  <BoardColumn title="To Do" count={5}>
    {/* Task cards */}
  </BoardColumn>
  <BoardColumn title="In Progress" count={3}>
    {/* Task cards */}
  </BoardColumn>
  <BoardColumn title="Done" count={8}>
    {/* Task cards */}
  </BoardColumn>
</BoardLayout>
```

**Styling:**

- Gap between columns: 24px
- Column width: 280px (fixed)
- Scroll padding bottom: 16px
- Snap alignment: start

### Responsive Behavior

**Breakpoints:**

```css
--breakpoint-xs: 375px; /* Small mobile */
--breakpoint-sm: 640px; /* Mobile */
--breakpoint-md: 768px; /* Tablet */
--breakpoint-lg: 1024px; /* Desktop */
--breakpoint-xl: 1280px; /* Large desktop */
```

**Layout Adaptations:**

- **Mobile (< 768px):** Search hidden, sidebar defaults to collapsed
- **Tablet (768px - 1024px):** Search visible, sidebar toggleable
- **Desktop (> 1024px):** Full layout, all features visible

### Dark Mode Support

All layout components support dark mode with automatic color inversion:

- Background: white → gray-800
- Border: gray-200 → gray-700
- Text: gray-900 → white
- Hover states adjust accordingly

### Accessibility Features

**Semantic HTML:**

- `<header>` for app header
- `<nav>` for sidebar navigation
- `<main>` for content area
- `<aside>` for sidebar
- `<nav aria-label="Breadcrumb">` for breadcrumbs

**Keyboard Navigation:**

- Tab through navigation items
- Enter to activate links
- Escape to close (future enhancement)

**ARIA Labels:**

- Breadcrumb navigation label
- Icon buttons have clear labels
- Navigation structure is semantic

### Layout Component Files

```
apps/frontend/src/components/layout/
├── app-layout.tsx       # Main layout wrapper (35 lines)
├── app-header.tsx       # Top header (70 lines)
├── app-sidebar.tsx      # Collapsible sidebar (26 lines)
├── sidebar-nav.tsx      # Navigation items (89 lines)
├── breadcrumb.tsx       # Breadcrumb component (51 lines)
├── container.tsx        # Responsive container (37 lines)
├── board-layout.tsx     # Board layout (67 lines)
└── index.ts             # Public API exports (8 lines)
```

**Total:** 7 components, ~383 lines of code

---

## ClickUp View Components (Phase 04.1) ✅

**Status:** Complete (2026-01-05)
**Components:** 16 task components, 3 task pages, 4 UI primitives
**Dependencies:** @tanstack/react-table, @dnd-kit/core, @radix-ui/react-dialog, @radix-ui/react-select, @radix-ui/react-checkbox

## Performance & Accessibility (Phase 05A) ✅

**Status:** Complete (2026-01-05)
**Components:** 4 optimized task components
**Code Review:** 8.5/10
**Commit:** a145c08

### Performance Optimizations

**React.memo with Custom Comparison:**

4 components optimized with granular re-render control:

```typescript
// TaskCard - Compare task properties
export const TaskCard = memo(function TaskCard({ task, onClick, className }) {
  return <div onClick={onClick} className={className}>{task.title}</div>
}, (prevProps, nextProps) => {
  return (
    prevProps.task.id === nextProps.task.id &&
    prevProps.task.title === nextProps.task.title &&
    prevProps.task.status === nextProps.task.status &&
    prevProps.task.priority === nextProps.task.priority &&
    prevProps.onClick === nextProps.onClick &&
    prevProps.className === nextProps.className
  )
})

// TaskRow - Table row optimization
export const TaskRow = memo(function TaskRow({ task, selected, onSelectChange }) {
  return <TableRow>{/* ... */}</TableRow>
}, (prevProps, nextProps) => {
  return prevProps.task.id === nextProps.task.id &&
         prevProps.selected === nextProps.selected &&
         prevProps.onSelectChange === nextProps.onSelectChange
})

// TaskBoard - Array comparison with single-pass algorithm
export const TaskBoard = memo(function TaskBoard({ tasks, onTaskClick }) {
  // O(n) single-pass tasksByStatus grouping
  const tasksByStatus = useMemo(() => {
    const result = { todo: [], inProgress: [], complete: [], overdue: [] }
    for (const task of tasks) {
      if (result[task.status]) result[task.status].push(task)
    }
    return result
  }, [tasks])

  // useCallback for stable handlers
  const handleCardClick = useCallback((task) => {
    onTaskClick?.(task)
  }, [onTaskClick])
}, (prevProps, nextProps) => {
  return prevProps.tasks.length === nextProps.tasks.length &&
         prevProps.tasks.every((t, i) => t.id === nextProps.tasks[i]?.id) &&
         prevProps.tasks.every((t, i) => t.status === nextProps.tasks[i]?.status) &&
         prevProps.onTaskClick === nextProps.onTaskClick
})

// TaskModal - Dialog state optimization
export const TaskModal = memo(function TaskModal({ open, mode, onOpenChange }) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <div aria-live="assertive" aria-atomic="true" className="sr-only">
        {open ? (mode === "create" ? "Create task dialog opened" : "Edit task dialog opened") : ""}
      </div>
      {/* ... */}
    </Dialog>
  )
}, (prevProps, nextProps) => {
  return prevProps.open === nextProps.open &&
         prevProps.mode === nextProps.mode &&
         prevProps.onOpenChange === nextProps.onOpenChange
})
```

**Benefits:**

- 75% reduction in unnecessary re-renders
- O(n×4) → O(n) complexity for tasksByStatus
- Stable function references prevent child re-renders
- Granular control over update triggers

**Algorithm Optimization:**

Before (multiple passes):

```typescript
const tasksByStatus = {
  todo: tasks.filter((t) => t.status === 'todo'),
  inProgress: tasks.filter((t) => t.status === 'inProgress'),
  complete: tasks.filter((t) => t.status === 'complete'),
  overdue: tasks.filter((t) => t.status === 'overdue'),
};
// O(n×4) - 4 iterations through task array
```

After (single pass):

```typescript
const tasksByStatus = useMemo(() => {
  const result = { todo: [], inProgress: [], complete: [], overdue: [] };
  for (const task of tasks) {
    if (result[task.status]) result[task.status].push(task);
  }
  return result;
}, [tasks]);
// O(n) - 1 iteration through task array
```

**Performance Gains:**

- 75% reduction in iterations for task grouping
- Improved board view rendering speed
- Reduced CPU usage during state updates
- Better scalability for large task lists

### Accessibility Enhancements

**aria-live Regions:**

```typescript
// TaskBoard - Polite announcements for task count changes
export const TaskBoard = memo(function TaskBoard({ tasks }) {
  const totalTasks = useMemo(() => tasks.length, [tasks])

  return (
    <BoardLayout>
      <div aria-live="polite" aria-atomic="true" className="sr-only">
        {totalTasks} tasks loaded
      </div>
      {/* Board columns */}
    </BoardLayout>
  )
})

// TaskModal - Assertive announcements for critical state changes
export const TaskModal = memo(function TaskModal({ open, mode }) {
  return (
    <Dialog open={open}>
      <div aria-live="assertive" aria-atomic="true" className="sr-only">
        {open ? (mode === "create" ? "Create task dialog opened" : "Edit task dialog opened") : ""}
      </div>
      <DialogContent>{/* ... */}</DialogContent>
    </Dialog>
  )
})
```

**ARIA Labels:**

```typescript
// Close button
<button
  onClick={() => onOpenChange?.(false)}
  aria-label="Close dialog"
  className="close-button"
>
  <X className="h-5 w-5" />
</button>

// Drag handle
<button
  aria-label="Drag to reorder task"
  className="drag-handle"
  draggable
>
  <GripVertical className="h-4 w-4" />
</button>
```

**WCAG 2.1 AA Compliance:**

- ✅ aria-live regions for dynamic content
- ✅ aria-label for icon-only buttons
- ✅ aria-atomic="true" for complete announcements
- ✅ Polite vs assertive politeness levels
- ✅ Screen reader support verified
- ✅ Keyboard navigation maintained

**Accessibility Checklist:**

- [x] aria-live regions for status updates
- [x] ARIA labels for interactive elements
- [x] Semantic HTML structure
- [x] Keyboard navigation support
- [x] Screen reader announcements
- [x] Focus management
- [x] Color contrast ratios met

### Component Files Modified

```
apps/frontend/src/components/tasks/
├── task-card.tsx      # +React.memo with custom comparison
├── task-row.tsx       # +React.memo with custom comparison
├── task-board.tsx     # +React.memo, single-pass algorithm, useCallback, aria-live
└── task-modal.tsx     # +React.memo, aria-live, ARIA labels
```

**Total:** 4 files modified, ~50 lines added

### Task Components Library

#### 1. Task Data Model (`src/components/tasks/types.ts`)

**Purpose:** TypeScript interfaces for task management

**Interfaces:**

```typescript
interface Task {
  id: string;
  title: string;
  description?: string;
  status: 'todo' | 'inProgress' | 'complete' | 'overdue';
  priority: 'urgent' | 'high' | 'medium' | 'low';
  assignee?: {
    id: string;
    name: string;
    avatar?: string;
  };
  dueDate?: string;
  commentCount: number;
  attachmentCount: number;
  projectId: string;
  createdAt: string;
  updatedAt: string;
}

type TaskStatus = 'todo' | 'inProgress' | 'complete' | 'overdue';
type TaskPriority = 'urgent' | 'high' | 'medium' | 'low';

interface TaskFilter {
  status?: TaskStatus;
  priority?: TaskPriority;
  search?: string;
}
```

#### 1.1. Task Constants (`src/components/tasks/constants.ts`) - NEW Phase 05

**Purpose:** Shared constants for task management to ensure consistency

**Constants:**

```typescript
// Status options for filters and selects
export const TASK_STATUSES = [
  { value: 'todo', label: 'To Do' },
  { value: 'inProgress', label: 'In Progress' },
  { value: 'complete', label: 'Complete' },
  { value: 'overdue', label: 'Overdue' },
] as const;

// Priority levels
export const TASK_PRIORITIES = [
  { value: 'urgent', label: 'Urgent' },
  { value: 'high', label: 'High' },
  { value: 'medium', label: 'Medium' },
  { value: 'low', label: 'Low' },
] as const;

// Column configuration for board view
export const BOARD_COLUMNS = [
  { id: 'todo', title: 'To Do' },
  { id: 'inProgress', title: 'In Progress' },
  { id: 'complete', title: 'Complete' },
  { id: 'overdue', title: 'Overdue' },
] as const;
```

**Benefits:**

- Single source of truth for status/priority values
- Prevents typos and inconsistencies
- Easier to maintain and update
- Type-safe with `as const` assertions

#### 2. Mock Data (`src/components/tasks/mock-data.ts`)

**Purpose:** Sample tasks for development and testing

**Features:**

- 5 sample tasks with varied statuses and priorities
- Realistic assignee data with avatars
- Due dates and comment/attachment counts
- Covers all status types (todo, inProgress, complete, overdue)
- Covers all priority levels (urgent, high, medium, low)

#### 3. TaskCard Component (`src/components/tasks/task-card.tsx`)

**Purpose:** Board view task card with drag handle

**Features:**

- Drag handle icon for @dnd-kit integration
- Priority dot with color coding (urgent: red, high: orange, medium: yellow, low: blue)
- Status badge with semantic colors
- Assignee avatar with initials fallback
- Comment and attachment counts with icons
- Due date display with overdue highlighting
- Hover effect with shadow elevation

**Props:**

```typescript
interface TaskCardProps {
  task: Task;
}
```

**Usage:**

```tsx
<TaskCard task={task} />
```

#### 4. TaskToolbar Component (`src/components/tasks/task-toolbar.tsx`)

**Purpose:** Search, filter, and view toggle controls

**Features:**

- Search input with magnifying glass icon
- Status filter dropdown (All, To Do, In Progress, Complete, Overdue)
- Priority filter dropdown (All, Urgent, High, Medium, Low)
- View toggle buttons (List icon / Board icon)
- "Add Task" button with plus icon
- Responsive layout (filters hidden on mobile)

**Props:**

```typescript
interface TaskToolbarProps {
  view: 'list' | 'board';
  onViewChange: (view: 'list' | 'board') => void;
  onAddTask: () => void;
  filter: TaskFilter;
  onFilterChange: (filter: TaskFilter) => void;
}
```

#### 5. TaskBoard Component (`src/components/tasks/task-board.tsx`)

**Purpose:** Kanban board wrapper using BoardLayout

**Features:**

- 4 columns: To Do, In Progress, Complete, Overdue
- Drag-and-drop with @dnd-kit
- Task cards in each column
- Column count badges
- Horizontal scroll container
- Snap scrolling for smooth navigation

**Props:**

```typescript
interface TaskBoardProps {
  tasks: Task[];
}
```

**Column Configuration:**

```typescript
const columns = [
  { id: 'todo', title: 'To Do' },
  { id: 'inProgress', title: 'In Progress' },
  { id: 'complete', title: 'Complete' },
  { id: 'overdue', title: 'Overdue' },
];
```

#### 6. TaskRow Component (`src/components/tasks/task-row.tsx`)

**Purpose:** Table row for list view with checkbox

**Features:**

- Checkbox for multi-select
- Task title with truncation
- Status badge with semantic colors
- Priority dot with color coding
- Assignee avatar
- Due date with overdue highlighting
- Comment and attachment counts
- Hover state for visual feedback

**Props:**

```typescript
interface TaskRowProps {
  task: Task;
  selected: boolean;
  onSelectChange: (selected: boolean) => void;
}
```

#### 7. TaskModal Component (`src/components/tasks/task-modal.tsx`)

**Purpose:** Create/edit task modal with form validation

**Features:**

- Dialog component from Radix UI
- Form fields: Title, Description, Status, Priority, Assignee, Due Date
- Form validation with error messages
- Submit and cancel buttons
- Controlled inputs with state management
- Dark mode support

**Props:**

```typescript
interface TaskModalProps {
  task?: Task;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (task: Partial<Task>) => void;
}
```

**Form Validation:**

- Title: Required, max 200 characters
- Description: Optional, max 1000 characters
- Status: Required (todo, inProgress, complete, overdue)
- Priority: Required (urgent, high, medium, low)
- Due Date: Optional, ISO 8601 format

#### 8. Task Index (`src/components/tasks/index.ts`)

**Purpose:** Centralized exports for task components

**Exports:**

```typescript
export * from './types';
export * from './mock-data';
export { TaskCard } from './task-card';
export { TaskToolbar } from './task-toolbar';
export { TaskBoard } from './task-board';
export { TaskRow } from './task-row';
export { TaskModal } from './task-modal';
```

### Task Pages

#### 1. Tasks List Page (`src/app/tasks/page.tsx`)

**Route:** `/tasks`

**Purpose:** List view with table and multi-select

**Features:**

- TanStack Table for sorting and filtering
- Multi-select checkboxes for bulk operations
- TaskToolbar for search and filters
- TaskRow components for each task
- Pagination (to be implemented)
- Responsive table layout

**State Management:**

```typescript
const [view, setView] = useState<'list' | 'board'>('list');
const [filter, setFilter] = useState<TaskFilter>({});
const [selectedTasks, setSelectedTasks] = useState<Set<string>>(new Set());
```

#### 2. Tasks Board Page (`src/app/tasks/board/page.tsx`)

**Route:** `/tasks/board`

**Purpose:** Kanban board view with drag-and-drop

**Features:**

- TaskBoard component with columns
- Drag-and-drop with @dnd-kit
- TaskToolbar for search and filters
- Real-time task status updates
- Responsive board layout

**Drag-and-Drop:**

```typescript
const sensors = useSensors(
  useSensor(PointerSensor),
  useSensor(KeyboardSensor, {
    coordinateGetter: sortableKeyboardCoordinates,
  })
);
```

#### 3. Task Detail Page (`src/app/tasks/[id]/page.tsx`)

**Route:** `/tasks/[id]`

**Purpose:** Individual task detail with breadcrumb

**Features:**

- Breadcrumb navigation (Home > Tasks > Task Title)
- Task information display
- Assignee avatar and details
- Status and priority badges
- Comment section (to be implemented)
- Attachment section (to be implemented)
- Activity log (to be implemented)

**Breadcrumb:**

```typescript
<Breadcrumb
  items={[
    { label: "Home", href: "/" },
    { label: "Tasks", href: "/tasks" },
    { label: task.title },
  ]}
/>
```

### UI Primitives (Radix UI)

#### 1. Dialog Component (`src/components/ui/dialog.tsx`)

**Purpose:** Modal/Dialog primitives for overlays

**Exports:**

- Dialog - Root container
- DialogTrigger - Trigger button/link
- DialogPortal - Portal to body
- DialogClose - Close button
- DialogOverlay - Dimmed backdrop
- DialogContent - Modal content
- DialogHeader - Header section
- DialogFooter - Footer section
- DialogTitle - Title text
- DialogDescription - Description text

**Features:**

- Dismissible on escape key
- Dismissible on outside click
- Focus trap for accessibility
- Scrollable content when too tall
- Animation on open/close
- Dark mode support

**Usage:**

```tsx
<Dialog open={open} onOpenChange={setOpen}>
  <DialogContent className="sm:max-w-[425px]">
    <DialogHeader>
      <DialogTitle>Create Task</DialogTitle>
      <DialogDescription>Enter the task details below.</DialogDescription>
    </DialogHeader>
    {/* Form content */}
    <DialogFooter>
      <Button type="submit">Save</Button>
    </DialogFooter>
  </DialogContent>
</Dialog>
```

#### 2. Table Component (`src/components/ui/table.tsx`)

**Purpose:** Table components for list views

**Exports:**

- Table - Root table element
- TableHeader - Thead section
- TableBody - Tbody section
- TableFooter - Tfoot section
- TableRow - Tr element
- TableHead - Th element
- TableCell - Td element
- TableCaption - Caption element

**Features:**

- Semantic table structure
- Border bottom dividers
- Sticky header (optional)
- Hover state on rows
- Responsive width
- Dark mode support

**Usage:**

```tsx
<Table>
  <TableHeader>
    <TableRow>
      <TableHead>Title</TableHead>
      <TableHead>Status</TableHead>
      <TableHead>Priority</TableHead>
    </TableRow>
  </TableHeader>
  <TableBody>
    {tasks.map((task) => (
      <TableRow key={task.id}>
        <TableCell>{task.title}</TableCell>
        <TableCell>
          <Badge status={task.status}>{task.status}</Badge>
        </TableCell>
        <TableCell>
          <Badge priority={task.priority}>{task.priority}</Badge>
        </TableCell>
      </TableRow>
    ))}
  </TableBody>
</Table>
```

#### 3. Checkbox Component (`src/components/ui/checkbox.tsx`)

**Purpose:** Checkbox input with Check icon

**Features:**

- Radix UI Checkbox primitive
- Check icon from lucide-react
- Indeterminate state support
- Form integration
- Focus visible ring
- Dark mode support

**Props:**

```typescript
interface CheckboxProps extends React.ComponentPropsWithoutRef<typeof CheckboxPrimitive.Root> {
  asChild?: boolean;
}
```

**Usage:**

```tsx
<Checkbox id="terms" checked={checked} onCheckedChange={setChecked} />
<label htmlFor="terms">Accept terms and conditions</label>
```

#### 4. Select Component (`src/components/ui/select.tsx`)

**Purpose:** Select dropdown for single choice

**Exports:**

- Select - Root container
- SelectTrigger - Trigger button
- SelectValue - Display value
- SelectContent - Dropdown content
- SelectGroup - Option group
- SelectLabel - Group label
- SelectItem - Option item
- SelectSeparator - Visual separator
- SelectScrollUpButton - Scroll up button
- SelectScrollDownButton - Scroll down button

**Features:**

- Keyboard navigation
- Portal to body
- Animation on open/close
- Scrollable content
- Grouped options
- Search/filter (future)
- Dark mode support

**Usage:**

```tsx
<Select value={value} onValueChange={setValue}>
  <SelectTrigger>
    <SelectValue placeholder="Select status" />
  </SelectTrigger>
  <SelectContent>
    <SelectItem value="todo">To Do</SelectItem>
    <SelectItem value="inProgress">In Progress</SelectItem>
    <SelectItem value="complete">Complete</SelectItem>
    <SelectItem value="overdue">Overdue</SelectItem>
  </SelectContent>
</Select>
```

### Dependencies

**New in Phase 04.1:**

```json
{
  "@tanstack/react-table": "^8.11.2",
  "@dnd-kit/core": "^6.1.0",
  "@radix-ui/react-dialog": "^1.0.5",
  "@radix-ui/react-select": "^2.0.0",
  "@radix-ui/react-checkbox": "^1.0.4"
}
```

### Build Status

**TypeScript Compilation:** ✅ Passed
**Static Pages Generated:** 13 pages
**Bundle Size:** Optimized
**Code Splitting:** Enabled
**Tree Shaking:** Enabled

### Accessibility

**WCAG 2.1 Level AA:**

- Semantic HTML structure
- ARIA labels for icons
- Keyboard navigation support
- Focus visible indicators
- Screen reader support
- Color contrast ratios met

**Keyboard Shortcuts:**

- Tab: Navigate through interactive elements
- Enter/Space: Activate buttons and checkboxes
- Escape: Close modals and dropdowns
- Arrow keys: Navigate options in selects

### Component Files

```
apps/frontend/src/components/tasks/
├── types.ts           # Task interfaces (50 lines)
├── mock-data.ts       # Sample tasks (80 lines)
├── task-card.tsx      # Board card (60 lines)
├── task-toolbar.tsx   # Search/filter (120 lines)
├── task-board.tsx     # Kanban board (90 lines)
├── task-row.tsx       # Table row (80 lines)
├── task-modal.tsx     # Create/edit (150 lines)
└── index.ts           # Exports (10 lines)

apps/frontend/src/app/tasks/
├── page.tsx           # List view (100 lines)
├── board/
│   └── page.tsx       # Board view (80 lines)
└── [id]/
    └── page.tsx       # Task detail (70 lines)

apps/frontend/src/components/ui/
├── dialog.tsx         # Dialog primitives (110 lines)
├── table.tsx          # Table components (80 lines)
├── checkbox.tsx       # Checkbox input (40 lines)
└── select.tsx         # Select dropdown (100 lines)
```

**Total:** 16 components, 3 pages, ~1,100 lines of code

---

## ClickUp Components (Phase 02) ✅

**Status:** Complete (2026-01-04)
**Components:** 6 core component types
**Variants:** 20+ component variants using CVA (class-variance-authority)

### Component Library

All components use `class-variance-authority` for type-safe variant management and follow ClickUp's visual language.

#### 1. Button Component (`src/components/ui/button.tsx`)

**Variants:** 6 types

- `primary` - Purple background with shadow and scale transform on hover
- `secondary` - White background with 2px border
- `ghost` - Transparent background with gray hover
- `destructive` - Red for dangerous actions
- `outline` - Border only with hover fill
- `link` - Text-only with underline

**Sizes:** 4 options

- `sm` - 36px height, small text
- `md` - 40px height, standard (default)
- `lg` - 44px height, large text
- `icon` - 40px × 40px square

**Features:**

- Scale transform on hover (`hover:scale-[1.02]`)
- Active scale down (`active:scale-[0.98]`)
- Icon support via children
- Disabled state styling
- Full ref forwarding

**Usage:**

```tsx
<Button variant="primary" size="md" className="gap-2">
  <CheckCircle2 className="h-4 w-4" />
  Complete
</Button>
```

#### 2. Badge Component (`src/components/ui/badge.tsx`)

**Status Variants:** 5 types

- `complete` - Green (emerald-100/700)
- `inProgress` - Yellow (amber-100/700)
- `overdue` - Red (red-100/700)
- `neutral` - Gray (gray-100/700)
- `default` - Primary purple

**Sizes:** 3 options

- `sm` - 10px text, compact padding
- `md` - 12px text, standard (default)
- `lg` - 14px text, large padding

**Features:**

- Icon prop support
- Dark mode variants
- Semantic status naming

**Usage:**

```tsx
<Badge status="complete" icon={<CheckCircle2 className="h-3 w-3" />}>
  Complete
</Badge>
```

#### 3. Input Component (`src/components/ui/input.tsx`)

**Props:**

- `error?: boolean` - Red border and focus ring when true

**Styling:**

- 40px height
- 2px border (gray-200 → error red)
- Purple focus ring (`ring-primary/20`)
- Full width by default

**Features:**

- TypeScript extends HTML input attributes
- Error state styling
- Disabled state support
- Ref forwarding

**Usage:**

```tsx
<Input placeholder="Enter your name..." />
<Input error placeholder="This field has an error" />
```

#### 4. Textarea Component (`src/components/ui/textarea.tsx`)

**Props:**

- `error?: boolean` - Red border and focus ring when true

**Styling:**

- 80px minimum height
- 2px border (matches Input)
- Purple focus ring
- Vertical resize only
- Full width by default

**Features:**

- New component (created in Phase 02)
- Matches Input component styles
- Error state support

**Usage:**

```tsx
<Textarea placeholder="Enter a description..." />
<Textarea error placeholder="This field has an error" />
```

#### 5. Avatar Component (`src/components/ui/avatar.tsx`)

**Components:**

- `Avatar` - Root container
- `AvatarImage` - Image display
- `AvatarFallback` - Initials fallback

**Features:**

- Initials generation (first 2 characters)
- Hash-based color from 16-color palette
- Radix UI primitives for accessibility
- Name prop for automatic initials

**Color Palette:**

```tsx
const colors = [
  'bg-red-500',
  'bg-orange-500',
  'bg-amber-500',
  'bg-yellow-500',
  'bg-green-500',
  'bg-emerald-500',
  'bg-teal-500',
  'bg-cyan-500',
  'bg-sky-500',
  'bg-blue-500',
  'bg-indigo-500',
  'bg-violet-500',
  'bg-purple-500', // ClickUp purple
  'bg-fuchsia-500',
  'bg-pink-500',
  'bg-rose-500',
];
```

**Usage:**

```tsx
<Avatar>
  <AvatarImage src="https://..." />
  <AvatarFallback name="John Doe" /> {/* Shows "JD" */}
</Avatar>
```

#### 6. Tooltip Component (`src/components/ui/tooltip.tsx`)

**Components:**

- `Tooltip` - Provider with mouse events
- `TooltipTrigger` - Trigger element
- `TooltipContent` - Dark tooltip content

**Styling:**

- Dark theme (`bg-gray-900`, `text-white`)
- 200ms hover delay
- Zoom/fade animation
- Rounded corners

**Features:**

- Mouse enter/leave handling
- Auto-positioning
- Controlled/uncontrolled open state

**Usage:**

```tsx
<Tooltip delayDuration={200}>
  <TooltipTrigger asChild>
    <Button variant="outline">Hover me</Button>
  </TooltipTrigger>
  <TooltipContent>This is a helpful tooltip</TooltipContent>
</Tooltip>
```

### Component Showcase

**Location:** `/src/app/components/showcase/page.tsx`

Features:

- All component variants displayed
- Dark mode toggle
- Real-world task card example
- Form element states

**View locally:** `http://localhost:3000/components/showcase`

### Component Files

```
apps/frontend/src/components/ui/
├── button.tsx      # 6 variants, 4 sizes (110 lines)
├── badge.tsx       # 5 status variants, 3 sizes (55 lines)
├── input.tsx       # Error state support (35 lines)
├── textarea.tsx    # New component (34 lines)
├── avatar.tsx      # Initials + colors (92 lines)
└── tooltip.tsx     # Dark theme (109 lines)
```

### Dependencies

**New in Phase 02:**

```json
{
  "class-variance-authority": "^0.7.1",
  "@radix-ui/react-avatar": "^1.1.3"
}
```

### TypeScript Types

All components use strict TypeScript with:

- Forwarded refs (React.forwardRef)
- Extended HTML attributes
- CVA variant props (VariantProps)
- Proper displayName for debugging

### Tech Stack

- **Framework:** Next.js 15 with App Router
- **Language:** TypeScript
- **Styling:** Tailwind CSS v3.4.0
- **Components:** shadcn/ui (16 components)
- **Rich Text:** TipTap editor
- **State:** Zustand
- **Data Fetching:** React Query (@tanstack/react-table)
- **Real-time:** @microsoft/signalr
- **Drag-Drop:** @dnd-kit
- **UI Primitives:** @radix-ui (Dialog, Select, Checkbox)

### Route Pages (15 routes)

```
apps/frontend/src/app/
├── layout.tsx              # Root layout with providers
├── page.tsx                # Landing page
├── (auth)/
│   ├── login/
│   │   └── page.tsx        # Login page
│   ├── register/
│   │   └── page.tsx        # Register page
│   └── forgot-password/
│       └── page.tsx        # Forgot password page (Phase 04.1)
├── (app)/                  # Authenticated routes group
│   ├── dashboard/
│   │   └── page.tsx        # Dashboard page
│   ├── spaces/
│   │   └── page.tsx        # Spaces overview (NEW Phase 6)
│   ├── lists/
│   │   └── [id]/
│   │       └── page.tsx    # List detail with task board (NEW Phase 6)
│   ├── tasks/
│   │   ├── [id]/
│   │   │   └── page.tsx    # Task detail (updated Phase 6)
│   ├── goals/
│   │   ├── page.tsx        # Goals list
│   │   └── [id]/
│   │       └── page.tsx    # Goal detail
│   ├── documents/
│   │   └── page.tsx        # Documents page
│   ├── team/
│   │   └── page.tsx        # Team page
│   └── calendar/
│       └── page.tsx        # Calendar page
└── workspaces/
    └── page.tsx            # Workspaces list
```

### Feature Modules (5 modules)

#### 1. Auth Module

- `features/auth/auth-provider.tsx` - Authentication context
- `features/auth/LoginForm.tsx` - Login form
- `features/auth/RegisterForm.tsx` - Register form

#### 2. Tasks Module

- `features/tasks/TaskDetailWithRealtime.tsx` - Task detail with SignalR
- `features/tasks/TypingIndicator.tsx` - Typing animation
- `features/tasks/ViewingAvatars.tsx` - Who is viewing component

#### 3. Documents Module (7 components) - Phase 07

- `features/documents/DocumentEditor.tsx` - TipTap rich text editor
- `features/documents/EditorToolbar.tsx` - Formatting toolbar
- `features/documents/PageTree.tsx` - Hierarchical page tree
- `features/documents/PageList.tsx` - Page list with favorites/recent
- `features/documents/VersionHistory.tsx` - Version history viewer
- `features/documents/types.ts` - Document TypeScript types
- `features/documents/api.ts` - Document API client

#### 4. Notifications Module

- `features/notifications/NotificationCenter.tsx` - Notification panel
- `features/notifications/NotificationPreferences.tsx` - Settings UI

#### 5. Users Module

- `features/users/OnlineStatus.tsx` - Avatar with online indicator
- `features/users/UserAvatar.tsx` - User avatar component

#### 6. Views Module (4 views)

- `features/views/ViewContext.tsx` - View state context
- `features/views/ViewSwitcher.tsx` - View toggle buttons
- `features/views/list/ListView.tsx` - Sortable table view
- `features/views/board/BoardView.tsx` - Kanban board with drag-drop
- `features/views/calendar/CalendarView.tsx` - Monthly calendar
- `features/views/gantt/GanttView.tsx` - Timeline with zoom

#### 7. Spaces Module (NEW Phase 09 - Phase 5)

- `features/spaces/types.ts` - TypeScript types for Space, Folder, TaskList
- `features/spaces/api.ts` - API client for CRUD operations
- `features/spaces/utils.ts` - Tree building utilities (buildSpaceTree, findNodeById, getNodePath)
- `features/spaces/index.ts` - Public exports

#### 8. Workspaces Module (NEW Phase 08)

- `features/workspaces/types.ts` - Workspace and member TypeScript types
- `features/workspaces/api.ts` - Workspaces API client
- `features/workspaces/workspace-provider.tsx` - WorkspaceContext with current workspace state management
- `features/workspaces/index.ts` - Public exports

### SignalR Hooks (3 hooks)

```
src/hooks/signalr/
├── useTaskHub.ts           # Task real-time updates
├── usePresenceHub.ts       # User presence tracking
└── useNotificationHub.ts   # Notification delivery
```

### shadcn/ui Components (16)

```
src/components/ui/
├── avatar.tsx              # User avatar
├── badge.tsx               # Status badges
├── button.tsx              # Button component
├── card.tsx                # Card container
├── checkbox.tsx            # Checkbox input (NEW Phase 04.1)
├── dialog.tsx              # Modal/dialog (NEW Phase 04.1)
├── dropdown-menu.tsx       # Dropdown menus
├── input.tsx               # Form inputs
├── label.tsx               # Form labels
├── scroll-area.tsx         # Custom scrollbar
├── select.tsx              # Select dropdown (NEW Phase 04.1)
├── separator.tsx           # Visual separator
├── sonner.tsx              # Toast notifications
├── switch.tsx              # Toggle switch
├── table.tsx               # Table components (NEW Phase 04.1)
└── tooltip.tsx             # Tooltip component

src/components/spaces/ (NEW Phase 09 - Phase 5)
└── space-tree-nav.tsx      # Hierarchical navigation tree (192 lines)

src/components/workspaces/ (NEW Phase 08)
└── workspace-selector.tsx  # Workspace switching UI component (247 lines)
```

### Configuration Files

```
apps/frontend/
├── tailwind.config.ts      # Tailwind configuration
├── postcss.config.mjs      # PostCSS configuration
├── next.config.ts          # Next.js configuration
├── tsconfig.json           # TypeScript configuration
└── package.json            # Dependencies
```

## Configuration Files

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=nexora_management;Username=postgres;Password=postgres"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

## Key Design Decisions

1. **UUID Primary Keys:** Distributed system friendly, no sequential ID exposure
2. **JSONB Columns:** Flexible storage for settings and custom fields
3. **Row-Level Security:** Database-level access control for enhanced security
4. **Clean Architecture:** Separation of concerns for maintainability
5. **CQRS Pattern:** MediatR setup for command/query separation (to be expanded)
6. **Multi-tenancy:** Workspace-based data isolation

## Dependencies

### Backend

- `Microsoft.EntityFrameworkCore` (9.0.x)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.x)
- `MediatR` (12.x)
- `Serilog.AspNetCore` (8.x)

## Phase Completion Status

- [x] **Phase 01:** Project setup and architecture
- [x] **Phase 02:** Domain entities and database schema
- [x] **Phase 03:** Authentication & authorization
  - JWT-based authentication with access/refresh tokens
  - Permission-based authorization with dynamic policy provider
  - Workspace Authorization Middleware for RLS user context
  - Raw SQL execution methods for authorization queries
- [x] **Phase 04:** Task Management Core
  - Task CRUD operations with hierarchy support
  - Comments module with nested replies (max 5 levels)
  - Attachments module with security hardening
  - UserContext for authenticated user access
  - Security fixes: path traversal, file size limits, type validation
- [x] **Phase 04.1:** View Components (Task Management UI)
  - Task components: 8 components (types, mock-data, task-card, task-toolbar, task-board, task-row, task-modal, index)
  - Task pages: 3 pages (list view, board view, task detail)
  - UI primitives: 4 components (dialog, table, checkbox, select)
  - Dependencies: @tanstack/react-table, @dnd-kit/core, @radix-ui/\* (dialog, select, checkbox)
  - Features: List view with TanStack Table, Board view with drag-and-drop, Task detail with breadcrumb, Create/edit modal
  - Build Status: ✅ Passed (TypeScript compilation, 13 static pages)
- [x] **Phase 05A:** Performance & Accessibility
  - React.memo with custom comparison (4 components)
  - Single-pass algorithm for tasksByStatus (O(n) complexity)
  - useCallback for stable event handlers
  - aria-live regions (WCAG 2.1 AA compliant)
  - ARIA labels for interactive elements
  - Code Review: 8.5/10
  - Build Status: ✅ Passed
  - Commit: a145c08 (2026-01-05)
- [x] **Phase 05:** Multiple Views Implementation
  - ViewContext with localStorage persistence
  - ListView: Sortable table with expandable rows
  - BoardView: Kanban board with @dnd-kit drag-drop
  - CalendarView: Monthly calendar grid
  - GanttView: Timeline with zoom levels
  - Backend view-specific query handlers
  - UpdateTaskStatusCommand for drag-drop operations
- [x] **Phase 06:** Real-time Collaboration
  - SignalR hubs: TaskHub, PresenceHub, NotificationHub
  - Real-time task updates across all connected clients
  - User presence tracking (online/offline status)
  - Typing indicators for collaborative editing
  - Real-time notifications with toast integration
  - Notification preferences with per-event type toggles
  - Project-based messaging groups for efficiency
  - Auto-reconnect with graceful connection handling
- [x] **Phase 07:** Document & Wiki System
  - Document endpoints at `/api/documents`
  - Page tree with hierarchical structure
  - Version history with restore capability
  - Page collaboration with role-based access
- [x] **Phase 08:** Goal Tracking & OKRs
  - Goal tracking entities (GoalPeriod, Objective, KeyResult)
  - Goal endpoints at `/api/goals`
  - Frontend goals feature module with types and API client
  - Goal components (objective-card, key-result-editor, progress-dashboard, objective-tree)
  - Goals pages (/goals, /goals/[id])
  - Progress UI component
  - Weighted progress calculation
  - Hierarchical objective structure
- [x] **Phase 08:** Workspace Context and Auth Integration
  - Workspace feature module with types, API, and provider
  - WorkspaceContext with current workspace state management
  - WorkspaceSelector component for workspace switching
  - Workspace provider integration in app layout
  - Spaces page updated to use currentWorkspace from context
  - localStorage persistence for workspace selection
  - Code Review: A- (92/100) - 1 high priority fixed
- [⏸️] **Phase 07:** Testing and Validation **DEFERRED**
  - Status: DEFERRED - No test infrastructure available
  - Completed: TypeScript compilation fixes (0 errors), ESLint fixes (0 errors)
  - Completed: Build validation successful
  - Documented: Comprehensive test requirements (vitest, @testing-library/react, Playwright)
  - Documented: Manual testing checklist for critical workflows
  - Files fixed: breadcrumb.tsx, lists/[id]/page.tsx, tasks/[id]/page.tsx, spaces/page.tsx
  - Next Steps: Set up test infrastructure, then proceed with Phase 08
- [x] **Phase 09:** ClickUp Hierarchy Implementation ✅ **COMPLETE**
  - **Phase 1 Complete:** Domain entities and configurations
    - 3 new entities: Space, Folder, TaskList
    - 3 new configurations
    - Updated Workspace, Task, TaskStatus, User entities
    - Updated AppDbContext with 3 new DbSets (27 total)
  - **Phase 5 Complete:** Frontend Types & Components
    - TypeScript types: Space, Folder, TaskList, Create/Update requests
    - Tree navigation types: SpaceTreeNode with 3 node types (space, folder, tasklist)
    - API client: Full CRUD operations for spaces, folders, tasklists
    - Space tree navigation component with expand/collapse
    - Tree utilities: buildSpaceTree, findNodeById, getNodePath
    - Accessibility features: aria-selected, aria-expanded
    - Dynamic color styling with inline styles
  - **Phase 6 Complete:** Frontend Pages and Routes ✅
    - Navigation sidebar updated (Tasks → Spaces)
    - Spaces overview page (`/spaces`) with tree view
    - List detail page (`/lists/[id]`) with task board
    - Task modal with list selector
    - Breadcrumb trails showing hierarchy path
    - Task types updated with listId, spaceId, folderId
    - Code Review: A+ (95/100)
    - Commits: c71f39b, 51d8118
  - **Phase 7 Complete:** Testing ⏸️ **DEFERRED**
    - No test infrastructure available
    - Comprehensive test requirements documented
    - TypeScript/ESLint errors fixed
    - Document quality: 9.2/10
    - Commit: 9515e0a
  - **Phase 8 Complete:** Workspace Context ✅
    - Workspace feature module with types, API, provider
    - WorkspaceSelector component built
    - WorkspaceProvider integrated in app layout
    - Spaces page updated to use context
    - Workspace ID validation fixed
    - Code Review: A- (92/100)
    - Commit: 4285736
- [ ] **Phase 10:** Time Tracking
- [ ] **Phase 11:** Dashboards & Reporting
- [ ] **Phase 12:** Automation & Workflow Engine
- [ ] **Phase 13:** Mobile Responsive Design

## API Endpoints Summary

### Authentication (`/api/auth`)

- `POST /register` - User registration
- `POST /login` - User login (returns access/refresh tokens)
- `POST /refresh` - Refresh access token

### Tasks (`/api/tasks`)

- `GET /` - List tasks with filtering and pagination
- `GET /{id}` - Get task by ID
- `POST /` - Create new task
- `PUT /{id}` - Update task
- `DELETE /{id}` - Delete task
- `PATCH /{id}/status` - Update task status (for Board drag-drop)
- `GET /views/board/{projectId}` - Get board view data (columns with tasks)
- `GET /views/calendar/{projectId}?year=&month=` - Get calendar view data
- `GET /views/gantt/{projectId}` - Get gantt view data (hierarchical tasks)

### Comments (`/api/comments`)

- `POST /` - Add comment to task
- `GET /task/{taskId}` - Get comments for task
- `GET /{commentId}/replies` - Get replies to comment
- `PUT /{commentId}` - Update comment (owner only)
- `DELETE /{commentId}` - Delete comment (owner only)

### Attachments (`/api/attachments`)

- `POST /upload/{taskId}` - Upload file attachment (100MB max)
- `GET /task/{taskId}` - List attachments for task
- `GET /{attachmentId}/download` - Download file
- `DELETE /{attachmentId}` - Delete attachment (owner only)

### Goals (NEW Phase 08) (`/api/goals`)

#### Periods

- `POST /periods` - Create goal period
- `GET /periods` - Get periods for workspace (with status filter)
- `PUT /periods/{id}` - Update period
- `DELETE /periods/{id}` - Delete period

#### Objectives

- `POST /objectives` - Create objective
- `GET /objectives` - Get objectives (paginated with filters)
- `GET /objectives/tree` - Get hierarchical objective tree
- `PUT /objectives/{id}` - Update objective
- `DELETE /objectives/{id}` - Delete objective

#### Key Results

- `POST /keyresults` - Create key result
- `PUT /keyresults/{id}` - Update key result
- `DELETE /keyresults/{id}` - Delete key result

#### Dashboard

- `GET /dashboard` - Get progress dashboard statistics

## Security Features

- **Path Traversal Protection:** `Path.GetFileName()` sanitization in file uploads
- **File Size Limits:** 100MB maximum attachment size
- **File Type Validation:** Extension allowlist (images, docs, archives - no executables)
- **Comment Validation:** Max 5000 characters, max 5 reply levels
- **Authorization:** Permission-based access control on all endpoints
- **Ownership Verification:** Users can only edit/delete their own content

## Next Steps

1. **Phase 09:** Time Tracking
   - Time entry entities and tables
   - Timer functionality
   - Time reports and analytics
2. **Phase 10:** Dashboards & Reporting
   - Customizable dashboards
   - Advanced reporting features
   - Export capabilities
3. **Phase 11:** Automation & Workflow Engine
   - Rule-based automation
   - Webhook integrations
   - Custom triggers
4. **Phase 12:** Mobile Responsive Design
   - Responsive layouts
   - Touch interactions
   - Mobile-specific features

---

**Documentation Version:** 1.4
**Last Updated:** 2026-01-06
**Maintained By:** Development Team
