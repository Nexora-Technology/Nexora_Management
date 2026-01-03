# System Architecture

**Last Updated:** 2026-01-03
**Version:** Phase 04 Complete (Task Management Core)

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

### Components

#### Entities (14 Domain Models)

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
├── Project
├── Task
├── TaskStatus
├── Comment
├── Attachment
└── ActivityLog
```

**Key Entities:**

1. **User**
   - Authentication and user profile
   - Relationships: Workspaces (owned), Projects (owned), Tasks (assigned), Comments, Attachments

2. **Workspace**
   - Top-level container for multi-tenancy
   - JSONB settings for flexible configuration
   - Owner and members relationship

3. **Project**
   - Workspace-scoped project management
   - Properties: Name, Description, Color, Icon, Status
   - Tasks and TaskStatus relationships

4. **Task**
   - Hierarchical tasks (parent-child via `ParentTaskId`)
   - Flexible scheduling (StartDate, DueDate)
   - Time tracking (EstimatedHours)
   - Priority levels (low, medium, high, urgent)
   - JSONB custom fields for extensibility
   - Position ordering for drag-and-drop

5. **Comment**
   - Threaded comments (self-referencing via `ParentCommentId`)
   - Associated with Tasks and Users

6. **Attachment**
   - File attachments for tasks
   - Metadata: FileName, FilePath, FileSizeBytes, MimeType

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
    // 13 DbSets for all entities
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    // ... (11 more)

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

#### EF Core Configurations (14 Files)

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
}
```

**Purpose:** Dependency inversion - allows mocking DbContext in tests

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

### Future Components (Planned)

- **Commands:** Create, Update, Delete operations for other domains
- **Queries:** Read operations with complex business logic
- **Validation:** FluentValidation rules
- **Mapping:** AutoMapper profiles

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

4. **Swagger/OpenAPI:**

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

6. **MediatR Registration:**

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

**Middleware Pipeline:**

1. Swagger (Development only)
2. HTTPS Redirection
3. CORS
4. Authentication
5. Authorization
6. Map Controllers

**Endpoints:**

- `GET /` - Welcome message with API info
- `GET /health` - Health check endpoint
- `/swagger` - Swagger UI (root in development)
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Token refresh
- `POST /api/tasks` - Create task
- `GET /api/tasks/{id}` - Get task by ID
- `GET /api/tasks` - List tasks with filters
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task

#### Database Migrations

**Location:** `/Persistence/Migrations/`

**Three Migration Files:**

1. **InitialCreate (20260103071610)**
   - Creates all 14 tables
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

**Model Snapshot:**

- `AppDbContextModelSnapshot.cs` - Current EF Core model state

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
 └─ AssignedTasks → Task → Project → Workspace
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

- Workspace → Projects, Tasks, Comments, Attachments
- Project → Tasks, TaskStatuses
- Task → Comments, Attachments
- Role → UserRoles, RolePermissions

### Indexing Strategy

**Unique Indexes:**

- `Users.Email`
- `Roles.Name`
- `TaskStatuses.ProjectId, OrderIndex`

**Foreign Key Indexes:**

- All foreign keys have indexes for JOIN performance

**Composite Indexes:**

- `Tasks.ProjectId, StatusId, PositionOrder` (task list queries)
- `ActivityLog.EntityType, EntityId, CreatedAt` (activity feed)
- `ActivityLog.WorkspaceId, CreatedAt` (workspace activity)

**GIN Indexes:**

- `Tasks.CustomFieldsJsonb` (JSONB queries)

**Filtered Indexes:**

- `Tasks.AssigneeId` WHERE assignee_id IS NOT NULL
- `Tasks.DueDate` WHERE due_date IS NOT NULL
- `Projects.WorkspaceId` WHERE status = 'active'

## Security Architecture

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

2. **Policy Example (Tasks SELECT):**
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

## Data Flow

### Example: Create Task

```
1. HTTP POST /api/tasks
   ↓ (API Layer)
2. TaskController.CreateCommand()
   ↓ (MediatR)
3. CreateTaskCommandHandler()
   ↓ (Application Layer)
4. Validates business rules
   ↓
5. Creates Task entity
   ↓
6. _taskRepository.Add(task)
   ↓ (Infrastructure Layer)
7. AppDbContext.Tasks.Add(task)
   ↓
8. AppDbContext.SaveChangesAsync()
   ↓ (EF Core)
9. Sets CreatedAt, UpdatedAt
   ↓
10. Generates SQL INSERT
    ↓ (PostgreSQL)
11. Executes INSERT with RLS check
    ↓
12. Returns new Task with Id
    ↓ (Back up the stack)
13. ApiResponse<Task> with Success=true
    ↓
14. HTTP 201 Created with task JSON
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
Backend API (.NET) :5000
    ↓ SQL
PostgreSQL :5432
```

### Docker Compose (Planned)

```yaml
services:
  frontend:
    image: nexora-frontend
    ports: ['3000:3000']

  backend:
    image: nexora-backend
    ports: ['5000:5000']
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;...

  postgres:
    image: postgres:16
    ports: ['5432:5432']
    volumes:
      - postgres_data:/var/lib/postgresql/data
```

## Performance Considerations

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

**Documentation Version:** 1.0
**Maintained By:** Development Team
