# Research Report: ClickUp-like Project Management Platform Architecture

**Date:** 2026-01-03
**Focus:** Architecture, core features, and implementation patterns for .NET/React/PostgreSQL stack
**Target Scale:** <30 users

---

## 1. Core Feature Set

ClickUp positions as "everything app for work" combining:

### Essential Features
- **Tasks**: Break down projects into customizable tasks with status, priority, assignees, due dates
- **Projects**: Container for tasks with workflows, statuses, permissions
- **15+ Views**: List, Board, Gantt, Calendar, Timeline, Mind Map, Table, Workload
- **Docs**: Rich text documents connected to tasks (wiki-style)
- **Whiteboards**: Virtual collaborative whiteboards that connect to tasks/docs (v3.0 released Mar 2025)
- **Goals**: Strategic tracking aligned with work
- **Chat**: Team messaging integrated with tasks
- **Time Tracking**: Built-in time tracking for tasks
- **Calendar**: View tasks/events across projects
- **Dashboards**: Analytics and reporting views
- **Automations**: Workflow automation (if-this-then-that rules)
- **AI Features**: ClickUp Brain with schema generation, context-aware assistance (2024-2025)

### MVP Recommendation for <30 Users
Start with: Tasks, Projects, List/Board views, Docs, basic permissions. Defer: Whiteboards, advanced automations, AI features until validated need.

---

## 2. Database Schema Patterns

### Core Tables Structure

```sql
-- Authentication & Authorization
Users (id, email, password_hash, name, avatar_url, created_at, updated_at)
Roles (id, name, description, is_system)
Permissions (id, resource, action) -- e.g., "projects:create", "tasks:update"
User_Roles (user_id, role_id)
Role_Permissions (role_id, permission_id)

-- Workspace Organization
Workspaces (id, name, owner_id, settings_json, created_at)
Workspace_Members (workspace_id, user_id, role_id) -- Team membership

-- Projects & Tasks
Projects (id, workspace_id, name, description, status, owner_id, color_icon, created_at)
Task_Statuses (id, project_id, name, order, color, type) -- Custom statuses per project
Tasks (id, project_id, parent_task_id, title, description, status_id, priority, assignee_id, due_date, position_order, created_at)

-- Comments & Attachments
Comments (id, task_id, user_id, content, created_at, updated_at)
Attachments (id, task_id, user_id, file_url, file_name, file_size, mime_type, created_at)

-- Time Tracking
Time_Entries (id, task_id, user_id, start_time, end_time, duration_seconds, description, created_at)

-- Activity Logging
Activity_Log (id, workspace_id, user_id, entity_type, entity_id, action, changes_json, created_at)

-- Docs (if including MVP)
Documents (id, workspace_id, project_id, title, content_json, created_by, created_at, updated_at)
```

### Key Relationships
- **Workspace → Projects**: One-to-many (logical isolation)
- **Project → Tasks**: One-to-many with parent_task_id for subtasks
- **Task → Comments/Attachments**: One-to-many
- **User → Task**: Many-to-many through Task_Assignees (for multiple assignees)
- **User → Workspace**: Many-to-many through Workspace_Members with roles

### PostgreSQL-Specific Optimizations
- **JSONB columns**: For flexible settings, custom fields, activity changes
- **Indexing strategy** (see Section 7)
- **Row-Level Security (RLS)**: For multi-tenant data isolation

---

## 3. Architecture Patterns

### Recommendation: Modular Monolith

For **<30 users**, **modular monolith** is optimal over microservices:

**Why Monolith?**
- Simpler development/debugging (single codebase)
- Faster time-to-market (no distributed system complexity)
- Lower operational overhead (one deployment, one database)
- Better performance (no network latency between services)
- Easier testing (straightforward integration tests)
- Single server easily handles 30 users (~100-500 concurrent requests)

**Modular Design Principles:**
- **Domain-driven boundaries**: Separate modules for Tasks, Projects, Users, Docs, etc.
- **Clean Architecture**: Layers (Presentation → Application → Domain → Infrastructure)
- **CQRS pattern**: Separate read/write operations for complex features (e.g., dashboards)
- **Event-driven internal**: Use domain events for module communication (not service calls)

### .NET Architecture Stack
```
Nexora.Management.API (Web API / GraphQL)
├── Controllers (REST endpoints)
├── GraphQL Schemas (if using GraphQL)
├── SignalR Hubs (real-time)
├── Middleware (auth, logging, exception handling)

Nexora.Management.Application (Business Logic)
├── Commands (write operations)
├── Queries (read operations)
├── DTOs
├── Validators (FluentValidation)

Nexora.Management.Domain (Core Business Rules)
├── Entities
├── Value Objects
├── Domain Events
├── Interfaces (repositories)

Nexora.Management.Infrastructure (External Concerns)
├── EF Core DbContext (PostgreSQL)
├── Repository Implementations
├── SignalR Service
├── Email Service
├── File Storage Service
```

### Microservices Migration Path
Start monolithic, extract microservices later if:
- Specific feature needs independent scaling (e.g., real-time service)
- Team grows beyond 10 developers
- Clear domain boundaries emerge

**Extraction candidates:** Real-time collaboration service, notification service, background job processor.

---

## 4. Scalability Considerations (<30 Users)

### Current Scale Requirements
- **30 users** = ~150-300 active tasks, ~10-20 projects
- **Concurrent load**: ~5-15 simultaneous users during peak
- **Data growth**: ~1-5 GB/year (attachments excluded)

### Performance Strategy

#### Database Level
1. **Connection Pooling**: EF Core default pool (max 100 connections sufficient)
2. **Read Replicas**: Not needed at this scale
3. **Caching**:
   - **In-memory cache**: User permissions, frequently accessed tasks
   - **Distributed cache**: Redis for session storage, real-time state (optional)
4. **Indexing**: Strategic indexes on foreign keys, filtered indexes on common queries

#### Application Level
1. **Async/Await**: All I/O operations
2. **Pagination**: All list queries (tasks, projects, comments)
3. **Lazy Loading**: Disabled in EF Core (use explicit Include/EagerLoading)
4. **Query Optimization**: Avoid N+1 queries, use projection for DTOs

#### Infrastructure
1. **Single server** (Azure App Service / AWS EC2) sufficient
2. **Managed PostgreSQL** (Azure Database / AWS RDS) with auto-scaling storage
3. **CDN**: For static assets (React bundle, attachments)
4. **Background jobs**: Hangfire for recurring tasks, email notifications

### When to Scale Up
- **100+ users**: Consider read replicas, dedicated cache server
- **500+ users**: Evaluate microservices extraction, database sharding

---

## 5. Security & Permissions

### Permission Model: Role-Based Access Control (RBAC)

#### Role Hierarchy
```
Workspace Owner (full control)
├── Admin (manage projects, members, settings)
├── Member (create tasks, comment, view assigned)
└── Guest (view-only, limited projects)
```

#### Permission Granularity
**Project-level permissions:**
- `projects:create` (create new projects)
- `projects:read` (view project)
- `projects:update` (edit project settings)
- `projects:delete` (archive/delete project)
- `projects:manage_members` (add/remove members)

**Task-level permissions:**
- `tasks:create` (create tasks in project)
- `tasks:read` (view tasks)
- `tasks:update` (edit task details)
- `tasks:delete` (delete tasks)
- `tasks:assign` (assign to others)
- `tasks:comment` (add comments)

**Custom permissions:**
- `time_tracking:log` (track time)
- `docs:create` (create documents)
- `docs:edit` (edit documents)

### Data Isolation Strategy

#### Option 1: Application-Level Filtering (Simpler)
- Filter queries by `workspace_id` in application layer
- Risk: Bugs could leak data across workspaces
- Suitable for: Single-tenant or low-security requirements

#### Option 2: PostgreSQL Row-Level Security (Recommended)
- **Database-enforced isolation**: RLS policies prevent cross-tenant access
- **Defense in depth**: Even if app has bugs, database blocks unauthorized access
- **Implementation**:

```sql
-- Enable RLS on Tasks table
ALTER TABLE Tasks ENABLE ROW LEVEL SECURITY;

-- Policy: Users can only see tasks in workspaces they're members of
CREATE POLICY tasks_isolation_policy ON Tasks
    FOR SELECT
    USING (
        workspace_id IN (
            SELECT workspace_id FROM Workspace_Members
            WHERE user_id = current_user_id()
        )
    );

-- Policy: Users can only create tasks in accessible workspaces
CREATE POLICY tasks_create_policy ON Tasks
    FOR INSERT
    WITH CHECK (
        workspace_id IN (
            SELECT workspace_id FROM Workspace_Members
            WHERE user_id = current_user_id()
        )
    );
```

- **Pros**: Security enforced at database level, no application bypass possible
- **Cons**: Slight performance overhead (~5-10%), need to set current user context

### Authentication Strategy
- **JWT tokens**: Stateless authentication, contains user_id, role_ids
- **Refresh tokens**: Secure storage in httpOnly cookies
- **API Key support**: For integrations (webhooks, external tools)
- **SSO ready**: Prepare for SAML/OAuth later (use IdentityServer4 or Duende)

### Security Best Practices
1. **Parameterized queries**: Prevent SQL injection (EF Core default)
2. **Input validation**: FluentValidation for all DTOs
3. **Rate limiting**: Per-user request limits (prevent abuse)
4. **CORS**: Restrict to frontend domain only
5. **HTTPS only**: Enforce in production
6. **Secrets management**: Azure Key Vault / AWS Secrets Manager

---

## 6. Real-Time Collaboration Needs

### Use Cases for Real-Time Features
1. **Task updates**: Live status changes, new comments, assignments
2. **Collaborative editing**: Real-time doc editing (like Google Docs)
3. **Presence indicators**: "User X is viewing Task Y"
4. **Notifications**: Instant push for @mentions, task assignments
5. **Dashboard updates**: Live charts/metrics refresh

### Technology: SignalR (Recommended for .NET)

**Why SignalR over raw WebSockets?**
- **Automatic fallback**: WebSocket → Server-Sent Events → Long Polling
- **Built-in reconnection**: Handles network interruptions gracefully
- **Group management**: Easy broadcast to project/task groups
- **Type-safe hubs**: Strongly-typed .NET interfaces
- **Integration**: Native .NET ecosystem support

### SignalR Architecture

```
Client (React/Next.js)
    ↓ (SignalR JavaScript client)
SignalR Hub (.NET)
    ↓ (broadcasts to groups)
Connected Clients
```

### Implementation Pattern

#### Hub Definition
```csharp
public class ProjectHub : Hub
{
    public async Task JoinProject(Guid projectId)
    {
        // Verify user has access to project
        if (await _authService.CanAccessProject(projectId, User.GetUserId()))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"project:{projectId}");
        }
    }

    public async Task LeaveProject(Guid projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project:{projectId}");
    }

    // Called when task is updated
    public async Task TaskUpdated(Guid projectId, TaskUpdatedEvent @event)
    {
        await Clients.Group($"project:{projectId}").SendAsync("TaskUpdated", @event);
    }
}
```

#### Scalability Considerations
- **Sticky sessions**: Required for SignalR (user stays on same server)
- **Backplane**: For multi-server deployments (Redis backplane)
- **Connection limits**: Single server handles ~10K concurrent connections
- **At 30 users**: Single server sufficient, no backplane needed

### Real-Time Editing Challenges (Advanced)

For collaborative doc editing (like Google Docs):

**Challenge 1: Conflict Resolution**
- **Operational Transform (OT)**: Complex, order-dependent
- **CRDTs (Conflict-free Replicated Data Types)**: Simpler, mathematically sound
  - Yjs is popular JS library
  - Automerge is another option
- **Recommendation**: Start with **last-write-wins**, upgrade to CRDTs if editing conflicts become problematic

**Challenge 2: Performance**
- **Throttle updates**: Send changes every 100-500ms, not every keystroke
- **Operational batching**: Combine multiple edits into one message
- **Differential sync**: Send only changed text, not entire document

**Challenge 3: Reconnection**
- **Buffer changes**: Queue edits while disconnected
- **Conflict detection**: Merge when reconnecting
- **Version vectors**: Track document state across clients

### Presence Tracking
```csharp
// Track active users per project
public class PresenceTracker
{
    private readonly ConcurrentDictionary<Guid, HashSet<string>> _projectUsers = new();

    public void UserConnected(Guid projectId, string connectionId, string userId)
    {
        _projectUsers.AddOrUpdate(projectId,
            new HashSet<string> { userId },
            (key, existing) => { existing.Add(userId); return existing; });
    }

    public HashSet<string> GetUsersInProject(Guid projectId)
        => _projectUsers.GetValueOrDefault(projectId, new HashSet<string>());
}
```

---

## 7. Key Technical Challenges

### Challenge 1: Complex Dashboard Queries

**Problem**: Dashboards need aggregations (task counts by status, time tracking summaries, workload distribution) across multiple tables with filters.

**Symptoms**:
- Slow page load (>3 seconds)
- Expensive queries with multiple JOINs
- N+1 query problems

**Solutions**:

#### A. Materialized Views (Recommended)
```sql
CREATE MATERIALIZED VIEW project_dashboard_stats AS
SELECT
    p.id as project_id,
    COUNT(t.id) FILTER (WHERE t.status_id = 1) as todo_count,
    COUNT(t.id) FILTER (WHERE t.status_id = 2) as in_progress_count,
    COUNT(t.id) FILTER (WHERE t.status_id = 3) as done_count,
    SUM(te.duration_seconds) as total_time_tracked
FROM Projects p
LEFT JOIN Tasks t ON t.project_id = p.id
LEFT JOIN Time_Entries te ON te.task_id = t.id
GROUP BY p.id;

-- Refresh periodically (cron job)
REFRESH MATERIALIZED VIEW project_dashboard_stats;
```

**Pros**: Sub-millisecond query response, pre-computed aggregations
**Cons**: Slightly stale data (refresh every 5-15 minutes acceptable for dashboards)

#### B. Dedicated Read Model (CQRS)
Create separate "DashboardStats" table updated via domain events:

```csharp
// When task changes
public async Task Handle(TaskUpdatedEvent @event)
{
    var stats = await _context.DashboardStats.FindAsync(@event.ProjectId);
    stats.TodoCount = await _context.Tasks
        .CountAsync(t => t.ProjectId == @event.ProjectId && t.StatusId == 1);
    await _context.SaveChangesAsync();
}
```

**Pros**: Eventually consistent, optimized for reads
**Cons**: Complexity in maintaining sync

#### C. Optimized Queries with Indexes
```sql
-- Covering index for dashboard queries
CREATE INDEX idx_tasks_project_status
ON Tasks(project_id, status_id)
INCLUDE (id, assignee_id, due_date);

-- Partial index for active tasks only
CREATE INDEX idx_tasks_active
ON Tasks(project_id, assignee_id)
WHERE status_id != 4; -- Exclude archived
```

**Query optimization tips**:
- **Avoid CTEs for performance-critical queries**: CTEs are optimization fences in PostgreSQL
- **Use JOINs instead**: Query planner can optimize JOIN order
- **Projection**: Select only needed columns (no `SELECT *`)
- **Pagination**: Always use `LIMIT/OFFSET` or keyset pagination

---

### Challenge 2: Hierarchical Task Structures

**Problem**: Tasks with subtasks (nested) require recursive queries. Examples:
- Show all tasks in a project tree
- Calculate total time tracking (including subtasks)
- Move entire task subtree

**Solution 1: Adjacency List (Simple)**
```sql
Tasks (id, parent_task_id, title, ...)

-- Recursive CTE to get all descendants
WITH RECURSIVE task_tree AS (
    SELECT id, parent_task_id, title, 1 as level
    FROM Tasks
    WHERE id = @rootTaskId

    UNION ALL

    SELECT t.id, t.parent_task_id, t.title, tt.level + 1
    FROM Tasks t
    INNER JOIN task_tree tt ON t.parent_task_id = tt.id
)
SELECT * FROM task_tree;
```

**Pros**: Simple schema, easy inserts
**Cons**: Slow recursive queries for deep trees (>100 levels)

**Solution 2: Nested Set (Optimized for reads)**
```sql
Tasks (id, left_val, right_val, title, ...)

-- Get all descendants of task 5
SELECT * FROM Tasks
WHERE left_val > (SELECT left_val FROM Tasks WHERE id = 5)
AND right_val < (SELECT right_val FROM Tasks WHERE id = 5);
```

**Pros**: Fast descendants query (no recursion)
**Cons**: Complex inserts/updates (rebalance tree)

**Recommendation**: Start with adjacency list, switch to nested set if performance issues arise. Most project trees are shallow (<5 levels), making recursive CTEs acceptable.

---

### Challenge 3: Flexible Task Custom Fields

**Problem**: Different projects need different custom fields (e.g., "Bug" projects need "Severity", "Client" projects need "Budget Code").

**Solution 1: EAV (Entity-Attribute-Value)**
```sql
Task_Custom_Values (task_id, custom_field_id, value_text, value_number, value_date)
Custom_Fields (id, project_id, name, field_type) -- field_type: text, number, date, dropdown
```

**Pros**: Flexible, add fields without schema changes
**Cons**: Complex queries, poor performance, weak type safety

**Solution 2: JSONB Column (Recommended)**
```sql
Tasks (id, ..., custom_fields_jsonb)

-- Query custom field
SELECT * FROM Tasks
WHERE custom_fields_jsonb->>'severity' = 'critical';

-- Index on specific custom field
CREATE INDEX idx_tasks_custom_severity
ON Tasks((custom_fields_jsonb->>'severity'))
WHERE custom_fields_jsonb ? 'severity';
```

**Pros**: Flexible, decent performance with indexes, schemaless
**Cons**: Less type safety, need to validate in application layer

**Solution 3: Concrete Columns (Strict)**
```sql
ALTER TABLE Tasks ADD COLUMN custom_severity VARCHAR(20);
ALTER TABLE Tasks ADD COLUMN custom_budget_code VARCHAR(50);
```

**Pros**: Type-safe, fast queries, foreign keys work
**Cons**: Requires migration for each new field, sparse table

**Recommendation**: **JSONB columns** for MVP flexibility. Add concrete columns for high-use fields (e.g., "severity", "priority") if performance needed.

---

### Challenge 4: Data Migrations & Schema Evolution

**Problem**: Adding features requires schema changes without downtime.

**Migration Tools**:
- **FluentMigrator**: .NET migration framework with fluent API
- **Entity Framework Core Migrations**: Built-in, good for simple changes
- **RoundhousE**: SQL-file-based migrations (preferred for control)

**Zero-Downtime Migration Strategies**:

#### Adding Columns (Non-Breaking)
```sql
-- Step 1: Add nullable column
ALTER TABLE Tasks ADD COLUMN priority INT;

-- Deploy application code (handles NULL priority)

-- Step 2: Backfill data
UPDATE Tasks SET priority = 1 WHERE priority IS NULL;

-- Step 3: Add NOT NULL constraint
ALTER TABLE Tasks ALTER COLUMN priority SET NOT NULL;
```

#### Renaming Columns (Breaking)
```sql
-- DON'T do this in production (breaks running app)
-- ALTER TABLE Tasks RENAME COLUMN status TO status_id;

-- INSTEAD: Add new column, migrate, then drop old
ALTER TABLE Tasks ADD COLUMN new_status_id INT;
UPDATE Tasks SET new_status_id = status;
-- Deploy app using new_status_id
ALTER TABLE Tasks DROP COLUMN status;
```

#### Adding Indexes (Careful with Large Tables)
```sql
-- Adding index locks table (blocks writes)
CREATE INDEX idx_tasks_project ON Tasks(project_id);

-- INSTEAD: Create index CONCURRENTLY (non-blocking)
CREATE INDEX CONCURRENTLY idx_tasks_project ON Tasks(project_id);
```

**Best Practices**:
1. **Version control all migrations**: Never run manual SQL in production
2. **Test rollback procedures**: Every migration must be reversible
3. **Use transactions**: Wrap migrations in transaction blocks
4. **Back up before migration**: Automated snapshots
5. **Feature flags**: Deploy code behind feature flag, enable after migration completes

---

### Challenge 5: Attachment Storage & Performance

**Problem**: Storing file attachments (screenshots, PDFs, docs) without bloating database.

**Solution 1: Database Blob Storage (Not Recommended)**
```sql
-- Store files as BYTEA in PostgreSQL
Attachments (id, task_id, file_data BYTEA, file_name, mime_type)
```

**Pros**: Simple backup (database includes files), ACID compliance
**Cons**: Bloats database, slow backups, memory issues

**Solution 2: File System Storage (Simple)**
```
/var/nexora/uploads/{workspace_id}/{year}/{month}/{file_id}.png
```

Store file path in database:
```sql
Attachments (id, task_id, file_path VARCHAR(500), file_name, file_size_bytes, mime_type)
```

**Pros**: Serves via Nginx/Apache (fast), CDN-friendly
**Cons**: Need separate backup strategy, file permissions

**Solution 3: Cloud Storage (Recommended)**
- **Azure Blob Storage** / **AWS S3** / **Google Cloud Storage**
- Store URL/Key in database
- Use signed URLs for private access

**Pros**: Infinite scalability, built-in CDN, cheap, redundant
**Cons**: External dependency, costs

**Implementation**:
```csharp
public async Task<string> UploadAttachmentAsync(Stream fileStream, string fileName, string contentType)
{
    var blobId = Guid.NewGuid();
    var blobName = $"attachments/{blobId}/{fileName}";

    await _blobService.UploadAsync(blobName, fileStream, contentType);

    // Store metadata in database
    var attachment = new Attachment
    {
        Id = blobId,
        FileName = fileName,
        ContentType = contentType,
        StorageUrl = blobName, // or full URL
        UploadedAt = DateTime.UtcNow
    };

    _context.Attachments.Add(attachment);
    await _context.SaveChangesAsync();

    return _blobService.GetPresignedUrl(blobName, expiresIn: TimeSpan.FromHours(1));
}
```

---

## 8. Recommended Tech Stack

### Backend (.NET 8+)
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core 8.0 (PostgreSQL provider: Npgsql)
- **Authentication**: JWT Bearer tokens (System.IdentityModel.Tokens.Jwt)
- **Real-time**: SignalR for WebSocket connections
- **Validation**: FluentValidation
- **Logging**: Serilog with structured logging (seq, Elasticsearch)
- **Background jobs**: Hangfire
- **API Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Testing**: xUnit, Moq, FluentAssertions

### Database (PostgreSQL 16+)
- **Managed service**: Azure Database for PostgreSQL / AWS RDS
- **Migrations**: FluentMigrator or EF Core Migrations
- **Connection pooling**: Npgsql connection pool
- **Monitoring**: pg_stat_statements, pgHero
- **Backup**: Point-in-time recovery (managed service feature)

### Frontend (React 18 / Next.js 14)
- **Framework**: Next.js 14 (App Router)
- **State management**: Zustand or React Query (TanStack Query)
- **UI components**: shadcn/ui, Radix UI primitives
- **Real-time**: @microsoft/signalr (JavaScript client)
- **Forms**: React Hook Form + Zod validation
- **Data tables**: TanStack Table (React Table)
- **Rich text editor**: Tiptap or Slate
- **File upload**: React Dropzone
- **Charts**: Recharts or Chart.js
- **Styling**: Tailwind CSS

### DevOps & Infrastructure
- **Hosting**: Azure App Service / AWS EC2 or ECS
- **CI/CD**: GitHub Actions or Azure DevOps
- **Containerization**: Docker for local development
- **Reverse proxy**: YARP (Yet Another Reverse Proxy) or Nginx
- **Monitoring**: Application Insights (Azure) or CloudWatch (AWS)
- **Error tracking**: Sentry
- **Performance monitoring**: New Relic or Datadog (optional at this scale)

---

## 9. Implementation Roadmap (MVP)

### Phase 1: Foundation (2-3 weeks)
- [ ] Database schema setup (Users, Workspaces, Projects, Tasks)
- [ ] Authentication & authorization (JWT, RBAC)
- [ ] Basic CRUD APIs (Projects, Tasks)
- [ ] Simple React dashboard with task list

### Phase 2: Core Features (3-4 weeks)
- [ ] Task details view (comments, attachments)
- [ ] Board view (Kanban-style)
- [ ] Task status workflow (custom statuses per project)
- [ ] User/team management (invite members, assign roles)
- [ ] File upload (cloud storage)

### Phase 3: Collaboration (2-3 weeks)
- [ ] Real-time updates (SignalR integration)
- [ ] Presence indicators ("User X is viewing")
- [ ] Comment threading with @mentions
- [ ] Email notifications

### Phase 4: Polish (2 weeks)
- [ ] Activity feed (audit log)
- [ ] Advanced filtering & search
- [ ] Dashboard analytics (task counts, workload)
- [ ] Performance optimization (indexes, caching)

---

## 10. Unresolved Questions & Areas for Further Research

1. **Whiteboard implementation**: Should we use Fabric.js, Konva.js, or build custom canvas? How to handle collaborative drawing without CRDTs?

2. **Document collaboration**: Should we integrate an existing editor (like Notion-style blocks) or build custom rich text? How to handle version history?

3. **Mobile app**: React Native or Progressive Web App (PWA)? What features are essential on mobile?

4. **Automation engine**: How to design workflow automation (like ClickUp automations)? Should we use rules engine or custom implementation?

5. **Time zone handling**: How to store/display due dates for distributed teams? Store UTC, display in user's timezone?

6. **Search implementation**: For <30 users, simple database `LIKE` queries suffice, or should we use PostgreSQL full-text search from start?

7. **Data export**: How to allow users to export their data (JSON, CSV, PDF)? Background job or synchronous?

8. **API rate limiting**: What limits are reasonable for <30 users? Per-endpoint or global limits?

9. **Audit log retention**: How long to keep activity logs? Forever or auto-archive after X months?

10. **Multi-language support**: Should we design i18n from start or defer until needed?

---

## Conclusion

For a **ClickUp-like platform with <30 users**, the optimal architecture is:

- **Modular monolith** (not microservices)
- **PostgreSQL** with Row-Level Security for data isolation
- **SignalR** for real-time collaboration
- **JWT + RBAC** for authorization
- **Materialized views** for dashboard performance
- **Cloud storage** (Azure Blob/AWS S3) for attachments
- **React/Next.js** frontend with Tailwind CSS

Focus on **MVP features first** (tasks, projects, basic views), iterate based on user feedback. Defer complex features (whiteboards, advanced automations) until validated need.

**Estimated development time**: 3-4 months for MVP with 1-2 developers.

---

**Sources**:
- ClickUp official documentation and feature pages (2024-2025)
- PostgreSQL performance optimization guides (TigerData, EnterpriseDB, AWS)
- AWS multi-tenant data isolation with RLS (May 2020)
- Real-time collaboration implementation patterns (Dev.to, Medium, 4Geeks, 2024-2025)
- Zero-downtime PostgreSQL migrations (Xata, Leapcell, 2024-2025)
- PostgreSQL CTE and join performance analysis (Chat2DB, EnterpriseDB, 2024-2025)
