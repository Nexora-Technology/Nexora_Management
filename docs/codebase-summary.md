# Codebase Summary

**Last Updated:** 2026-01-03
**Version:** Phase 04 Complete (Task Management Core)

## Project Overview

Nexora Management is a ClickUp-inspired project management platform built with .NET 9.0 backend and Next.js 15 frontend. The platform provides comprehensive task management, collaboration, and productivity tracking features.

## Technology Stack

### Backend

- **Framework:** .NET 9.0 / ASP.NET Core Web API
- **ORM:** Entity Framework Core 9.0
- **Database:** PostgreSQL 16
- **Language:** C# 12
- **Architecture:** Clean Architecture (Domain, Infrastructure, Application, API layers)

### Frontend

- **Framework:** Next.js 15 (App Router)
- **Language:** TypeScript
- **Styling:** Tailwind CSS
- **Components:** shadcn/ui
- **State Management:** Zustand
- **Data Fetching:** React Query

## Architecture

### Clean Architecture Layers

#### 1. Domain Layer (`/apps/backend/src/Nexora.Management.Domain/`)

**Purpose:** Core business logic and enterprise rules

**Components:**

- **Entities** (14 domain models):
  - `User` - User accounts and authentication
  - `Role` - User roles (Admin, Member, Guest)
  - `Permission` - Granular permissions (Create, Read, Update, Delete)
  - `UserRole` - Many-to-many relationship between Users and Roles
  - `RolePermission` - Many-to-many relationship between Roles and Permissions
  - `Workspace` - Workspaces as top-level containers
  - `WorkspaceMember` - Workspace membership management
  - `Project` - Projects within workspaces
  - `Task` - Tasks within projects
  - `TaskStatus` - Custom task statuses (To Do, In Progress, Done)
  - `Comment` - Threaded comments on tasks
  - `Attachment` - File attachments for tasks
  - `ActivityLog` - Audit trail for all activities

- **Common:**
  - `BaseEntity` - Base entity with Id, CreatedAt, UpdatedAt
  - `IAuditable` - Audit interface

#### 2. Infrastructure Layer (`/apps/backend/src/Nexora.Management.Infrastructure/`)

**Purpose:** External concerns and data access

**Components:**

- **Persistence** (`/Persistence/`):
  - `AppDbContext` - EF Core DbContext with 13 DbSets
  - **Configurations** (14 EF Core configurations):
    - `UserConfiguration` - User entity mapping
    - `RoleConfiguration` - Role entity mapping
    - `PermissionConfiguration` - Permission entity mapping
    - `UserRoleConfiguration` - UserRole junction table
    - `RolePermissionConfiguration` - RolePermission junction table
    - `WorkspaceConfiguration` - Workspace with JSONB settings
    - `WorkspaceMemberConfiguration` - Workspace membership
    - `ProjectConfiguration` - Project with color/icon/status
    - `TaskConfiguration` - Task with hierarchical relationships
    - `TaskStatusConfiguration` - Custom statuses per project
    - `CommentConfiguration` - Threaded comments
    - `AttachmentConfiguration` - File attachments
    - `ActivityLogConfiguration` - Audit logging

- **Interfaces:**
  - `IAppDbContext` - Abstraction for DbContext

#### 3. Application Layer (`/apps/backend/src/Nexora.Management.Application/`)

**Purpose:** Application logic and use cases

**Components:**

- **Common:**
  - `Result` / `Result<T>` - Non-generic and generic result patterns for operation outcomes
  - `ApiResponse<T>` - Standardized API response wrapper
  - MediatR setup for CQRS pattern

- **Tasks:**
  - **Commands:** CreateTask, UpdateTask, DeleteTask
  - **Queries:** GetTaskById, GetTasks (with filtering)
  - **DTOs:** TaskDto, CreateTaskRequest, UpdateTaskRequest, GetTasksQueryRequest

#### 4. API Layer (`/apps/backend/src/Nexora.Management.API/`)

**Purpose:** Presentation and external interfaces

**Components:**

- **Endpoints:**
  - `TaskEndpoints.cs` - Task CRUD endpoints at `/api/tasks`
  - **Program.cs** - Application entry point
    - Serilog configuration
    - CORS setup
    - DbContext registration
    - Swagger/OpenAPI setup
    - Health check endpoint
    - Welcome endpoint

- **Persistence/Migrations** (3 migration files):
  - `20260103071610_InitialCreate` - Initial schema creation
  - `20260103071738_EnableRowLevelSecurity` - RLS policies
  - `20260103071908_SeedRolesAndPermissions` - Initial data seeding
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
                              └───── (N) ActivityLog

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
│   │   └── Common/
│   │       └── ApiResponse.cs
│   │
│   └── Nexora.Management.API/
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
- [ ] **Phase 04:** Core workspace functionality
- [x] **Phase 05:** Task management CRUD
- [ ] **Phase 06:** Real-time updates via SignalR
- [ ] **Phase 07:** File attachments
- [ ] **Phase 08:** Comments and collaboration
- [ ] **Phase 09:** Advanced filtering and search
- [ ] **Phase 10:** Mobile responsive design
- [ ] **Phase 11:** Performance optimization
- [ ] **Phase 12:** Deployment to production

## Next Steps

1. **Phase 04:** Complete workspace management endpoints
2. **Phase 06:** Add real-time updates via SignalR
3. **Phase 07:** Implement file attachment handling
4. **Future:** Add advanced search, comments, performance optimization

---

**Documentation Version:** 1.0
**Maintained By:** Development Team
