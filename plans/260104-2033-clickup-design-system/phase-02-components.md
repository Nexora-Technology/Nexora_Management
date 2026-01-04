# Phase 02: Components - Core UI Components

**Date:** 2026-01-04
**Priority:** High
**Status:** Done
**Estimated Time:** 8 hours
**Completed:** 2026-01-04 22:10

## Context

**Overview:** [plan.md](./plan.md)
**Prerequisite:** [phase-01-foundation.md](./phase-01-foundation.md) must be complete

Builds the core component library that powers the entire interface. All components use ClickUp's visual language established in Phase 01. Components are built on top of shadcn/ui primitives with ClickUp-specific styling.

**Why It Matters:**
- Reusable components ensure consistency
- Faster feature development with pre-built components
- Type-safe components with TypeScript
- Accessible by default (ARIA, keyboard nav, focus states)

## Key Insights

- **shadcn/ui provides base components** - extend rather than replace
- **ClickUp buttons use gradients** - primary button has purple gradient
- **Status badges are color-coded** - green (complete), yellow (in progress), red (overdue)
- **Forms have distinct focus states** - purple ring on focus
- **Avatars support initials fallback** - hash-based color generation
- **Tooltips follow cursor** - smart positioning

## Requirements

### Functional Requirements
- **FR-01:** Button component with 4 variants (primary, secondary, ghost, icon)
- **FR-02:** Status badge component (complete, in progress, overdue)
- **FR-03:** Form elements (input, textarea, select, checkbox, radio)
- **FR-04:** Avatar component with 5 sizes and initials fallback
- **FR-05:** Tooltip component with smart positioning
- **FR-06:** All components support dark mode
- **FR-07:** All components meet WCAG 2.1 AA

### Non-Functional Requirements
- **NFR-01:** Components must be fully typed with TypeScript
- **NFR-02:** Components must accept className prop for customization
- **NFR-03:** Components must forward refs
- **NFR-04:** Components must have proper ARIA attributes
- **NFR-05:** Component bundle size increase < 50KB total

## Architecture

### Component Structure

```
apps/frontend/src/components/ui/
├── button.tsx          # Primary, secondary, ghost, icon variants
├── badge.tsx           # Status badges with color coding
├── input.tsx           # Text input with floating label option
├── textarea.tsx        # Multi-line input
├── select.tsx          # Dropdown select
├── checkbox.tsx        # Checkbox with label
├── radio-group.tsx     # Radio button group
├── avatar.tsx          # User avatar with initials
└── tooltip.tsx         # Tooltip with positioning
```

### Component Variants System

Use `class-variance-authority` (cva) for variant management:

```typescript
const buttonVariants = cva(
  "inline-flex items-center justify-center gap-2 font-medium transition-all",
  {
    variants: {
      variant: {
        primary: "bg-gradient...",
        secondary: "bg-white border...",
        ghost: "bg-transparent...",
        icon: "w-10 h-10...",
      },
      size: {
        sm: "h-9 px-3 text-sm",
        md: "h-10 px-4",
        lg: "h-11 px-6",
      },
    },
  }
)
```

## Related Code Files

### Files to Modify
- `/apps/frontend/src/components/ui/button.tsx` - Extend with ClickUp variants
- `/apps/frontend/src/components/ui/badge.tsx` - Add status variants
- `/apps/frontend/src/components/ui/input.tsx` - Add ClickUp focus styles
- `/apps/frontend/src/components/ui/avatar.tsx` - Add initials fallback

### Files to Create
- `/apps/frontend/src/components/ui/textarea.tsx`
- `/apps/frontend/src/components/ui/select.tsx`
- `/apps/frontend/src/components/ui/checkbox.tsx`
- `/apps/frontend/src/components/ui/radio-group.tsx`
- `/apps/frontend/src/components/ui/tooltip.tsx`

## Implementation Steps

### Step 1: Button Component (1.5 hours)

**1.1 Install Dependencies**

```bash
npm install class-variance-authority
npm install -D @types/node
```

**1.2 Extend shadcn/ui Button**

```typescript
// apps/frontend/src/components/ui/button.tsx
import * as React from "react"
import { Slot } from "@radix-ui/react-slot"
import { cva, type VariantProps } from "class-variance-authority"

const buttonVariants = cva(
  "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/50 disabled:pointer-events-none disabled:opacity-50",
  {
    variants: {
      variant: {
        // ClickUp primary - purple gradient
        primary: "bg-gradient-to-r from-primary to-primary-hover text-white shadow-sm hover:shadow-md hover:scale-[1.02] active:scale-[0.98]",

        // ClickUp secondary - white with border
        secondary: "bg-white border-2 border-gray-200 text-gray-900 hover:bg-gray-50 hover:border-gray-300",

        // Ghost - transparent background
        ghost: "bg-transparent text-gray-600 hover:bg-gray-100 hover:text-gray-900",

        // Icon only
        icon: "h-10 w-10 p-0 hover:bg-gray-100",
      },
      size: {
        sm: "h-9 px-3 text-xs",
        md: "h-10 px-4",
        lg: "h-11 px-6 text-base",
        icon: "h-10 w-10",
      },
    },
    defaultVariants: {
      variant: "primary",
      size: "md",
    },
  }
)

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement>,
    VariantProps<typeof buttonVariants> {
  asChild?: boolean
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant, size, asChild = false, ...props }, ref) => {
    const Comp = asChild ? Slot : "button"
    return (
      <Comp
        className={cn(buttonVariants({ variant, size, className }))}
        ref={ref}
        {...props}
      />
    )
  }
)
Button.displayName = "Button"

export { Button, buttonVariants }
```

**1.3 Test Button Variants**

Create test page or Storybook stories to verify all variants work in light/dark modes.

### Step 2: Status Badge Component (1 hour)

**2.1 Create Badge Component**

```typescript
// apps/frontend/src/components/ui/badge.tsx
import * as React from "react"
import { cva, type VariantProps } from "class-variance-authority"

const badgeVariants = cva(
  "inline-flex items-center gap-1.5 rounded px-2 py-0.5 text-xs font-semibold transition-colors",
  {
    variants: {
      status: {
        // Complete - green
        complete: "bg-green-100 text-green-700 border border-green-200 dark:bg-green-900/30 dark:text-green-400 dark:border-green-800",

        // In Progress - yellow
        inProgress: "bg-yellow-100 text-yellow-700 border border-yellow-200 dark:bg-yellow-900/30 dark:text-yellow-400 dark:border-yellow-800",

        // Overdue - red
        overdue: "bg-red-100 text-red-700 border border-red-200 dark:bg-red-900/30 dark:text-red-400 dark:border-red-800",

        // Neutral
        neutral: "bg-gray-100 text-gray-700 border border-gray-200 dark:bg-gray-800 dark:text-gray-400 dark:border-gray-700",
      },
      size: {
        sm: "text-[10px] px-1.5 py-0",
        md: "text-xs px-2 py-0.5",
        lg: "text-sm px-2.5 py-1",
      },
    },
    defaultVariants: {
      status: "neutral",
      size: "md",
    },
  }
)

export interface BadgeProps
  extends React.HTMLAttributes<HTMLDivElement>,
    VariantProps<typeof badgeVariants> {
  icon?: React.ReactNode
}

const Badge = React.forwardRef<HTMLDivElement, BadgeProps>(
  ({ className, status, size, icon, children, ...props }, ref) => {
    return (
      <div ref={ref} className={cn(badgeVariants({ status, size }), className)} {...props}>
        {icon}
        {children}
      </div>
    )
  }
)
Badge.displayName = "Badge"

export { Badge, badgeVariants }
```

**2.2 Usage Example**

```tsx
<Badge status="complete" icon={<Check className="w-3 h-3" />}>
  Complete
</Badge>
<Badge status="inProgress" icon={<Clock className="w-3 h-3" />}>
  In Progress
</Badge>
<Badge status="overdue" icon={<AlertCircle className="w-3 h-3" />}>
  Overdue
</Badge>
```

### Step 3: Form Elements (2.5 hours)

**3.1 Text Input**

```typescript
// apps/frontend/src/components/ui/input.tsx
import * as React from "react"

export interface InputProps
  extends React.InputHTMLAttributes<HTMLInputElement> {
  error?: boolean
}

const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ className, type, error, ...props }, ref) => {
    return (
      <input
        type={type}
        className={cn(
          "flex h-10 w-full rounded-md border-2 bg-white px-3 py-2 text-sm transition-colors",
          "placeholder:text-gray-400",
          "focus:outline-none focus:ring-2 focus:ring-primary/20",
          "disabled:cursor-not-allowed disabled:opacity-50",
          error
            ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
            : "border-gray-200 focus:border-primary",
          className
        )}
        ref={ref}
        {...props}
      />
    )
  }
)
Input.displayName = "Input"

export { Input }
```

**3.2 Textarea**

```typescript
// apps/frontend/src/components/ui/textarea.tsx
import * as React from "react"

export interface TextareaProps
  extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  error?: boolean
}

const Textarea = React.forwardRef<HTMLTextAreaElement, TextareaProps>(
  ({ className, error, ...props }, ref) => {
    return (
      <textarea
        className={cn(
          "flex min-h-[80px] w-full rounded-md border-2 bg-white px-3 py-2 text-sm transition-colors",
          "placeholder:text-gray-400",
          "focus:outline-none focus:ring-2 focus:ring-primary/20",
          "disabled:cursor-not-allowed disabled:opacity-50",
          "resize-vertical",
          error
            ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
            : "border-gray-200 focus:border-primary",
          className
        )}
        ref={ref}
        {...props}
      />
    )
  }
)
Textarea.displayName = "Textarea"

export { Textarea }
```

**3.3 Select (Using shadcn/ui base)**

```bash
npx shadcn@latest add select
```

Extend with ClickUp styles in the generated component.

**3.4 Checkbox**

```typescript
// apps/frontend/src/components/ui/checkbox.tsx
import * as React from "react"
import * as CheckboxPrimitive from "@radix-ui/react-checkbox"
import { Check } from "lucide-react"

const Checkbox = React.forwardRef<
  React.ElementRef<typeof CheckboxPrimitive.Root>,
  React.ComponentPropsWithoutRef<typeof CheckboxPrimitive.Root>
>(({ className, ...props }, ref) => (
  <CheckboxPrimitive.Root
    ref={ref}
    className={cn(
      "peer h-4 w-4 shrink-0 rounded-sm border-2 border-gray-300",
      "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/50",
      "disabled:cursor-not-allowed disabled:opacity-50",
      "data-[state=checked]:bg-primary data-[state=checked]:border-primary",
      "transition-all duration-200",
      className
    )}
    {...props}
  >
    <CheckboxPrimitive.Indicator
      className={cn("flex items-center justify-center text-white")}
    >
      <Check className="h-3.5 w-3.5" strokeWidth={3} />
    </CheckboxPrimitive.Indicator>
  </CheckboxPrimitive.Root>
))
Checkbox.displayName = CheckboxPrimitive.Root.displayName

export { Checkbox }
```

**3.5 Radio Group**

```bash
npx shadcn@latest add radio-group
```

Extend with ClickUp styles.

### Step 4: Avatar Component (1.5 hours)

**4.1 Create Avatar with Initials**

```typescript
// apps/frontend/src/components/ui/avatar.tsx
import * as React from "react"
import * as AvatarPrimitive from "@radix-ui/react-avatar"

const Avatar = React.forwardRef<
  React.ElementRef<typeof AvatarPrimitive.Root>,
  React.ComponentPropsWithoutRef<typeof AvatarPrimitive.Root>
>(({ className, ...props }, ref) => (
  <AvatarPrimitive.Root
    ref={ref}
    className={cn(
      "relative flex shrink-0 overflow-hidden rounded-full",
      className
    )}
    {...props}
  />
))
Avatar.displayName = AvatarPrimitive.Root.displayName

const AvatarImage = React.forwardRef<
  React.ElementRef<typeof AvatarPrimitive.Image>,
  React.ComponentPropsWithoutRef<typeof AvatarPrimitive.Image>
>(({ className, ...props }, ref) => (
  <AvatarPrimitive.Image
    ref={ref}
    className={cn("aspect-square h-full w-full", className)}
    {...props}
  />
))
AvatarImage.displayName = AvatarPrimitive.Image.displayName

const AvatarFallback = React.forwardRef<
  React.ElementRef<typeof AvatarPrimitive.Fallback>,
  React.ComponentPropsWithoutRef<typeof AvatarPrimitive.Fallback> & {
    name?: string
  }
>(({ className, name, ...props }, ref) => {
  // Generate initials from name
  const initials = name
    ? name
        .split(" ")
        .map((n) => n[0])
        .join("")
        .toUpperCase()
        .slice(0, 2)
    : null

  // Generate color from name hash
  const colors = [
    "bg-red-500",
    "bg-orange-500",
    "bg-amber-500",
    "bg-yellow-500",
    "bg-green-500",
    "bg-emerald-500",
    "bg-teal-500",
    "bg-cyan-500",
    "bg-sky-500",
    "bg-blue-500",
    "bg-indigo-500",
    "bg-violet-500",
    "bg-purple-500",
    "bg-fuchsia-500",
    "bg-pink-500",
    "bg-rose-500",
  ]

  const colorIndex = name
    ? name.split("").reduce((acc, char) => acc + char.charCodeAt(0), 0) % colors.length
    : 0

  return (
    <AvatarPrimitive.Fallback
      ref={ref}
      className={cn(
        "flex h-full w-full items-center justify-center rounded-full text-white font-semibold",
        colors[colorIndex],
        className
      )}
      {...props}
    >
      {initials}
    </AvatarPrimitive.Fallback>
  )
})
AvatarFallback.displayName = AvatarPrimitive.Fallback.displayName

export { Avatar, AvatarImage, AvatarFallback }
```

**4.2 Avatar Sizes**

```tsx
// Usage examples
<Avatar className="h-5 w-5">  {/* XS - 20px */}
<Avatar className="h-6 w-6">  {/* SM - 24px */}
<Avatar className="h-8 w-8">  {/* MD - 32px */}
<Avatar className="h-10 w-10"> {/* LG - 40px */}
<Avatar className="h-12 w-12"> {/* XL - 48px */}
```

### Step 5: Tooltip Component (1 hour)

**5.1 Install and Create Tooltip**

```bash
npx shadcn@latest add tooltip
```

**5.2 Extend with ClickUp Styles**

Update the generated component to match ClickUp's dark tooltip style:

```typescript
// apps/frontend/src/components/ui/tooltip.tsx
// Update TooltipContent styling
const TooltipContent = React.forwardRef<
  React.ElementRef<typeof TooltipPrimitive.Content>,
  React.ComponentPropsWithoutRef<typeof TooltipPrimitive.Content>
>(({ className, sideOffset = 4, ...props }, ref) => (
  <TooltipPrimitive.Content
    ref={ref}
    sideOffset={sideOffset}
    className={cn(
      "z-50 overflow-hidden rounded-md bg-gray-900 px-3 py-1.5 text-xs text-white animate-in fade-in-0 zoom-in-95",
      "data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=closed]:zoom-out-95",
      "data-[side=bottom]:slide-in-from-top-2",
      "data-[side=left]:slide-in-from-right-2",
      "data-[side=right]:slide-in-from-left-2",
      "data-[side=top]:slide-in-from-bottom-2",
      className
    )}
    {...props}
  />
))
```

### Step 6: Component Testing & Documentation (1.5 hours)

**6.1 Create Component Showcase**

Create `/apps/frontend/src/app/components/showcase/page.tsx`:

```tsx
// Show all components with all variants
// Test in light and dark modes
// Verify keyboard navigation
// Check ARIA attributes
```

**6.2 Update Design Guidelines**

Document all components in `/docs/design-guidelines.md`:
- Component API
- Props reference
- Usage examples
- Accessibility notes

## Todo List

### Buttons
- [ ] Install class-variance-authority
- [ ] Create button variants (primary, secondary, ghost, icon)
- [ ] Add sizes (sm, md, lg)
- [ ] Test button interactions
- [ ] Verify dark mode

### Badges
- [ ] Create badge component with cva
- [ ] Add status variants (complete, in progress, overdue)
- [ ] Add size variants (sm, md, lg)
- [ ] Add icon support
- [ ] Test color contrast

### Form Elements
- [ ] Extend input component with error state
- [ ] Create textarea component
- [ ] Add select component (shadcn)
- [ ] Create checkbox component
- [ ] Add radio group (shadcn)
- [ ] Test all form interactions
- [ ] Verify focus states

### Avatar
- [ ] Extend shadcn avatar
- [ ] Add initials fallback
- [ ] Add hash-based color generation
- [ ] Test with different names
- [ ] Verify all sizes

### Tooltip
- [ ] Add shadcn tooltip
- [ ] Update styles for ClickUp dark theme
- [ ] Test positioning
- [ ] Verify keyboard accessibility

### Testing
- [ ] Create component showcase page
- [ ] Test all components in light mode
- [ ] Test all components in dark mode
- [ ] Test keyboard navigation
- [ ] Verify ARIA attributes
- [ ] Check color contrast ratios
- [ ] Update design-guidelines.md

## Success Criteria

- [ ] All 5 component types implemented (buttons, badges, forms, avatars, tooltips)
- [ ] Button has 4 variants (primary, secondary, ghost, icon)
- [ ] Status badges show correct colors for all states
- [ ] All form elements have proper focus states
- [ ] Avatar generates initials and colors correctly
- [ ] Tooltip positions correctly on all sides
- [ ] All components work in dark mode
- [ ] All components meet WCAG 2.1 AA
- [ ] TypeScript types are correct (no any types)
- [ ] Components documented in design-guidelines.md

## Risk Assessment

**Risk:** Component variants may not match ClickUp exactly
- **Mitigation:** Use design analysis document as reference, allow small deviations

**Risk:** Form elements may not work well with React Hook Form
- **Mitigation:** Ensure all components forward refs properly, test with RHF

**Risk:** Avatar color generation may produce low-contrast colors
- **Mitigation:** Use pre-defined color palette, ensure white text has sufficient contrast

**Risk:** Tooltip positioning may break on edge cases
- **Mitigation:** Use Radix UI's built-in collision detection, test extensively

## Next Steps

After completing this phase:
1. Proceed to **Phase 03: Layouts** to build app layout and navigation
2. Use components from this phase in layout construction
3. Test component composition (buttons in cards, etc.)

---

**Phase Status:** Ready to start (after Phase 01)
**Dependencies:** Phase 01 complete
**Blocked By:** None
