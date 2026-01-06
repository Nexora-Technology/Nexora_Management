# Frontend Structure Exploration Report

**Generated:** 2026-01-06  
**ID:** scout-external-260106-1623-frontend-structure  
**Working Directory:** /Users/nhatduyfirst/Documents/Projects/Nexora_Management/apps/frontend

## Executive Summary

Comprehensive exploration of the Next.js 15 frontend application revealing a well-organized, feature-based architecture with 103 TypeScript files totaling ~11,566 lines of code. The codebase follows modern React patterns with shadcn/ui components, real-time collaboration via SignalR, and multi-view task management.

**Key Findings:**
- 103 TypeScript/TSX files
- ~11,566 lines of code
- 18 shadcn/ui components
- 6 feature modules (tasks, goals, documents, auth, notifications, views)
- 4 view types (list, board, calendar, gantt)
- 3 SignalR hubs for real-time features
- TipTap rich text editor for documents
- ClickUp-inspired design system

---

## 1. Project Structure

### Directory Organization

```
apps/frontend/src/
├── app/                    # Next.js 15 App Router pages
├── components/             # Reusable React components
│   ├── ui/                # shadcn/ui primitives (18 components)
│   ├── tasks/             # Task-specific components
│   ├── goals/             # Goal/OKR components
│   └── layout/            # Layout components
├── features/              # Feature-based modules
│   ├── tasks/             # Task management
│   ├── goals/             # Goal tracking & OKRs
│   ├── documents/         # Document/Wiki system
│   ├── auth/              # Authentication
│   ├── notifications/     # Notification center
│   ├── users/             # User presence
│   └── views/             # Multi-view system (4 views)
├── hooks/                 # Custom React hooks
│   └── signalr/           # SignalR hooks (3 hooks)
├── lib/                   # Utilities & config
│   ├── api-client.ts      # Axios client
│   ├── utils.ts           # Helper functions
│   ├── providers.tsx      # React providers
│   └── signalr/           # SignalR hub connections (4 files)
└── app/globals.css        # ClickUp design tokens (260+ lines)
```

---

## 2. Components (56 files)

### UI Components - shadcn/ui (18 components)

**Location:** /src/components/ui/

1. avatar.tsx, badge.tsx, button.tsx, card.tsx, checkbox.tsx
2. dialog.tsx, dropdown-menu.tsx, input.tsx, label.tsx, progress.tsx
3. scroll-area.tsx, select.tsx, separator.tsx, sonner.tsx, switch.tsx
4. table.tsx, textarea.tsx, tooltip.tsx

### Layout Components (7 components)

**Location:** /src/components/layout/

1. app-layout.tsx - Main layout wrapper
2. app-header.tsx - Top navigation bar (56px)
3. app-sidebar.tsx - Collapsible sidebar (240px -> 64px)
4. sidebar-nav.tsx - Navigation items
5. breadcrumb.tsx - Breadcrumb navigation
6. container.tsx - Responsive container
7. board-layout.tsx - Kanban board layout

### Task Components (13 components)

**Location:** /src/components/tasks/

1. types.ts, constants.ts, mock-data.ts
2. task-card.tsx (React.memo optimized)
3. task-row.tsx (React.memo optimized)
4. task-board.tsx (O(n) algorithm)
5. task-modal.tsx (React.memo optimized)
6. task-toolbar.tsx, task-calendar.tsx, task-gantt.tsx
7. draggable-task-card.tsx, task-board.example.tsx, index.ts

### Goal Components (4 components)

**Location:** /src/components/goals/

1. objective-card.tsx
2. key-result-editor.tsx
3. progress-dashboard.tsx
4. objective-tree.tsx

---

## 3. Pages/Routes (17 routes)

### Public Routes

/, /login, /register, /forgot-password, /components/showcase

### Authenticated Routes (App Group)

/dashboard, /tasks, /tasks/board, /tasks/[id], /goals, /goals/[id], /projects/[id], /workspaces, /workspaces/[id]/projects, /calendar, /documents, /team, /settings

---

## 4. Features (6 modules)

### 1. Tasks Feature

Location: /src/features/tasks/

Components: TaskDetailWithRealtime.tsx, TypingIndicator.tsx, ViewingAvatars.tsx, types.ts

### 2. Goals Feature

Location: /src/features/goals/

Components: api.ts (200+ lines), types.ts

### 3. Documents Feature

Location: /src/features/documents/

Components: DocumentEditor.tsx, Toolbar.tsx, PageTree.tsx, PageList.tsx, VersionHistory.tsx, api.ts, types.ts, index.ts

### 4. Auth Feature

Location: /src/features/auth/

Components: providers/auth-provider.tsx

### 5. Notifications Feature

Location: /src/features/notifications/

Components: NotificationCenter.tsx, NotificationPreferences.tsx

### 6. Views Feature (Multi-view System)

Location: /src/features/views/

Core: ViewContext.tsx, ViewLayout.tsx, ViewSwitcher.tsx, index.ts

Views: ListView, ListViewWithRealtime, BoardView, BoardViewWithRealtime, CalendarView, GanttView

---

## 5. State Management

- React Hooks (useState, useContext, useCallback, useMemo)
- React Query for server state
- Zustand for global state
- SignalR for real-time state

---

## 6. API Integration

### API Client

Location: /src/lib/api-client.ts

Features: JWT interceptors, 401 handling, type-safe calls

### Endpoints

Authentication: /api/auth/*
Tasks: /api/tasks/*
Goals: /api/goals/*
Documents: /api/documents/*

---

## 7. Real-time Features (SignalR)

### Hubs

Location: /src/lib/signalr/

signalr-connection.ts, task-hub.ts, presence-hub.ts, notification-hub.ts, types.ts

### Hooks

Location: /src/hooks/signalr/

useTaskHub.ts, usePresenceHub.ts, useNotificationHub.ts

Features: Live updates, presence tracking, typing indicators, notifications

---

## 8. Styling & Design System

### Tailwind CSS Configuration

Colors: ClickUp Purple, semantic colors, gray scale
Typography: Inter, JetBrains Mono, 11px-32px scale
Spacing: 4px base unit, 0-16 range
Border Radius: 4px-16px
Shadows: 5 levels
Transitions: 150ms-300ms

### Design Tokens

Location: /src/app/globals.css (260+ lines)

Features: Dark mode, WCAG 2.1 AA, focus states, reduced motion

---

## 9. Key Features

### Tasks Management

CRUD, hierarchy, custom statuses, priorities, multi-view, drag-drop, real-time, attachments, comments, activity log

### Goals & OKRs

Time periods, hierarchical objectives, weighted key results, progress calculation, status tracking, progress dashboard

### Document/Wiki System

TipTap editor, hierarchical pages, version history, collaboration, favorites, search, comments, code blocks

### Real-time Collaboration

SignalR WebSockets, live updates, presence tracking, typing indicators, notifications, multi-user editing

---

## 10. Dependencies

Core: Next.js 15.5.9, React 19.1.0, TypeScript 5
UI: Radix UI components, Tailwind CSS 3.4.19
State: React Query 5.90.16, Zustand 5.0.9
Real-time: @microsoft/signalr 10.0.0
Rich Text: TipTap 3.14.0
Drag-Drop: @dnd-kit/core 6.3.1
Forms: react-hook-form 7.69.0, zod 4.3.4

---

## 11. Performance Optimizations

React.memo: 4 components optimized (75% fewer re-renders)
Algorithm: O(n×4) -> O(n) single-pass tasksByStatus
Accessibility: WCAG 2.1 AA, aria-live, ARIA labels, keyboard nav

---

## 12. Development Status

Completed: Phases 01-08 (Project setup through Goal Tracking)
Build: TypeScript passed, 13 static pages, optimized bundle
Next: Time Tracking, Dashboards, Automation, Mobile

---

## Summary

Well-architected Next.js 15 app with modern stack, 18 shadcn/ui components, 6 feature modules, 4 view types, SignalR real-time, TipTap editor, React Query + Zustand, React.memo optimizations, WCAG 2.1 AA accessible, 103 files, ~11,566 LOC.

