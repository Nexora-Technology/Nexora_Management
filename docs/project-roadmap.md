# Project Roadmap

**Last Updated:** 2026-01-03

## Project Phases

### Phase 01: Project Setup and Architecture ✅ **COMPLETE**

**Timeline:** Completed
**Status:** ✅ Done

**Deliverables:**

- [x] Repository initialization with Git
- [x] Monorepo structure with Turborepo
- [x] Backend solution setup (.NET 9.0)
- [x] Frontend setup (Next.js 15)
- [x] Clean Architecture layers defined
- [x] Docker configuration
- [x] Development environment setup
- [x] CI/CD pipelines (GitHub Actions)
- [x] Code quality tools (ESLint, Prettier, Husky)

**Technical Decisions:**

- .NET 9.0 for backend with Clean Architecture
- Next.js 15 with App Router for frontend
- PostgreSQL 16 as primary database
- Entity Framework Core 9 for ORM
- Docker Compose for local development

---

### Phase 02: Domain Entities and Database Schema ✅ **COMPLETE**

**Timeline:** Completed 2026-01-03
**Status:** ✅ Done

**Deliverables:**

- [x] 14 Domain entities created
  - User, Role, Permission, UserRole, RolePermission
  - Workspace, WorkspaceMember
  - Project, Task, TaskStatus
  - Comment, Attachment, ActivityLog
- [x] BaseEntity with audit fields (Id, CreatedAt, UpdatedAt)
- [x] 14 EF Core configurations
- [x] AppDbContext with 13 DbSets
- [x] Database migrations (3 migration files)
  - InitialCreate schema
  - Row-Level Security policies
  - Roles and Permissions seeding
- [x] PostgreSQL extensions (uuid-ossp, pg_trgm)
- [x] Comprehensive indexing strategy
- [x] DbContext registration in API layer
- [x] IAppDbContext interface for testability

**Database Schema:**

- Multi-tenancy via Workspace-based isolation
- Hierarchical task management (Workspace → Project → Task)
- Row-Level Security on 5 tables
- JSONB columns for flexibility (Settings, CustomFields, ActivityLog)
- 30+ indexes for performance
- Cascade delete relationships defined

**Key Features:**

- UUID primary keys
- Auto-auditing (CreatedAt, UpdatedAt)
- Workspace-based multi-tenancy
- Threaded comments (self-referencing)
- Hierarchical tasks (parent-child)
- Custom statuses per project
- Position ordering for drag-and-drop

**Documentation:**

- [x] codebase-summary.md
- [x] system-architecture.md

---

### Phase 03: Authentication and Authorization ✅ **COMPLETE**

**Timeline:** Completed 2026-01-03
**Status:** ✅ Done

**Deliverables:**

- [x] JWT token generation and validation service
- [x] Password hashing with ASP.NET Core Identity PasswordHasher
- [x] Login endpoint (POST /api/auth/login)
- [x] Registration endpoint (POST /api/auth/register)
- [x] Token refresh endpoint (POST /api/auth/refresh)
- [x] RefreshToken entity for token rotation
- [x] JWT authentication middleware
- [x] JWT configuration in appsettings.json
- [x] AuthEndpoints with minimal API structure
- [x] CQRS commands (RegisterCommand, LoginCommand, RefreshTokenCommand)
- [x] Auth DTOs (requests and responses)
- [ ] Password reset flow (future)
- [ ] Email verification (future)
- [ ] Integration with RLS (set_current_user_id, future)

**Technical Implementation:**

- **JWT Settings**: Configurable secret, issuer, audience, expiration
- **Access Token**: 15-minute expiration with user claims
- **Refresh Token**: 7-day expiration, stored in database, rotation on refresh
- **Password Hashing**: BCrypt via IPasswordHasher<User>
- **Token Validation**: Microsoft JWT Bearer authentication
- **Endpoints**: Minimal API pattern with MediatR integration

**Security Features:**

- Short-lived access tokens (15 min)
- Long-lived refresh tokens (7 days)
- Refresh token rotation
- Token revocation support (IsUsed, IsRevoked flags)
- Secure password storage

---

### Phase 04: Core Workspace Functionality 🔄 **IN PROGRESS**

**Timeline:** Q1 2026
**Status:** 🔄 In Progress (50%)
**Progress:** Core CRUD operations complete. Pending: tests, validation, attachments, bulk operations.

**Planned Deliverables:**

- [ ] Create workspace endpoint
- [ ] Update workspace endpoint
- [ ] Delete workspace endpoint
- [ ] Get workspace by ID
- [ ] List user's workspaces
- [ ] Add member to workspace
- [ ] Remove member from workspace
- [ ] Update member role
- [ ] Workspace settings management
- [ ] Workspace invitation system (future)

**Endpoints:**

- `POST /api/workspaces`
- `GET /api/workspaces`
- `GET /api/workspaces/{id}`
- `PUT /api/workspaces/{id}`
- `DELETE /api/workspaces/{id}`
- `POST /api/workspaces/{id}/members`
- `DELETE /api/workspaces/{id}/members/{userId}`
- `PUT /api/workspaces/{id}/members/{userId}/role`

**Features:**

- Workspace CRUD operations
- Member management
- Role assignment per workspace
- Workspace ownership transfer
- Workspace visibility settings

---

### Phase 05: Task Management CRUD ✅ **COMPLETE**

**Timeline:** Completed 2026-01-03
**Status:** ✅ Done

**Deliverables:**

- [x] Create task endpoint (POST /api/tasks)
- [x] Update task endpoint (PUT /api/tasks/{id})
- [x] Delete task endpoint (DELETE /api/tasks/{id})
- [x] Get task by ID (GET /api/tasks/{id})
- [x] List tasks with filters (GET /api/tasks)
- [ ] Task status management (future)
- [x] Task assignment (via AssigneeId)
- [x] Task priority levels
- [x] Due date management
- [x] Task nesting (via ParentTaskId)
- [ ] Bulk task operations (future)
- [x] Task search (via search parameter)

**API Endpoints:**

- `POST /api/tasks` - Create task
- `GET /api/tasks/{id}` - Get task by ID
- `GET /api/tasks` - List tasks with filters (ProjectId, StatusId, AssigneeId, Search, SortBy, Page, PageSize)
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task

**Features Implemented:**

- Task CRUD operations with CQRS pattern
- Filtering by project, status, assignee
- Full-text search on title/description
- Sorting (any field, ascending/descending)
- Pagination (Page, PageSize)
- Hierarchical tasks (parent-child via ParentTaskId)
- Priority levels (low, medium, high, urgent)
- Date tracking (StartDate, DueDate)
- Time estimation (EstimatedHours)
- Result pattern for error handling (non-generic + generic Result types)

**Endpoints:**

- `POST /api/projects/{projectId}/tasks`
- `GET /api/projects/{projectId}/tasks`
- `GET /api/tasks/{id}`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`
- `PATCH /api/tasks/{id}/status`
- `PATCH /api/tasks/{id}/assignee`
- `POST /api/tasks/bulk-update`

**Filters:**

- Status, Priority, Assignee, Due Date, Created Date
- Search by title/description
- Custom field filters

---

### Phase 06: Real-time Updates via SignalR ⏳ **PLANNED**

**Timeline:** Q1 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Create project endpoint
- [ ] Update project endpoint
- [ ] Delete project endpoint
- [ ] Get project by ID
- [ ] List workspace projects
- [ ] Project status management
- [ ] Project color/icon customization
- [ ] Project settings
- [ ] Archive/restore projects

**Endpoints:**

- `POST /api/workspaces/{workspaceId}/projects`
- `GET /api/workspaces/{workspaceId}/projects`
- `GET /api/projects/{id}`
- `PUT /api/projects/{id}`
- `DELETE /api/projects/{id}`
- `PATCH /api/projects/{id}/status`

**Features:**

- Project CRUD operations
- Custom statuses per project
- Color and icon customization
- Project archiving
- Project cloning (future)

---

### Phase 07: Project Management ⏳ **PLANNED**

**Timeline:** Q1 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Create project endpoint
- [ ] Update project endpoint
- [ ] Delete project endpoint
- [ ] Get project by ID
- [ ] List workspace projects
- [ ] Project status management
- [ ] Project color/icon customization
- [ ] Project settings
- [ ] Archive/restore projects

**Endpoints:**

- `POST /api/workspaces/{workspaceId}/projects`
- `GET /api/workspaces/{workspaceId}/projects`
- `GET /api/projects/{id}`
- `PUT /api/projects/{id}`
- `DELETE /api/projects/{id}`
- `PATCH /api/projects/{id}/status`

**Features:**

- Project CRUD operations
- Custom statuses per project
- Color and icon customization
- Project archiving
- Project cloning (future)

---

### Phase 08: Real-time Updates via SignalR ⏳ **PLANNED**

**Timeline:** Q2 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] SignalR hub setup
- [ ] Task update notifications
- [ ] Comment notifications
- [ ] Member activity feed
- [ ] Online presence indicators
- [ ] Typing indicators
- [ ] Real-time collaboration

**Events:**

- TaskCreated, TaskUpdated, TaskDeleted
- CommentAdded, CommentUpdated
- MemberJoined, MemberLeft
- ProjectCreated, ProjectUpdated
- WorkspaceUpdated

**Features:**

- WebSocket connections
- Group membership by workspace
- Automatic reconnection
- Connection management
- Message queuing for offline users (future)

---

### Phase 09: File Attachments ⏳ **PLANNED**

**Timeline:** Q2 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] File upload endpoint
- [ ] File download endpoint
- [ ] File deletion endpoint
- [ ] List task attachments
- [ ] File size validation
- [ ] File type validation
- [ ] Thumbnail generation (images)
- [ ] Storage service abstraction
- [ ] Local file storage
- [ ] S3 integration (optional)

**Endpoints:**

- `POST /api/tasks/{taskId}/attachments`
- `GET /api/tasks/{taskId}/attachments`
- `GET /api/attachments/{id}/download`
- `DELETE /api/attachments/{id}`

**Features:**

- Multi-file upload
- Drag-and-drop support
- File preview
- Image thumbnails
- Storage quota management
- Virus scanning (future)

---

### Phase 10: Comments and Collaboration ⏳ **PLANNED**

**Timeline:** Q2 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Add comment endpoint
- [ ] Update comment endpoint
- [ ] Delete comment endpoint
- [ ] List task comments
- [ ] Threaded replies
- [ ] Comment mentions (@username)
- [ ] Comment reactions (emoji)
- [ ] Rich text editor (Markdown)
- [ ] Comment notifications

**Endpoints:**

- `POST /api/tasks/{taskId}/comments`
- `GET /api/tasks/{taskId}/comments`
- `PUT /api/comments/{id}`
- `DELETE /api/comments/{id}`
- `POST /api/comments/{id}/replies`

**Features:**

- Nested comment threads
- Markdown support
- @mentions with notifications
- Emoji reactions
- Comment editing history
- Rich text with sanitization

---

### Phase 11: Advanced Filtering and Search ⏳ **PLANNED**

**Timeline:** Q2 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Advanced task filtering
- [ ] Full-text search
- [ ] Saved filters
- [ ] Quick filters (My Tasks, Due Soon, Overdue)
- [ ] Multi-field search
- [ ] Search suggestions
- [ ] Export search results

**Endpoints:**

- `POST /api/tasks/search`
- `GET /api/tasks/filters/saved`
- `POST /api/tasks/filters/saved`
- `DELETE /api/tasks/filters/saved/{id}`

**Features:**

- PostgreSQL full-text search (tsvector)
- Trigram-based fuzzy search
- Faceted search
- Filter presets
- Filter sharing
- Export to CSV/Excel

---

### Phase 12: Activity Logging and Audit Trail ⏳ **PLANNED**

**Timeline:** Q2 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Automatic activity logging
- [ ] Activity feed endpoint
- [ ] Entity history tracking
- [ ] Change notifications
- [ ] Audit log export
- [ ] Activity search and filtering

**Endpoints:**

- `GET /api/workspaces/{workspaceId}/activity`
- `GET /api/tasks/{taskId}/history`
- `GET /api/audit-logs`

**Tracked Events:**

- Entity creation, update, deletion
- Field-level changes
- User actions
- Permission changes
- Member additions/removals

---

### Phase 13: Task Status Management ⏳ **PLANNED**

**Timeline:** Q2 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Create custom status endpoint
- [ ] Update status endpoint
- [ ] Delete status endpoint
- [ ] Reorder statuses
- [ ] Status types (open, in_progress, done)
- [ ] Default statuses per project
- [ ] Status color customization

**Endpoints:**

- `POST /api/projects/{projectId}/statuses`
- `GET /api/projects/{projectId}/statuses`
- `PUT /api/statuses/{id}`
- `DELETE /api/statuses/{id}`
- `PATCH /api/statuses/reorder`

**Features:**

- Custom status creation
- Drag-and-drop reordering
- Status type restrictions
- Color-coded statuses
- Status transitions rules (future)

---

### Phase 14: Frontend Development ⏳ **PLANNED**

**Timeline:** Q2-Q3 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Authentication UI (Login, Register)
- [ ] Workspace management UI
- [ ] Project dashboard UI
- [ ] Task list UI (board, list, timeline views)
- [ ] Task detail modal
- [ ] Comment system UI
- [ ] File upload UI
- [ ] User profile UI
- [ ] Settings pages
- [ ] Responsive design
- [ ] Dark mode support

**Components:**

- Layout components (Sidebar, Header)
- Task components (TaskCard, TaskList, KanbanBoard)
- Form components (TaskForm, ProjectForm, WorkspaceForm)
- UI components (Modal, Dropdown, DatePicker)
- shadcn/ui integration

**State Management:**

- Zustand stores for auth, workspaces, tasks
- React Query for data fetching
- Real-time updates via SignalR

---

### Phase 15: Mobile Responsive Design ⏳ **PLANNED**

**Timeline:** Q3 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Mobile-optimized layouts
- [ ] Touch-friendly interactions
- [ ] Mobile navigation
- [ ] Responsive breakpoints
- [ ] Mobile-specific features
- [ ] PWA capabilities (optional)

**Focus Areas:**

- Task management on mobile
- Quick actions
- Swipe gestures
- Bottom navigation
- Optimized forms for mobile

---

### Phase 16: Performance Optimization ⏳ **PLANNED**

**Timeline:** Q3 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Database query optimization
- [ ] Caching strategy (Redis)
- [ ] Response compression
- [ ] Image optimization
- [ ] Lazy loading
- [ ] Code splitting
- [ ] Bundle size optimization
- [ ] CDN integration
- [ ] Database connection pooling
- [ ] Background jobs (Hangfire)

**Performance Targets:**

- API response < 200ms (p95)
- Page load < 2s
- First Contentful Paint < 1s
- Time to Interactive < 3s

---

### Phase 17: Testing and Quality Assurance ⏳ **PLANNED**

**Timeline:** Q3 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Unit tests (xUnit for backend)
- [ ] Integration tests
- [ ] API tests
- [ ] Frontend tests (Jest, React Testing Library)
- [ ] E2E tests (Playwright)
- [ ] Load tests (Gatling/k6)
- [ ] Security audits
- [ ] Code coverage (>80%)

**Test Coverage:**

- Domain entities logic
- Application use cases
- API endpoints
- Database operations
- Frontend components
- User flows

---

### Phase 18: Deployment and DevOps ⏳ **PLANNED**

**Timeline:** Q4 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Production Docker images
- [ ] Kubernetes manifests
- [ ] CI/CD pipeline optimization
- [ ] Automated deployments
- [ ] Blue-green deployments
- [ ] Database migration automation
- [ ] Monitoring setup (Prometheus, Grafana)
- [ ] Logging aggregation (ELK stack)
- [ ] Error tracking (Sentry)
- [ ] Health checks
- [ ] Backup strategy
- [ ] Disaster recovery plan

**Infrastructure:**

- Cloud provider (AWS/Azure/GCP)
- Managed PostgreSQL (RDS/Cloud SQL)
- CDN (CloudFront/Cloudflare)
- Load balancer
- SSL certificates
- Auto-scaling

---

### Phase 19: Advanced Features ⏳ **PLANNED**

**Timeline:** Q4 2026
**Status:** 📋 Planned

**Planned Deliverables:**

- [ ] Task dependencies
- [ ] Gantt chart view
- [ ] Time tracking
- [ ] Sprint planning
- [ ] Burndown charts
- [ ] Reporting and analytics
- [ ] Custom workflows
- [ ] Automations (webhooks, triggers)
- [ ] Integrations (Slack, Teams, GitHub)
- [ ] Calendar sync
- [ ] Email notifications
- [ ] Mobile apps (React Native)

---

## Milestones

- [x] **M1:** Project Setup (Phase 01) - Q4 2025
- [x] **M2:** Core Database (Phase 02) - Q1 2026
- [x] **M3:** Authentication (Phase 03) - Q1 2026
- [x] **M4:** Task Management (Phase 05) - Q1 2026
- [ ] **M5:** Workspace Management (Phase 04) - Q1 2026
- [ ] **M6:** Real-time Features (Phase 08) - Q2 2026
- [ ] **M7:** Collaboration (Phases 09-10) - Q2 2026
- [ ] **M8:** Frontend Complete (Phase 14) - Q3 2026
- [ ] **M9:** Production Ready (Phases 16-18) - Q4 2026

## Dependencies

**Phase 03 depends on:** Phase 02
**Phase 04 depends on:** Phase 03
**Phase 05 depends on:** Phase 03
**Phase 07 depends on:** Phase 04
**Phase 08 depends on:** Phase 05
**Phase 09 depends on:** Phase 05
**Phase 10 depends on:** Phase 05
**Phase 14 depends on:** Phases 03-13
**Phase 18 depends on:** All previous phases

## Risk Assessment

**High Risk:**

- Real-time features scaling (Phase 07)
- File upload security (Phase 08)
- Performance at scale (Phase 15)

**Medium Risk:**

- RLS policy complexity
- Frontend state management
- Third-party integrations

**Low Risk:**

- Basic CRUD operations
- Authentication (standard patterns)
- Database schema (well-defined)

---

**Documentation Version:** 1.0
**Last Updated:** 2026-01-03
**Maintained By:** Development Team
