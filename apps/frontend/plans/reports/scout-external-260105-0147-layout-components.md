# Layout Components Documentation Report

**Date:** 2026-01-05  
**Scout:** scout-external  
**Target:** `src/components/layout/`  
**Purpose:** Documentation update for layout components

---

## Executive Summary

**Component Count:** 10 files (2 duplicates identified)  
**Active Components:** 7 unique  
**Pattern:** Client-side layout components with ClickUp design system integration  
**Status:** ✅ Well-structured, minor cleanup needed

---

## Component Inventory

### 1. AppLayout (app-layout.tsx) ✅ PRIMARY
**Purpose:** Main application layout wrapper  
**File:** `/apps/frontend/src/components/layout/app-layout.tsx`

**Features:**
- Full-screen flex layout (h-screen)
- Collapsible sidebar state management
- Header + Sidebar + Content structure
- Dark mode support (gray-50/gray-900 backgrounds)

**Props:**
```tsx
interface AppLayoutProps {
  children: React.ReactNode
}
```

**Dimensions:**
- Header: 56px fixed
- Sidebar: 240px → 64px (collapsed)
- Content: flex-1 with overflow-auto

**Design System Integration:**
- Uses `bg-gray-50 dark:bg-gray-900`
- Border colors: `border-gray-200 dark:border-gray-700`
- Transition: `transition-all duration-200`

---

### 2. AppHeader (app-header.tsx) ✅ PRIMARY
**Purpose:** Application header with navigation and user controls  
**File:** `/apps/frontend/src/components/layout/app-header.tsx`

**Features:**
- Sidebar toggle button
- Logo with gradient branding
- Global search input (md+ breakpoint)
- Notification bell
- Settings button
- User avatar

**Props:**
```tsx
interface AppHeaderProps {
  sidebarCollapsed: boolean
  onToggleSidebar: () => void
}
```

**Design System Integration:**
- Button: `ghost` variant, `h-9 w-9` size
- Input: `h-9 w-64` with search icon
- Avatar: `h-9 w-9` with fallback
- Icons: Lucide React (Menu, Search, Bell, Settings)

**Responsive:**
- Search: Hidden on mobile, visible on md+
- Header height: 56px (h-14)

---

### 3. AppSidebar (app-sidebar.tsx) ✅ PRIMARY
**Purpose:** Collapsible sidebar navigation container  
**File:** `/apps/frontend/src/components/layout/app-sidebar.tsx`

**Features:**
- Collapsible width (240px → 64px)
- Border separator
- Overflow-y scrolling
- Smooth transitions

**Props:**
```tsx
interface AppSidebarProps {
  collapsed?: boolean
}
```

**Design System Integration:**
- Background: `bg-white dark:bg-gray-800`
- Border: `border-gray-200 dark:border-gray-700`
- Transition: `transition-all duration-200`

---

### 4. SidebarNav (sidebar-nav.tsx) ✅ PRIMARY
**Purpose:** Navigation links with active state highlighting  
**File:** `/apps/frontend/src/components/layout/sidebar-nav.tsx`

**Features:**
- 6 navigation items (Home, Tasks, Projects, Team, Calendar, Settings)
- Active state detection via usePathname
- Icon-only mode when collapsed
- Chevron indicator for active item

**Props:**
```tsx
interface SidebarNavProps {
  collapsed?: boolean
}
```

**Navigation Items:**
```tsx
const navItems = [
  { title: "Home", href: "/", icon: Home },
  { title: "Tasks", href: "/tasks", icon: CheckSquare },
  { title: "Projects", href: "/projects", icon: Folder },
  { title: "Team", href: "/team", icon: Users },
  { title: "Calendar", href: "/calendar", icon: Calendar },
  { title: "Settings", href: "/settings", icon: Settings },
]
```

**Design System Integration:**
- Active: `bg-primary/10 text-primary`
- Inactive: `text-gray-600 dark:text-gray-400`
- Hover: `hover:bg-gray-100 dark:hover:bg-gray-700`
- Icons: Lucide React

---

### 5. Breadcrumb (breadcrumb.tsx) ✅ PRIMARY
**Purpose:** Hierarchical navigation breadcrumb  
**File:** `/apps/frontend/src/components/layout/breadcrumb.tsx`

**Features:**
- Dynamic item rendering
- Optional href for navigation
- Last item as non-link (current page)
- Chevron separators

**Props:**
```tsx
interface BreadcrumbItem {
  label: string
  href?: string
}

interface BreadcrumbProps {
  items: BreadcrumbItem[]
  className?: string
}
```

**Design System Integration:**
- Text: `text-gray-500 dark:text-gray-400`
- Hover: `hover:text-gray-900 dark:hover:text-gray-200`
- Current: `text-gray-900 dark:text-gray-200`
- Separator: ChevronRight icon

---

### 6. Container (container.tsx) ✅ PRIMARY
**Purpose:** Responsive content container with max-width  
**File:** `/apps/frontend/src/components/layout/container.tsx`

**Features:**
- 5 size variants
- Responsive horizontal padding
- Auto-centering (mx-auto)

**Props:**
```tsx
interface ContainerProps {
  children: React.ReactNode
  size?: "sm" | "md" | "lg" | "xl" | "full"
  className?: string
}
```

**Size Classes:**
```tsx
sm: "max-w-3xl",    // 768px
md: "max-w-4xl",    // 896px
lg: "max-w-6xl",    // 1152px (ClickUp default)
xl: "max-w-7xl",    // 1280px
full: "max-w-full"
```

**Design System Integration:**
- Padding: `px-4 sm:px-6 lg:px-8`
- Centering: `mx-auto`
- Default size: `lg` (ClickUp standard)

---

### 7. BoardLayout + BoardColumn (board-layout.tsx) ✅ PRIMARY
**Purpose:** Kanban-style board layout for tasks  
**File:** `/apps/frontend/src/components/layout/board-layout.tsx`

**Features:**
- Horizontal scrolling with snap points
- Fixed-width columns (280px)
- Prevent column shrinkage
- Optional count badges

**Props:**
```tsx
interface BoardLayoutProps {
  children: React.ReactNode
  className?: string
}

interface BoardColumnProps {
  title: string
  count?: number
  children: React.ReactNode
  className?: string
}
```

**Design System Integration:**
- Layout: `flex gap-6 overflow-x-auto pb-4`
- Snap: `snap-x snap-mandatory`
- Column: `w-[280px] flex-shrink-0 snap-start`
- Text: `text-sm font-semibold text-gray-900 dark:text-white`
- Count: `text-xs text-gray-500 dark:text-gray-400`

**Usage Example:**
```tsx
<BoardLayout>
  <BoardColumn title="To Do" count={5}>
    {taskCards}
  </BoardColumn>
</BoardLayout>
```

---

## Duplicate Files ⚠️

### 1. AppLayout.tsx (PascalCase)
**File:** `/apps/frontend/src/components/layout/AppLayout.tsx`

**Status:** OUTDATED - Legacy version  
**Differences:**
- No sidebar functionality
- No dark mode support
- Simpler structure (header + main only)
- Uses `bg-background` (old design token)

**Recommendation:** DELETE - Superseded by app-layout.tsx

---

### 2. AppHeader.tsx (PascalCase)
**File:** `/apps/frontend/src/components/layout/AppHeader.tsx`

**Status:** OUTDATED - Legacy version with SignalR  
**Differences:**
- SignalR integration for real-time notifications
- No sidebar collapse button
- Different layout (nav links instead of sidebar)
- Connection status indicator
- NotificationCenter integration

**Recommendation:** REVIEW - May have valuable notification logic to migrate

**Unique Features to Preserve:**
- SignalR notification hub integration
- NotificationCenter component usage
- Notification state management (mark as read, delete)
- Real-time connection status indicator

---

## Design System Integration

### Color Tokens Used
```tsx
// Backgrounds
bg-gray-50, dark:bg-gray-900      // AppLayout
bg-white, dark:bg-gray-800        // AppSidebar, Header

// Borders
border-gray-200, dark:border-gray-700

// Primary (active states)
bg-primary/10, text-primary

// Text colors
text-gray-900, dark:text-white    // Headers
text-gray-600, dark:text-gray-400 // Inactive nav
text-gray-500, dark:text-gray-400 // Secondary text
```

### Typography
```tsx
// Font sizes
text-lg    // Logo
text-sm    // Nav items, labels
text-xs    // Counts, timestamps

// Font weights
font-semibold  // Headers, active nav
font-medium    // Nav items
```

### Spacing
```tsx
gap-2   // Icon spacing
gap-3   // Nav items
gap-4   // Header sections
gap-6   // Board columns
py-2    // Nav padding
py-4    // Sidebar nav
py-6    // Main container
px-3    // Nav padding
px-4    // Header padding
```

### Border Radius
```tsx
rounded-lg  // Nav items, buttons
rounded-full  // Avatar, connection status
```

### Transitions
```tsx
transition-all duration-200  // Sidebar collapse
transition-all  // Nav hover states
```

---

## Responsive Patterns

### Breakpoint Usage
```tsx
// Mobile-first approach
md:flex    // Header nav (hidden on mobile)
md:block   // Search input
hidden sm:flex  // Connection status
```

### Responsive Padding (Container)
```tsx
px-4 sm:px-6 lg:px-8
```

### Responsive Dimensions
- Sidebar: Mobile collapses to icon-only (64px)
- Search: Hidden on mobile, 264px on md+
- Header: Fixed 56px on all screens

---

## Usage Examples

### 1. AppLayout + AppHeader + AppSidebar
```tsx
import { AppLayout } from "@/components/layout"

export default function DashboardPage() {
  return (
    <AppLayout>
      {/* Page content */}
    </AppLayout>
  )
}
```

### 2. Breadcrumb
```tsx
import { Breadcrumb } from "@/components/layout"

<Breadcrumb
  items={[
    { label: "Projects", href: "/projects" },
    { label: "Nexora", href: "/projects/nexora" },
    { label: "Settings" }  // Current page (no href)
  ]}
/>
```

### 3. Container
```tsx
import { Container } from "@/components/layout"

<Container size="lg">
  <h1>Page Title</h1>
  {/* Content */}
</Container>
```

### 4. BoardLayout
```tsx
import { BoardLayout, BoardColumn } from "@/components/layout"

<BoardLayout>
  <BoardColumn title="To Do" count={tasks.todo.length}>
    {tasks.todo.map(task => <TaskCard key={task.id} task={task} />)}
  </BoardColumn>
  <BoardColumn title="In Progress" count={tasks.inProgress.length}>
    {tasks.inProgress.map(task => <TaskCard key={task.id} task={task} />)}
  </BoardColumn>
</BoardLayout>
```

---

## Dependencies

### External Libraries
- **Lucide React:** Icons (Menu, Search, Bell, Settings, ChevronRight, Home, CheckSquare, Folder, Users, Calendar, Settings)
- **Next.js:** Link, usePathname, Route type

### Internal Components
- **UI Components:** Button, Avatar, AvatarFallback, Input, Badge, ScrollArea, DropdownMenu
- **Features:** NotificationCenter, useNotificationHub
- **Utils:** cn (classnames utility)

---

## Accessibility Features

### Keyboard Navigation
- All interactive elements focusable
- Sidebar toggle via keyboard
- Navigation links via Tab

### ARIA Support
- Breadcrumb: `aria-label="Breadcrumb"`
- Board announcements: `aria-live="polite" aria-atomic="true"`
- Screen reader-only content: `sr-only` class

### Focus States
- Inherits from globals.css focus-visible styles
- 2px outline with primary color
- Proper offset for visibility

---

## Performance Considerations

### Client-Side Rendering
All layout components use `"use client"` directive for:
- State management (sidebar collapse)
- Navigation hooks (usePathname)
- Interactive features (collapsible sidebar)

### Memoization
BoardLayout usage example shows React.memo pattern for optimization:
```tsx
export const TaskBoard = memo(function TaskBoard({ ... }) {
  // Component logic
}, (prevProps, nextProps) => {
  // Custom comparison
})
```

### Transitions
- Smooth sidebar collapse (200ms duration)
- Snap scrolling for boards (hardware accelerated)

---

## Integration Points

### SignalR Integration (AppHeader.tsx - Legacy)
The legacy AppHeader includes real-time notification features:
- `useNotificationHub` hook for WebSocket connection
- Notification state management
- Connection status indicator
- NotificationCenter component integration

**Migration Path:**
Extract notification logic from AppHeader.tsx and integrate into app-header.tsx before deletion.

### Document Management
Layout components support document/collaboration features:
- BoardLayout for document organization
- Breadcrumb for document hierarchy
- Container for document content

---

## Unresolved Questions

1. **SignalR Migration Strategy:** Should notification features from legacy AppHeader.tsx be integrated into app-header.tsx, or moved to a separate notification provider?

2. **Legacy File Cleanup:** AppLayout.tsx is clearly outdated. Can it be safely deleted, or is it referenced elsewhere in the codebase?

3. **Mobile Sidebar Behavior:** Should sidebar be overlay (modal) on mobile instead of inline collapse?

4. **Container Size Default:** Is `lg` (1152px) the right default for all use cases, or should different pages have different defaults?

5. **Breadcrumb Auto-Generation:** Should breadcrumb be auto-generated from route structure, or always manual?

6. **Board Column Width:** 280px is hardcoded. Should this be configurable for different use cases?

---

## Recommendations

### Immediate Actions
1. **Delete AppLayout.tsx** - Outdated, superseded by app-layout.tsx
2. **Migrate SignalR logic** from AppHeader.tsx to app-header.tsx or separate provider
3. **Add Container size prop** to page layouts where appropriate
4. **Document mobile behavior** for sidebar (current: icon-only mode)

### Future Enhancements
1. **Auto-generated breadcrumbs** from Next.js route structure
2. **Sidebar overlay mode** for mobile screens
3. **Configurable board column widths**
4. **Sticky header option** for long-scrolling pages
5. **Collapsible sections** in sidebar navigation

---

## Metrics

| Metric | Value | Status |
|--------|-------|--------|
| Total Files | 10 | ⚠️ 2 duplicates |
| Active Components | 7 | ✅ |
| Client Components | 7/7 | ✅ |
| TypeScript Coverage | 100% | ✅ |
| Design System Compliance | High | ✅ |
| Accessibility Support | Good | ✅ |
| Responsive Support | Good | ✅ |
| Dark Mode Support | Complete | ✅ |

---

## Conclusion

The layout components are well-structured with strong ClickUp design system integration. The primary issues are:
1. Duplicate files that need cleanup
2. SignalR notification features in legacy file that need migration
3. Minor enhancements for mobile experience

**Status:** ✅ Ready for documentation update after cleanup

**Next Steps:**
1. Delete AppLayout.tsx
2. Migrate SignalR features from AppHeader.tsx
3. Update component documentation
4. Consider mobile sidebar improvements
