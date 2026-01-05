# Component Usage Guide

**Last Updated:** 2026-01-05
**Version:** Phase 05B Complete

This guide provides usage examples and best practices for Nexora Management's ClickUp Design System components.

## Table of Contents

1. [UI Primitives](#ui-primitives)
2. [Task Components](#task-components)
3. [Layout Components](#layout-components)
4. [Best Practices](#best-practices)
5. [Accessibility Guidelines](#accessibility-guidelines)

---

## UI Primitives

### Button

**Location:** `src/components/ui/button.tsx`

A versatile button component with 6 variants and 4 sizes, built with ClickUp's visual language.

#### Variants

- `primary` - Purple gradient with shadow (default)
- `secondary` - White with 2px border
- `ghost` - Transparent with gray hover
- `destructive` - Red for dangerous actions
- `outline` - Border only with hover fill
- `link` - Text-only with underline

#### Sizes

- `sm` - 36px height
- `md` - 40px height (default)
- `lg` - 44px height
- `icon` - 40px × 40px square

#### Usage Examples

```tsx
import { Button } from "@/components/ui/button"

// Primary action
<Button variant="primary" onClick={handleSave}>
  Save Task
</Button>

// With icon
<Button variant="primary" className="gap-2">
  <CheckCircle2 className="h-4 w-4" />
  Complete
</Button>

// Secondary action
<Button variant="secondary" onClick={handleCancel}>
  Cancel
</Button>

// Destructive action
<Button variant="destructive" onClick={handleDelete}>
  Delete Task
</Button>

// Icon button
<Button variant="ghost" size="icon">
  <MoreVertical className="h-4 w-4" />
</Button>

// Link button
<Button variant="link" onClick={handleNavigate}>
  Learn More
</Button>
```

#### Best Practices

- Use `primary` for main actions (Create, Save, Submit)
- Use `secondary` for cancel or alternative actions
- Use `destructive` for irreversible actions (Delete, Remove)
- Use `ghost` for icon-only buttons in toolbars
- Use `link` for inline navigation or tertiary actions

---

### Input

**Location:** `src/components/ui/input.tsx`

A styled input component with error state support and ClickUp's focus ring.

#### Props

```typescript
interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  error?: boolean  // Adds red border and focus ring
}
```

#### Usage Examples

```tsx
import { Input } from "@/components/ui/input"

// Standard input
<Input
  type="text"
  placeholder="Enter task name..."
  value={title}
  onChange={(e) => setTitle(e.target.value)}
/>

// With error state
<Input
  type="text"
  error={hasError}
  placeholder="This field has an error"
  className="border-red-500"
/>

// Date input
<Input
  type="date"
  value={dueDate}
  onChange={(e) => setDueDate(e.target.value)}
/>

// Disabled input
<Input
  type="text"
  disabled
  value={readonlyValue}
  placeholder="Read-only field"
/>
```

#### Best Practices

- Always provide a `placeholder` for context
- Use `error` prop when validation fails
- Pair with `<label>` for accessibility
- Use appropriate `type` (text, email, date, number)
- Add `aria-describedby` for error messages

---

### Card

**Location:** `src/components/ui/card.tsx`

A flexible container component with header, content, and footer sections.

#### Components

- `Card` - Root container
- `CardHeader` - Header section with title and description
- `CardTitle` - Title text
- `CardDescription` - Subtitle/description text
- `CardContent` - Main content area
- `CardFooter` - Footer with actions

#### Usage Examples

```tsx
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "@/components/ui/card"

// Task card
<Card>
  <CardHeader>
    <CardTitle>Task Title</CardTitle>
    <CardDescription>Due: {task.dueDate}</CardDescription>
  </CardHeader>
  <CardContent>
    <p>{task.description}</p>
  </CardContent>
  <CardFooter className="gap-2">
    <Button variant="primary" size="sm">Edit</Button>
    <Button variant="ghost" size="sm">Delete</Button>
  </CardFooter>
</Card>

// Simple card
<Card className="p-6">
  <h3>Statistics</h3>
  <p>Total tasks: {count}</p>
</Card>

// Hoverable card
<Card className="hover:shadow-md transition-shadow cursor-pointer">
  <CardHeader>
    <CardTitle>Project Name</CardTitle>
  </CardHeader>
  <CardContent>
    <p>Click to view details</p>
  </CardContent>
</Card>
```

#### Best Practices

- Use `CardHeader` for titles and descriptions
- Use `CardContent` for main information
- Use `CardFooter` for action buttons
- Add hover effects for interactive cards
- Maintain consistent padding across cards

---

## Task Components

### TaskCard

**Location:** `src/components/tasks/task-card.tsx`

A board view task card with drag handle, priority dot, status badge, and assignee avatar.

#### Props

```typescript
interface TaskCardProps {
  task: Task          // Task object with all properties
  onClick?: () => void  // Click handler
  className?: string  // Additional CSS classes
}
```

#### Usage Examples

```tsx
import { TaskCard } from "@/components/tasks"

// Basic task card
<TaskCard
  task={{
    id: "1",
    title: "Design new landing page",
    status: "inProgress",
    priority: "high",
    assignee: { name: "John Doe", avatar: "/avatar.jpg" },
    dueDate: "2026-01-10",
    commentCount: 5,
    attachmentCount: 2,
    projectId: "proj-1",
    createdAt: "2026-01-01",
    updatedAt: "2026-01-05"
  }}
  onClick={() => router.push(`/tasks/${task.id}`)}
/>

// With custom styling
<TaskCard
  task={task}
  onClick={handleTaskClick}
  className="border-l-4 border-l-purple-500"
/>

// Disabled state
<TaskCard
  task={task}
  onClick={undefined}  // No click handler
  className="opacity-60 cursor-not-allowed"
/>
```

#### Best Practices

- Always provide an `onClick` handler for interaction
- Use `className` for custom styling (borders, backgrounds)
- Ensure task object has all required properties
- Display in board columns with proper spacing
- Add hover effects for visual feedback

#### Accessibility

- Keyboard accessible (Tab, Enter, Space)
- ARIA label on drag handle ("Drag to reorder task")
- Focus visible ring for keyboard navigation
- Role="button" with tabIndex={0}

---

### TaskModal

**Location:** `src/components/tasks/task-modal.tsx`

A dialog component for creating and editing tasks with form validation.

#### Props

```typescript
interface TaskModalProps {
  open?: boolean                        // Dialog open state
  onOpenChange?: (open: boolean) => void  // Open state handler
  task?: Task                          // Task for edit mode
  onSubmit?: (task: Partial<Task>) => void  // Form submit handler
  mode?: "create" | "edit"             // Modal mode
  isLoading?: boolean                   // Loading state for submit
  className?: string                   // Additional CSS classes
}
```

#### Usage Examples

```tsx
import { TaskModal } from "@/components/tasks"

// Create mode
<TaskModal
  open={isCreateModalOpen}
  onOpenChange={setIsCreateModalOpen}
  mode="create"
  onSubmit={(taskData) => {
    createTask(taskData)
    setIsCreateModalOpen(false)
  }}
/>

// Edit mode
<TaskModal
  open={isEditModalOpen}
  onOpenChange={setIsEditModalOpen}
  task={selectedTask}
  mode="edit"
  onSubmit={(taskData) => {
    updateTask(selectedTask.id, taskData)
    setIsEditModalOpen(false)
  }}
  isLoading={isUpdating}
/>

// With custom styling
<TaskModal
  open={open}
  onOpenChange={setOpen}
  task={task}
  mode="edit"
  onSubmit={handleSubmit}
  className="max-w-2xl"
/>
```

#### Form Validation

- Title: Required, max 200 characters
- Description: Optional, max 1000 characters
- Status: Required (todo, inProgress, complete, overdue)
- Priority: Required (urgent, high, medium, low)
- Due Date: Optional, ISO 8601 format

#### Best Practices

- Use `mode="create"` for new tasks
- Use `mode="edit"` with `task` prop for editing
- Provide `onSubmit` handler to save changes
- Show loading state with `isLoading` prop
- Close modal after successful submit
- Validate form before calling `onSubmit`

#### Accessibility

- Dialog trap focus (Radix UI Dialog)
- aria-live announcements for open/close
- Escape key to close
- Click outside to close
- ARIA labels for all form fields

---

## Layout Components

### AppLayout

**Location:** `src/components/layout/app-layout.tsx`

Main application wrapper providing consistent layout structure with header, sidebar, and content area.

#### Usage

```tsx
import { AppLayout } from "@/components/layout/app-layout"

export default function Layout({ children }: { children: React.ReactNode }) {
  return <AppLayout>{children}</AppLayout>
}
```

#### Features

- Fixed header (56px) with logo, search, notifications
- Collapsible sidebar (240px → 64px)
- Scrollable main content area
- Responsive design
- Dark mode support

---

### AppHeader

**Location:** `src/components/layout/app-header.tsx`

Top navigation bar with search, notifications, and profile menu.

#### Props

```typescript
interface AppHeaderProps {
  sidebarCollapsed: boolean
  onToggleSidebar: () => void
}
```

#### Usage

```tsx
<AppHeader
  sidebarCollapsed={collapsed}
  onToggleSidebar={() => setCollapsed(!collapsed)}
/>
```

---

### AppSidebar

**Location:** `src/components/layout/app-sidebar.tsx`

Collapsible navigation sidebar with menu items.

#### Props

```typescript
interface AppSidebarProps {
  collapsed?: boolean
}
```

#### Usage

```tsx
<AppSidebar collapsed={collapsed} />
```

---

## Best Practices

### Component Composition

**DO:**
```tsx
// Compose components for flexibility
<Card>
  <CardHeader>
    <CardTitle>{task.title}</CardTitle>
    <CardDescription>{task.dueDate}</CardDescription>
  </CardHeader>
  <CardContent>
    <TaskCard task={task} />
  </CardContent>
</Card>
```

**DON'T:**
```tsx
// Avoid monolithic components
<ComplexTaskCardWithEverythingInside />
```

### State Management

**DO:**
```tsx
// Lift state to parent
const [isOpen, setIsOpen] = useState(false)
<TaskModal open={isOpen} onOpenChange={setIsOpen} />
```

**DON'T:**
```tsx
// Avoid managing state in multiple places
// Keep single source of truth
```

### TypeScript

**DO:**
```tsx
// Use proper types from component exports
import type { TaskCardProps, TaskModalProps } from "@/components/tasks"
```

**DON'T:**
```tsx
// Avoid 'any' types
const onClick: any = () => {}
```

### Styling

**DO:**
```tsx
// Use Tailwind utility classes
<TaskCard className="hover:shadow-md transition-shadow" />
```

**DON'T:**
```tsx
// Avoid inline styles
<TaskCard style={{ boxShadow: '0 4px 6px rgba(0,0,0,0.1)' }} />
```

---

## Accessibility Guidelines

### WCAG 2.1 AA Compliance

All components meet WCAG 2.1 Level AA requirements:

1. **Color Contrast**
   - Text: 4.5:1 minimum (normal), 3:1 (large)
   - UI Components: 3:1 minimum
   - Focus indicators: Visible with 2px outline

2. **Keyboard Navigation**
   - Tab: Navigate through interactive elements
   - Enter/Space: Activate buttons and links
   - Escape: Close modals and dropdowns
   - Arrow keys: Navigate options in selects

3. **Screen Reader Support**
   - ARIA labels on icon-only buttons
   - aria-live regions for dynamic content
   - Semantic HTML (header, nav, main, aside)
   - Role attributes where needed

4. **Focus Management**
   - Focus visible rings (2px purple outline)
   - Focus trap in modals
   - Logical tab order
   - Skip links (future enhancement)

### Testing Checklist

- [ ] All functions available via keyboard
- [ ] Focus visible on all interactive elements
- [ ] Screen reader announces changes
- [ ] Color contrast meets AA standards
- [ ] Forms have proper labels and error messages
- [ ] Modals trap focus and announce state
- [ ] Buttons have accessible names
- [ ] Links describe their destination

---

## Resources

- **Design Guidelines:** `docs/design-guidelines.md`
- **Codebase Summary:** `docs/codebase-summary.md`
- **Component Showcase:** http://localhost:3000/components/showcase
- **Tailwind Config:** `apps/frontend/tailwind.config.ts`
- **Global Styles:** `apps/frontend/src/app/globals.css`

---

**Document Version:** 1.0
**Last Updated:** 2026-01-05
**Maintained By:** Development Team
