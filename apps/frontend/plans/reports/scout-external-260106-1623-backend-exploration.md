# Backend Codebase Exploration Report

**Date**: 2026-01-06  
**Agent**: scout-external  
**ID**: a86ba2a  
**Target**: apps/backend/

## Executive Summary

Nexora Management Backend is a **.NET 9.0 Web API** following **Clean Architecture** principles with 4 distinct layers. The codebase implements a comprehensive project management system with tasks, goals, documents, real-time collaboration via SignalR, and role-based access control.

## 1. Project Structure

### Root Structure
```
apps/backend/
├── src/                                   # Source code
│   ├── Nexora.Management.API/            # Presentation layer
│   ├── Nexora.Management.Application/     # Application layer (CQRS/MediatR)
│   ├── Nexora.Management.Domain/          # Domain layer (entities, interfaces)
│   └── Nexora.Management.Infrastructure/  # Infrastructure (DB, external services)
├── tests/                                 # Test projects
├── Nexora.Management.sln                  # Solution file
└── README.md
```

### Clean Architecture Layers

| Layer | Purpose | Dependencies |
|-------|---------|--------------|
| **Domain** | Core business logic, entities | None (core) |
| **Application** | Use cases, DTOs, CQRS handlers | Domain |
| **Infrastructure** | DB, JWT, file storage, SignalR | Domain |
| **API** | HTTP endpoints, middleware | Application, Infrastructure |

## 2. Source Code Organization

### API Layer (`Nexora.Management.API/`)

**Key Directories:**
- `/Endpoints/` - Minimal API endpoint definitions (6 endpoint files)
- `/Middleware/` - Custom middleware (workspace auth)
- `/Middlewares/` - User context implementation
- `/Hubs/` - SignalR hubs (TaskHub, PresenceHub, NotificationHub)
- `/Services/` - SignalR services (NotificationService, PresenceService)
- `/Extensions/` - Authorization extensions
- `/Persistence/Migrations/` - EF Core migrations (6 migrations)

**Key Files:**
- `Program.cs` - Application entry point, DI configuration
- `DesignTimeDbContextFactory.cs` - Design-time DB context factory

### Application Layer (`Nexora.Management.Application/`)

**Feature Modules:**
```
/Authentication/         # Auth commands/queries/DTOs
/Tasks/                  # Task management (Commands, Queries, DTOs, Views)
/Comments/               # Comments (CRUD operations)
/Attachments/            # File attachments
/Goals/                  # OKR goal tracking
/Documents/              # Document management
/Authorization/          # Custom authorization attributes
/Common/                 # Shared DTOs, exceptions, interfaces
/DTOs/SignalR/           # Real-time message DTOs
```

**Pattern:** CQRS with MediatR
- `Commands/` - Write operations (Create, Update, Delete)
- `Queries/` - Read operations (Get, List, Search)
- `DTOs/` - Data transfer objects

### Domain Layer (`Nexora.Management.Domain/`)

**Directories:**
- `/Entities/` - 21 domain entities
- `/Common/` - BaseEntity base class
- `/Interfaces/` - Domain interfaces (if any)

**Entities:**
- Identity: User, Role, Permission, UserRole, RolePermission, RefreshToken
- Workspace: Workspace, WorkspaceMember
- Project: Project, TaskStatus
- Task: Task, Comment, Attachment, ActivityLog
- Realtime: UserPresence, Notification, NotificationPreference
- Documents: Page, PageVersion, PageComment, PageCollaborator
- Goals: GoalPeriod, Objective, KeyResult

### Infrastructure Layer (`Nexora.Management.Infrastructure/`)

**Directories:**
- `/Persistence/` - DbContext, configurations, migrations
- `/Authentication/` - JWT token service, settings
- `/Services/` - File storage service
- `/Interfaces/` - IAppDbContext, IJwtTokenService, IPresenceService, INotificationService

**Key Files:**
- `AppDbContext.cs` - EF Core DbContext with 23 DbSets
- 22 entity configuration files (Fluent API)

## 3. Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "PostgreSQL connection"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Jwt": {
    "Secret": "JWT signing key",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  },
  "Serilog": {
    "WriteTo": ["Console", "File"]
  }
}
```

### Launch Settings
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: Root URL in Development

## 4. Dependencies & Frameworks

### .NET Projects
```
Nexora.Management.API (net9.0)
├── Nexora.Management.Application
└── Nexora.Management.Infrastructure

Nexora.Management.Application (net9.0)
├── Nexora.Management.Domain
└── Nexora.Management.Infrastructure

Nexora.Management.Infrastructure (net9.0)
└── Nexora.Management.Domain

Nexora.Management.Domain (net9.0)
```

### Key NuGet Packages

**API Layer:**
- `Microsoft.AspNetCore.Authentication.JwtBearer` (9.0.3)
- `Microsoft.AspNetCore.OpenApi` (9.0.7)
- `Microsoft.EntityFrameworkCore.Design` (9.0.3)
- `Serilog.AspNetCore` (10.0.0)

**Application Layer:**
- `MediatR` (14.0.0) - CQRS pattern
- `FluentValidation` (12.1.1) - Input validation
- `Microsoft.AspNetCore.Authorization` (9.0.3)
- `Microsoft.EntityFrameworkCore` (9.0.3)

**Infrastructure Layer:**
- `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.3)
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (9.0.3)
- `Microsoft.Extensions.Caching.StackExchangeRedis` (9.0.3)
- `System.IdentityModel.Tokens.Jwt` (8.3.0)

**Domain Layer:**
- No external dependencies (pure domain)

## 5. API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login with email/password
- `POST /api/auth/refresh` - Refresh access token

### Tasks (`/api/tasks`)
- `POST /api/tasks` - Create task
- `GET /api/tasks` - List tasks (filterable)
- `GET /api/tasks/{id}` - Get task by ID
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- `PATCH /api/tasks/{id}/status` - Update task status
- `GET /api/tasks/views/board/{projectId}` - Board view
- `GET /api/tasks/views/calendar/{projectId}` - Calendar view
- `GET /api/tasks/views/gantt/{projectId}` - Gantt view

### Goals (`/api/goals`) - Authorization Required
**Periods:**
- `POST /api/goals/periods` - Create period
- `GET /api/goals/periods` - List periods
- `PUT /api/goals/periods/{id}` - Update period
- `DELETE /api/goals/periods/{id}` - Delete period

**Objectives:**
- `POST /api/goals/objectives` - Create objective
- `GET /api/goals/objectives` - List objectives
- `GET /api/goals/objectives/tree` - Get objective tree
- `PUT /api/goals/objectives/{id}` - Update objective
- `DELETE /api/goals/objectives/{id}` - Delete objective

**Key Results:**
- `POST /api/goals/keyresults` - Create key result
- `PUT /api/goals/keyresults/{id}` - Update key result
- `DELETE /api/goals/keyresults/{id}` - Delete key result

**Dashboard:**
- `GET /api/goals/dashboard` - Progress dashboard

### Comments (`/api/comments`)
- CRUD operations for task comments
- Reply threading support

### Attachments (`/api/attachments`)
- Upload, download, delete file attachments

### Documents (`/api/documents`)
- Document management endpoints

### System
- `GET /health` - Health check
- `GET /` - Welcome endpoint

### SignalR Hubs
- `/hubs/tasks` - Real-time task updates
- `/hubs/presence` - User presence tracking
- `/hubs/notifications` - Push notifications

## 6. Database

### Entity Framework Core Setup

**Provider:** Npgsql.EntityFrameworkCore.PostgreSQL (9.0.3)  
**Database:** PostgreSQL 15+  
**ORM:** EF Core 9.0

### DbContext (`AppDbContext`)

**23 DbSets:**
- Users, Roles, Permissions, UserRoles, RolePermissions
- Workspaces, WorkspaceMembers
- Projects, TaskStatuses, Tasks
- Comments, Attachments, ActivityLogs
- RefreshTokens
- UserPresences, Notifications, NotificationPreferences
- Pages, PageVersions, PageComments
- GoalPeriods, Objectives, KeyResults

### Entity Configurations

**22 Fluent API Configuration Files:**
- UserConfiguration, RoleConfiguration, PermissionConfiguration
- UserRoleConfiguration, RolePermissionConfiguration
- WorkspaceConfiguration, WorkspaceMemberConfiguration
- ProjectConfiguration, TaskConfiguration, TaskStatusConfiguration
- CommentConfiguration, AttachmentConfiguration
- ActivityLogConfiguration, RefreshTokenConfiguration
- NotificationConfiguration, NotificationPreferenceConfiguration
- UserPresenceConfiguration
- PageConfiguration, PageVersionConfiguration, PageCommentConfiguration
- GoalEntitiesConfiguration

### Migrations (6 total)

1. `20260103071610_InitialCreate` - Initial schema
2. `20260103071738_EnableRowLevelSecurity` - RLS support
3. `20260103071908_SeedRolesAndPermissions` - Seed data
4. `20260103171029_AddRealtimeCollaborationTables` - SignalR tables
5. `20260104112014_AddDocumentTables` - Document management
6. `20260105165809_AddGoalTrackingTables` - OKR goals

### PostgreSQL Extensions
- `uuid-ossp` - UUID generation
- `pg_trgm` - Trigram text search

### Row-Level Security (RLS)
- Implemented via `WorkspaceAuthorizationMiddleware`
- Filters data based on user's workspace access

## 7. Key Features & Business Logic

### Authentication & Authorization
- JWT-based authentication (15min access token, 7-day refresh token)
- Role-based access control (RBAC)
- Permission-based policies with `RequirePermission` attribute
- Password hashing with ASP.NET Core Identity

### Task Management
- Hierarchical tasks (parent/child relationships)
- Multiple views: Board, Calendar, Gantt
- Custom fields via JSONB
- Status workflow per project
- Priority levels, assignees, due dates
- Estimated hours tracking

### Goal Tracking (OKR)
- Periods (Q1, Q2, etc.)
- Hierarchical objectives
- Key results with metrics
- Progress tracking dashboard
- Weight-based scoring

### Real-Time Collaboration
- SignalR hubs for tasks, presence, notifications
- Live task updates broadcast to project groups
- User presence tracking
- Push notifications

### Document Management
- Pages with versioning
- Real-time collaboration
- Comments on pages
- Collaborator tracking

### Comments & Attachments
- Threaded comments
- File upload/download
- Activity logging

## 8. Architecture Patterns

### CQRS with MediatR
- Commands: Create, Update, Delete operations
- Queries: Read operations with DTOs
- Separation of read/write models

### Clean Architecture
- Dependency inversion (domain at core)
- Layer isolation
- Testability

### Repository Pattern
- `IAppDbContext` interface
- Direct DbContext usage (no generic repository)

### Minimal APIs
- Endpoint routing in `Program.cs`
- Extension methods for endpoint groups
- OpenAPI/Swagger documentation

## 9. Services

### Infrastructure Services
- `IJwtTokenService` / `JwtTokenService` - Token generation/validation
- `IFileStorageService` / `LocalFileStorageService` - File storage
- `IPresenceService` / `PresenceService` - User presence
- `INotificationService` / `NotificationService` - Notifications

### Application Services
- MediatR command/query handlers
- FluentValidation validators
- Authorization handlers

## 10. Middleware Pipeline

1. HTTPS Redirection
2. CORS ("AllowFrontend" policy)
3. Authentication (JWT Bearer)
4. Authorization
5. Workspace Authorization (custom RLS middleware)
6. Controllers
7. Mapped Endpoints
8. SignalR Hubs

## Unresolved Questions

- Q1: Why is auto-migration disabled in Program.cs (commented out)?
- Q2: What's the bug in InitialCreate migration mentioned in comments?
- Q3: Are there integration tests in the tests/ directory?
- Q4: What's the deployment strategy (Docker, containers)?
- Q5: Are there API versioning plans?
- Q6: Is Redis caching fully implemented or just configured?
