# Phase 03: Layouts - Layout Patterns

**Date:** 2026-01-04
**Priority:** High
**Status:** Done
**Estimated Time:** 6 hours
**Completed:** 2026-01-05 00:30

## Context

**Overview:** [plan.md](./plan.md)
**Prerequisites:** [phase-01-foundation.md](./phase-01-foundation.md) and [phase-02-components.md](./phase-02-components.md) must be complete

Creates the structural layout patterns that organize content. These layouts provide the framework for views and ensure consistent spatial relationships across the application.

**Why It Matters:**
- Consistent layouts reduce cognitive load
- Responsive patterns work across all screen sizes
- Reusable layouts speed up feature development
- Proper structure supports accessibility (landmarks, ARIA)

## Key Insights

- **ClickUp uses 3-column layout** - sidebar (240px), main content (flex), and optional right panel
- **Header is 56px tall** - contains search, notifications, profile
- **Sidebar collapses to 64px** - icons only on tablet/mobile
- **Main content has max-width** - 1280px centered on large screens
- **Breadcrumbs show navigation path** - 32px tall, gray text
- **Boards use horizontal scroll** - columns don't shrink below 280px

## Requirements

### Functional Requirements
- **FR-01:** Main app layout with header, sidebar, and content area
- **FR-02:** Sidebar with collapsible navigation
- **FR-03:** Breadcrumb component for navigation hierarchy
- **FR-04:** Container system with responsive padding
- **FR-05:** Board view layout with horizontal scroll
- **FR-06:** Responsive behavior for mobile/tablet/desktop

### Non-Functional Requirements
- **NFR-01:** Layouts must be responsive (mobile-first approach)
- **NFR-02:** Sidebar state must persist across navigation
- **NFR-03:** Layouts must use semantic HTML (nav, main, header, aside)
- **NFR-04:** All layouts must support dark mode
- **NFR-05:** Layout bundle size increase < 30KB

## Architecture

### Layout Component Structure

```
apps/frontend/src/components/layout/
├── app-layout.tsx          # Main layout wrapper
├── app-header.tsx          # Top header (56px)
├── app-sidebar.tsx         # Collapsible sidebar
├── sidebar-nav.tsx         # Navigation items
├── breadcrumb.tsx          # Breadcrumb component
├── container.tsx           # Responsive container
└── board-layout.tsx        # Board view layout
```

### Layout Hierarchy

```
AppLayout
├── AppHeader (fixed top, 56px)
└── div.flex (full height - 56px)
    ├── AppSidebar (240px → 64px collapsed)
    └── main (flex-1, overflow-auto)
        ├── Container (max-width 1280px)
        │   ├── Breadcrumb (optional, 32px)
        │   └── Page Content
        └── BoardLayout (for board views)
```

## Related Code Files

### Files to Create
- `/apps/frontend/src/components/layout/app-layout.tsx`
- `/apps/frontend/src/components/layout/app-header.tsx`
- `/apps/frontend/src/components/layout/app-sidebar.tsx`
- `/apps/frontend/src/components/layout/sidebar-nav.tsx`
- `/apps/frontend/src/components/layout/breadcrumb.tsx`
- `/apps/frontend/src/components/layout/container.tsx`
- `/apps/frontend/src/components/layout/board-layout.tsx`

### Files to Modify
- `/apps/frontend/src/app/layout.tsx` - Add app layout wrapper
- `/docs/design-guidelines.md` - Document layout patterns

## Implementation Steps

### Step 1: App Layout Wrapper (1 hour)

**1.1 Create AppLayout Component**

```typescript
// apps/frontend/src/components/layout/app-layout.tsx
"use client"

import * as React from "react"
import { AppHeader } from "./app-header"
import { AppSidebar } from "./app-sidebar"

interface AppLayoutProps {
  children: React.ReactNode
}

export function AppLayout({ children }: AppLayoutProps) {
  const [sidebarCollapsed, setSidebarCollapsed] = React.useState(false)

  return (
    <div className="flex h-screen flex-col bg-gray-50 dark:bg-gray-900">
      {/* Header - 56px */}
      <AppHeader
        sidebarCollapsed={sidebarCollapsed}
        onToggleSidebar={() => setSidebarCollapsed(!sidebarCollapsed)}
      />

      {/* Main Content Area */}
      <div className="flex flex-1 overflow-hidden">
        {/* Sidebar - 240px → 64px */}
        <AppSidebar collapsed={sidebarCollapsed} />

        {/* Content */}
        <main className="flex-1 overflow-auto">
          {children}
        </main>
      </div>
    </div>
  )
}
```

**1.2 Integrate in Root Layout**

```typescript
// apps/frontend/src/app/layout.tsx
import { AppLayout } from "@/components/layout/app-layout"

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body>
        <AppLayout>
          {children}
        </AppLayout>
      </body>
    </html>
  )
}
```

### Step 2: App Header (1 hour)

**2.1 Create Header Component**

```typescript
// apps/frontend/src/components/layout/app-header.tsx
"use client"

import * as React from "react"
import { Menu, Search, Bell, Settings } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Avatar } from "@/components/ui/avatar"
import { Input } from "@/components/ui/input"

interface AppHeaderProps {
  sidebarCollapsed: boolean
  onToggleSidebar: () => void
}

export function AppHeader({ sidebarCollapsed, onToggleSidebar }: AppHeaderProps) {
  return (
    <header className="flex h-14 items-center justify-between border-b border-gray-200 bg-white px-4 dark:bg-gray-800 dark:border-gray-700">
      {/* Left Section */}
      <div className="flex items-center gap-4">
        {/* Collapse Button */}
        <Button
          variant="ghost"
          size="icon"
          onClick={onToggleSidebar}
          className="h-9 w-9"
        >
          <Menu className="h-5 w-5" />
        </Button>

        {/* Logo */}
        <div className="flex items-center gap-2">
          <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-primary to-primary-hover" />
          <span className="text-lg font-semibold text-gray-900 dark:text-white">
            Nexora
          </span>
        </div>

        {/* Search */}
        <div className="hidden md:block ml-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
            <Input
              type="search"
              placeholder="Search..."
              className="h-9 w-64 pl-9"
            />
          </div>
        </div>
      </div>

      {/* Right Section */}
      <div className="flex items-center gap-2">
        {/* Notifications */}
        <Button variant="ghost" size="icon" className="h-9 w-9">
          <Bell className="h-5 w-5" />
        </Button>

        {/* Settings */}
        <Button variant="ghost" size="icon" className="h-9 w-9">
          <Settings className="h-5 w-5" />
        </Button>

        {/* Profile */}
        <Avatar className="h-9 w-9 cursor-pointer">
          <AvatarFallback name="User" />
        </Avatar>
      </div>
    </header>
  )
}
```

### Step 3: App Sidebar (1.5 hours)

**3.1 Create Sidebar Component**

```typescript
// apps/frontend/src/components/layout/app-sidebar.tsx
"use client"

import * as React from "react"
import { cn } from "@/lib/utils"
import { SidebarNav } from "./sidebar-nav"

interface AppSidebarProps {
  collapsed?: boolean
}

export function AppSidebar({ collapsed = false }: AppSidebarProps) {
  return (
    <aside
      className={cn(
        "flex-shrink-0 border-r border-gray-200 bg-white transition-all duration-200 dark:bg-gray-800 dark:border-gray-700",
        collapsed ? "w-16" : "w-60"
      )}
    >
      {/* Navigation */}
      <nav className="flex h-full flex-col overflow-y-auto py-4">
        <SidebarNav collapsed={collapsed} />
      </nav>
    </aside>
  )
}
```

**3.2 Create Navigation Items**

```typescript
// apps/frontend/src/components/layout/sidebar-nav.tsx
"use client"

import * as React from "react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import {
  Home,
  CheckSquare,
  Folder,
  Users,
  Calendar,
  Settings,
  ChevronRight,
} from "lucide-react"
import { cn } from "@/lib/utils"

const navItems = [
  {
    title: "Home",
    href: "/",
    icon: Home,
  },
  {
    title: "Tasks",
    href: "/tasks",
    icon: CheckSquare,
  },
  {
    title: "Projects",
    href: "/projects",
    icon: Folder,
  },
  {
    title: "Team",
    href: "/team",
    icon: Users,
  },
  {
    title: "Calendar",
    href: "/calendar",
    icon: Calendar,
  },
  {
    title: "Settings",
    href: "/settings",
    icon: Settings,
  },
]

interface SidebarNavProps {
  collapsed?: boolean
}

export function SidebarNav({ collapsed = false }: SidebarNavProps) {
  const pathname = usePathname()

  return (
    <div className="space-y-1 px-3">
      {navItems.map((item) => {
        const isActive = pathname === item.href
        const Icon = item.icon

        return (
          <Link
            key={item.href}
            href={item.href}
            className={cn(
              "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-all",
              "hover:bg-gray-100 dark:hover:bg-gray-700",
              isActive
                ? "bg-primary/10 text-primary"
                : "text-gray-600 dark:text-gray-400",
              collapsed && "justify-center px-0"
            )}
          >
            <Icon className="h-5 w-5 flex-shrink-0" />
            {!collapsed && (
              <>
                <span className="flex-1">{item.title}</span>
                {isActive && <ChevronRight className="h-4 w-4" />}
              </>
            )}
          </Link>
        )
      })}
    </div>
  )
}
```

### Step 4: Breadcrumb Component (45 minutes)

**4.1 Create Breadcrumb**

```typescript
// apps/frontend/src/components/layout/breadcrumb.tsx
"use client"

import * as React from "react"
import Link from "next/link"
import { ChevronRight } from "lucide-react"
import { cn } from "@/lib/utils"

interface BreadcrumbItem {
  label: string
  href?: string
}

interface BreadcrumbProps {
  items: BreadcrumbItem[]
  className?: string
}

export function Breadcrumb({ items, className }: BreadcrumbProps) {
  return (
    <nav
      className={cn(
        "flex items-center gap-2 text-sm",
        "text-gray-500 dark:text-gray-400",
        className
      )}
      aria-label="Breadcrumb"
    >
      {items.map((item, index) => (
        <React.Fragment key={index}>
          {index > 0 && (
            <ChevronRight className="h-4 w-4 flex-shrink-0" />
          )}
          {item.href ? (
            <Link
              href={item.href}
              className="hover:text-gray-900 dark:hover:text-gray-200"
            >
              {item.label}
            </Link>
          ) : (
            <span className="text-gray-900 dark:text-gray-200">
              {item.label}
            </span>
          )}
        </React.Fragment>
      ))}
    </nav>
  )
}
```

**4.2 Usage Example**

```tsx
<Breadcrumb
  items={[
    { label: "Home", href: "/" },
    { label: "Tasks", href: "/tasks" },
    { label: "Task Detail" },
  ]}
/>
```

### Step 5: Container Component (30 minutes)

**5.1 Create Responsive Container**

```typescript
// apps/frontend/src/components/layout/container.tsx
import * as React from "react"
import { cn } from "@/lib/utils"

interface ContainerProps {
  children: React.ReactNode
  size?: "sm" | "md" | "lg" | "xl" | "full"
  className?: string
}

export function Container({
  children,
  size = "lg",
  className,
}: ContainerProps) {
  const sizeClasses = {
    sm: "max-w-3xl", // 768px
    md: "max-w-4xl", // 896px
    lg: "max-w-6xl", // 1152px (ClickUp default)
    xl: "max-w-7xl", // 1280px
    full: "max-w-full",
  }

  return (
    <div
      className={cn(
        "mx-auto px-4 sm:px-6 lg:px-8",
        sizeClasses[size],
        className
      )}
    >
      {children}
    </div>
  )
}
```

### Step 6: Board Layout (1.5 hours)

**6.1 Create Board View Layout**

```typescript
// apps/frontend/src/components/layout/board-layout.tsx
"use client"

import * as React from "react"
import { cn } from "@/lib/utils"

interface BoardLayoutProps {
  children: React.ReactNode
  className?: string
}

export function BoardLayout({ children, className }: BoardLayoutProps) {
  return (
    <div
      className={cn(
        "flex gap-6 overflow-x-auto pb-4",
        // Horizontal scroll
        "snap-x snap-mandatory",
        // Prevent column shrink
        "[&>*]:flex-shrink-0 [&>*]:snap-start",
        className
      )}
    >
      {children}
    </div>
  )
}

interface BoardColumnProps {
  title: string
  count?: number
  children: React.ReactNode
  className?: string
}

export function BoardColumn({
  title,
  count,
  children,
  className,
}: BoardColumnProps) {
  return (
    <div
      className={cn(
        "w-[280px] flex-shrink-0 snap-start",
        className
      )}
    >
      {/* Column Header */}
      <div className="mb-3 flex items-center justify-between">
        <h3 className="text-sm font-semibold text-gray-900 dark:text-white">
          {title}
        </h3>
        {count !== undefined && (
          <span className="text-xs text-gray-500 dark:text-gray-400">
            {count}
          </span>
        )}
      </div>

      {/* Column Content */}
      <div className="space-y-2">
        {children}
      </div>
    </div>
  )
}
```

**6.2 Usage Example**

```tsx
<BoardLayout>
  <BoardColumn title="To Do" count={5}>
    {/* Task cards */}
  </BoardColumn>
  <BoardColumn title="In Progress" count={3}>
    {/* Task cards */}
  </BoardColumn>
  <BoardColumn title="Done" count={8}>
    {/* Task cards */}
  </BoardColumn>
</BoardLayout>
```

## Todo List

### App Layout
- [ ] Create AppLayout wrapper component
- [ ] Create AppHeader component
- [ ] Add logo, search, notifications
- [ ] Test header responsive behavior

### Sidebar
- [ ] Create AppSidebar component
- [ ] Implement collapse/expand animation
- [ ] Create SidebarNav with nav items
- [ ] Add active state highlighting
- [ ] Test collapsed state
- [ ] Persist sidebar state to localStorage

### Breadcrumb
- [ ] Create Breadcrumb component
- [ ] Add chevron separators
- [ ] Support links and plain text
- [ ] Test with various depths

### Container
- [ ] Create Container component
- [ ] Add size variants (sm, md, lg, xl, full)
- [ ] Test responsive padding

### Board Layout
- [ ] Create BoardLayout component
- [ ] Implement horizontal scroll
- [ ] Create BoardColumn component
- [ ] Add column header with count
- [ ] Test scroll snap behavior

### Testing
- [ ] Test layouts on mobile (375px)
- [ ] Test layouts on tablet (768px)
- [ ] Test layouts on desktop (1280px+)
- [ ] Test dark mode on all layouts
- [ ] Test sidebar collapse animation
- [ ] Verify semantic HTML (landmarks)
- [ ] Test keyboard navigation

## Success Criteria

- [ ] App layout renders with header, sidebar, content
- [ ] Header is 56px tall with proper spacing
- [ ] Sidebar expands to 240px, collapses to 64px
- [ ] Sidebar state persists across page navigation
- [ ] Breadcrumb shows navigation path correctly
- [ ] Container centers content with proper max-width
- [ ] Board layout scrolls horizontally
- [ ] Board columns maintain 280px minimum width
- [ ] All layouts are responsive (mobile, tablet, desktop)
- [ ] All layouts work in dark mode
- [ ] Semantic HTML used (header, nav, main, aside)
- [ ] Keyboard navigation works (Tab, Enter, Escape)

## Risk Assessment

**Risk:** Sidebar collapse animation may cause layout shift
- **Mitigation:** Use CSS transitions, reserve space for collapsed state

**Risk:** Board horizontal scroll may not work on all devices
- **Mitigation:** Use standard CSS overflow, test on mobile touch devices

**Risk:** Container max-width may be too narrow for some content
- **Mitigation:** Provide multiple size options, allow override via className

**Risk:** Breadcrumb may get too long on mobile
- **Mitigation:** Truncate with ellipsis, show only last 2 items on small screens

## Next Steps

After completing this phase:
1. Proceed to **Phase 04: Views** to build complete task views
2. Use layouts to structure task list, board, and detail views
3. Test layouts with real content and data

---

**Phase Status:** Ready to start (after Phase 02)
**Dependencies:** Phase 01 and 02 complete
**Blocked By:** None
