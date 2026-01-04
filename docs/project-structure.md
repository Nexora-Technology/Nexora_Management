# Nexora Management - Project Structure

Complete directory structure of the Nexora Management platform as of **2026-01-04**.

## Overview

Nexora Management is a **monorepo** built with:

- **Backend**: .NET 9.0 Web API (Clean Architecture)
- **Frontend**: Next.js 15 with App Router (Feature-based structure)
- **Database**: PostgreSQL with EF Core 9
- **Real-time**: SignalR for live updates

---

## Root Structure

```
Nexora_Management/
├── .claude/                    # Claude Code configuration & skills
├── .github/                    # GitHub Actions CI/CD
│   └── workflows/
│       ├── pr-checks.yml
│       └── build.yml
├── apps/                       # Monorepo applications
│   ├── backend/                # .NET 9.0 Web API
│   └── frontend/               # Next.js 15 frontend
├── docs/                       # Documentation (see below)
├── plans/                      # Implementation plans
│   ├── 2026-01-03-nexora-management-platform/
│   │   └── phase-07-document-wiki-system.md
│   └── 2026-01-04-phase-07-document-wiki-implementation.md
├── docker-compose.yml          # Container orchestration
├── turbo.json                  # Turborepo config
├── package.json                # Root package.json
└── README.md
```

---

## Backend Structure

**Location**: `apps/backend/src/`

**Architecture**: Clean Architecture (Onion DDD)

### Layer Overview

```
apps/backend/src/
├── Nexora.Management.Domain/          # Innermost layer - Entities & interfaces
├── Nexora.Management.Application/     # Business logic - CQRS, Use cases
├── Nexora.Management.Infrastructure/  # External concerns - DB, Services
└── Nexora.Management.API/             # Outermost layer - Endpoints, Middleware
```

### Domain Layer (`Nexora.Management.Domain/`)

Core business entities and interfaces. No dependencies on other layers.

```
Nexora.Management.Domain/
├── Common/                    # Base classes, shared types
│   ├── BaseEntity.cs
│   ├── Result.cs
│   └── Errors.cs
├── Entities/                  # Domain entities
│   ├── ActivityLog.cs
│   ├── Attachment.cs
│   ├── Comment.cs
│   ├── Notification.cs
│   ├── NotificationPreference.cs
│   ├── Permission.cs
│   ├── Project.cs
│   ├── RefreshToken.cs
│   ├── Role.cs
│   ├── RolePermission.cs
│   ├── Task.cs
│   ├── TaskStatus.cs
│   ├── User.cs
│   ├── UserPresence.cs
│   ├── UserRole.cs
│   ├── Workspace.cs
│   └── WorkspaceMember.cs
└── Interfaces/                # Domain contracts
    └── *Repository.cs
```

### Application Layer (`Nexora.Management.Application/`)

Business logic, CQRS handlers, DTOs. References Domain only.

```
Nexora.Management.Application/
├── Attachments/               # Attachment feature
│   ├── Commands/
│   │   ├── UploadAttachmentCommand.cs
│   │   └── DeleteAttachmentCommand.cs
│   ├── Queries/
│   │   └── GetAttachmentQuery.cs
│   └── DTOs/
│       └── AttachmentDTOs.cs
├── Authentication/            # Auth feature
│   ├── Commands/
│   │   ├── RegisterCommand.cs
│   │   ├── LoginCommand.cs
│   │   └── RefreshTokenCommand.cs
│   └── DTOs/
│       └── AuthDTOs.cs
├── Comments/                  # Comment feature
│   ├── Commands/
│   │   ├── CreateCommentCommand.cs
│   │   └── DeleteCommentCommand.cs
│   ├── Queries/
│   │   └── GetCommentsQuery.cs
│   └── DTOs/
│       └── CommentDTOs.cs
├── Tasks/                     # Task management feature
│   ├── Commands/
│   │   ├── CreateTask/
│   │   │   ├── CreateTaskCommand.cs
│   │   │   ├── CreateTaskCommandHandler.cs
│   │   │   └── CreateTaskCommandValidator.cs
│   │   ├── UpdateTask/
│   │   ├── UpdateTaskStatus/
│   │   └── DeleteTask/
│   ├── Queries/
│   │   ├── TaskQueries.cs
│   │   └── ViewQueries/
│   │       ├── BoardViewQuery.cs
│   │       ├── CalendarViewQuery.cs
│   │       └── GanttViewQuery.cs
│   └── DTOs/
│       ├── TaskDTOs.cs
│       └── ViewDTOs.cs
├── Common/                    # Shared application logic
├── DTOs/                      # Shared DTOs
│   └── SignalR/
│       └── NotificationDTOs.cs
└── Interfaces/                # Application services
```

**CQRS Pattern**: Each feature has:

- **Commands** (write): Create/Update/Delete
- **Queries** (read): Fetch data
- **DTOs**: Data transfer objects
- **Validators**: FluentValidation rules

### Infrastructure Layer (`Nexora.Management.Infrastructure/`)

Data access, external services, implementations of Domain interfaces.

```
Nexora.Management.Infrastructure/
├── Persistence/               # EF Core DbContext
│   ├── AppDbContext.cs
│   └── Configurations/        # EF entity configs
│       ├── ActivityLogConfiguration.cs
│       ├── AttachmentConfiguration.cs
│       ├── CommentConfiguration.cs
│       ├── NotificationConfiguration.cs
│       ├── NotificationPreferenceConfiguration.cs
│       ├── PermissionConfiguration.cs
│       ├── ProjectConfiguration.cs
│       ├── RefreshTokenConfiguration.cs
│       ├── RoleConfiguration.cs
│       ├── RolePermissionConfiguration.cs
│       ├── TaskConfiguration.cs
│       ├── TaskStatusConfiguration.cs
│       ├── UserConfiguration.cs
│       ├── UserPresenceConfiguration.cs
│       ├── UserRoleConfiguration.cs
│       ├── WorkspaceConfiguration.cs
│       └── WorkspaceMemberConfiguration.cs
├── Authentication/            # JWT implementation
│   ├── JwtService.cs
│   └── PasswordHasher.cs
├── Services/                  # External services
└── Middlewares/               # Custom middleware
```

### API Layer (`Nexora.Management.API/`)

Controllers, endpoints, SignalR hubs, middleware.

```
Nexora.Management.API/
├── Endpoints/                 # Minimal API endpoints
│   ├── AuthEndpoints.cs
│   ├── TaskEndpoints.cs
│   ├── CommentEndpoints.cs
│   └── AttachmentEndpoints.cs
├── Hubs/                      # SignalR hubs
│   └── NotificationHub.cs
├── Middlewares/               # API middleware
│   └── ExceptionMiddleware.cs
├── Extensions/                # API extensions
│   └── ServiceExtensions.cs
├── Persistence/               # Migration files
│   └── Migrations/
├── Services/                  # API services
└── Program.cs                 # Application entry point
```

---

## Frontend Structure

**Location**: `apps/frontend/src/`

**Architecture**: Feature-based with App Router

### Overall Structure

```
apps/frontend/src/
├── app/                       # Next.js App Router (file-based routing)
├── components/                # Shared React components
├── features/                  # Feature modules (self-contained)
├── hooks/                     # Custom React hooks
├── lib/                       # Utilities, configurations
└── public/                    # Static assets
```

### App Router (`app/`)

Next.js 15 file-based routing with layouts.

```
app/
├── (auth)/                    # Auth route group
│   ├── login/
│   │   └── page.tsx
│   └── register/
│       └── page.tsx
├── dashboard/                 # Dashboard route
│   └── page.tsx
├── projects/                  # Projects route
│   └── [projectId]/
│       └── page.tsx
├── workspaces/                # Workspaces route
│   └── page.tsx
├── layout.tsx                 # Root layout
└── page.tsx                   # Home page
```

### Components (`components/`)

Reusable UI components organized by domain.

```
components/
├── layout/                    # Layout components
│   ├── Header.tsx
│   ├── Sidebar.tsx
│   └── Footer.tsx
└── ui/                        # shadcn/ui components
    ├── button.tsx
    ├── input.tsx
    ├── dialog.tsx
    └── ...
```

### Features (`features/`)

**Feature-based architecture**: Each feature is self-contained with its own components, hooks, types.

```
features/
├── auth/                      # Authentication feature
│   ├── LoginForm.tsx
│   ├── RegisterForm.tsx
│   ├── useAuth.ts
│   └── types.ts
├── tasks/                     # Task management feature
│   ├── TaskDetailWithRealtime.tsx
│   ├── TaskList.tsx
│   ├── TaskCard.tsx
│   ├── ViewingAvatars.tsx
│   ├── TypingIndicator.tsx
│   └── types.ts
├── views/                     # Views feature (Board/Calendar/Gantt)
│   ├── BoardView.tsx
│   ├── CalendarView.tsx
│   ├── GanttView.tsx
│   ├── ViewSwitcher.tsx
│   └── components/
│       ├── BoardColumn.tsx
│       ├── CalendarGrid.tsx
│       └── GanttTimeline.tsx
├── users/                     # User management feature
│   ├── UserAvatar.tsx
│   ├── UserSelect.tsx
│   └── UserProfile.tsx
├── notifications/             # Notifications feature
│   ├── NotificationToast.tsx
│   ├── NotificationPanel.tsx
│   └── useNotifications.ts
└── documents/                 # 📋 Documents feature (Phase 07 - NEW)
    ├── editor/
    │   ├── DocumentEditor.tsx
    │   ├── Toolbar.tsx
    │   ├── SlashMenu.tsx
    │   └── extensions/
    │       └── custom-extensions.ts
    ├── pages/
    │   ├── PageList.tsx
    │   ├── PageTree.tsx
    │   ├── MovePageDialog.tsx
    │   └── CreatePageDialog.tsx
    ├── versions/
    │   ├── VersionHistory.tsx
    │   ├── VersionDiff.tsx
    │   └── RestoreVersion.tsx
    ├── comments/
    │   ├── DocumentComments.tsx
    │   ├── CommentItem.tsx
    │   └── CommentForm.tsx
    ├── collaboration/
    │   ├── CollaborationCursors.tsx
    │   └── TypingIndicator.tsx
    ├── hooks/
    │   ├── use-pages.ts
    │   ├── use-editor.ts
    │   └── use-versions.ts
    ├── types.ts
    └── index.ts
```

**Feature Pattern**: Each feature folder contains:

- **Components**: Feature-specific UI
- **Hooks**: Custom React hooks
- **Types**: TypeScript interfaces
- **API**: Data fetching logic
- **Utils**: Feature utilities

### Hooks (`hooks/`)

Global custom hooks used across features.

```
hooks/
├── signalr/                   # SignalR hooks
│   ├── useSignalR.ts
│   └── useNotificationHub.ts
└── *other-hooks.ts
```

### Library (`lib/`)

Utilities, configurations, API clients.

```
lib/
├── signalr/                   # SignalR setup
│   ├── createHubConnection.ts
│   └── hubs.ts
├── utils.ts                   # General utilities
├── cn.ts                      # Class names (clsx + tailwind-merge)
└── api.ts                     # API client (axios)
```

---

## Documentation Structure

```
docs/
├── README.md                          # Docs index
├── project-overview-pdr.md            # Project overview
├── codebase-summary.md                # Quick reference
├── code-standards.md                  # Coding conventions
├── design-guidelines.md               # UI/UX principles
├── system-architecture.md             # Clean Architecture overview
├── deployment-guide.md                # Build & run instructions
├── infrastructure-setup.md            # Monorepo, Docker, CI/CD
├── development-standards.md           # Git, linting, workflows
├── project-roadmap.md                 # Feature phases
├── tech-stack.md                      # Technology choices
├── adr/                               # Architecture Decision Records
│   └── 001-architecture-decisions.md
├── development/                       # Dev guides
│   └── local-setup.md
└── research/                          # Research docs
    ├── clickup-research.md
    └── shadcn-ui-research.md
```

---

## Plans Structure

```
plans/
├── 2026-01-03-nexora-management-platform/
│   ├── phase-01-foundation.md
│   ├── phase-02-database-schema.md
│   ├── phase-03-authentication.md
│   ├── phase-04-task-management.md
│   ├── phase-05-views-system.md
│   ├── phase-06-realtime-collaboration.md
│   └── phase-07-document-wiki-system.md
└── 2026-01-04-phase-07-document-wiki-implementation.md
```

---

## Configuration Files

### Root Level

- `docker-compose.yml` - Multi-container setup
- `turbo.json` - Turborepo build pipeline
- `package.json` - Root npm scripts
- `.gitignore` - Git ignore rules
- `.husky/` - Git hooks

### Backend

- `apps/backend/src/Nexora.Management.API/appsettings.json`
- `apps/backend/src/Nexora.Management.API/appsettings.Development.json`

### Frontend

- `apps/frontend/package.json` - Frontend dependencies
- `apps/frontend/next.config.js` - Next.js config
- `apps/frontend/tailwind.config.ts` - Tailwind CSS
- `apps/frontend/tsconfig.json` - TypeScript config

---

## Database Migrations

Located in: `apps/backend/src/Nexora.Management.API/Persistence/Migrations/`

Each migration represents a schema change:

- `InitialCreate.cs` - Base schema
- `AddNotificationTables.cs` - Notifications feature
- `AddUserPresenceTables.cs` - User presence tracking
- (More added as features grow)

---

## Key Patterns & Conventions

### Backend

1. **Clean Architecture**: Domain → Application → Infrastructure → API
2. **CQRS**: Separate commands (write) and queries (read)
3. **MediatR**: In-memory request/response handling
4. **FluentValidation**: Request validation
5. **Result Pattern**: `Result<T>` for error handling

### Frontend

1. **Feature-based**: Self-contained feature modules
2. **App Router**: File-based routing with layouts
3. **Server Components**: Default, Client Components when needed
4. **React Query**: Data fetching & caching
5. **Zustand**: Global state management
6. **SignalR**: Real-time updates

### Naming Conventions

- **Backend**: PascalCase for classes, camelCase for methods
- **Frontend**: PascalCase for components, camelCase for utilities
- **API**: kebab-case for endpoints (`/api/workspaces/{id}`)
- **Database**: snake_case for tables/columns

---

## Phase 07 Additions (New Structure)

### Backend - Documents Feature

```
Nexora.Management.Domain/Entities/
├── Page.cs                    # NEW - Wiki page entity
├── PageVersion.cs             # NEW - Version history
├── PageCollaborator.cs        # NEW - Page collaborators
└── PageComment.cs             # NEW - Page comments

Nexora.Management.Infrastructure/Persistence/Configurations/
├── PageConfiguration.cs       # NEW
├── PageVersionConfiguration.cs # NEW
└── PageCommentConfiguration.cs # NEW

Nexora.Management.Application/Documents/
├── Commands/
│   ├── CreatePage/
│   ├── UpdatePage/
│   ├── DeletePage/
│   ├── RestorePageVersion/
│   ├── ToggleFavorite/
│   └── MovePage/
├── Queries/
│   ├── GetPageById/
│   ├── GetPageTree/
│   ├── GetPageHistory/
│   ├── SearchPages/
│   └── GetFavoritePages/
└── DTOs/
    └── DocumentDTOs.cs

Nexora.Management.API/Endpoints/
└── DocumentEndpoints.cs       # NEW
```

### Frontend - Documents Feature

```
features/documents/            # NEW - Documents feature (Phase 07)
├── DocumentEditor.tsx         # TipTap rich text editor
├── Toolbar.tsx                # Formatting toolbar
├── PageTree.tsx               # Hierarchical page tree with search
├── PageList.tsx               # Page list with favorites/recent
├── VersionHistory.tsx         # Version history with restore
├── types.ts                   # Document types
├── api.ts                     # Documents API client
└── index.ts                   # Exports
```

---

## File Count Summary

| Layer                  | Files          | Purpose                                   |
| ---------------------- | -------------- | ----------------------------------------- |
| Backend Domain         | 22 entities    | Core business entities (+4 for Documents) |
| Backend Application    | ~60 handlers   | CQRS handlers (+10 for Documents)         |
| Backend Infrastructure | 21 configs     | EF configs (+4 for Documents)             |
| Backend API            | 5 endpoints    | API routes (+Documents)                   |
| Frontend App           | ~10 routes     | File-based routing                        |
| Frontend Features      | 7 features     | Feature modules (+Documents)              |
| Frontend Components    | ~40 components | Reusable UI (+7 Document components)      |

---

## Quick Navigation

- **Backend API**: `apps/backend/src/Nexora.Management.API/`
- **Backend Domain**: `apps/backend/src/Nexora.Management.Domain/Entities/`
- **Backend App**: `apps/backend/src/Nexora.Management.Application/`
- **Frontend Features**: `apps/frontend/src/features/`
- **Frontend Routes**: `apps/frontend/src/app/`
- **Documentation**: `docs/`
- **Plans**: `plans/`

---

**Last Updated**: 2026-01-04
**Total Features**: 7 (Auth, Tasks, Views, Notifications, Users, Comments, Documents)
**Tech Stack**: .NET 9.0, Next.js 15, PostgreSQL, SignalR, TipTap
**Phase 07 Status**: Backend 100% complete, Frontend 100% complete, Database pending
