# Backend Codebase Structure Report
**Date:** 2026-01-09  
**Path:** `/Users/nhatduyfirst/Documents/Projects/Nexora_Management/apps/backend`  
**Report ID:** scout-external-260109-1545-backend-structure

## Executive Summary

Clean Architecture .NET 9.0 Web API with 4-layer separation, CQRS pattern with MediatR, and comprehensive ClickUp hierarchy implementation. **203 total C# files** with **24,790 total lines** of code.

## Architecture Overview

### Four-Layer Clean Architecture

```
src/
├── Nexora.Management.Domain/         # Core business entities (27 files)
├── Nexora.Management.Application/    # CQRS use cases, DTOs (92 files)
├── Nexora.Management.Infrastructure/ # DB, external services (25 configs)
└── Nexora.Management.API/            # Web API, endpoints (40+ files)
```

### Technology Stack
- **.NET 9.0** - Runtime
- **ASP.NET Core** - Web framework
- **Entity Framework Core 9** - ORM
- **PostgreSQL** - Database
- **MediatR** - CQRS implementation
- **SignalR** - Real-time collaboration
- **JWT** - Authentication
- **Serilog** - Logging
- **FluentValidation** - Input validation

## Detailed Directory Structure

### 1. Domain Layer (`Nexora.Management.Domain`)

**Purpose:** Core business entities, no external dependencies

```
Domain/
├── Common/
│   └── BaseEntity.cs                # Base entity with Id, CreatedAt, UpdatedAt
└── Entities/                        # 27 entity files
    ├── ActivityLog.cs
    ├── Attachment.cs
    ├── Comment.cs
    ├── Folder.cs                    # ClickUp hierarchy: optional grouping
    ├── GoalEntities.cs              # Period, Objective, KeyResult
    ├── Notification.cs
    ├── NotificationPreference.cs
    ├── Page.cs                      # Document management
    ├── PageCollaborator.cs
    ├── PageComment.cs
    ├── PageVersion.cs
    ├── Permission.cs
    ├── Project.cs                   # Legacy (deprecated)
    ├── RefreshToken.cs
    ├── Role.cs
    ├── RolePermission.cs
    ├── Space.cs                     # ClickUp hierarchy: workspace subdivision
    ├── Task.cs
    ├── TaskList.cs                  # ClickUp hierarchy: task container
    ├── TaskStatus.cs
    ├── User.cs
    ├── UserPresence.cs
    ├── UserRole.cs
    ├── Workspace.cs                 # ClickUp hierarchy: top-level
    └── WorkspaceMember.cs
```

**Key Entity - Workspace:**
```csharp
public class Workspace : BaseEntity
{
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public Dictionary<string, object> SettingsJsonb { get; set; }
    
    // Navigation properties
    public User Owner { get; set; }
    public ICollection<WorkspaceMember> Members { get; set; }
    public ICollection<Project> Projects { get; set; }        // Legacy
    public ICollection<Space> Spaces { get; set; }            // NEW
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<ActivityLog> ActivityLogs { get; set; }
}
```

**Statistics:**
- 27 entity files
- ~1,500 lines of entity code

### 2. Application Layer (`Nexora.Management.Application`)

**Purpose:** Business logic, CQRS pattern, DTOs

**Structure:**
```
Application/
├── Common/                          # Shared utilities
│   ├── ApiResponse.cs               # Standard API response wrapper
│   ├── Exceptions.cs                # Custom exceptions
│   ├── IUserContext.cs              # Current user abstraction
│   ├── PagedResponse.cs             # Pagination wrapper
│   └── Result.cs                    # Operation result (Success/Failure)
│
├── Attachments/                     # 2 commands, 1 query
│   ├── Commands/
│   │   ├── DeleteAttachment/
│   │   └── UploadAttachment/
│   ├── DTOs/
│   │   └── AttachmentDto.cs
│   └── Queries/
│       └── GetAttachments/
│
├── Authentication/                  # 3 commands
│   ├── Commands/
│   │   ├── Login/
│   │   ├── RefreshToken/
│   │   └── Register/
│   └── DTOs/
│       ├── AuthRequests.cs
│       └── AuthResponses.cs
│
├── Authorization/
│   ├── PermissionAuthorizationHandler.cs
│   └── RequirePermissionAttribute.cs
│
├── Comments/                        # 3 commands, 2 queries
│   ├── Commands/
│   │   ├── AddComment/
│   │   ├── DeleteComment/
│   │   └── UpdateComment/
│   ├── DTOs/
│   │   └── CommentDto.cs
│   └── Queries/
│       ├── GetComments/
│       └── GetCommentReplies/
│
├── Documents/                       # 6 commands, 4 queries
│   ├── Commands/
│   │   ├── CreatePage/
│   │   ├── DeletePage/
│   │   ├── MovePage/
│   │   ├── RestorePageVersion/
│   │   ├── ToggleFavorite/
│   │   └── UpdatePage/
│   ├── DTOs/
│   │   └── DocumentDTOs.cs
│   └── Queries/
│       ├── GetPageByIdQuery.cs
│       ├── GetPageHistoryQuery.cs
│       ├── GetPageTreeQuery.cs
│       └── SearchPagesQuery.cs
│
├── Folders/                         # 4 commands, 2 queries
│   ├── Commands/
│   │   ├── CreateFolder/
│   │   ├── DeleteFolder/
│   │   ├── UpdateFolder/
│   │   └── UpdateFolderPosition/
│   ├── DTOs/
│   │   └── FolderDto.cs
│   └── Queries/
│       ├── GetFolderById/
│       └── GetFoldersBySpace/
│
├── Goals/                           # 9 commands, 4 queries
│   ├── Commands/
│   │   ├── CreateKeyResult/
│   │   ├── CreateObjective/
│   │   ├── CreatePeriod/
│   │   ├── DeleteKeyResult/
│   │   ├── DeleteObjective/
│   │   ├── DeletePeriod/
│   │   ├── UpdateKeyResult/
│   │   ├── UpdateObjective/
│   │   └── UpdatePeriod/
│   ├── DTOs/
│   │   └── GoalDTOs.cs
│   └── Queries/
│       ├── GetObjectiveTree/
│       ├── GetObjectives/
│       ├── GetPeriods/
│       └── GetProgressDashboard/
│
├── Spaces/                          # 3 commands, 2 queries
│   ├── Commands/
│   │   ├── CreateSpace/
│   │   ├── DeleteSpace/
│   │   └── UpdateSpace/
│   ├── DTOs/
│   │   └── SpaceDto.cs
│   └── Queries/
│       ├── GetSpaceById/
│       └── GetSpacesByWorkspace/
│
├── TaskLists/                       # 4 commands, 2 queries
│   ├── Commands/
│   │   ├── CreateTaskList/
│   │   ├── DeleteTaskList/
│   │   ├── UpdateTaskList/
│   │   └── UpdateTaskListPosition/
│   ├── DTOs/
│   │   └── TaskListDto.cs
│   └── Queries/
│       ├── GetTaskListById/
│       └── GetTaskLists/
│
├── Tasks/                           # 4 commands, 5 queries
│   ├── Commands/
│   │   ├── CreateTask/
│   │   ├── DeleteTask/
│   │   ├── UpdateTask/
│   │   └── UpdateTaskStatus/
│   ├── DTOs/
│   │   ├── TaskDTOs.cs
│   │   └── ViewDTOs.cs
│   └── Queries/
│       ├── TaskQueries.cs           # GetTaskById, GetTasks
│       └── ViewQueries/
│           ├── BoardViewQuery.cs
│           ├── CalendarViewQuery.cs
│           └── GanttViewQuery.cs
│
├── Workspaces/                      # 3 commands, 2 queries
│   ├── Commands/
│   │   ├── CreateWorkspace/
│   │   ├── DeleteWorkspace/
│   │   └── UpdateWorkspace/
│   ├── DTOs/
│   │   ├── CreateWorkspaceRequest.cs
│   │   ├── UpdateWorkspaceRequest.cs
│   │   └── WorkspaceDto.cs
│   └── Queries/
│       ├── GetWorkspaceById/
│       └── GetWorkspaces/
│
├── DTOs/SignalR/                    # Real-time messages
│   ├── AttachmentUpdatedMessage.cs
│   ├── CommentUpdatedMessage.cs
│   ├── NotificationMessage.cs
│   ├── TaskUpdatedMessage.cs
│   ├── TypingIndicatorMessage.cs
│   └── UserPresenceMessage.cs
│
└── Interfaces/                      # Empty (interfaces in Infrastructure)
```

**CQRS Pattern Example:**
```csharp
// Command
public record CreateWorkspaceCommand(
    string Name,
    Guid OwnerId,
    Dictionary<string, object>? SettingsJsonb
) : IRequest<Result<WorkspaceDto>>;

// Handler
public class CreateWorkspaceCommandHandler : IRequestHandler<CreateWorkspaceCommand, Result<WorkspaceDto>>
{
    private readonly IAppDbContext _db;
    
    public async Task<Result<WorkspaceDto>> Handle(CreateWorkspaceCommand request, CancellationToken ct)
    {
        var workspace = new Workspace { /* ... */ };
        _db.Workspaces.Add(workspace);
        await _db.SaveChangesAsync(ct);
        return Result<WorkspaceDto>.Success(workspaceDto);
    }
}
```

**Statistics:**
- 92 C# files (excluding obj/bin)
- 38+ Commands
- 21+ Queries
- 15+ DTO files
- ~9,594 lines of application code

### 3. Infrastructure Layer (`Nexora.Management.Infrastructure`)

**Purpose:** External concerns, database, services

```
Infrastructure/
├── Authentication/
│   ├── JwtSettings.cs               # JWT configuration
│   └── JwtTokenService.cs           # Token generation/validation
│
├── Interfaces/
│   ├── IAppDbContext.cs             # DB context abstraction
│   ├── IJwtTokenService.cs
│   ├── INotificationService.cs
│   └── IPresenceService.cs
│
├── Middlewares/
│   └── (Empty - middleware in API layer)
│
├── Persistence/
│   ├── AppDbContext.cs              # EF Core context (99 lines)
│   ├── AppDbContextFactory.cs       # Design-time factory
│   └── Configurations/              # 25 EF Core entity configs
│       ├── ActivityLogConfiguration.cs
│       ├── AttachmentConfiguration.cs
│       ├── CommentConfiguration.cs
│       ├── FolderConfiguration.cs
│       ├── GoalEntitiesConfiguration.cs
│       ├── NotificationConfiguration.cs
│       ├── NotificationPreferenceConfiguration.cs
│       ├── PageCollaboratorConfiguration.cs
│       ├── PageCommentConfiguration.cs
│       ├── PageConfiguration.cs
│       ├── PageVersionConfiguration.cs
│       ├── PermissionConfiguration.cs
│       ├── ProjectConfiguration.cs
│       ├── RefreshTokenConfiguration.cs
│       ├── RoleConfiguration.cs
│       ├── RolePermissionConfiguration.cs
│       ├── SpaceConfiguration.cs
│       ├── TaskConfiguration.cs
│       ├── TaskListConfiguration.cs
│       ├── TaskStatusConfiguration.cs
│       ├── UserConfiguration.cs
│       ├── UserPresenceConfiguration.cs
│       ├── UserRoleConfiguration.cs
│       ├── WorkspaceConfiguration.cs
│       └── WorkspaceMemberConfiguration.cs
│
└── Services/
    ├── IFileStorageService.cs       # File storage abstraction
    └── LocalFileStorageService.cs   # Local filesystem implementation
```

**Statistics:**
- 25 entity configurations
- ~3,000 lines of infrastructure code

### 4. API Layer (`Nexora.Management.API`)

**Purpose:** HTTP endpoints, middleware, SignalR hubs

```
API/
├── Endpoints/                       # Minimal API endpoints
│   ├── AttachmentEndpoints.cs
│   ├── AuthEndpoints.cs
│   ├── CommentEndpoints.cs
│   ├── DocumentEndpoints.cs        # 226 lines
│   ├── FolderEndpoints.cs
│   ├── GoalEndpoints.cs            # 379 lines - largest
│   ├── SpaceEndpoints.cs           # 205 lines
│   ├── TaskEndpoints.cs            # 280 lines
│   ├── TaskListEndpoints.cs
│   └── WorkspaceEndpoints.cs       # 121 lines
│
├── Extensions/
│   └── AuthorizationExtensions.cs
│
├── Hubs/                           # SignalR real-time
│   ├── NotificationHub.cs
│   ├── PresenceHub.cs
│   └── TaskHub.cs
│
├── Middleware/                     # Custom middleware
│   └── WorkspaceAuthorizationMiddleware.cs
│
├── Middlewares/                    # (duplicate directory)
│   └── UserContext.cs              # HTTP context -> user
│
├── Persistence/                    # EF Migrations (API layer)
│   └── Migrations/
│       ├── 20260103071610_InitialCreate.cs
│       ├── 20260103071738_EnableRowLevelSecurity.cs
│       ├── 20260103071908_SeedRolesAndPermissions.cs
│       ├── 20260103171029_AddRealtimeCollaborationTables.cs
│       ├── 20260104112014_AddDocumentTables.cs
│       ├── 20260105165809_AddGoalTrackingTables.cs
│       ├── 20260106184122_AddClickUpHierarchyTables.cs
│       └── AppDbContextModelSnapshot.cs
│
├── Services/                       # Real-time services
│   ├── NotificationService.cs
│   └── PresenceService.cs
│
├── Program.cs                      # App entry point (247 lines)
├── appsettings.json
└── appsettings.Development.json
```

**Endpoint Pattern Example:**
```csharp
public static void MapWorkspaceEndpoints(this IEndpointRouteBuilder app)
{
    var group = app.MapGroup("/api/workspaces")
        .WithTags("Workspaces")
        .WithOpenApi();

    group.MapPost("/", async (CreateWorkspaceRequest request, ISender sender) =>
    {
        var command = new CreateWorkspaceCommand(/* ... */);
        var result = await sender.Send(command);
        return result.IsFailure 
            ? Results.BadRequest(new { error = result.Error })
            : Results.Created($"/api/workspaces/{result.Value.Id}", result.Value);
    });
}
```

**Statistics:**
- 11 endpoint files
- 7 migrations (ClickUp hierarchy latest)
- 3 SignalR hubs
- ~12,000 lines of API code (including migrations)

### 5. Test Layer (`tests/Nexora.Management.Tests`)

```
Tests/
└── UnitTest1.cs                    # Placeholder (10 lines)
```

**Note:** Test coverage minimal - only placeholder test file exists.

## ClickUp Hierarchy Implementation

### Hierarchy Structure
```
Workspace (top level)
  └─ Space (workspace subdivision)
      ├─ TaskList (direct in space)
      └─ Folder (optional grouping)
          └─ TaskList (in folder)
              └─ Task (individual work item)
```

### Related Files

**Entities:**
- `/Domain/Entities/Workspace.cs`
- `/Domain/Entities/Space.cs`
- `/Domain/Entities/Folder.cs`
- `/Domain/Entities/TaskList.cs`
- `/Domain/Entities/Task.cs`

**CQRS Commands/Queries:**
- `/Application/Workspaces/` - 3 commands, 2 queries
- `/Application/Spaces/` - 3 commands, 2 queries
- `/Application/Folders/` - 4 commands, 2 queries
- `/Application/TaskLists/` - 4 commands, 2 queries
- `/Application/Tasks/` - 4 commands, 5 queries (including views)

**API Endpoints:**
- `/API/Endpoints/WorkspaceEndpoints.cs`
- `/API/Endpoints/SpaceEndpoints.cs`
- `/API/Endpoints/FolderEndpoints.cs`
- `/API/Endpoints/TaskListEndpoints.cs`
- `/API/Endpoints/TaskEndpoints.cs`

**Migration:**
- `/API/Persistence/Migrations/20260106184122_AddClickUpHierarchyTables.cs` (484 lines)

### Migration Scripts

**Location:** `/scripts/`
- `MigrateProjectsToTaskLists.sql` (5,864 bytes)
- `MigrateTasksToTaskLists.sql` (7,797 bytes)
- `RollbackMigration.sql` (6,686 bytes)
- `ValidateMigration.sql` (9,298 bytes)

**Documentation:** `/docs/migration/`
- `MIGRATION_README.md` (10,257 bytes)
- `ROLLBACK_PROCEDURES.md` (11,319 bytes)

## Recent Changes (Based on Git History)

### Latest Commits (2026-01-01 to 2026-01-09)

1. **cac2734** - feat(backend): Phase 03 & 04 - API Endpoints & CQRS Complete
2. **711750d** - feat(backend): Phase 2 - Database Migration for ClickUp Hierarchy
3. **3c9bf4a** - feat(backend): Phase 3 - Add ClickUp hierarchy API endpoints
4. **bfab22f** - feat(backend): Phase 2 - Add ClickUp hierarchy database migration
5. **190875d** - feat(backend): Phase 01 - ClickUp Hierarchy Entity Design

### Key Features Added

**Phase 08:** Goal Tracking & OKRs
- Period, Objective, KeyResult entities
- 9 commands, 4 queries
- Progress dashboard

**Phase 07:** Document Management
- Page, PageVersion, PageCollaborator entities
- 6 commands, 4 queries
- Version history, favorites

**Phase 06:** Real-time Collaboration
- SignalR hubs (Notification, Presence, Task)
- UserPresence entity
- Real-time DTOs

**Phase 04-05:** Multiple Task Views
- Board view (Kanban)
- Calendar view
- Gantt view
- List view with pagination

## Statistics Summary

### File Counts (Excluding obj/bin)

| Layer | C# Files | Lines (approx) |
|-------|----------|----------------|
| Domain | 27 | ~1,500 |
| Application | 92 | ~9,594 |
| Infrastructure | 25 | ~3,000 |
| API | 40+ | ~12,000 |
| **Total** | **203** | **~24,790** |

### Breakdown by Type

| Type | Count |
|------|-------|
| Entities | 27 |
| Commands | 38+ |
| Queries | 21+ |
| DTOs | 15+ |
| Endpoints | 11 |
| Migrations | 7 |
| Configurations | 25 |
| Hubs | 3 |

### Largest Files (Non-Migration)

1. `GoalEndpoints.cs` - 379 lines
2. `TaskEndpoints.cs` - 280 lines
3. `DocumentEndpoints.cs` - 226 lines
4. `SpaceEndpoints.cs` - 205 lines
5. `Program.cs` - 247 lines

## Configuration Files

### Project Files
- `Nexora.Management.sln` - Solution file
- `src/Nexora.Management.API/Nexora.Management.API.csproj`
- `src/Nexora.Management.Application/Nexora.Management.Application.csproj`
- `src/Nexora.Management.Domain/Nexora.Management.Domain.csproj`
- `src/Nexora.Management.Infrastructure/Nexora.Management.Infrastructure.csproj`
- `tests/Nexora.Management.Tests/Nexora.Management.Tests.csproj`

### App Settings
- `src/Nexora.Management.API/appsettings.json`
- `src/Nexora.Management.API/appsettings.Development.json`

## Database Schema

### Migrations (Chronological)

1. **InitialCreate** - Base schema (users, roles, permissions)
2. **EnableRowLevelSecurity** - RLS policies
3. **SeedRolesAndPermissions** - Initial data
4. **AddRealtimeCollaborationTables** - SignalR support
5. **AddDocumentTables** - Page management
6. **AddGoalTrackingTables** - OKRs
7. **AddClickUpHierarchyTables** - Workspace→Space→Folder→TaskList→Task

## Key Design Patterns

### 1. CQRS with MediatR
- Separate commands (write) and queries (read)
- Handlers implement `IRequestHandler<TRequest, TResponse>`
- Result pattern for error handling

### 2. Repository Pattern (via IAppDbContext)
- Abstracted DbContext in Infrastructure
- Direct DbSet access in Application layer
- No explicit repository classes (simplified)

### 3. Dependency Injection
- Scoped services (DbContext, Handlers)
- Singleton services (JWT settings, SignalR)
- ISender for MediatR dispatch

### 4. Minimal API
- Endpoint routing (no controllers)
- Extension methods for organization
- OpenAPI/Swagger integration

### 5. Clean Architecture
- Domain: No dependencies
- Application: Depends on Domain
- Infrastructure: Implements Application interfaces
- API: Orchestrates all layers

## Unresolved Questions

1. **Test Coverage:** Only placeholder test exists. Need unit/integration tests?
2. **Validation:** FluentValidation mentioned in README but no validator files found.
3. **WorkspaceAuthorizationMiddleware:** Implementation location unclear (Middleware/ vs Middlewares/).
4. **Class1.cs:** Temporary files in Application/Domain/Infrastructure not removed.
5. **UserContext:** How is current user resolved in commands? (UserContext.cs vs IUserContext.cs)
6. **Project Entity:** Marked as legacy but still in schema. Migration incomplete?
7. **Handler Registration:** Are handlers auto-registered with MediatR or manual?

## Recommendations

1. **Remove Class1.cs** placeholder files from Domain, Application, Infrastructure
2. **Consolidate middleware directories** (Middleware/ vs Middlewares/)
3. **Add FluentValidation validators** for commands/queries
4. **Implement test coverage** (currently 1 placeholder test)
5. **Document UserContext resolution** in commands
6. **Complete Project entity deprecation** or clarify coexistence with TaskList
7. **Add API versioning** for future breaking changes

## Conclusion

Well-structured Clean Architecture implementation with comprehensive ClickUp hierarchy support. CQRS pattern consistently applied. Strong separation of concerns. Migration from Projects→TaskLists in progress with rollback procedures. Test coverage gap identified as primary area for improvement.

**Report Generated:** 2026-01-09  
**Agent:** scout-external (a45525d)
