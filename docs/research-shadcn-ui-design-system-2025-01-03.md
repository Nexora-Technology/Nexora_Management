# shadcn/ui Design System Research Report
**Date:** 2025-01-03
**Purpose:** Create ClickUp-inspired design guidelines using shadcn/ui

---

## 1. DESIGN TOKENS

### Color System (CSS Variables)

**Default Colors (OKLCH Format):**
```css
:root {
  /* Base Colors */
  --background: oklch(1 0 0);          /* Pure white */
  --foreground: oklch(0.147 0.004 49.25);  /* Near black */

  /* Component Colors */
  --card: oklch(1 0 0);
  --card-foreground: oklch(0.147 0.004 49.25);
  --popover: oklch(1 0 0);
  --popover-foreground: oklch(0.147 0.004 49.25);

  /* Primary Actions */
  --primary: oklch(0.216 0.006 56.043);     /* Dark slate */
  --primary-foreground: oklch(0.985 0.001 106.423);  /* White */

  /* Secondary */
  --secondary: oklch(0.97 0.001 106.424);   /* Light gray */
  --secondary-foreground: oklch(0.216 0.006 56.043);

  /* Muted States */
  --muted: oklch(0.97 0.001 106.424);
  --muted-foreground: oklch(0.553 0.013 58.071);

  /* Accent */
  --accent: oklch(0.97 0.001 106.424);
  --accent-foreground: oklch(0.216 0.006 56.043);

  /* Destructive */
  --destructive: oklch(0.577 0.245 27.325);  /* Red */

  /* Borders & Inputs */
  --border: oklch(0.923 0.003 48.717);
  --input: oklch(0.923 0.003 48.717);
  --ring: oklch(0.709 0.01 56.259);

  /* Border Radius */
  --radius: 0.625rem;  /* 10px */
}
```

**Dark Mode:**
```css
.dark {
  --background: oklch(0.147 0.004 49.25);
  --foreground: oklch(0.985 0.001 106.423);
  --primary: oklch(0.923 0.003 48.717);
  --primary-foreground: oklch(0.216 0.006 56.043);
  --border: oklch(1 0 0 / 10%);
  --input: oklch(1 0 0 / 15%);
}
```

### Border Radius Scale
```css
--radius-sm: calc(var(--radius) - 4px);  /* 6px */
--radius-md: calc(var(--radius) - 2px);  /* 8px */
--radius-lg: var(--radius);               /* 10px */
--radius-xl: calc(var(--radius) + 4px);  /* 14px */
```

### Spacing Scale (Tailwind Defaults)
Uses Tailwind's default spacing scale:
- `0.5rem` (8px), `1rem` (16px), `1.5rem` (24px), `2rem` (32px), etc.
- Accessed via utilities: `p-4`, `gap-6`, `mt-8`

---

## 2. TYPOGRAPHY

**Default Font:** System sans-serif (via Tailwind's `font-sans`)

**Font Configuration:**
```css
@theme inline {
  --font-sans: "Inter", ui-sans-serif, system-ui, sans-serif;
}
```

**Typography Scale (Tailwind Utilities):**
```tsx
// Text sizes with defaults
text-xs   // 0.75rem (12px)
text-sm   // 0.875rem (14px)
text-base // 1rem (16px)
text-lg   // 1.125rem (18px)
text-xl   // 1.25rem (20px)
text-2xl  // 1.5rem (24px)
text-3xl  // 1.875rem (30px)

// Font weights
font-light    // 300
font-normal   // 400
font-medium   // 500
font-semibold // 600
font-bold     // 700
```

**Line Heights:**
- Default line height: `1.5`
- Tight: `leading-tight` (1.25)
- Relaxed: `leading-relaxed` (1.625)

---

## 3. COMPONENT STYLING

### Button Variants
```tsx
// Variant styles
<Button>Default</Button>           // Primary filled
<Button variant="secondary">      // Gray filled
<Button variant="outline">        // Bordered
<Button variant="ghost">          // Hover background only
<Button variant="destructive">    // Red filled
<Button variant="link">           // Text only, underline

// Size variants
<Button size="sm">Small</Button>  // h-9 px-3
<Button size="default">Default</Button> // h-10 px-4
<Button size="lg">Large</Button>  // h-11 px-8

// Icon-only buttons
<Button size="icon-sm"><Icon /></Button> // h-8 w-8
<Button size="icon"><Icon /></Button>    // h-10 w-10
<Button size="icon-lg"><Icon /></Button> // h-12 w-12
```

**Button CVA Configuration:**
```typescript
const buttonVariants = cva({
  base: "inline-flex items-center justify-center rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50",
  variants: {
    variant: {
      default: "bg-primary text-primary-foreground hover:bg-primary/90",
      destructive: "bg-destructive text-destructive-foreground hover:bg-destructive/90",
      outline: "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
      secondary: "bg-secondary text-secondary-foreground hover:bg-secondary/80",
      ghost: "hover:bg-accent hover:text-accent-foreground",
      link: "text-primary underline-offset-4 hover:underline",
    },
    size: {
      default: "h-10 px-4 py-2",
      sm: "h-9 rounded-md px-3",
      lg: "h-11 rounded-md px-8",
      icon: "h-10 w-10",
    },
  },
})
```

### Card Component
```tsx
<Card className="bg-card text-card-foreground border border-border">
  <CardHeader>
    <CardTitle>Title</CardTitle>
    <CardDescription>Description</CardDescription>
  </CardHeader>
  <CardContent>
    Content
  </CardContent>
  <CardFooter>
    Footer actions
  </CardFooter>
</Card>
```

**Card Styling:**
- Background: `bg-card` (white in light mode)
- Border: `border-border` (light gray)
- Shadow: Default shadow, no rounded corners by default
- Add `rounded-lg` for rounded corners

### Input Fields
```tsx
<Input
  type="text"
  placeholder="Enter text..."
  className="bg-background border border-input px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
/>
```

**Input States:**
- Default: `border-input` (light gray border)
- Focus: `ring-2 ring-ring` (blue outline)
- Error: Add `border-destructive` class
- Disabled: `disabled:opacity-50 disabled:cursor-not-allowed`

### Modal/Dialog
```tsx
<Dialog>
  <DialogContent className="bg-background border border-border rounded-lg shadow-lg">
    <DialogHeader>
      <DialogTitle>Title</DialogTitle>
    </DialogHeader>
    Content...
    <DialogFooter>Actions</DialogFooter>
  </DialogContent>
</Dialog>
```

---

## 4. THEMING

### Theme Configuration (components.json)
```json
{
  "style": "default",
  "tailwind": {
    "config": "tailwind.config.ts",
    "css": "src/app/globals.css",
    "baseColor": "zinc",
    "cssVariables": true  // Use CSS variables for theming
  },
  "rsc": false,
  "aliases": {
    "utils": "~/lib/utils",
    "components": "~/components"
  }
}
```

### Light/Dark Mode Implementation
```tsx
// Theme provider component
export function ThemeProvider({ children }) {
  const [theme, setTheme] = useState("light")

  useEffect(() => {
    const root = window.document.documentElement
    root.classList.remove("light", "dark")
    root.classList.add(theme)
  }, [theme])

  return (
    <ThemeProviderContext.Provider value={{ theme, setTheme }}>
      {children}
    </ThemeProviderContext.Provider>
  )
}

// Usage
<button onClick={() => setTheme(theme === "light" ? "dark" : "light")}>
  Toggle theme
</button>
```

### Custom Theme Creation
```css
/* Override CSS variables in globals.css */
:root {
  /* ClickUp-inspired primary */
  --primary: oklch(0.55 0.22 270);  /* Purple-ish */
  --primary-foreground: oklch(1 0 0);

  /* ClickUp accent colors */
  --accent: oklch(0.65 0.18 200);  /* Blue */
  --accent-foreground: oklch(1 0 0);

  /* Custom border radius */
  --radius: 0.5rem;  /* 8px, slightly less rounded */
}
```

---

## 5. TAILWIND INTEGRATION

### Accessing Design Tokens via Tailwind
```tsx
// Color utilities (mapped from CSS variables)
<div className="bg-background text-foreground">    // Base colors
<div className="bg-primary text-primary-foreground">  // Primary
<div className="bg-muted text-muted-foreground">  // Muted
<div className="border-border">                   // Border color
<div className="focus:ring-ring">                 // Focus ring

// Radius utilities
<div className="rounded-sm">  // var(--radius-sm)
<div className="rounded-md">  // var(--radius-md)
<div className="rounded-lg">  // var(--radius-lg)
<div className="rounded-xl">  // var(--radius-xl)

// Custom radius
<div style={{ borderRadius: 'var(--radius)' }}>
```

### Tailwind Theme Mapping (in globals.css)
```css
@theme inline {
  --color-background: var(--background);
  --color-foreground: var(--foreground);
  --color-primary: var(--primary);
  --color-primary-foreground: var(--primary-foreground);
  --color-secondary: var(--secondary);
  --color-secondary-foreground: var(--secondary-foreground);
  --color-muted: var(--muted);
  --color-muted-foreground: var(--muted-foreground);
  --color-accent: var(--accent);
  --color-accent-foreground: var(--accent-foreground);
  --color-destructive: var(--destructive);
  --color-border: var(--border);
  --color-input: var(--input);
  --color-ring: var(--ring);
  --radius-sm: calc(var(--radius) - 4px);
  --radius-md: calc(var(--radius) - 2px);
  --radius-lg: var(--radius);
  --radius-xl: calc(var(--radius) + 4px);
}
```

### Component-Level Styling
```tsx
// Using Tailwind @layer components
@layer components {
  .card {
    @apply bg-card text-card-foreground border border-border rounded-lg shadow-sm;
  }

  .input-field {
    @apply bg-background border border-input px-3 py-2 text-sm rounded-md;
  }
}
```

---

## 6. CLICKUP-INSPIRED CUSTOMIZATION

### ClickUp Brand Colors
```css
:root {
  /* ClickUp core colors */
  --shark: #292D34;        /* Dark charcoal background */
  --white: #FFFFFF;        /* Primary text on dark */
  --cornflower-blue: #7B68EE;  /* Primary actions */
  --hot-pink: #FD71AF;     /* Accent 1 */
  --malibu: #49CCF9;       /* Accent 2 */
  --supernova: #FFC800;    /* Accent 3 */

  /* Map to shadcn variables */
  --background: oklch(0.18 0.01 270);  /* Shark equivalent */
  --foreground: oklch(1 0 0);          /* White */
  --primary: oklch(0.55 0.15 270);     /* Purple (cornflower) */
  --primary-foreground: oklch(1 0 0);
  --accent: oklch(0.65 0.18 200);      /* Blue (malibu) */
  --accent-foreground: oklch(1 0 0);
  --destructive: oklch(0.65 0.22 350); /* Pink (hot pink) */
}
```

### ClickUp-Inspired Button Styles
```tsx
// Primary button (purple gradient)
<button className="bg-gradient-to-r from-[#7B68EE] to-[#FD71AF] text-white font-semibold rounded-lg px-6 py-2.5 hover:opacity-90 transition-opacity">
  Primary Action
</button>

// Secondary button (outline)
<button className="border-2 border-[#292D34] text-[#292D34] font-medium rounded-lg px-6 py-2.5 hover:bg-[#292D34] hover:text-white transition-colors">
  Secondary
</button>

// Ghost button (subtle)
<button className="text-[#7B68EE] font-medium px-4 py-2 hover:bg-[#7B68EE]/10 rounded-md transition-colors">
  Ghost
</button>
```

### ClickUp-Inspired Card Design
```tsx
<Card className="bg-white border-0 shadow-xl rounded-2xl overflow-hidden hover:shadow-2xl transition-shadow">
  <CardHeader className="bg-gradient-to-r from-[#7B68EE]/10 to-[#FD71AF]/10 px-6 py-4">
    <CardTitle className="text-[#292D34] font-bold text-lg">
      Card Title
    </CardTitle>
  </CardHeader>
  <CardContent className="p-6">
    <p className="text-gray-600 text-sm leading-relaxed">
      Card content goes here...
    </p>
  </CardContent>
</Card>
```

### Custom Input Styles (ClickUp-like)
```tsx
<Input
  className="border-2 border-gray-200 rounded-lg px-4 py-3 text-sm focus:border-[#7B68EE] focus:ring-2 focus:ring-[#7B68EE]/20 transition-all placeholder:text-gray-400"
  placeholder="Task name..."
/>
```

### Status Color System (ClickUp style)
```tsx
// Status badge component
const statusColors = {
  complete: "bg-[#49CCF9]/10 text-[#49CCF9] border-[#49CCF9]/20",
  inProgress: "bg-[#7B68EE]/10 text-[#7B68EE] border-[#7B68EE]/20",
  overdue: "bg-[#FD71AF]/10 text-[#FD71AF] border-[#FD71AF]/20",
  review: "bg-[#FFC800]/10 text-[#B59400] border-[#FFC800]/20",
}

<Badge className={statusColors.inProgress}>
  In Progress
</Badge>
```

### ClickUp-style Sidebar
```tsx
<Sidebar className="bg-[#292D34] border-r border-gray-800">
  <SidebarHeader className="bg-[#292D34] px-4 py-3 border-b border-gray-800">
    <Logo />
  </SidebarHeader>
  <SidebarContent className="bg-[#292D34]">
    <SidebarMenuItem className="text-gray-300 hover:text-white hover:bg-white/10 rounded-lg px-3 py-2 transition-colors">
      Menu Item
    </SidebarMenuItem>
  </SidebarContent>
</Sidebar>
```

### Custom Radius & Spacing
```css
:root {
  /* ClickUp uses slightly more rounded corners */
  --radius: 0.75rem;  /* 12px instead of 10px */

  /* Map to Tailwind */
  --radius-sm: calc(var(--radius) - 4px);  /* 8px */
  --radius-md: calc(var(--radius) - 2px);  /* 10px */
  --radius-lg: var(--radius);               /* 12px */
  --radius-xl: calc(var(--radius) + 4px);  /* 16px */
  --radius-2xl: calc(var(--radius) + 8px); /* 20px */
}
```

### Custom Font Setup (ClickUp uses Inter)
```tsx
// next.config.js or similar
import { Inter } from 'next/font/google'

const inter = Inter({
  subsets: ['latin'],
  variable: '--font-inter',
})

// globals.css
:root {
  --font-sans: var(--font-inter), ui-sans-serif, system-ui;
}
```

---

## 7. EASY CUSTOMIZATION STRATEGIES

### Strategy 1: CSS Variable Override (Recommended)
```css
/* globals.css - Single source of truth */
:root {
  /* Override only what you need */
  --primary: #7B68EE;
  --radius: 12px;
}
/* All components update automatically */
```

### Strategy 2: Component Variants (CVA)
```typescript
// Create custom variant for ClickUp style
const buttonVariants = cva({
  variants: {
    variant: {
      clickup: "bg-gradient-to-r from-purple-600 to-pink-500 text-white font-semibold rounded-lg hover:opacity-90",
    }
  }
})
```

### Strategy 3: Tailwind @layer
```css
@layer components {
  .btn-clickup-primary {
    @apply bg-gradient-to-r from-[#7B68EE] to-[#FD71AF] text-white font-semibold rounded-lg px-6 py-2.5;
  }
}
```

### Strategy 4: Component Wrapper
```tsx
// Create wrapper components with custom styling
export function ClickUpButton({ children, ...props }) {
  return (
    <button
      className="bg-gradient-to-r from-[#7B68EE] to-[#FD71AF] text-white font-semibold rounded-lg px-6 py-2.5 hover:opacity-90 transition-opacity"
      {...props}
    >
      {children}
    </button>
  )
}
```

---

## 8. IMPLEMENTATION RECOMMENDATIONS

### Setup Steps
1. **Initialize shadcn/ui** with CSS variables enabled
2. **Override core variables** in `globals.css` with ClickUp colors
3. **Create custom component variants** using CVA for consistent styling
4. **Use Tailwind @layer** for reusable utility combinations
5. **Build wrapper components** for frequently used ClickUp-style patterns

### File Structure
```
src/
├── app/
│   └── globals.css          # Theme variables & custom styles
├── components/
│   ├── ui/                  # shadcn/ui components (don't edit)
│   └── clickup/             # ClickUp-style wrapper components
│       ├── clickup-button.tsx
│       ├── clickup-card.tsx
│       └── clickup-badge.tsx
└── lib/
    └── utils.ts             # cn() helper
```

### Best Practices
- **Never modify shadcn/ui components directly** - they'll be overwritten
- **Use CSS variables** for global theme changes
- **Create wrapper components** for project-specific styling
- **Leverage Tailwind's @layer** for reusable styles
- **Test in both light/dark modes** to ensure contrast ratios

---

## UNRESOLVED QUESTIONS

1. Should we maintain shadcn/ui's default colors as fallbacks, or fully replace them?
2. How do we handle gradient buttons in dark mode while maintaining accessibility?
3. What's the migration path if we need to update shadcn/ui components?
4. Should we create a custom Tailwind plugin for ClickUp-specific utilities?
5. How do we ensure color contrast ratios meet WCAG standards with ClickUp's palette?
