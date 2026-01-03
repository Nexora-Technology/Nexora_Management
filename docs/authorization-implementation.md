# Authorization Implementation Guide

**Last Updated:** 2026-01-03
**Version:** Phase 03 Complete (Authentication & Authorization)

## Overview

This document provides a comprehensive guide to the authorization system implemented in Phase 03, including permission-based access control, Row-Level Security (RLS), and best practices for securing endpoints.

## Architecture Overview

The authorization system consists of three complementary layers:

1. **Authentication Layer** - JWT-based user authentication (verify WHO the user is)
2. **Authorization Layer** - Permission-based access control (verify WHAT the user can do)
3. **Data Layer** - Row-Level Security (enforce data isolation at database level)

```
Request → JWT Validation → Permission Check → RLS Enforcement → Business Logic
          (Who are you?)    (Can you do this?)  (Which data?)      (Execute)
```

## Components

### 1. Permission-Based Authorization

#### PermissionRequirement

**Location:** `apps/backend/src/Nexora.Management.Application/Authorization/PermissionAuthorizationHandler.cs`

```csharp
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Resource { get; }
    public string Action { get; }

    public PermissionRequirement(string resource, string action)
    {
        Resource = resource;
        Action = action;
    }
}
```

Represents a permission requirement in the format `{resource}:{action}`.

#### PermissionAuthorizationHandler

**Location:** `apps/backend/src/Nexora.Management.Application/Authorization/PermissionAuthorizationHandler.cs`

**Purpose:** Validates permissions against user's roles in their workspace context.

**Key Features:**

- Executes raw SQL query for efficient permission lookup
- Validates permission format to prevent SQL injection
- Checks user's roles across all workspace memberships
- Registered as Scoped service for DbContext resolution

**SQL Query:**

```sql
SELECT EXISTS (
    SELECT 1
    FROM "Users" u
    JOIN "WorkspaceMembers" wm ON u."Id" = wm."UserId"
    JOIN "Roles" r ON wm."RoleId" = r."Id"
    JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
    JOIN "Permissions" p ON rp."PermissionId" = p."Id"
    WHERE u."Id" = {0}
    AND p."Name" = {1}
    LIMIT 1
)
```

**Permission Format Validation:**

```csharp
private static bool IsValidPermissionFormat(string value)
{
    if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
        return false;

    // Only allow alphanumeric, colon, hyphen, underscore
    return value.All(c => char.IsLetterOrDigit(c) || c == ':' || c == '-' || c == '_');
}
```

#### PermissionAuthorizationPolicyProvider

**Location:** `apps/backend/src/Nexora.Management.Application/Authorization/PermissionAuthorizationHandler.cs`

**Purpose:** Dynamically generates authorization policies at runtime.

**Policy Format:** `Permission:{resource}:{action}`

**Example:**

```csharp
[RequirePermission("tasks", "create")]
// Generates policy: "Permission:tasks:create"
```

**Registration (Program.cs):**

```csharp
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
```

#### RequirePermissionAttribute

**Location:** `apps/backend/src/Nexora.Management.Application/Authorization/RequirePermissionAttribute.cs`

**Usage:** Apply to endpoints or controllers to require specific permissions.

**Examples:**

```csharp
// Method-level authorization
[RequirePermission("tasks", "create")]
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskRequest request)
{
    // Only users with "tasks:create" permission can access
}

// Class-level authorization
[RequirePermission("projects", "manage")]
public class ProjectController : ControllerBase
{
    // All endpoints require "projects:manage" permission
}

// Multiple permissions (AND logic)
[RequirePermission("tasks", "update")]
[RequirePermission("tasks", "assign")]
public async Task<IActionResult> AssignTask(Guid taskId, Guid userId)
{
    // User must have BOTH "tasks:update" AND "tasks:assign"
}
```

### 2. Workspace Authorization Middleware

#### WorkspaceAuthorizationMiddleware

**Location:** `apps/backend/src/Nexora.Management.API/Middleware/WorkspaceAuthorizationMiddleware.cs`

**Purpose:** Sets user context for Row-Level Security (RLS) in PostgreSQL.

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

**Registration (Program.cs):**

```csharp
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkspaceAuthorization(); // MUST be after UseAuthorization
```

**Key Features:**

- Extracts user ID from JWT claims
- Sets PostgreSQL session variable `app.current_user_id`
- Executes `SET LOCAL` for request-scoped context
- Enables RLS policies to filter queries automatically

### 3. Raw SQL Execution Support

#### IAppDbContext Extensions

**Location:** `apps/backend/src/Nexora.Management.Infrastructure/Interfaces/IAppDbContext.cs`

**New Methods:**

```csharp
public interface IAppDbContext
{
    // Existing DbSets and SaveChangesAsync...

    // For raw SQL execution (needed for RLS and authorization queries)
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);

    // For raw SQL queries with return type
    Task<List<T>> SqlQueryRawAsync<T>(string sql, params object[] parameters);

    // For raw SQL query that returns single result
    Task<T> SqlQuerySingleAsync<T>(string sql, params object[] parameters);
}
```

**Implementation (AppDbContext.cs):**

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

**Usage Examples:**

```csharp
// RLS user context (WorkspaceAuthorizationMiddleware)
await db.ExecuteSqlRawAsync(
    "SET LOCAL app.current_user_id = {0}", userId);

// Permission check (PermissionAuthorizationHandler)
var hasPermission = await db.SqlQuerySingleAsync<bool>(
    """
    SELECT EXISTS (
        SELECT 1 FROM "Users" u
        JOIN "WorkspaceMembers" wm ON u."Id" = wm."UserId"
        JOIN "Roles" r ON wm."RoleId" = r."Id"
        JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
        JOIN "Permissions" p ON rp."PermissionId" = p."Id"
        WHERE u."Id" = {0}
        AND p."Name" = {1}
    )
    """,
    userId,
    "tasks:create"
);

// Custom query example
var taskCount = await db.SqlQuerySingleAsync<int>(
    """
    SELECT COUNT(*)
    FROM "Tasks" t
    JOIN "Projects" p ON t."ProjectId" = p."Id"
    WHERE p."WorkspaceId" = {0}
    """,
    workspaceId
);
```

## Row-Level Security (RLS)

### Database Function

**Migration:** `20260103071738_EnableRowLevelSecurity`

```sql
CREATE FUNCTION set_current_user_id(user_id UUID)
RETURNS VOID AS $$
BEGIN
    PERFORM set_config('app.current_user_id', user_id::TEXT, true);
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;
```

### RLS Policies

**Tasks Table (Example):**

```sql
-- Enable RLS
ALTER TABLE "Tasks" ENABLE ROW LEVEL SECURITY;

-- SELECT Policy
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

-- INSERT Policy
CREATE POLICY tasks_insert_policy ON "Tasks"
FOR INSERT
WITH CHECK (
    "ProjectId" IN (
        SELECT "Id" FROM "Projects"
        WHERE "WorkspaceId" IN (
            SELECT "WorkspaceId" FROM "WorkspaceMembers"
            WHERE "UserId" = current_setting('app.current_user_id', true)::UUID
        )
    )
);

-- UPDATE Policy
CREATE POLICY tasks_update_policy ON "Tasks"
FOR UPDATE
USING (
    "ProjectId" IN (
        SELECT "Id" FROM "Projects"
        WHERE "WorkspaceId" IN (
            SELECT "WorkspaceId" FROM "WorkspaceMembers"
            WHERE "UserId" = current_setting('app.current_user_id', true)::UUID
        )
    )
);

-- DELETE Policy
CREATE POLICY tasks_delete_policy ON "Tasks"
FOR DELETE
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

### Protected Tables

- **Tasks** (4 policies: SELECT, INSERT, UPDATE, DELETE)
- **Projects** (4 policies)
- **Comments** (4 policies)
- **Attachments** (3 policies: SELECT, INSERT, DELETE)
- **ActivityLog** (1 policy: SELECT)

### Unprotected Tables

- **Users** (authentication layer handles access)
- **Roles**, **Permissions** (static system data)
- **WorkspaceMembers**, **UserRoles**, **RolePermissions** (junction tables)

## Permission System

### Permission Seeding

**Migration:** `20260103071908_SeedRolesAndPermissions`

**Base Permissions:**

```sql
-- Base permissions for each resource
INSERT INTO "Permissions" ("Name", "Description") VALUES
('users:create', 'Create new users'),
('users:read', 'View user information'),
('users:update', 'Update user information'),
('users:delete', 'Delete users'),
('workspaces:create', 'Create workspaces'),
('workspaces:read', 'View workspaces'),
('workspaces:update', 'Update workspaces'),
('workspaces:delete', 'Delete workspaces'),
('workspaces:manage', 'Manage workspace settings'),
('projects:create', 'Create projects'),
('projects:read', 'View projects'),
('projects:update', 'Update projects'),
('projects:delete', 'Delete projects'),
('projects:manage', 'Manage project settings'),
('tasks:create', 'Create tasks'),
('tasks:read', 'View tasks'),
('tasks:update', 'Update tasks'),
('tasks:delete', 'Delete tasks'),
('tasks:assign', 'Assign tasks to users'),
('comments:create', 'Create comments'),
('comments:read', 'View comments'),
('comments:update', 'Update own comments'),
('comments:delete', 'Delete own comments'),
('attachments:create', 'Upload attachments'),
('attachments:read', 'View attachments'),
('attachments:delete', 'Delete own attachments');
```

**Roles:**

- **Admin** - All permissions
- **Member** - Read + Create + Update permissions (no delete)
- **Guest** - Read-only permissions

### Adding New Permissions

**Step 1:** Add permission to database

```sql
INSERT INTO "Permissions" ("Name", "Description") VALUES
('reports:generate', 'Generate workspace reports');
```

**Step 2:** Assign to roles

```sql
-- Assign to Admin role
INSERT INTO "RolePermissions" ("RoleId", "PermissionId")
SELECT r."Id", p."Id"
FROM "Roles" r, "Permissions" p
WHERE r."Name" = 'Admin' AND p."Name" = 'reports:generate';
```

**Step 3:** Use in endpoint

```csharp
[RequirePermission("reports", "generate")]
[HttpGet("reports")]
public async Task<IActionResult> GenerateReport()
{
    // Only users with "reports:generate" permission
}
```

## Best Practices

### 1. Always Use RequirePermission

```csharp
// Good
[RequirePermission("tasks", "create")]
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskRequest request)
{
    return await _mediator.Send(command);
}

// Bad - No authorization
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskRequest request)
{
    return await _mediator.Send(command);
}
```

### 2. Use Specific Resource-Action Pairs

```csharp
// Good - Granular permissions
[RequirePermission("tasks", "create")]
[HttpPost]
public async Task<IActionResult> CreateTask(...) { }

[RequirePermission("tasks", "update")]
[HttpPut]
public async Task<IActionResult> UpdateTask(...) { }

[RequirePermission("tasks", "delete")]
[HttpDelete]
public async Task<IActionResult> DeleteTask(...) { }

// Acceptable - Broader permission
[RequirePermission("tasks", "manage")]
public class TaskManagementController : ControllerBase
{
    // All CRUD operations
}
```

### 3. Never Bypass RLS

```csharp
// Good - Let RLS filter data
var tasks = await _db.Tasks
    .Where(t => t.ProjectId == projectId)
    .ToListAsync(); // RLS automatically filters by workspace

// Bad - Attempting to bypass RLS
await _db.ExecuteSqlRawAsync(
    "SET LOCAL app.current_user_id = {0}", adminUserId); // Don't do this!
```

### 4. Use Parameterized Queries

```csharp
// Good - Parameterized
await db.SqlQuerySingleAsync<bool>(
    """
    SELECT EXISTS (
        SELECT 1 FROM "Users" WHERE "Id" = {0}
    )
    """,
    userId
);

// Bad - String concatenation (SQL injection risk)
var sql = $"SELECT * FROM \"Users\" WHERE \"Id\" = '{userId}'";
await db.ExecuteSqlRawAsync(sql);
```

### 5. Validate Permission Formats

When creating permissions programmatically:

```csharp
private bool IsValidPermissionFormat(string resource, string action)
{
    var validChars = new HashSet<char>(
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789:-_".ToArray());

    return !string.IsNullOrWhiteSpace(resource)
        && !string.IsNullOrWhiteSpace(action)
        && resource.Length <= 50
        && action.Length <= 50
        && resource.All(validChars.Contains)
        && action.All(validChars.Contains);
}
```

## Security Considerations

### SQL Injection Protection

The permission format validation in `PermissionAuthorizationHandler` prevents SQL injection:

```csharp
private static bool IsValidPermissionFormat(string value)
{
    if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
        return false;

    // Only allow alphanumeric, colon, hyphen, underscore
    return value.All(c => char.IsLetterOrDigit(c) || c == ':' || c == '-' || c == '_');
}
```

### Permission Caching

**Current Implementation:** No permission caching - always queries database.

**Rationale:**
- Permissions can change dynamically
- Users can have different roles in different workspaces
- Ensures always-fresh permission checks
- Database queries are optimized with indexes

**Future Enhancement:** Consider short-lived caching (1-2 minutes) if performance issues arise.

### Workspace Context

Permissions are evaluated in the context of a user's workspace membership:

```sql
-- User can have different roles in different workspaces
SELECT wm."WorkspaceId", r."Name" as RoleName, p."Name" as PermissionName
FROM "WorkspaceMembers" wm
JOIN "Roles" r ON wm."RoleId" = r."Id"
JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
JOIN "Permissions" p ON rp."PermissionId" = p."Id"
WHERE wm."UserId" = {0}
```

This means a user can be an Admin in Workspace A and a Guest in Workspace B.

## Testing Authorization

### Unit Testing Permission Checks

```csharp
public class PermissionAuthorizationHandlerTests
{
    [Fact]
    public async Task HandleRequirementAsync_UserHasPermission_Succeeds()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var db = CreateTestDbContext();
        await SeedUserWithPermission(db, user.Id, "tasks:create");

        var handler = new PermissionAuthorizationHandler(db);
        var requirement = new PermissionRequirement("tasks", "create");
        var context = CreateAuthorizationContext(user);

        // Act
        await handler.HandleRequirementAsync(context, requirement);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }
}
```

### Integration Testing RLS

```csharp
[Fact]
public async Task GetTasks_UserOnlySeesWorkspaceTasks()
{
    // Arrange
    var user1 = await CreateUserInWorkspace("Workspace A");
    var user2 = await CreateUserInWorkspace("Workspace B");

    await CreateTaskInWorkspace("Task 1", "Workspace A");
    await CreateTaskInWorkspace("Task 2", "Workspace B");

    // Act
    var user1Tasks = await GetUserTasks(user1.Id);
    var user2Tasks = await GetUserTasks(user2.Id);

    // Assert
    user1Tasks.Should().ContainSingle(t => t.Title == "Task 1");
    user1Tasks.Should().NotContain(t => t.Title == "Task 2");

    user2Tasks.Should().ContainSingle(t => t.Title == "Task 2");
    user2Tasks.Should().NotContain(t => t.Title == "Task 1");
}
```

## Troubleshooting

### Permission Denied Issues

**Problem:** User gets 403 Forbidden despite having the role.

**Debug Steps:**

1. **Check User's Roles:**

```sql
SELECT u."Email", r."Name" as RoleName, w."Name" as WorkspaceName
FROM "Users" u
JOIN "WorkspaceMembers" wm ON u."Id" = wm."UserId"
JOIN "Roles" r ON wm."RoleId" = r."Id"
JOIN "Workspaces" w ON wm."WorkspaceId" = w."Id"
WHERE u."Email" = 'user@example.com';
```

2. **Check Role Permissions:**

```sql
SELECT r."Name" as RoleName, p."Name" as PermissionName
FROM "Roles" r
JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
JOIN "Permissions" p ON rp."PermissionId" = p."Id"
WHERE r."Name" = 'Member';
```

3. **Verify Permission Format:**

```csharp
// Should be: "tasks:create" (lowercase, colon-separated)
[RequirePermission("tasks", "create")]
```

### RLS Not Filtering Data

**Problem:** User sees data from other workspaces.

**Debug Steps:**

1. **Check Middleware Registration:**

```csharp
// Program.cs - MUST be in this order
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkspaceAuthorization(); // After UseAuthorization
```

2. **Verify User Context is Set:**

```sql
-- Add logging to middleware
Console.WriteLine($"Set app.current_user_id = {userId}");
```

3. **Test RLS Policy Directly:**

```sql
-- Set user context manually
SELECT set_current_user_id('user-uuid-here'::UUID);

-- Query should be filtered
SELECT * FROM "Tasks";

-- Reset
SELECT set_config('app.current_user_id', '', true);
```

## Migration Guide

### Adding Authorization to Existing Endpoints

**Before:**

```csharp
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskRequest request)
{
    var command = new CreateTaskCommand(request);
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

**After:**

```csharp
[RequirePermission("tasks", "create")] // Add this
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskRequest request)
{
    var command = new CreateTaskCommand(request);
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

### Creating New Permissions

**1. Database Migration:**

```sql
INSERT INTO "Permissions" ("Name", "Description") VALUES
('resource:action', 'Description of permission');
```

**2. Assign to Roles:**

```sql
INSERT INTO "RolePermissions" ("RoleId", "PermissionId")
SELECT r."Id", p."Id"
FROM "Roles" r, "Permissions" p
WHERE r."Name" = 'Admin' AND p."Name" = 'resource:action';
```

**3. Use in Code:**

```csharp
[RequirePermission("resource", "action")]
[HttpPost]
public async Task<IActionResult> DoSomething()
{
    // ...
}
```

## Performance Considerations

### Database Indexes

**Permission Query Indexes:**

```sql
-- WorkspaceMembers lookup
CREATE INDEX ix_workspace_members_user_id
ON "WorkspaceMembers" ("UserId");

-- RolePermissions lookup
CREATE INDEX ix_role_permissions_role_id
ON "RolePermissions" ("RoleId");

-- Permissions name lookup
CREATE INDEX ix_permissions_name
ON "Permissions" ("Name");
```

### Query Optimization

**Efficient Permission Check:**

```sql
-- Good - Single query with EXISTS
SELECT EXISTS (
    SELECT 1 FROM "Users" u
    JOIN "WorkspaceMembers" wm ON u."Id" = wm."UserId"
    JOIN "Roles" r ON wm."RoleId" = r."Id"
    JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
    JOIN "Permissions" p ON rp."PermissionId" = p."Id"
    WHERE u."Id" = {0} AND p."Name" = {1}
    LIMIT 1
);

-- Bad - Multiple queries or fetching all data
SELECT * FROM "Users" u
JOIN "WorkspaceMembers" wm ON u."Id" = wm."UserId"
JOIN "Roles" r ON wm."RoleId" = r."Id"
JOIN "RolePermissions" rp ON r."Id" = rp."RoleId"
JOIN "Permissions" p ON rp."PermissionId" = p."Id"
WHERE u."Id" = {0};
```

## Summary

The authorization system provides:

- **Declarative Authorization:** `[RequirePermission]` attribute
- **Dynamic Policy Generation:** No pre-registration needed
- **Workspace-Scoped Permissions:** Different roles per workspace
- **Database-Level Security:** RLS enforces data isolation
- **SQL Injection Protection:** Permission format validation
- **Efficient Queries:** Single-query permission checks
- **Raw SQL Support:** For RLS and custom queries

All endpoints must use `RequirePermission` for authorization, and RLS ensures data isolation at the database level.

---

**Documentation Version:** 1.0
**Maintained By:** Development Team
