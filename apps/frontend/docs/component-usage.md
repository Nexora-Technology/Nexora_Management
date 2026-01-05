# ClickUp Design System - Component Usage Guide

**Last Updated:** 2026-01-05
**Version:** Phase 05B Complete

## Overview

The ClickUp Design System provides a consistent UI component library built with:
- **Next.js 15** (App Router)
- **React 19** with TypeScript
- **Tailwind CSS** for styling
- **Radix UI** primitives for complex components

## Quick Start

### Import Paths

```tsx
// UI Components
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from "@/components/ui/card"

// Task Components
import { TaskCard } from "@/components/tasks/task-card"
import { TaskModal } from "@/components/tasks/task-modal"
import { TaskBoard } from "@/components/tasks/task-board"

// Layout Components
import { AppLayout } from "@/components/layout/app-layout"
import { AppHeader } from "@/components/layout/app-header"
import { AppSidebar } from "@/components/layout/app-sidebar"
```

## Core Components

### Button

Multi-variant button with hover/active animations.

```tsx
import { Button } from "@/components/ui/button"

// Variants
<Button variant="primary">Primary Action</Button>
<Button variant="secondary">Secondary</Button>
<Button variant="ghost">Ghost Button</Button>
<Button variant="destructive">Delete</Button>
<Button variant="outline">Outline</Button>
<Button variant="link">Link</Button>

// Sizes
<Button size="sm">Small</Button>
<Button size="md">Medium</Button>
<Button size="lg">Large</Button>
<Button size="icon"><Icon /></Button>

// With icon
<Button variant="primary">
  <Plus className="h-4 w-4" />
  Add Task
</Button>
```

**Props:**
| Prop | Type | Default | Description |
|------|------|---------|-------------|
| variant | `'primary' \| 'secondary' \| 'ghost' \| 'destructive' \| 'outline' \| 'link'` | `'primary'` | Visual style |
| size | `'sm' \| 'md' \| 'lg' \| 'icon'` | `'md'` | Button size |
| asChild | `boolean` | `false` | Render as child (Radix Slot) |

---

### Input

Form input with error state support.

```tsx
import { Input } from "@/components/ui/input"

<Input placeholder="Enter task title" />
<Input type="email" error={hasError} />
<Input disabled value="Read only" />
```

**Props:**
| Prop | Type | Default | Description |
|------|------|---------|-------------|
| error | `boolean` | `false` | Show red error border |
| type | `string` | `'text'` | HTML input type |

---

### Card

Container for grouping related content.

```tsx
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from "@/components/ui/card"

<Card>
  <CardHeader>
    <CardTitle>Task Statistics</CardTitle>
    <CardDescription>Overview of your tasks</CardDescription>
  </CardHeader>
  <CardContent>
    <p>12 tasks completed this week</p>
  </CardContent>
  <CardFooter>
    <Button>View Details</Button>
  </CardFooter>
</Card>
```

**Components:**
- `Card` - Main container
- `CardHeader` - Header section
- `CardTitle` - Title text
- `CardDescription` - Subtitle/description
- `CardContent` - Main content area
- `CardFooter` - Footer/actions

---

## Task Components

### TaskCard

Kanban-style card for displaying tasks.

```tsx
import { TaskCard } from "@/components/tasks/task-card"

<TaskCard
  task={task}
  onClick={() => openDetail(task.id)}
/>
```

**Features:**
- Keyboard navigation (Enter/Space)
- Priority indicator (colored dot)
- Status badge
- Assignee avatar
- Comment/attachment counts
- Optimized with React.memo

**Props:**
| Prop | Type | Required | Description |
|------|------|----------|-------------|
| task | `Task` | ✅ | Task data object |
| onClick | `() => void` | ❌ | Click handler |
| className | `string` | ❌ | Additional classes |

---

### TaskModal

Modal dialog for creating/editing tasks.

```tsx
import { TaskModal } from "@/components/tasks/task-modal"

<TaskModal
  open={isOpen}
  onOpenChange={setIsOpen}
  mode="create"
  onSubmit={handleSubmit}
  isLoading={isSubmitting}
/>
```

**Features:**
- Create/edit modes
- Form validation
- Loading state
- Accessible (ARIA)

**Props:**
| Prop | Type | Default | Description |
|------|------|---------|-------------|
| open | `boolean` | `false` | Modal open state |
| onOpenChange | `(open: boolean) => void` | - | State change handler |
| task | `Task` | - | Task data (edit mode) |
| mode | `'create' \| 'edit'` | `'create'` | Create or edit |
| onSubmit | `(task) => void` | - | Form submit handler |
| isLoading | `boolean` | `false` | Show loading state |

---

### TaskBoard

4-column kanban board layout.

```tsx
import { TaskBoard } from "@/components/tasks/task-board"

<TaskBoard
  tasks={tasks}
  onTaskClick={(task) => openModal(task)}
/>
```

**Columns:** To Do, In Progress, Complete, Overdue

**Props:**
| Prop | Type | Required | Description |
|------|------|----------|-------------|
| tasks | `Task[]` | ✅ | Array of tasks |
| onTaskClick | `(task: Task) => void` | ❌ | Click handler |
| className | `string` | ❌ | Additional classes |

---

## Layout Components

### AppLayout

Main layout wrapper with header + sidebar.

```tsx
import { AppLayout } from "@/components/layout/app-layout"

export default function Layout({ children }: { children: React.ReactNode }) {
  return <AppLayout>{children}</AppLayout>
}
```

**Used in:** `src/app/(app)/layout.tsx` for authenticated pages.

---

### AppHeader

Header with sidebar toggle, search, notifications.

```tsx
import { AppHeader } from "@/components/layout/app-header"

<AppHeader
  sidebarCollapsed={collapsed}
  onToggleSidebar={() => setCollapsed(!collapsed)}
/>
```

---

### AppSidebar

Collapsible sidebar navigation.

```tsx
import { AppSidebar } from "@/components/layout/app-sidebar"

<AppSidebar collapsed={collapsed} />
```

**Navigation items:** Home, Tasks, Projects, Team, Calendar, Settings

---

## Live Examples

Visit the component showcase page for live examples:
```
/components/showcase
```

## Animation Tokens

```tsx
// Duration
className="duration-fast"    // 150ms
className="duration-base"     // 200ms
className="duration-slow"     // 300ms

// Keyframes
className="animate-fade-in"   // Fade in
className="animate-slide-up"  // Slide up
className="animate-scale-in"  // Scale in
```

## Best Practices

1. **Use semantic variants** - `primary` for main actions, `ghost` for secondary
2. **Provide error feedback** - Use `error` prop on Input for validation
3. **Optimize rendering** - Task components use React.memo (no manual optimization needed)
4. **Keyboard accessibility** - All interactive elements support keyboard navigation
5. **Loading states** - Use `isLoading` prop for async actions

## TypeScript Support

All components have full TypeScript support. Hover over props in your IDE for type information and JSDoc documentation.

## Related Documentation

- [Design Guidelines](./design-guidelines.md)
- [Code Standards](./code-standards.md)
- [System Architecture](./system-architecture.md)
