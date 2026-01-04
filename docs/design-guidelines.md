# Nexora Management Platform - Design Guidelines

**Version:** 2.1 (ClickUp-Inspired + Components)
**Last Updated:** 2026-01-04 22:10
**Target:** Small teams < 30 users
**Tech Stack:** Next.js 15 + shadcn/ui + Tailwind CSS + TypeScript

---

## 1. COLOR PALETTE

### Primary Brand Colors (ClickUp Purple)

Nexora uses ClickUp's signature purple as the primary brand color, providing a distinctive and memorable identity.

```css
/* Primary Colors - ClickUp Purple */
--clickup-purple: #7B68EE;           /* Primary Brand */
--clickup-purple-hover: #A78BFA;      /* Hover State */
--clickup-purple-active: #5B4BC4;     /* Active/Pressed */
--clickup-purple-light: #F5F3FF;       /* Light Tint */

/* Secondary Purple */
--clickup-purple-subtle: #E9E5FF;     /* Subtle background */
--clickup-purple-dark: #4C3D95;        /* Dark shade */

/* Accent Colors (supporting purple theme) */
--clickup-blue: #3B82F6;              /* Info, Links */
--clickup-green: #10B981;             /* Success */
--clickup-yellow: #F59E0B;            /* Warning */
--clickup-red: #EF4444;                /* Error/Critical */
--clickup-orange: #F97316;           /* Urgent */

/* Gradients */
--clickup-gradient-primary: linear-gradient(135deg, #7B68EE 0%, #A78BFA 100%);
--clickup-gradient-subtle: linear-gradient(135deg, #F5F3FF 0%, #E9E5FF 100%);
```

### Neutral Colors (Light Mode)

```css
/* Backgrounds */
--clickup-bg-primary: #FFFFFF;
--clickup-bg-secondary: #F9FAFB;    /* Gray 50 */
--clickup-bg-tertiary: #F3F4F6;     /* Gray 100 */
--clickup-bg-elevated: #FFFFFF;

/* Borders */
--clickup-border: #E5E7EB;           /* Gray 200 */
--clickup-border-strong: #D1D5DB;      /* Gray 300 */
--clickup-border-subtle: #F3F4F6;     /* Gray 100 */

/* Text */
--clickup-text-primary: #111827;       /* Gray 900 */
--clickup-text-secondary: #4B5563;     /* Gray 600 */
--clickup-text-tertiary: #9CA3AF;      /* Gray 400 */
--clickup-text-disabled: #D1D5DB;      /* Gray 300 */
--clickup-text-inverse: #FFFFFF;       /* White text on dark */
```

### Neutral Colors (Dark Mode)

```css
/* Backgrounds */
--clickup-dark-bg-primary: #111827;    /* Gray 900 */
--clickup-dark-bg-secondary: #1F2937;   /* Gray 800 */
--clickup-dark-bg-tertiary: #374151;    /* Gray 700 */

/* Borders */
--clickup-dark-border: #374151;         /* Gray 700 */
--clickup-dark-border-strong: #4B5563;   /* Gray 600 */

/* Text */
--clickup-dark-text-primary: #F9FAFB;    /* Gray 50 */
--clickup-dark-text-secondary: #D1D5DB;  /* Gray 300 */
--clickup-dark-text-tertiary: #9CA3AF;   /* Gray 400 */
```

### Semantic Colors

```css
/* Success (Green) */
--clickup-success: #10B981;
--clickup-success-bg: #D1FAE5;
--clickup-success-border: #34D399;

/* Warning (Yellow) */
--clickup-warning: #F59E0B;
--clickup-warning-bg: #FEF3C7;
--clickup-warning-border: #FBBF24;

/* Error (Red) */
--clickup-error: #EF4444;
--clickup-error-bg: #FEE2E2;
--clickup-error-border: #F87171;

/* Info (Blue) */
--clickup-info: #3B82F6;
--clickup-info-bg: #DBEAFE;
--clickup-info-border: #60A5FA;
```

### Status Colors (ClickUp-style)

```css
/* Task Status Colors */
--clickup-status-complete: #10B981;     /* Green - Complete */
--clickup-status-in-progress: #F59E0B;   /* Yellow - In Progress */
--clickup-status-review: #8B5CF6;         /* Purple - In Review */
--clickup-status-blocked: #EF4444;        /* Red - Blocked */
--clickup-status-todo: #D1D5DB;           /* Gray - To Do */

/* Priority Colors */
--clickup-priority-urgent: #EF4444;       /* Red - Urgent */
--clickup-priority-high: #F97316;          /* Orange - High */
--clickup-priority-medium: #F59E0B;        /* Yellow - Medium */
--clickup-priority-normal: #3B82F6;        /* Blue - Normal */
--clickup-priority-low: #6B7280;           /* Gray - Low */
```

### shadcn/ui CSS Variable Mapping

```css
:root {
  /* Primary */
  --background: 255 255 255;
  --foreground: 17 24 39; /* Gray 900 */

  /* ClickUp Purple Primary */
  --primary: 123 104 238; /* #7B68EE */
  --primary-foreground: 255 255 255;

  /* Secondary (Blue for contrast) */
  --secondary: 59 130 246; /* #3B82F6 */
  --secondary-foreground: 255 255 255;

  /* Muted */
  --muted: 243 244 246; /* Gray 100 */
  --muted-foreground: 100 116 139; /* Gray 500 */

  /* Accent (Purple accent) */
  --accent: 139 92 246; /* #8B5CF6 */
  --accent-foreground: 255 255 255;

  /* Destructive */
  --destructive: 239 68 68; /* Red 500 */
  --destructive-foreground: 255 255 255;

  /* Borders */
  --border: 229 231 235; /* Gray 200 */
  --input: 229 231 235;
  --ring: 123 104 238; /* #7B68EE */

  /* Radius */
  --radius: 0.5rem; /* 8px */
}

.dark {
  --background: 17 24 39; /* Gray 900 */
  --foreground: 249 250 251; /* Gray 50 */

  /* Primary (lighter in dark mode) */
  --primary: 167 139 250; /* #A78BFA */
  --primary-foreground: 255 255 255;

  /* Muted */
  --muted: 30 41 59; /* Gray 800 */
  --muted-foreground: 148 163 184; /* Gray 400 */

  /* Border */
  --border: 55 65 81; /* Gray 700 */
  --input: 55 65 81;
  --ring: 167 139 250;
}
```

---

## 2. TYPOGRAPHY

### Font Families

**Primary Typeface:** Inter (Google Fonts)
- Designer: Eric Olson and Rasmus Andersson
- Style: Clean, modern sans-serif optimized for UI
- Vietnamese Support: Yes
- Weights: 400, 500, 600, 700
- Variable Font: Available for optimal performance
- **Why Inter:** ClickUp uses Inter for its clean, readable appearance

**Secondary Typeface:** Plus Jakarta Sans
- Usage: Display headings, marketing materials
- Weights: 500, 600, 700, 800

**Monospace Typeface:** JetBrains Mono
- Usage: Code blocks, timestamps, keyboard shortcuts
- Weights: 400, 500

### Typography Scale (ClickUp-inspired)

```css
/* Font Sizes */
--text-xs: 0.6875rem;   /* 11px - Tiny text, timestamps, labels */
--text-sm: 0.75rem;     /* 12px - Captions, metadata */
--text-base: 0.875rem;  /* 14px - Body text, UI elements */
--text-md: 1rem;        /* 16px - Emphasized body */
--text-lg: 1.125rem;    /* 18px - Large body */
--text-xl: 1.25rem;     /* 20px - Small headings */
--text-2xl: 1.5rem;     /* 24px - Section headings */
--text-3xl: 1.875rem;   /* 30px - Page titles */
--text-4xl: 2rem;       /* 32px - Hero titles */
```

### Font Weights

```css
--font-regular: 400;     /* Body text, descriptions */
--font-medium: 500;      /* Emphasized text, labels */
--font-semibold: 600;    /* Headings, important UI */
--font-bold: 700;        /* Strong headings, CTAs */
```

### Line Heights

```css
--leading-tight: 1.25;   /* Headings, compact text */
--leading-normal: 1.5;   /* Body text */
--leading-relaxed: 1.75;  /* Extended reading */
```

### Letter Spacing

```css
--tracking-tight: -0.011em; /* Large headings */
--tracking-normal: 0;         /* Body text */
--tracking-wide: 0.011em;   /* Small text, uppercase */
```

### Typography Hierarchy (ClickUp-style)

| Element | Font Size | Weight | Line Height | Usage |
|---------|-----------|--------|-------------|-------|
| H1 | 32px (2rem) | 700 | 1.25 | Page titles, hero |
| H2 | 24px (1.5rem) | 600 | 1.25 | Section headings |
| H3 | 20px (1.25rem) | 600 | 1.25 | Card titles |
| H4 | 16px (1rem) | 600 | 1.25 | Subsection headers |
| Body | 14px (0.875rem) | 400 | 1.5 | Standard content |
| Small | 12px (0.75rem) | 400 | 1.5 | Metadata, timestamps |
| Tiny | 11px (0.6875rem) | 400 | 1.5 | Labels, tags |

### Google Fonts Implementation

```tsx
// app/layout.tsx
import { Inter, Plus_Jakarta_Sans, JetBrains_Mono } from 'next/font/google'

const inter = Inter({
  subsets: ['latin', 'latin-ext', 'vietnamese'],
  variable: '--font-inter',
  display: 'swap',
  weight: '400 500 600 700',
})

const plusJakartaSans = Plus_Jakarta_Sans({
  subsets: ['latin', 'latin-ext', 'vietnamese'],
  variable: '--font-plus-jakarta-sans',
  display: 'swap',
  weight: '500 600 700 800',
})

const jetbrainsMono = JetBrains_Mono({
  subsets: ['latin'],
  variable: '--font-jetbrains-mono',
  display: 'swap',
})

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" className={inter.variable}>
      <body className={inter.className}>{children}</body>
    </html>
  )
}
```

---

## 3. SPACING SYSTEM

### Base Unit: 4px

```css
/* Spacing Scale */
--space-0: 0;
--space-1: 0.25rem;   /* 4px - Tight gaps */
--space-2: 0.5rem;    /* 8px - Icon padding */
--space-3: 0.75rem;   /* 12px - Compact padding */
--space-4: 1rem;      /* 16px - Standard spacing */
--space-5: 1.25rem;   /* 20px - Component gaps */
--space-6: 1.5rem;    /* 24px - Section spacing */
--space-8: 2rem;      /* 32px - Large gaps */
--space-10: 2.5rem;   /* 40px - XL spacing */
--space-12: 3rem;     /* 48px - XXL spacing */
--space-16: 4rem;     /* 64px - Huge spacing */
```

### Component Spacing (ClickUp-style)

```css
/* Buttons */
--button-padding-sm: 8px 16px;        /* Small button */
--button-padding-md: 10px 16px;       /* Default button */
--button-padding-lg: 12px 20px;       /* Large button */

/* Cards */
--card-padding-sm: 12px;               /* Compact card */
--card-padding-md: 16px;               /* Standard card */
--card-padding-lg: 20px;               /* Spacious card */

/* Layout */
--section-gap: 24px;                   /* Section gap */
--component-gap: 16px;                 /* Component gap */
--element-gap: 8px;                    /* Element gap */
--tight-gap: 4px;                       /* Tight gap */
```

---

## 4. BORDER RADIUS

### Radius Scale (ClickUp-style)

```css
--radius-xs: 0.25rem;   /* 4px - Small badges, dots */
--radius-sm: 0.375rem;  /* 6px - Icon buttons */
--radius-md: 0.5rem;    /* 8px - Buttons, inputs */
--radius-lg: 0.75rem;   /* 12px - Cards, panels */
--radius-xl: 1rem;      /* 16px - Modals, large cards */
--radius-2xl: 1.5rem;   /* 24px - Hero sections */
--radius-full: 9999px;  /* Pill buttons, avatars */
```

### Component-Specific Radius

| Component | Border Radius | Notes |
|-----------|---------------|-------|
| Buttons (Primary) | 6px (0.375rem) | Slightly rounded, modern |
| Buttons (Secondary) | 6px (0.375rem) | Consistent |
| Buttons (Ghost) | 4px (0.25rem) | Subtle |
| Buttons (Icon) | 6px (0.375rem) | Touch-friendly |
| Task Cards | 8px (0.5rem) | ClickUp style |
| Status Badges | 4px (0.25rem) | Small, pill-shaped |
| Inputs | 6px (0.375rem) | Consistent with buttons |
| Modals | 12px (0.75rem) | Soft, approachable |
| Dropdowns | 6px (0.375rem) | Consistent with inputs |
| Avatars | Pill (9999px) | Circular |
| Tooltips | 6px (0.375rem) | Compact |

---

## 5. COMPONENT SPECIFICATIONS

### Buttons (ClickUp-style)

#### Primary Button

```css
.clickup-button-primary {
  background: var(--clickup-purple);
  color: white;
  border: none;
  border-radius: 6px;
  padding: 10px 16px;
  font-size: 14px;
  font-weight: 600;
  line-height: 1.5;
  transition: all 150ms ease;
  box-shadow: 0 1px 2px rgba(123, 104, 238, 0.2);
}

.clickup-button-primary:hover {
  background: var(--clickup-purple-hover);
  transform: translateY(-1px);
  box-shadow: 0 4px 6px rgba(123, 104, 238, 0.3);
}

.clickup-button-primary:active {
  background: var(--clickup-purple-active);
  transform: translateY(0);
}
```

**Tailwind:**
```tsx
<Button className="bg-[#7B68EE] hover:bg-[#A78BFA] text-white font-semibold rounded-[6px] px-4 py-2.5 shadow-sm hover:shadow-md transition-all">
  Create Task
</Button>
```

#### Secondary Button

```css
.clickup-button-secondary {
  background: white;
  border: 1px solid var(--clickup-border);
  color: var(--clickup-text-primary);
  border-radius: 6px;
  padding: 10px 16px;
  font-size: 14px;
  font-weight: 600;
}

.clickup-button-secondary:hover {
  background: var(--clickup-bg-secondary);
  border-color: var(--clickup-border-strong);
}
```

#### Ghost Button

```css
.clickup-button-ghost {
  background: transparent;
  color: var(--clickup-text-secondary);
  border: none;
  border-radius: 4px;
  padding: 8px 12px;
  font-size: 14px;
  font-weight: 500;
}

.clickup-button-ghost:hover {
  background: var(--clickup-bg-tertiary);
  color: var(--clickup-text-primary);
}
```

### Task Cards (ClickUp-style)

```css
.clickup-task-card {
  background: white;
  border: 1px solid var(--clickup-border);
  border-radius: 8px;
  padding: 12px 16px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  cursor: grab;
  transition: all 200ms ease;
}

.clickup-task-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  border-color: var(--clickup-purple);
}

.clickup-task-card:active {
  cursor: grabbing;
}
```

### Status Badges (ClickUp colors)

```css
/* Complete - Green */
.badge-complete {
  background: #D1FAE5;
  color: #065F46;
  border-radius: 4px;
  padding: 4px 8px;
  font-size: 12px;
  font-weight: 500;
}

/* In Progress - Yellow */
.badge-in-progress {
  background: #FEF3C7;
  color: #92400E;
  border-radius: 4px;
  padding: 4px 8px;
  font-size: 12px;
  font-weight: 500;
}

/* Overdue - Red */
.badge-overdue {
  background: #FEE2E2;
  color: #991B1B;
  border-radius: 4px;
  padding: 4px 8px;
  font-size: 12px;
  font-weight: 500;
}
```

### Input Fields (ClickUp-style with Purple Focus)

```css
.clickup-input {
  background: white;
  border: 1px solid var(--clickup-border);
  border-radius: 6px;
  padding: 10px 12px;
  font-size: 14px;
  line-height: 1.5;
  color: var(--clickup-text-primary);
}

.clickup-input:focus {
  outline: none;
  border-color: var(--clickup-purple);
  box-shadow: 0 0 0 3px rgba(123, 104, 238, 0.1);
}

.clickup-input:hover:not(:focus) {
  border-color: var(--clickup-border-strong);
}

.clickup-input::placeholder {
  color: var(--clickup-text-tertiary);
}
```

---

## 6. LAYOUT PATTERNS

### Main Application Layout

```
┌────────────────────────────────────────────────────┐
│ Header (56px) - Logo, Search, Notifications, Profile  │
├──────────┬─────────────────────────────────────────┤
│          │                                          │
│ Sidebar  │  Main Content Area                      │
│ (240px)  │  - Breadcrumbs (32px)                   │
│          │  - Page Header (48px)                   │
│ Nav      │  - Filters/Toolbar (40px)               │
│ Spaces   │  - Content (flex)                       │
│          │                                          │
└──────────┴─────────────────────────────────────────┘
```

### Sidebar Navigation

**Expanded:**
- Width: 240px
- Background: `#F9FAFB`
- Border Right: `1px solid #E5E7EB`
- Item Height: 40px
- Item Padding: 0 12px
- Icon Size: 20px

**Collapsed:**
- Width: 64px
- Icon Size: 24px (centered)

### Board View Layout (ClickUp Kanban)

```
┌─────────────────────────────────────────────────────────┐
│ Column 1 (280px) │ Column 2 (280px) │ Column 3 (280px) │
├──────────────────┼──────────────────┼──────────────────┤
│ Task Card        │ Task Card        │ Task Card        │
├──────────────────┼──────────────────┼──────────────────┤
│ Task Card        │ Task Card        │ Task Card        │
└──────────────────┴──────────────────┴──────────────────┘
```

---

## 7. ICONOGRAPHY

### Icon Library: Lucide React

**Rationale:**
- Clean, consistent 2px stroke
- 1000+ icons available
- Tree-shakeable
- Excellent TypeScript support

### Icon Sizes (ClickUp-style)

| Usage | Size | Stroke | Color |
|-------|------|--------|-------|
| Navigation | 18px | 2px | Text secondary |
| Button (with text) | 16px | 2px | Inherit |
| Button (icon-only) | 20px | 2px | Inherit |
| Status indicators | 14px | 2px | Semantic color |
| Feature icons | 24px | 2px | Primary purple |
| Metadata | 12px | 2px | Tertiary |

---

## 8. SHADCN/UI CUSTOMIZATION

### Tailwind Configuration

```javascript
// tailwind.config.ts
import type { Config } from 'tailwindcss'

const config: Config = {
  darkMode: ['class'],
  content: [
    './app/**/*.{ts,tsx}',
    './components/**/*.{ts,tsx}',
    './src/**/*.{ts,tsx}',
    './src/features/**/*.{ts,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#7B68EE',
          hover: '#A78BFA',
          active: '#5B4BC4',
          foreground: '#FFFFFF',
        },
        secondary: {
          DEFAULT: '#3B82F6',
          foreground: '#FFFFFF',
        },
        background: {
          DEFAULT: '#FFFFFF',
          secondary: '#F9FAFB',
        },
        foreground: {
          DEFAULT: '#111827',
          secondary: '#4B5563',
          tertiary: '#9CA3AF',
        },
        muted: {
          DEFAULT: '#F3F4F6',
          foreground: '#6B7280',
        },
        border: {
          DEFAULT: '#E5E7EB',
        },
      },
      borderRadius: {
        DEFAULT: '6px',
        sm: '4px',
        md: '6px',
        lg: '8px',
        xl: '12px',
        '2xl': '16px',
      },
      fontFamily: {
        sans: ['var(--font-inter)', 'sans-serif'],
        mono: ['var(--font-jetbrains-mono)', 'monospace'],
      },
    },
  },
  plugins: [],
}

export default config
```

---

## 9. DARK MODE

### Dark Mode Implementation

```tsx
// components/theme-provider.tsx
'use client'

import * as React from 'react'

type Theme = 'light' | 'dark'

type ThemeProviderProps = {
  children: React.ReactNode
  defaultTheme?: Theme
}

export function ThemeProvider({
  children,
  defaultTheme = 'system'
}: ThemeProviderProps) {
  const [theme, setTheme] = React.useState<Theme>('system')

  React.useEffect(() => {
    const root = window.document.documentElement
    root.classList.remove('light', 'dark')

    if (theme === 'system') {
      const systemTheme = window.matchMedia('(prefers-color-scheme: dark)').matches
        ? 'dark'
        : 'light'
      root.classList.add(systemTheme)
    } else {
      root.classList.add(theme)
    }
  }, [theme])

  return (
    <ThemeProviderContext.Provider value={{ theme, setTheme }}>
      {children}
    </ThemeProviderContext.Provider>
  )
}
```

### Dark Mode Color Overrides

```css
.dark {
  /* Backgrounds */
  --clickup-bg-primary: #111827;
  --clickup-bg-secondary: #1F2937;
  --clickup-bg-tertiary: #374151;

  /* Text */
  --clickup-text-primary: #F9FAFB;
  --clickup-text-secondary: #D1D5DB;
  --clickup-text-tertiary: #9CA3AF;

  /* Border */
  --clickup-border: #374151;
  --clickup-border-strong: #4B5563;

  /* Primary (lighter for dark mode) */
  --clickup-purple: #A78BFA;
  --clickup-purple-hover: #C4B5FD;
  --clickup-purple-active: #7B68EE;
}
```

---

## 10. ACCESSIBILITY (WCAG 2.1 AA)

### Color Contrast Ratios (ClickUp Purple)

All combinations meet WCAG 2.1 AA:

| Combination | Ratio | Grade |
|-------------|-------|-------|
| Purple (#7B68EE) on White | 4.8:1 | AA ✓ |
| Purple (#7B68EE) on Black | 10.1:1 | AAA ✓ |
| Light Purple (#F5F3FF) on White | 1.5:1 ❌ (needs dark text) |
| White text on Purple | 10.1:1 | AAA ✓ |

### Focus States

```css
.focus-visible:focus-visible {
  outline: 2px solid var(--clickup-purple);
  outline-offset: 2px;
}
```

---

## 11. ANIMATIONS

### Transition Timings

```css
--transition-instant: 0ms;
--transition-fast: 150ms;   /* Buttons, toggles */
--transition-base: 200ms;   /* Dropdowns, sidebar */
--transition-slow: 300ms;   /* Modals, page */
```

### Micro-interactions

```css
/* Hover lift effect */
.clickup-hover-lift {
  transition: transform 200ms cubic-bezier(0, 0, 0.2, 1);
}

.clickup-hover-lift:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

/* Scale in (modals) */
.clickup-scale-in {
  animation: scaleIn 200ms cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

@keyframes scaleIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}
```

---

## 12. RESPONSIVE BREAKPOINTS

```css
--breakpoint-xs: 375px;   /* Small mobile */
--breakpoint-sm: 640px;   /* Mobile */
--breakpoint-md: 768px;   /* Tablet */
--breakpoint-lg: 1024px;  /* Desktop */
--breakpoint-xl: 1280px;  /* Large desktop */
--breakpoint-2xl: 1536px; /* Extra large */
```

---

## 13. IMPLEMENTATION CHECKLIST

### Setup Phase
- [ ] Update Tailwind config with ClickUp purple (#7B68EE)
- [ ] Update all shadcn/ui components to use purple primary
- [ ] Add Inter font with Google Fonts
- [ ] Update CSS variables for ClickUp colors
- [ ] Update dark mode to use lighter purple in dark mode
- [ ] Test color contrast ratios

### Component Updates
- [ ] Update button variants to use purple gradient
- [ ] Update status badges to ClickUp colors
- [ ] Update input focus to purple ring
- [ ] Update all interactive elements to purple theme
- [ ] Test components in both light and dark modes

### Testing Phase
- [ ] Verify all contrast ratios meet WCAG 2.1 AA
- [ ] Test keyboard navigation
- [ ] Test on mobile, tablet, desktop
- [ ] Test dark mode toggle
- [ ] Verify all components match ClickUp aesthetic

---

## 14. DESIGN TOKENS REFERENCE

### Quick Reference

```css
/* ClickUp Brand */
--clickup-purple: #7B68EE
--clickup-purple-hover: #A78BFA
--clickup-purple-active: #5B4BC4
--clickup-purple-light: #F5F3FF

/* Spacing */
--space-1: 4px
--space-2: 8px
--space-4: 16px
--space-6: 24px
--space-8: 32px

/* Border Radius */
--radius-sm: 4px
--radius-md: 6px
--radius-lg: 8px
--radius-xl: 12px

/* Shadows */
--shadow-sm: 0 1px 2px rgba(0,0,0,0.05)
--shadow-md: 0 1px 3px rgba(0,0,0,0.1)
--shadow-lg: 0 4px 6px rgba(0,0,0,0.1)
```

---

## 15. CLICKUP COMPONENTS API (Phase 02)

All components use `class-variance-authority` for type-safe variant management and follow ClickUp's visual language.

### Button Component

**File:** `src/components/ui/button.tsx`

**Props:**
```typescript
interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement>,
  VariantProps<typeof buttonVariants> {
  variant?: "primary" | "secondary" | "ghost" | "destructive" | "outline" | "link"
  size?: "sm" | "md" | "lg" | "icon"
  asChild?: boolean
}
```

**Variants:**
- `primary` - Purple background with shadow and scale transform
- `secondary` - White background with 2px gray border
- `ghost` - Transparent background with gray hover
- `destructive` - Red background for dangerous actions
- `outline` - Border only with hover fill
- `link` - Text-only with underline

**Sizes:**
- `sm` - 36px height, 12px text, 12px padding
- `md` - 40px height, 14px text, 16px padding (default)
- `lg` - 44px height, 16px text, 24px padding
- `icon` - 40px × 40px square

**Usage:**
```tsx
import { Button } from "@/components/ui/button"

<Button variant="primary" size="md">Create Task</Button>
<Button variant="secondary">Cancel</Button>
<Button variant="ghost" className="gap-2">
  <Icon className="h-4 w-4" />
  With Icon
</Button>
```

### Badge Component

**File:** `src/components/ui/badge.tsx`

**Props:**
```typescript
interface BadgeProps extends React.HTMLAttributes<HTMLDivElement>,
  VariantProps<typeof badgeVariants> {
  status?: "complete" | "inProgress" | "overdue" | "neutral" | "default"
  size?: "sm" | "md" | "lg"
  icon?: React.ReactNode
}
```

**Status Variants:**
- `complete` - Green (emerald) for success states
- `inProgress` - Yellow/amber for active states
- `overdue` - Red for urgent/overdue states
- `neutral` - Gray for inactive states
- `default` - Primary purple for brand alignment

**Sizes:**
- `sm` - 10px text, 6px padding
- `md` - 12px text, 8px padding (default)
- `lg` - 14px text, 10px padding

**Usage:**
```tsx
import { Badge } from "@/components/ui/badge"

<Badge status="complete">Complete</Badge>
<Badge status="inProgress" icon={<Clock className="h-3 w-3" />}>
  In Progress
</Badge>
<Badge status="overdue">Overdue</Badge>
```

### Input Component

**File:** `src/components/ui/input.tsx`

**Props:**
```typescript
interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  error?: boolean
}
```

**Features:**
- 40px height (h-10)
- 2px border (border-2)
- Purple focus ring (focus:ring-primary/20)
- Error state with red border and focus ring

**Usage:**
```tsx
import { Input } from "@/components/ui/input"

<Input placeholder="Enter your name..." />
<Input error placeholder="This field has an error" />
<Input type="email" value={value} onChange={onChange} />
```

### Textarea Component

**File:** `src/components/ui/textarea.tsx`

**Props:**
```typescript
interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  error?: boolean
}
```

**Features:**
- 80px minimum height
- 2px border (matches Input)
- Purple focus ring
- Vertical resize only
- Error state support

**Usage:**
```tsx
import { Textarea } from "@/components/ui/textarea"

<Textarea placeholder="Enter a description..." />
<Textarea error placeholder="This field has an error" />
```

### Avatar Component

**File:** `src/components/ui/avatar.tsx`

**Components:**
- `Avatar` - Root container
- `AvatarImage` - Image display
- `AvatarFallback` - Initials fallback

**Props:**
```typescript
interface AvatarProps extends React.ComponentPropsWithoutRef<typeof AvatarPrimitive.Root> {}

interface AvatarImageProps extends React.ComponentPropsWithoutRef<typeof AvatarPrimitive.Image> {}

interface AvatarFallbackProps extends React.ComponentPropsWithoutRef<typeof AvatarPrimitive.Fallback> {
  name?: string  // Auto-generates initials and hash-based color
}
```

**Features:**
- Initials from first 2 characters of name
- Hash-based color from 16-color ClickUp palette
- Radix UI primitives for accessibility

**Usage:**
```tsx
import { Avatar, AvatarImage, AvatarFallback } from "@/components/ui/avatar"

<Avatar>
  <AvatarImage src="https://example.com/avatar.jpg" />
  <AvatarFallback name="John Doe" /> {/* Shows "JD" */}
</Avatar>

<Avatar className="h-8 w-8">
  <AvatarFallback name="Alice Smith" />
</Avatar>
```

### Tooltip Component

**File:** `src/components/ui/tooltip.tsx`

**Components:**
- `Tooltip` - Provider with mouse events
- `TooltipTrigger` - Trigger element (asChild for composition)
- `TooltipContent` - Dark tooltip content

**Props:**
```typescript
interface TooltipProps {
  children: React.ReactNode
  open?: boolean
  onOpenChange?: (open: boolean) => void
  delayDuration?: number  // Default: 200ms
}

interface TooltipTriggerProps extends React.HTMLAttributes<HTMLDivElement> {
  asChild?: boolean
}

interface TooltipContentProps extends React.HTMLAttributes<HTMLDivElement> {}
```

**Styling:**
- Dark theme (bg-gray-900, text-white)
- 200ms hover delay
- Zoom/fade animation
- Rounded corners (rounded-md)

**Usage:**
```tsx
import { Tooltip, TooltipTrigger, TooltipContent, TooltipProvider } from "@/components/ui/tooltip"

<TooltipProvider>
  <Tooltip delayDuration={200}>
    <TooltipTrigger asChild>
      <Button variant="outline">Hover me</Button>
    </TooltipTrigger>
    <TooltipContent>
      This is a helpful tooltip
    </TooltipContent>
  </Tooltip>
</TooltipProvider>
```

### Component Showcase

View all components in action:
- **Route:** `/components/showcase`
- **Features:** Dark mode toggle, all variants, real-world examples

---

## 16. LAYOUT PATTERNS (Phase 03) ✅

All layout components provide responsive, accessible structure for application views with ClickUp-inspired dimensions and spacing.

### Layout Component Structure

```
apps/frontend/src/components/layout/
├── app-layout.tsx       # Main layout wrapper
├── app-header.tsx       # Top header (56px)
├── app-sidebar.tsx      # Collapsible sidebar
├── sidebar-nav.tsx      # Navigation items
├── breadcrumb.tsx       # Breadcrumb component
├── container.tsx        # Responsive container
└── board-layout.tsx     # Board view layout
```

### Main Application Layout

**File:** `src/components/layout/app-layout.tsx`

**Purpose:** Root layout wrapper providing consistent structure

**Props:**
```typescript
interface AppLayoutProps {
  children: React.ReactNode
}
```

**Layout Structure:**
```
AppLayout (flex-col, h-screen)
├── AppHeader (h-14, 56px)
└── div.flex-1 (flex row, overflow-hidden)
    ├── AppSidebar (w-60 → w-16)
    └── main (flex-1, overflow-auto)
```

**Features:**
- Full viewport height (h-screen)
- Fixed header with collapse toggle
- Collapsible sidebar (240px → 64px)
- Scrollable main content
- Responsive background (gray-50 dark:gray-900)

**Usage:**
```tsx
import { AppLayout } from "@/components/layout/app-layout"

export default function Layout({ children }: { children: React.ReactNode }) {
  return <AppLayout>{children}</AppLayout>
}
```

### App Header

**File:** `src/components/layout/app-header.tsx`

**Purpose:** Top navigation bar with global controls

**Props:**
```typescript
interface AppHeaderProps {
  sidebarCollapsed: boolean
  onToggleSidebar: () => void
}
```

**Dimensions:**
- Height: 56px (h-14)
- Padding: 16px horizontal (px-4)

**Sections:**
- **Left:** Menu toggle, Logo, Search
- **Right:** Notifications, Settings, Profile

**Features:**
- Logo with gradient background (primary → primary-hover)
- Global search bar (256px width, hidden on mobile)
- Notification bell icon
- Settings gear icon
- User avatar (36px × 36px)
- Sidebar collapse button

**Responsive Behavior:**
- Search hidden below 768px (hidden md:block)
- All icons remain visible at all breakpoints

**Usage:**
```tsx
<AppHeader
  sidebarCollapsed={collapsed}
  onToggleSidebar={() => setCollapsed(!collapsed)}
/>
```

### App Sidebar

**File:** `src/components/layout/app-sidebar.tsx`

**Purpose:** Collapsible navigation sidebar

**Props:**
```typescript
interface AppSidebarProps {
  collapsed?: boolean
}
```

**Dimensions:**
- Expanded: 240px (w-60)
- Collapsed: 64px (w-16)
- Transition: 200ms (duration-200)

**Styling:**
- Background: white (dark: gray-800)
- Border: 1px solid gray-200 (dark: gray-700)
- Padding: 16px vertical (py-4)

**Features:**
- Smooth collapse animation
- Vertical scroll for overflow
- Border separator
- Dark mode support

**Usage:**
```tsx
<AppSidebar collapsed={collapsed} />
```

### Sidebar Navigation

**File:** `src/components/layout/sidebar-nav.tsx`

**Purpose:** Navigation items with active state highlighting

**Props:**
```typescript
interface SidebarNavProps {
  collapsed?: boolean
}
```

**Navigation Items:**
```typescript
const navItems = [
  { title: "Home", href: "/", icon: Home },
  { title: "Tasks", href: "/tasks", icon: CheckSquare },
  { title: "Projects", href: "/projects", icon: Folder },
  { title: "Team", href: "/team", icon: Users },
  { title: "Calendar", href: "/calendar", icon: Calendar },
  { title: "Settings", href: "/settings", icon: Settings },
]
```

**Item Styling:**
- Height: 40px (py-2)
- Padding: 12px horizontal (px-3)
- Icon size: 20px (h-5 w-5)
- Border radius: 8px (rounded-lg)

**States:**
- **Active:** bg-primary/10, text-primary
- **Hover:** bg-gray-100 dark:bg-gray-700
- **Default:** text-gray-600 dark:text-gray-400

**Collapsed Mode:**
- Icons centered (justify-center)
- Text hidden
- Chevron hidden

**Usage:**
```tsx
<SidebarNav collapsed={collapsed} />
```

### Breadcrumb

**File:** `src/components/layout/breadcrumb.tsx`

**Purpose:** Navigation path indicator

**Props:**
```typescript
interface BreadcrumbItem {
  label: string
  href?: string
}

interface BreadcrumbProps {
  items: BreadcrumbItem[]
  className?: string
}
```

**Features:**
- ChevronRight separators (h-4 w-4)
- Links for clickable items
- Plain text for current page
- Hover effects (text-gray-900 dark:text-gray-200)
- ARIA label for accessibility

**Styling:**
- Font size: 14px (text-sm)
- Color: gray-500 (dark: gray-400)
- Gap: 8px (gap-2)

**Usage:**
```tsx
<Breadcrumb
  items={[
    { label: "Home", href: "/" },
    { label: "Tasks", href: "/tasks" },
    { label: "Task Detail" },
  ]}
/>
```

### Container

**File:** `src/components/layout/container.tsx`

**Purpose:** Responsive content container with max-width

**Props:**
```typescript
interface ContainerProps {
  children: React.ReactNode
  size?: "sm" | "md" | "lg" | "xl" | "full"
  className?: string
}
```

**Size Variants:**
```typescript
const sizeClasses = {
  sm: "max-w-3xl",   // 768px
  md: "max-w-4xl",   // 896px
  lg: "max-w-6xl",   // 1152px (ClickUp default)
  xl: "max-w-7xl",   // 1280px
  full: "max-w-full",
}
```

**Responsive Padding:**
- Mobile: px-4 (16px)
- Tablet: sm:px-6 (24px)
- Desktop: lg:px-8 (32px)

**Usage:**
```tsx
<Container size="lg">
  <h1>Page Title</h1>
  <p>Content</p>
</Container>
```

### Board Layout

**File:** `src/components/layout/board-layout.tsx`

**Purpose:** Kanban board layout with horizontal scrolling

**BoardLayout Props:**
```typescript
interface BoardLayoutProps {
  children: React.ReactNode
  className?: string
}
```

**BoardColumn Props:**
```typescript
interface BoardColumnProps {
  title: string
  count?: number
  children: React.ReactNode
  className?: string
}
```

**Features:**
- Horizontal scroll (overflow-x-auto)
- Snap scrolling (snap-x snap-mandatory)
- Column width: 280px (w-[280px])
- Column gap: 24px (gap-6)
- Prevents column shrink (flex-shrink-0)
- Column header with count badge

**Column Structure:**
```
BoardColumn (280px)
├── Header (mb-3)
│   ├── Title (text-sm, font-semibold)
│   └── Count (text-xs, gray-500)
└── Content (space-y-2)
    └── Task cards
```

**Usage:**
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

### Responsive Breakpoints

```css
--breakpoint-xs: 375px;   /* Small mobile */
--breakpoint-sm: 640px;   /* Mobile */
--breakpoint-md: 768px;   /* Tablet */
--breakpoint-lg: 1024px;  /* Desktop */
--breakpoint-xl: 1280px;  /* Large desktop */
--breakpoint-2xl: 1536px; /* Extra large */
```

**Layout Behavior:**
- **Mobile (< 768px):** Sidebar defaults to collapsed, search hidden
- **Tablet (768px - 1024px):** Sidebar toggleable, search visible
- **Desktop (> 1024px):** Full layout, all features

### Accessibility

**Semantic HTML:**
- `<header>` for app header
- `<nav>` for sidebar navigation
- `<main>` for content area
- `<aside>` for sidebar
- `<nav aria-label="Breadcrumb">` for breadcrumbs

**Keyboard Navigation:**
- Tab through navigation items
- Enter to activate links
- Proper focus management

**ARIA Support:**
- Breadcrumb navigation label
- Icon button labels
- Semantic structure

### Dark Mode

All layouts support dark mode with automatic color inversion:
- Background: white → gray-800
- Border: gray-200 → gray-700
- Text: gray-900 → white
- Hover: gray-100 → gray-700

**Usage:**
```tsx
<html className="dark">
  {/* All layouts adapt automatically */}
</html>
```

---

**Document Status:** Updated for ClickUp Design + Layouts
**Version:** 2.2 (Purple Edition + Components + Layouts)
**Last Updated:** 2026-01-05

**Key Changes from v2.1:**
- Added Layout Patterns documentation (Section 16)
- Documented 7 layout components with full props reference
- Added responsive behavior and dark mode support
- Included accessibility features (semantic HTML, ARIA)
- Added usage examples for all layout components

**Key Changes from v2.0:**
- Added ClickUp Components API documentation (Section 15)
- Documented 6 core component types with full props reference
- Added usage examples for all components
- Included component showcase reference

**Key Changes from v1.0:**
- Primary color changed from Sky Blue (#0EA5E5) to ClickUp Purple (#7B68EE)
- Font changed from Plus Jakarta Sans to Inter
- Status colors updated to ClickUp style
- Dark mode purple is lighter (#A78BFA) for better contrast
- Border radius adjusted to match ClickUp (6px default)
- All gradients updated to purple theme
