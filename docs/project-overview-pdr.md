# Project Overview & Product Development Requirements (PDR)

**Last Updated:** 2026-01-09
**Version:** Phase 09 Complete + Docker Testing Phase
**Document Status:** Active
**Production Readiness:** Grade B- (82/100) - Not Ready
**Test Coverage:** 0% (Critical Issue)

## Executive Summary

Nexora Management is a comprehensive, ClickUp-inspired project management platform built with modern .NET 9.0 backend and Next.js 15 frontend technologies. The platform enables teams to collaborate effectively through workspaces, projects, tasks, documents, and real-time communication features.

### Vision Statement

To provide a powerful, flexible, and intuitive project management solution that scales from small teams to enterprise organizations, with emphasis on real-time collaboration, customizable workflows, and seamless user experience.

### Current Status

**Phase 09 (ClickUp Hierarchy - Phases 6, 7, 8):** Complete ✅

**Previous Phase (08 - Goal Tracking & OKRs):** Complete ✅

**Latest Updates (January 2026):**

- **Backend CQRS Layer:** Workspace CRUD with 3 commands, 2 queries
- **Swagger UI:** Enabled with Swashbuckle 7.2.0 (access: /swagger)
- **Docker Configuration:** Fixed CORS and API port issues
  - API: localhost:5001 (Docker network: backend:8080)
  - CORS: AllowAnyOrigin() (⚠️ security issue)
- **Docker Testing Results:**
  - PostgreSQL: Healthy (5/5 health checks)
  - Redis: Healthy (5/5 health checks)
  - Backend: Healthy (5/5 health checks)
  - Frontend: Unhealthy (15/15 failures - endpoint missing)
- **Critical Issues Found:**
  1. Test Coverage: 0% (1 placeholder test for 24,563 LOC)
  2. CORS: AllowAnyOrigin() breaks JWT auth
  3. Database: RolePermissions seed data bug
  4. Migration: Projects→TaskLists not executed

- **Phase 6 - Frontend Pages & Routes:** 100% complete
  - Navigation sidebar updated (Tasks → Spaces)
  - Spaces overview page (`/spaces`) with hierarchical tree navigation
  - List detail page (`/lists/[id]`) with task board
  - Task detail page breadcrumbs updated
  - Task modal with list selector
  - TypeScript errors fixed (Route type casting)
  - Code Review: A+ (95/100)
  - Commits: c71f39b, 51d8118

- **Phase 7 - Testing:** DEFERRED ⏸️
  - No test infrastructure available
  - Comprehensive test requirements documented
  - TypeScript/ESLint errors fixed (removed 'as any')
  - Document quality: 9.2/10
  - Marked as DEFERRED
  - Commit: 9515e0a

- **Phase 8 - Workspace Context:** 100% complete
  - Workspace feature module with types, API, provider
  - WorkspaceSelector component built (247 lines)
  - WorkspaceProvider integrated in app layout
  - Spaces page updated to use context
  - Workspace ID validation fixed (high priority)
  - Code Review: A- (92/100)
  - Commit: 4285736

**Previous Phase (08 - Goal Tracking & OKRs):** Complete ✅

- Goal Tracking System: 100% complete
  - GoalPeriod, Objective, KeyResult entities
  - 12 API endpoints (periods, objectives, key results, dashboard)
  - Frontend components (ObjectiveCard, KeyResultEditor, ProgressDashboard, ObjectiveTree)
  - Goals pages (list and detail views)
  - Weighted progress calculation
  - Auto-status calculation (on-track/at-risk/off-track)
- Backend: 100% complete (entities, CQRS, endpoints, migration)
- Frontend: 100% complete (types, API client, components, pages)
- Test Results: ✅ Passed (Backend: 0 errors, Frontend: 0 TypeScript errors)
- Code Review: 8.5/10
- Commit: Latest (2026-01-06)

**Phase 07 (Document & Wiki System):** 100% Complete ✅

- Backend: 100% complete (entities, CQRS, endpoints)
- Frontend: 100% complete (7 components with TipTap editor)
- Database: 100% complete (migration applied)

**Phase 06 (Real-time Collaboration):** Complete ✅

- SignalR hubs: TaskHub, PresenceHub, NotificationHub
- Real-time task updates across all connected clients
- User presence tracking (online/offline status)
- Typing indicators for collaborative editing
- Real-time notifications with toast integration
- Notification preferences with per-event type toggles
- Project-based messaging groups for efficiency
- Auto-reconnect with graceful connection handling

**Recent Improvements (January 2026):** Complete ✅

- Drag and Drop Functionality: 100% complete
  - Fixed Kanban board drag and drop functionality
  - Tasks can be dragged anywhere on the card
  - Tasks can be dragged between columns to change status
  - Added @dnd-kit/core 6.3.1, @dnd-kit/modifiers 9.0.0, @dnd-kit/sortable 10.0.0, @dnd-kit/utilities 3.2.2

## Product Goals

### Primary Objectives

1. **Team Collaboration**: Enable seamless real-time collaboration across distributed teams
2. **Task Management**: Provide flexible task organization with multiple views (list, board, calendar, gantt)
3. **Document Management**: Offer robust wiki/documentation capabilities with version control
4. **Workspace Organization**: Support multi-tenancy through workspace-based data isolation
5. **Security & Compliance**: Implement enterprise-grade security with RBAC and RLS
6. **Scalability**: Architecture designed to scale from startups to enterprises

### Success Metrics

- **User Engagement**: 80% monthly active user retention
- **Performance**: <200ms API response time (p95), <2s page load
- **Reliability**: 99.9% uptime SLA
- **Security**: Zero critical vulnerabilities, annual security audit passing
- **Satisfaction**: 4.5+ star rating, <5% churn rate

## Product Development Requirements (PDR)

### Functional Requirements

#### FR1: Authentication & Authorization (Phase 03) ✅

**Priority:** P0 (Critical)
**Status:** Complete

- **FR1.1:** JWT-based authentication with access/refresh token flow
  - Access token expiry: 15 minutes
  - Refresh token expiry: 7 days
  - Token rotation on refresh
  - Secure password hashing (BCrypt)

- **FR1.2:** Role-Based Access Control (RBAC)
  - System roles: Admin, Member, Guest
  - Workspace-scoped role assignment
  - Permission-based authorization (resource:action format)

- **FR1.3:** Row-Level Security (RLS)
  - Database-level access control
  - Workspace membership validation
  - Automatic query filtering

#### FR2: Workspace Management (Phase 04) 🔄

**Priority:** P0 (Critical)
**Status:** 50% Complete

- **FR2.1:** Workspace CRUD operations
- **FR2.2:** Member management (add, remove, update roles)
- **FR2.3:** Workspace ownership transfer
- **FR2.4:** Workspace settings (visibility, default permissions)
- **FR2.5:** Multi-workspace membership per user

#### FR3: Project Management (Phase 05) ✅

**Priority:** P0 (Critical)
**Status:** Complete

- **FR3.1:** Project CRUD operations within workspaces
- **FR3.2:** Project metadata (name, description, color, icon, status)
- **FR3.3:** Custom task statuses per project
- **FR3.4:** Project-based access control

#### FR4: Task Management (Phase 05) ✅

**Priority:** P0 (Critical)
**Status:** Complete

- **FR4.1:** Task CRUD operations
- **FR4.2:** Hierarchical tasks (parent-child relationships)
- **FR4.3:** Task attributes:
  - Title, description
  - Priority (low, medium, high, urgent)
  - Status (custom per project)
  - Assignee (user reference)
  - Dates (start, due)
  - Time estimation (hours)
  - Custom fields (JSONB)
  - Position ordering (for drag-drop)

- **FR4.4:** Task filtering and search
  - By project, status, assignee, priority
  - Full-text search on title/description
  - Sorting and pagination

- **FR4.5:** Multiple views
  - List view (sortable table)
  - Board view (Kanban)
  - Calendar view (monthly grid)
  - Gantt view (timeline)

#### FR5: Comments & Attachments (Phase 04) ✅

**Priority:** P1 (High)
**Status:** Complete

- **FR5.1:** Threaded comments on tasks
  - Nested replies (max 5 levels)
  - Markdown support
  - @mentions

- **FR5.2:** File attachments
  - Upload/download (100MB limit)
  - File type validation (no executables)
  - Security: path traversal protection

#### FR6: Real-time Collaboration (Phase 06) ✅

**Priority:** P1 (High)
**Status:** Complete

- **FR6.1:** SignalR-based real-time updates
  - Task events (created, updated, deleted, status changed)
  - Comment events
  - Attachment events

- **FR6.2:** User presence tracking
  - Online/offline status
  - Current view (task/project being viewed)
  - Typing indicators

- **FR6.3:** Notification system
  - In-app notifications with bell icon
  - Notification preferences (per-event toggles)
  - Quiet hours configuration
  - Toast integration

#### FR7: Document & Wiki System (Phase 07) 🔄

**Priority:** P1 (High)
**Status:** 60% Complete

- **FR7.1:** Rich text editing (TipTap)
  - Markdown support
  - Formatting toolbar
  - Slash commands (pending)

- **FR7.2:** Page management
  - Hierarchical page structure (parent-child)
  - Unique slug generation
  - Auto-versioning on content changes

- **FR7.3:** Version control
  - Version history
  - Restore previous versions
  - Version comparison (future)

- **FR7.4:** Page organization
  - Page tree navigation
  - Favorites and recent views
  - Full-text search

- **FR7.5:** Collaboration
  - Page collaborators with roles (Owner, Editor, Viewer)
  - Comments on pages
  - Real-time co-editing (via SignalR, planned)

#### FR8: Goal Tracking & OKRs (Phase 08) ✅

**Priority:** P1 (High)
**Status:** Complete

- **FR8.1:** Goal period management
  - Time-based goal tracking (Q1, Q2, FY, etc.)
  - Period status management (active, archived)
  - Workspace-scoped periods

- **FR8.2:** Objective management
  - Hierarchical objectives (3 levels max)
  - Owner assignment to users
  - Weight-based priority (1-10)
  - Status tracking (on-track, at-risk, off-track, completed)
  - Progress percentage (0-100) calculated from weighted average of key results
  - Position ordering for drag-and-drop

- **FR8.3:** Key result management
  - Metric types: number, percentage, currency
  - Current and target values for progress tracking
  - Unit specification (%, $, count, etc.)
  - Due date for time-bound key results
  - Progress percentage (0-100) calculated as (CurrentValue / TargetValue) \* 100
  - Weight-based priority for weighted average calculation

- **FR8.4:** Progress dashboard
  - Total objectives count
  - Average progress across all objectives
  - Status breakdown (on-track, at-risk, off-track, completed)
  - Top performing objectives
  - Bottom performing objectives

#### FR9: Advanced Features (Future Phases)

**Priority:** P2 (Medium)
**Status:** Planned

- **FR9.1:** Time tracking (Phase 09)
- **FR9.2:** Dashboards & reporting (Phase 10)
- **FR9.3:** Automation & workflow engine (Phase 11)
- **FR9.4:** Mobile responsive design (Phase 12)

### Non-Functional Requirements

#### NFR1: Performance

- **API Response Time:** <200ms (p95), <500ms (p99)
- **Page Load Time:** <2s (First Contentful Paint <1s)
- **Database Query Time:** <100ms for common queries
- **Concurrent Users:** Support 10,000+ simultaneous users
- **Data Volume:** Support 1M+ tasks, 100K+ users

**Strategies:**

- Database indexing on foreign keys and search fields
- Redis caching for frequently accessed data
- CDN for static assets
- Code splitting and lazy loading
- Connection pooling

#### NFR2: Security

- **Authentication:** JWT with secure token storage
- **Authorization:** RBAC + RLS for defense in depth
- **Data Encryption:** TLS 1.3 for data in transit
- **Password Security:** BCrypt hashing, min 8 characters
- **Input Validation:** Server-side validation on all inputs
- **SQL Injection Prevention:** Parameterized queries via EF Core
- **XSS Prevention:** Output encoding, CSP headers
- **CSRF Protection:** Anti-forgery tokens for state-changing operations

#### NFR3: Reliability

- **Availability:** 99.9% uptime SLA (43.8 minutes/month downtime)
- **Data Durability:** 99.999% (5 nines) via database replication
- **Backup:** Daily automated backups, 30-day retention
- **Disaster Recovery:** RTO <4 hours, RPO <1 hour
- **Error Handling:** Graceful degradation, comprehensive error logging

#### NFR4: Scalability

- **Horizontal Scaling:** Stateless API layer supports multiple instances
- **Database Scaling:** Read replicas for query scaling
- **Caching Layer:** Redis for session and data caching
- **Load Balancing:** Support for application load balancers
- **Auto-scaling:** Cloud-native auto-scaling support

#### NFR5: Maintainability

- **Code Quality:** Clean Architecture, SOLID principles
- **Documentation:** Comprehensive technical documentation
- **Testing:** >80% code coverage target
- **Monitoring:** Application performance monitoring (APM)
- **Logging:** Structured logging with correlation IDs

#### NFR6: Usability

- **Accessibility:** WCAG 2.1 Level AA compliance
- **Responsive Design:** Support desktop (1920x1080), tablet (768x1024), mobile (375x667)
- **Browser Support:** Chrome, Firefox, Safari, Edge (latest 2 versions)
- **Internationalization:** English (primary), multi-language support (future)
- **Onboarding:** Interactive tutorial for new users

## Technical Architecture

### Backend Stack

- **Framework:** .NET 9.0 / ASP.NET Core Web API
- **Architecture:** Clean Architecture (Domain, Infrastructure, Application, API layers)
- **ORM:** Entity Framework Core 9.0
- **Database:** PostgreSQL 16 with Row-Level Security
- **Real-time:** SignalR (WebSocket)
- **Authentication:** JWT Bearer tokens
- **API Pattern:** Minimal APIs with CQRS (MediatR)

### Frontend Stack

- **Framework:** Next.js 15 (App Router)
- **Language:** TypeScript
- **Styling:** Tailwind CSS v3.4.0
- **Components:** shadcn/ui (16 components integrated)
- **Rich Text:** TipTap editor
- **State Management:** Zustand
- **Data Fetching:** React Query (@tanstack/react-table)
- **Real-time:** @microsoft/signalr
- **Drag-Drop:** @dnd-kit
- **UI Primitives:** @radix-ui (Dialog, Select, Checkbox)

### Infrastructure

- **Containerization:** Docker & Docker Compose
- **Build System:** Turborepo (monorepo)
- **CI/CD:** GitHub Actions
- **Version Control:** Git
- **Code Quality:** ESLint, Prettier, Husky, lint-staged
- **Testing:** xUnit (backend), Jest (frontend)

## Database Schema

### Core Entities (24 Domain Models)

**Authentication & Authorization:**

- User, Role, Permission, UserRole, RolePermission, RefreshToken

**Workspace & Projects:**

- Workspace, WorkspaceMember, Project

**Task Management:**

- Task, TaskStatus, Comment, Attachment, ActivityLog

**Document System:**

- Page, PageVersion, PageCollaborator, PageComment

**Goal Tracking & OKRs (NEW Phase 08):**

- GoalPeriod, Objective, KeyResult

**Real-time:**

- Notification, NotificationPreference, UserPresence

### Key Relationships

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
                              │                   ├───── (N) Comment
                              │                   └───── (N) Attachment
                              │
                              ├───── (N) Page ──── (1) ParentPage (self-ref)
                              │         │
                              │         ├───── (N) PageVersion
                              │         ├───── (N) PageComment
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

Role (1) ────< (N) RolePermission >─── (N) Permission
```

## API Endpoints Summary

### Authentication (`/api/auth`)

- POST /register
- POST /login
- POST /refresh

### Tasks (`/api/tasks`)

- GET /
- GET /{id}
- POST /
- PUT /{id}
- DELETE /{id}
- PATCH /{id}/status
- GET /views/board/{projectId}
- GET /views/calendar/{projectId}
- GET /views/gantt/{projectId}

### Comments (`/api/comments`)

- POST /
- GET /task/{taskId}
- GET /{commentId}/replies
- PUT /{commentId}
- DELETE /{commentId}

### Attachments (`/api/attachments`)

- POST /upload/{taskId}
- GET /task/{taskId}
- GET /{attachmentId}/download
- DELETE /{attachmentId}

### Documents (`/api/documents`) - Phase 07

- POST /
- GET /{id}
- PUT /{id}
- DELETE /{id}
- GET /tree/{workspaceId}
- POST /{id}/favorite
- GET /{id}/versions
- POST /{id}/restore
- POST /{id}/move
- GET /search

### Goals (`/api/goals`) - Phase 08

#### Periods

- POST /periods
- GET /periods
- PUT /periods/{id}
- DELETE /periods/{id}

#### Objectives

- POST /objectives
- GET /objectives
- GET /objectives/tree
- PUT /objectives/{id}
- DELETE /objectives/{id}

#### Key Results

- POST /keyresults
- PUT /keyresults/{id}
- DELETE /keyresults/{id}

#### Dashboard

- GET /dashboard

### SignalR Hubs

- /hubs/tasks - Task real-time updates
- /hubs/presence - User presence tracking
- /hubs/notifications - Notification delivery

## Development Phases

### Phase 01: Project Setup & Architecture ✅

**Completed:** Q4 2025

### Phase 02: Domain & Database ✅

**Completed:** 2026-01-03

### Phase 03: Authentication & Authorization ✅

**Completed:** 2026-01-03

### Phase 04: Task Management Core ✅

**Completed:** 2026-01-03

### Phase 04.1: View Components (Task UI) ✅

**Completed:** 2026-01-05
**Components:** 16 task components, 3 task pages, 4 UI primitives
**Features:** List view, board view, task detail, create/edit modal

### Phase 05A: Performance & Accessibility ✅

**Completed:** 2026-01-05
**Components:** 4 optimized task components
**Features:**

- React.memo with custom comparison functions
- Single-pass tasksByStatus algorithm (O(n) complexity)
- useCallback for stable event handlers
- aria-live regions for dynamic content
- ARIA labels for interactive elements
- WCAG 2.1 AA compliance
  **Code Review:** 8.5/10
  **Commit:** a145c08

### Phase 05: Multiple Views ✅

**Completed:** 2026-01-03

### Phase 06: Real-time Collaboration ✅

**Completed:** 2026-01-04

### Phase 07: Document & Wiki System 🔄

**Status:** 60% Complete (Backend 100%, Frontend 100%, Integration Pending)

### Phase 08-19: Future Phases

**See:** [project-roadmap.md](project-roadmap.md)

## Compliance & Standards

### Data Privacy

- **GDPR Compliance:** User data export, right to deletion, consent management
- **Data Residency:** EU/US region selection (future)
- **Data Retention:** Configurable retention policies

### Accessibility

- **WCAG 2.1 Level AA:** Full compliance target
- **Keyboard Navigation:** Full keyboard accessibility
- **Screen Reader Support:** ARIA labels and roles
- **Color Contrast:** Minimum 4.5:1 ratio

### Code Standards

- **C#:** StyleCop analyzer, C# 12 features
- **TypeScript:** ESLint + Prettier, strict mode
- **Git:** Conventional commits, main branch protection
- **Documentation:** Markdown, ADR for major decisions

## Risk Assessment

### Technical Risks

| Risk                          | Likelihood | Impact | Mitigation                           |
| ----------------------------- | ---------- | ------ | ------------------------------------ |
| SignalR scaling issues        | Medium     | High   | Load testing, Redis backplane        |
| Database performance at scale | Medium     | High   | Indexing strategy, read replicas     |
| RLS policy complexity         | Low        | Medium | Comprehensive testing, documentation |
| Frontend bundle size          | High       | Medium | Code splitting, lazy loading         |

### Operational Risks

| Risk                       | Likelihood | Impact   | Mitigation                            |
| -------------------------- | ---------- | -------- | ------------------------------------- |
| Downtime during deployment | Medium     | High     | Blue-green deployments, health checks |
| Data loss                  | Low        | Critical | Automated backups, replication        |
| Security breach            | Low        | Critical | Security audits, penetration testing  |

## Success Criteria

### Phase 07 Completion Criteria

- [ ] Database migration applied successfully
- [ ] Document routes integrated (/documents, /documents/[id])
- [ ] Frontend components wired to backend API
- [ ] Real-time collaboration enabled (SignalR)
- [ ] Page collaboration UI implemented
- [ ] E2E testing complete
- [ ] Documentation updated

### Production Readiness Assessment

**Overall Grade:** B- (82/100)

**Status:** NOT READY - Critical issues must be addressed

**Breakdown:**

| Category | Score | Status | Notes |
|----------|-------|--------|-------|
| Feature Completeness | 95/100 | ✅ Excellent | Core features implemented |
| Code Quality | 85/100 | ✅ Good | Clean architecture, some tech debt |
| Test Coverage | 0/100 | ❌ Critical | Only 1 placeholder test |
| Security | 65/100 | ⚠️ Warning | CORS issue, no audit |
| Performance | 80/100 | ✅ Good | No benchmarks, but optimized |
| Documentation | 90/100 | ✅ Excellent | Comprehensive docs |
| Deployment | 75/100 | ⚠️ Warning | Docker works, config issues |

**Critical Blockers:**

1. **Test Coverage (0%)**
   - Only 1 placeholder test exists
   - Need unit, integration, and E2E tests
   - Estimated effort: 3-5 days

2. **Security Issues**
   - CORS: AllowAnyOrigin() breaks JWT
   - Hardcoded credentials in docker-compose.yml
   - No security audit performed
   - Estimated effort: 2-3 days

3. **Database Issues**
   - RolePermissions seed data bug
   - Projects→TaskLists migration not executed
   - Estimated effort: 1 day

**Estimated Time to Production-Ready:** 6-9 days

**Next Steps:**

1. Set up test infrastructure (vitest, xUnit, Playwright)
2. Write unit tests for critical components
3. Fix CORS configuration (use specific origins)
4. Execute and validate Projects→TaskLists migration
5. Perform security audit
6. Set up monitoring and logging
7. Create production deployment runbook

## Production Readiness Criteria (Revised)

- [x] All P0 and P1 requirements implemented (95%)
- [ ] > 80% test coverage achieved (0% - CRITICAL)
- [ ] Performance benchmarks met (not measured)
- [ ] Security audit passed (not done)
- [ ] Monitoring and logging configured (partial)
- [ ] Disaster recovery tested (not done)
- [ ] User documentation complete (90%)

## Appendix

### Glossary

- **Workspace:** Top-level container for multi-tenancy
- **Project:** Container for tasks within a workspace
- **Task:** Work item with attributes, hierarchy, and status
- **CQRS:** Command Query Responsibility Segregation pattern
- **RLS:** Row-Level Security (PostgreSQL feature)
- **RBAC:** Role-Based Access Control
- **SignalR:** Real-time WebSocket library for .NET

### References

- [System Architecture](system-architecture.md)
- [Codebase Summary](codebase-summary.md)
- [Project Roadmap](project-roadmap.md)
- [Development Standards](development-standards.md)
- [Deployment Guide](deployment-guide.md)

---

**Document Version:** 1.0
**Last Updated:** 2026-01-04
**Maintained By:** Development Team
**Review Frequency:** Monthly
**Next Review:** 2026-02-04
