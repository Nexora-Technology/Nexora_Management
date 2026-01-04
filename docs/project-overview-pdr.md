# Project Overview & Product Development Requirements (PDR)

**Last Updated:** 2026-01-05
**Version:** Phase 04 Complete (View Components - Task Management UI)
**Document Status:** Active

## Executive Summary

Nexora Management is a comprehensive, ClickUp-inspired project management platform built with modern .NET 9.0 backend and Next.js 15 frontend technologies. The platform enables teams to collaborate effectively through workspaces, projects, tasks, documents, and real-time communication features.

### Vision Statement

To provide a powerful, flexible, and intuitive project management solution that scales from small teams to enterprise organizations, with emphasis on real-time collaboration, customizable workflows, and seamless user experience.

### Current Status

**Phase 04 (View Components):** Complete ✅

- Task Management UI: 100% complete
- List View: Table with multi-select, sorting, filtering
- Board View: Kanban board with drag-and-drop
- Task Detail: Breadcrumb navigation, task information
- UI Components: Dialog, Table, Checkbox, Select (Radix UI)
- Build Status: ✅ Passed (TypeScript compilation, 13 static pages)

**Phase 07 (Document & Wiki System):** 60% Complete

- Backend: 100% complete (entities, CQRS, endpoints)
- Frontend: 100% complete (7 components with TipTap editor)
- Integration: Pending (migration, routes, API wiring)

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

#### FR8: Advanced Features (Future Phases)

**Priority:** P2 (Medium)
**Status:** Planned

- **FR8.1:** Goal tracking & OKRs (Phase 08)
- **FR8.2:** Time tracking (Phase 09)
- **FR8.3:** Dashboards & reporting (Phase 10)
- **FR8.4:** Automation & workflow engine (Phase 11)
- **FR8.5:** Mobile responsive design (Phase 12)

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

### Core Entities (21 Domain Models)

**Authentication & Authorization:**

- User, Role, Permission, UserRole, RolePermission, RefreshToken

**Workspace & Projects:**

- Workspace, WorkspaceMember, Project

**Task Management:**

- Task, TaskStatus, Comment, Attachment, ActivityLog

**Document System:**

- Page, PageVersion, PageCollaborator, PageComment

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
                              └───── (N) ActivityLog

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

### Production Readiness Criteria

- [ ] All P0 and P1 requirements implemented
- [ ] > 80% test coverage achieved
- [ ] Performance benchmarks met
- [ ] Security audit passed
- [ ] Monitoring and logging configured
- [ ] Disaster recovery tested
- [ ] User documentation complete

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
