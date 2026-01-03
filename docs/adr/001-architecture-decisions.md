# ADR 001: Core Architecture Decisions

**Status:** Accepted
**Date:** 2025-01-03
**Decision Makers:** Development Team
**Related ADRs:** None

## Context

Nexora Management is a ClickUp-like project management platform requiring:
- Multi-user collaboration with real-time updates
- Complex domain model (workspaces, tasks, comments, attachments)
- Scalability to support growing user base
- Maintainability over long-term development
- Separation of concerns for team collaboration

We needed to choose an architecture and technology stack that supports these requirements while maintaining code quality and developer productivity.

## Decision

We have adopted the following architecture and technology choices:

### 1. Clean Architecture

**Status:** ✅ Accepted

**Rationale:**
- **Separation of Concerns:** Clear separation between domain logic, application logic, infrastructure, and presentation
- **Testability:** Business logic can be tested without dependencies on external systems
- **Independence from Frameworks:** Domain layer doesn't depend on ASP.NET Core, Entity Framework, or other frameworks
- **Maintainability:** Easier to understand and modify code over time
- **Scalability:** Well-defined boundaries allow for easier scaling and refactoring

**Implementation:**
```
apps/backend/src/
├── Core/              # Domain entities, value objects, interfaces
│   ├── Entities/
│   ├── Interfaces/
│   └── Exceptions/
├── Application/       # Use cases, application services, DTOs
│   ├── Commands/
│   ├── Queries/
│   ├── Interfaces/
│   └── DTOs/
├── Infrastructure/    # Data access, external services
│   ├── Data/
│   ├── Services/
│   └── Persistence/
└── API/              # Controllers, middleware, configuration
    ├── Controllers/
    ├── Middleware/
    └── Configuration/
```

**Trade-offs:**
- **Pros:** Better maintainability, testability, flexibility
- **Cons:** More upfront complexity, additional layers to navigate

**Consequences:**
- All dependencies point inward (no circular dependencies)
- Business logic is framework-agnostic
- Easier to swap out implementations (e.g., change ORM)

### 2. Monorepo with Turborepo

**Status:** ✅ Accepted

**Rationale:**
- **Shared Code:** Easy to share types and utilities between backend and frontend
- **Simplified Development:** Single repository for entire application
- **Atomic Commits:** Changes across backend and frontend in one commit
- **Code Review:** PRs include all related changes
- **CI/CD:** Single pipeline for entire application
- **Local Development:** Run everything with one command

**Implementation:**
```
/
├── apps/
│   ├── backend/      # .NET 9.0 API
│   └── frontend/     # Next.js 15 application
├── packages/         # Shared packages (future)
├── turbo.json        # Turborepo configuration
└── package.json      # Root scripts
```

**Trade-offs:**
- **Pros:** Simplified workflow, better developer experience
- **Cons:** Larger repository, longer clone times

**Consequences:**
- All developers work in single repository
- Shared TypeScript types possible
- Consistent tooling across packages

### 3. PostgreSQL with Row-Level Security (RLS)

**Status:** ✅ Accepted

**Rationale:**
- **Data Security:** RLS provides automatic data isolation at database level
- **Multi-tenancy:** Natural support for multi-tenant workspaces
- **Performance:** Database-level filtering is more efficient
- **Compliance:** Additional security layer for data protection
- **Simplicity:** Reduces application-level filtering logic

**Implementation:**
```sql
-- Example RLS policy
CREATE POLICY workspace_isolation ON tasks
  FOR ALL
  TO authenticated_users
  USING (workspace_id IN (
    SELECT id FROM workspaces WHERE user_id = current_user_id()
  ));
```

**Trade-offs:**
- **Pros:** Enhanced security, simpler application code
- **Cons:** Database vendor lock-in, additional complexity

**Consequences:**
- Database handles data access control
- Must plan migrations carefully to avoid breaking RLS
- Development requires understanding of RLS policies

### 4. SignalR for Real-time Communication

**Status:** ✅ Accepted

**Rationale:**
- **Real-time Updates:** Instant notifications for task changes, comments
- **Bidirectional Communication:** Server can push updates to clients
- **Automatic Reconnection:** Built-in reconnection handling
- **Scalability:** Can scale with Azure SignalR Service or Redis backplane
- **Type Safety:** Strongly-typed Hub APIs with .NET and TypeScript

**Implementation:**
```csharp
// Backend Hub
public class TaskHub : Hub
{
    public async Task JoinWorkspace(Guid workspaceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"workspace:{workspaceId}");
    }

    public async Task TaskUpdated(Guid taskId, TaskUpdateDto update)
    {
        await Clients.OthersInGroup($"workspace:{update.WorkspaceId}")
            .SendAsync("TaskUpdated", taskId, update);
    }
}
```

```typescript
// Frontend client
const connection = new HubConnectionBuilder()
  .withUrl('/hubs/tasks')
  .build();

connection.on('TaskUpdated', (taskId, update) => {
  // Handle update
});
```

**Trade-offs:**
- **Pros:** Rich real-time features, excellent DX
- **Cons:** WebSocket infrastructure complexity, scaling considerations

**Consequences:**
- State management must handle real-time updates
- Testing requires WebSocket mocking or integration tests
- Monitoring connection health is important

### 5. Docker-first Development

**Status:** ✅ Accepted

**Rationale:**
- **Consistency:** Same environment across development, testing, production
- **Simplified Onboarding:** New developers can start with one command
- **Isolation:** No conflicts with local system dependencies
- **Testing:** Easy to test against real database in CI/CD
- **Deployment:** Containers can be deployed directly

**Implementation:**
```yaml
# docker-compose.yml
services:
  backend:
    build: ./apps/backend
    ports:
      - "5000:80"
    depends_on:
      - postgres

  frontend:
    build: ./apps/frontend
    ports:
      - "3000:3000"

  postgres:
    image: postgres:16
    environment:
      POSTGRES_DB: nexora
```

**Trade-offs:**
- **Pros:** Consistency, simplified onboarding, production parity
- **Cons:** Learning curve, resource overhead

**Consequences:**
- All documentation assumes Docker usage
- Development primarily in containers
- Production deployment is container-based

## Technology Stack Summary

### Backend
- **Framework:** .NET 9.0 / ASP.NET Core
- **Architecture:** Clean Architecture
- **Database:** PostgreSQL 16 with RLS
- **ORM:** Entity Framework Core 9
- **Real-time:** SignalR
- **Authentication:** JWT with refresh tokens

### Frontend
- **Framework:** Next.js 15 (App Router)
- **Language:** TypeScript
- **Styling:** Tailwind CSS
- **Components:** shadcn/ui
- **State:** Zustand + React Query
- **Real-time:** SignalR client

### DevOps
- **Build System:** Turborepo
- **Containerization:** Docker & Docker Compose
- **CI/CD:** GitHub Actions
- **Code Quality:** ESLint, Prettier, Husky, lint-staged

## Alternatives Considered

### 1. Microservices Architecture
**Rejected because:**
- Over-engineering for current scale
- Additional operational complexity
- Distributed transaction challenges
- Slower development velocity

### 2. MongoDB Database
**Rejected because:**
- PostgreSQL RLS provides better security
- Relational model better fits our domain
- PostgreSQL's JSONB covers NoSQL use cases
- Better tooling and ecosystem maturity

### 3. GraphQL
**Rejected because:**
- REST is sufficient for current requirements
- Simpler caching strategies
- Easier to monitor and debug
- Better built-in tooling in .NET

### 4. WebSockets without SignalR
**Rejected because:**
- More boilerplate code
- Manual reconnection handling
- No built-in type safety
- SignalR provides better abstractions

## Implementation Status

- [x] Clean Architecture project structure
- [x] Turborepo configuration
- [x] Docker Compose setup
- [x] PostgreSQL with RLS policies
- [ ] SignalR Hubs implementation
- [ ] Authentication and authorization
- [ ] Core domain model
- [ ] Real-time features

## Future Considerations

1. **Horizontal Scaling:**
   - Add Redis for SignalR backplane
   - Consider message queue for background jobs
   - Database read replicas

2. **Performance:**
   - Caching layer (Redis)
   - Database query optimization
   - Frontend code splitting

3. **Security:**
   - Rate limiting
   - API key management for integrations
   - Audit logging

4. **Observability:**
   - Application performance monitoring
   - Structured logging
   - Distributed tracing

## References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft .NET Architecture Guides](https://docs.microsoft.com/en-us/dotnet/architecture/)
- [Turborepo Documentation](https://turbo.build/repo/docs)
- [PostgreSQL Row-Level Security](https://www.postgresql.org/docs/current/ddl-rowsecurity.html)
- [ASP.NET Core SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/)

---

**Next Review Date:** 2025-06-01 or when architectural changes are proposed
