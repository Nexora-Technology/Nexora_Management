# Frontend Src Directory Exploration Report

**Generated:** 2026-01-07  
**Scope:** /Users/nhatduyfirst/Documents/Projects/Nexora_Management/apps/frontend/src  
**Thoroughness:** Medium

## Overview

- **Total Files:** 117 TypeScript/TSX files
- **Total Lines:** ~13,029 lines
- **Architecture:** Feature-based modular structure with route groups

## Directory Structure

```
src/
├── app/              # Next.js App Router (24 route files)
├── components/       # Reusable UI components (40 files)
├── features/         # Feature modules (36 files)
├── lib/             # Utilities & core (8 files)
└── hooks/           # Custom React hooks
```

## Feature Modules (src/features/)

### Core Features

1. **tasks** (4 files)
   - TaskDetailWithRealtime.tsx
   - ViewingAvatars.tsx
   - TypingIndicator.tsx
   - types.ts

2. **goals** (2 files)
   - api.ts
   - types.ts

3. **auth** (1 file)
   - providers/auth-provider.tsx

4. **documents** (6 files)
   - DocumentEditor.tsx
   - PageList.tsx
   - PageTree.tsx
   - Toolbar.tsx
   - VersionHistory.tsx
   - api.ts, types.ts

5. **users** (1 file)
   - OnlineStatus.tsx

6. **notifications** (2 files)
   - NotificationCenter.tsx
   - NotificationPreferences.tsx

### New Features (Recent)

7. **spaces** (4 files) - NEW
   - api.ts
   - types.ts
   - utils.ts
   - index.ts

8. **workspaces** (4 files) - NEW
   - api.ts
   - types.ts
   - workspace-provider.tsx
   - index.ts

### View System

9. **views** (10+ files)
   - ViewSwitcher.tsx
   - ViewContext.tsx
   - ViewLayout.tsx
   - calendar/CalendarView.tsx
   - board/BoardView.tsx, BoardViewWithRealtime.tsx
   - gantt/GanttView.tsx
   - list/ListView.tsx, ListViewWithRealtime.tsx

## New Components Added (src/components/)

### Spaces Feature

- **spaces/space-tree-nav.tsx** - Space navigation tree component
- **spaces/index.ts** - Export barrel

### Workspaces Feature

- **workspaces/workspace-selector.tsx** - Workspace selection dropdown
- **workspaces/index.ts** - Export barrel

### Layout Enhancements

- **layout/app-layout.tsx** - Main app layout wrapper
- **layout/app-header.tsx** - Top navigation header
- **layout/app-sidebar.tsx** - Main sidebar navigation
- **layout/sidebar-nav.tsx** - Sidebar navigation items
- **layout/breadcrumb.tsx** - Breadcrumb navigation
- **layout/board-layout.tsx** - Specialized layout for board views
- **layout/container.tsx** - Layout container utility

### Goals Feature

- **goals/objective-card.tsx** - Objective display card
- **goals/objective-tree.tsx** - Tree view for objectives
- **goals/key-result-editor.tsx** - KR editing interface
- **goals/progress-dashboard.tsx** - Progress visualization

### Tasks Feature (10 files)

- task-board.tsx, task-calendar.tsx, task-gantt.tsx
- task-card.tsx, task-modal.tsx, task-toolbar.tsx
- draggable-task-card.tsx, task-row.tsx
- types.ts, constants.ts, mock-data.ts
- task-board.example.tsx

## New Page Routes (app/(app)/)

### Core Routes

- **/dashboard** - Main dashboard page
- **/spaces** - Spaces management page (NEW)
- **/projects/[id]** - Project detail page
- **/workspaces** - Workspaces list page (NEW)
- **/workspaces/[id]/projects** - Workspace projects page (NEW)

### Task Routes

- **/tasks** - Tasks list page
- **/tasks/[id]** - Task detail page
- **/tasks/board** - Kanban board view

### Goal Routes

- **/goals** - Goals list page
- **/goals/[id]** - Goal detail page

### Document Routes

- **/documents** - Documents management page (NEW)

### Other Routes

- **/calendar** - Calendar view
- **/lists/[id]** - List detail
- **/settings** - Settings page
- **/team** - Team management page

## Route Group Structure

**app/(app)/** - 15 authenticated routes with shared layout
**app/(auth)/** - 3 auth routes (login, register, forgot-password)

## File Count by Category

| Category   | Files   | Notes                          |
| ---------- | ------- | ------------------------------ |
| Routes     | 24      | App pages with Next.js 13+     |
| Components | 40      | UI + feature components        |
| Features   | 36      | Feature-specific logic         |
| Lib        | 8       | Utilities, API client, SignalR |
| Hooks      | ?       | Custom hooks directory         |
| **TOTAL**  | **117** | + ~13K lines of code           |

## Key Architectural Patterns

1. **Feature-based organization** - Each feature has its own module with api, types, components
2. **Route groups** - (app) and (auth) for layout separation
3. **Real-time collaboration** - SignalR integration, typing indicators, viewing avatars
4. **Multi-view support** - Calendar, board, gantt, list views for tasks
5. **Workspace hierarchy** - Workspaces > Spaces > Projects > Tasks
6. **Document management** - Pages, versions, tree structure
7. **Goal tracking** - Objectives with key results

## Technology Stack

- Next.js 13+ (App Router)
- TypeScript
- SignalR (real-time)
- UI components (custom + shadcn/ui style)

## Unresolved Questions

- None - exploration completed successfully
