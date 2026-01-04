# Codebase Summary

**Last Updated:** 2026-01-04 22:10
**Version:** Phase 07 In Progress (Document & Wiki System - 60% Complete)
**Backend Files:** 144 files
**Frontend Lines:** ~6,200 lines

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
- **Components:** shadcn/ui
- **Rich Text:** TipTap (for document editor)
- **State Management:** Zustand
- **Data Fetching:** React Query
- **Real-time:** @microsoft/signalr

## Architecture

### Clean Architecture Layers

#### 1. Domain Layer (`/apps/backend/src/Nexora.Management.Domain/`)

**Purpose:** Core business logic and enterprise rules

**Components:**

- **Entities** (21 domain models):
  - `User` - User accounts and authentication
  - `Role` - User roles (Admin, Member, Guest)
  - `Permission` - Granular permissions (Create, Read, Update, Delete)
  - `UserRole` - Many-to-many relationship between Users and Roles
  - `RolePermission` - Many-to-many relationship between Roles and Permissions
  - `RefreshToken` - JWT refresh token storage
  - `Workspace` - Workspaces as top-level containers
  - `WorkspaceMember` - Workspace membership management
  - `Project` - Projects within workspaces
  - `Task` - Tasks within projects
  - `TaskStatus` - Custom task statuses (To Do, In Progress, Done)
  - `Comment` - Threaded comments on tasks
  - `Attachment` - File attachments for tasks
  - `ActivityLog` - Audit trail for all activities
  - `UserPresence` - Real-time user online/offline status
  - `Notification` - User notifications
  - `NotificationPreference` - User notification settings
  - **NEW Phase 07:**
    - `Page` - Wiki/document pages with hierarchical structure
    - `PageVersion` - Page version history for restore capability
    - `PageCollaborator` - Page collaboration with role-based access
    - `PageComment` - Comments on document pages

- **Common:**
  - `BaseEntity` - Base entity with Id, CreatedAt, UpdatedAt
  - `IAuditable` - Audit interface

#### 2. Infrastructure Layer (`/apps/backend/src/Nexora.Management.Infrastructure/`)

**Purpose:** External concerns and data access

**Components:**

- **Persistence** (`/Persistence/`):
  - `AppDbContext` - EF Core DbContext with 21 DbSets
  - **Configurations** (25 EF Core configurations):
    - `UserConfiguration` - User entity mapping
    - `RoleConfiguration` - Role entity mapping
    - `PermissionConfiguration` - Permission entity mapping
    - `UserRoleConfiguration` - UserRole junction table
    - `RolePermissionConfiguration` - RolePermission junction table
    - `RefreshTokenConfiguration` - Refresh token storage
    - `WorkspaceConfiguration` - Workspace with JSONB settings
    - `WorkspaceMemberConfiguration` - Workspace membership
    - `ProjectConfiguration` - Project with color/icon/status
    - `TaskConfiguration` - Task with hierarchical relationships
    - `TaskStatusConfiguration` - Custom statuses per project
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

- **Interfaces:**
  - `IAppDbContext` - Abstraction for DbContext

#### 3. Application Layer (`/apps/backend/src/Nexora.Management.Application/`)

**Purpose:** Application logic and use cases

**Components:**

- **Common:**
  - `Result` / `Result<T>` - Non-generic and generic result patterns for operation outcomes
  - `ApiResponse<T>` - Standardized API response wrapper
  - MediatR setup for CQRS pattern

**CQRS Modules Summary (59 files across 8 feature modules):**
- **Authentication:** 3 Commands, 3 DTOs (9 files)
- **Authorization:** 4 components (Handler, Provider, Attribute, Requirement) (4 files)
- **Tasks:** 4 Commands, 5 Queries, 5 DTOs (14 files)
- **Comments:** 3 Commands, 2 Queries, 3 DTOs (8 files)
- **Attachments:** 2 Commands, 1 Query, 3 DTOs (6 files)
- **Documents:** 6 Commands, 4 Queries, 7 DTOs (17 files) - Phase 07
- **Common:** Result patterns, ApiResponse, IUserContext (3 files)
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

- **Documents** (NEW Phase 07):
  - **Commands:** CreatePage, UpdatePage, DeletePage, ToggleFavorite, MovePage, RestorePageVersion
  - **Queries:** GetPageById, GetPageTree, GetPageHistory, SearchPages
  - **DTOs:** PageDto, PageDetailDto, PageTreeNodeDto, PageVersionDto, PageCommentDto, CreatePageRequest, UpdatePageRequest, MovePageRequest, SearchPagesRequest

#### 4. API Layer (`/apps/backend/src/Nexora.Management.API/`)

**Purpose:** Presentation and external interfaces

**Components:**

- **Endpoints:** (5 Minimal API groups)
  - `AuthEndpoints.cs` - Authentication endpoints at `/api/auth`
  - `TaskEndpoints.cs` - Task CRUD endpoints at `/api/tasks`
  - `CommentEndpoints.cs` - Comment endpoints at `/api/comments`
  - `AttachmentEndpoints.cs` - File attachment endpoints at `/api/attachments`
  - `DocumentEndpoints.cs` (NEW Phase 07) - Document endpoints at `/api/documents`

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

- **Persistence/Migrations** (6 migration files):
  - `20260103071610_InitialCreate` - Initial schema creation
  - `20260103071738_EnableRowLevelSecurity` - RLS policies
  - `20260103071908_SeedRolesAndPermissions` - Initial data seeding
  - `20260103171029_AddRealtimeCollaborationTables` - Real-time features (presence, notifications)
  - `20260104112014_AddDocumentTables` (Phase 07) - Document/Wiki system tables
  - `AppDbContextModelSnapshot` - EF Core model snapshot

- **appsettings.json** - Configuration including connection string

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

2. **Hierarchical Task Management:**
   - Workspace → Project → Task hierarchy
   - Task nesting (parent-child relationships)
   - Custom statuses per project
   - Position ordering for drag-and-drop

3. **Row-Level Security (RLS):**
   - Applied to Tasks, Projects, Comments, Attachments, ActivityLog
   - Policies enforce workspace membership
   - User context set via `set_current_user_id()` function
   - Automatic filtering at database level

4. **Audit Trail:**
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
--primary: 250 73% 68%;        /* #7B68EE - ClickUp Purple */
--primary-hover: 250 73% 76%;  /* #A78BFA - Light Purple */
--primary-active: 250 62% 55%; /* #5B4BC4 - Dark Purple */
--primary-bg: 250 100% 97%;    /* #F5F3FF - Purple Tint */
```

**Semantic Colors:**
- `--success: 158 64% 42%` - Green (#10B981)
- `--warning: 38 92% 50%` - Yellow (#F59E0B)
- `--error: 0 72% 51%` - Red (#EF4444)
- `--info: 217 91% 60%` - Blue (#3B82F6)

**Gray Scale (HSL format for consistent theming):**
```css
--gray-50: 220 20% 98%;   /* Lightest */
--gray-100: 220 15% 95%;
--gray-200: 220 10% 90%;
--gray-300: 220 10% 75%;
--gray-400: 220 10% 60%;
--gray-500: 220 10% 45%;
--gray-600: 220 10% 35%;
--gray-700: 220 15% 25%;
--gray-800: 220 20% 15%;
--gray-900: 220 25% 10%;  /* Darkest */
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
--text-xs: 0.6875rem;   /* 11px - Tiny/Timestamps */
--text-sm: 0.75rem;     /* 12px - Small/Captions */
--text-base: 0.875rem;  /* 14px - Body text */
--text-md: 1rem;        /* 16px - H3/Subsection */
--text-lg: 1.25rem;     /* 20px - H2/Card titles */
--text-xl: 1.5rem;      /* 24px - H1/Section headers */
--text-2xl: 2rem;       /* 32px - Display/Page titles */
```

**Font Weights:**
```css
--font-regular: 400;    /* Body text */
--font-medium: 500;     /* Emphasized text, labels */
--font-semibold: 600;   /* Headings, important UI */
--font-bold: 700;       /* Strong headings, CTAs */
```

**Line Heights:**
```css
--leading-tight: 1.25;   /* Headings, compact text */
--leading-normal: 1.5;   /* Body text */
```

#### Spacing System

**Base Unit:** 4px (powers of 2 scale)

```css
--space-0: 0;
--space-1: 0.25rem;   /* 4px - Tight gaps */
--space-2: 0.5rem;    /* 8px - Icon padding */
--space-3: 0.75rem;   /* 12px - Compact padding */
--space-4: 1rem;      /* 16px - Standard spacing */
--space-5: 1.25rem;   /* 20px - Component gaps */
--space-6: 1.5rem;    /* 24px - Section spacing */
--space-8: 2rem;      /* 32px - Large gaps */
--space-10: 2.5rem;   /* 40px - XL spacing */
--space-12: 3rem;     /* 48px - XXL spacing */
--space-16: 4rem;     /* 64px - Huge spacing */
```

#### Border Radius Scale

```css
--radius-sm: 4px;      /* Small badges, dots */
--radius-md: 6px;      /* Buttons, inputs (default) */
--radius-lg: 8px;      /* Cards, panels */
--radius-xl: 12px;     /* Modals, large cards */
--radius-2xl: 16px;    /* Hero sections */
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
--shadow-sm: 0 1px 2px rgba(0,0,0,0.05);
--shadow-md: 0 1px 3px rgba(0,0,0,0.1), 0 1px 2px rgba(0,0,0,0.06);
--shadow-lg: 0 4px 6px rgba(0,0,0,0.1), 0 2px 4px rgba(0,0,0,0.06);
--shadow-xl: 0 10px 15px rgba(0,0,0,0.1), 0 4px 6px rgba(0,0,0,0.05);
--shadow-2xl: 0 20px 25px rgba(0,0,0,0.15), 0 10px 10px rgba(0,0,0,0.04);
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
--transition-fast: 150ms;   /* Buttons, toggles */
--transition-base: 200ms;   /* Dropdowns, sidebar */
--transition-slow: 300ms;   /* Modals, page */
```

**Easing Functions:**
```css
--ease-out: cubic-bezier(0, 0, 0.2, 1);      /* Deceleration */
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
  children: React.ReactNode
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
import { AppLayout } from "@/components/layout/app-layout"

export default function Layout({ children }: { children: React.ReactNode }) {
  return <AppLayout>{children}</AppLayout>
}
```

#### 2. AppHeader Component (`src/components/layout/app-header.tsx`)

**Purpose:** Top navigation bar with logo, search, notifications, and profile

**Props:**
```typescript
interface AppHeaderProps {
  sidebarCollapsed: boolean
  onToggleSidebar: () => void
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
<AppHeader
  sidebarCollapsed={collapsed}
  onToggleSidebar={() => setCollapsed(!collapsed)}
/>
```

#### 3. AppSidebar Component (`src/components/layout/app-sidebar.tsx`)

**Purpose:** Collapsible navigation sidebar

**Props:**
```typescript
interface AppSidebarProps {
  collapsed?: boolean
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
  collapsed?: boolean
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
  { title: "Home", href: "/", icon: Home },
  { title: "Tasks", href: "/tasks", icon: CheckSquare },
  { title: "Projects", href: "/projects", icon: Folder },
  { title: "Team", href: "/team", icon: Users },
  { title: "Calendar", href: "/calendar", icon: Calendar },
  { title: "Settings", href: "/settings", icon: Settings },
]
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
  label: string
  href?: string  // Optional link
}

interface BreadcrumbProps {
  items: BreadcrumbItem[]
  className?: string
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
    { label: "Home", href: "/" },
    { label: "Tasks", href: "/tasks" },
    { label: "Task Detail" },
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
  children: React.ReactNode
  size?: "sm" | "md" | "lg" | "xl" | "full"
  className?: string
}
```

**Size Variants:**
```typescript
const sizeClasses = {
  sm: "max-w-3xl",   // 768px
  md: "max-w-4xl",   // 896px
  lg: "max-w-6xl",   // 1152px (ClickUp default)
  xl: "max-w-7xl",   // 1280px
  full: "max-w-full",
}
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
  children: React.ReactNode
  className?: string
}
```

**BoardColumn Props:**
```typescript
interface BoardColumnProps {
  title: string
  count?: number
  children: React.ReactNode
  className?: string
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
--breakpoint-xs: 375px;   /* Small mobile */
--breakpoint-sm: 640px;   /* Mobile */
--breakpoint-md: 768px;   /* Tablet */
--breakpoint-lg: 1024px;  /* Desktop */
--breakpoint-xl: 1280px;  /* Large desktop */
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
  "bg-red-500", "bg-orange-500", "bg-amber-500", "bg-yellow-500",
  "bg-green-500", "bg-emerald-500", "bg-teal-500", "bg-cyan-500",
  "bg-sky-500", "bg-blue-500", "bg-indigo-500", "bg-violet-500",
  "bg-purple-500", // ClickUp purple
  "bg-fuchsia-500", "bg-pink-500", "bg-rose-500",
]
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
- **Components:** shadcn/ui (12 components)
- **Rich Text:** TipTap editor
- **State:** Zustand
- **Data Fetching:** React Query
- **Real-time:** @microsoft/signalr
- **Drag-Drop:** @dnd-kit

### Route Pages (8 routes)

```
apps/frontend/src/app/
├── layout.tsx              # Root layout with providers
├── page.tsx                # Landing page
├── (auth)/
│   ├── login/
│   │   └── page.tsx        # Login page
│   └── register/
│       └── page.tsx        # Register page
├── dashboard/
│   └── page.tsx            # Dashboard page
├── workspaces/
│   └── page.tsx            # Workspaces list
└── projects/
    └── [id]/
        └── page.tsx        # Project detail with task views
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

### SignalR Hooks (3 hooks)

```
src/hooks/signalr/
├── useTaskHub.ts           # Task real-time updates
├── usePresenceHub.ts       # User presence tracking
└── useNotificationHub.ts   # Notification delivery
```

### shadcn/ui Components (12)

```
src/components/ui/
├── avatar.tsx              # User avatar
├── badge.tsx               # Status badges
├── button.tsx              # Button component
├── card.tsx                # Card container
├── dropdown-menu.tsx       # Dropdown menus
├── input.tsx               # Form inputs
├── label.tsx               # Form labels
├── scroll-area.tsx         # Custom scrollbar
├── separator.tsx           # Visual separator
├── sonner.tsx              # Toast notifications
├── switch.tsx              # Toggle switch
└── tooltip.tsx             # Tooltip component
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
- [ ] **Phase 07:** Document & Wiki System
- [ ] **Phase 08:** Goal Tracking & OKRs
- [ ] **Phase 09:** Time Tracking
- [ ] **Phase 10:** Dashboards & Reporting
- [ ] **Phase 11:** Automation & Workflow Engine
- [ ] **Phase 12:** Mobile Responsive Design

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

## Security Features

- **Path Traversal Protection:** `Path.GetFileName()` sanitization in file uploads
- **File Size Limits:** 100MB maximum attachment size
- **File Type Validation:** Extension allowlist (images, docs, archives - no executables)
- **Comment Validation:** Max 5000 characters, max 5 reply levels
- **Authorization:** Permission-based access control on all endpoints
- **Ownership Verification:** Users can only edit/delete their own content

## Next Steps

1. **Phase 07:** Complete Document & Wiki system integration
   - Apply database migration
   - Create document routes
   - Wire frontend to backend API
   - Add real-time collaboration
   - Implement slash commands
2. **Phase 08:** Goal Tracking & OKRs
3. **Phase 09:** Time Tracking
4. **Phase 10:** Dashboards & Reporting

---

**Documentation Version:** 1.2
**Last Updated:** 2026-01-04
**Maintained By:** Development Team
