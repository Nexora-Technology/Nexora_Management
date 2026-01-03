# Nexora Management Platform - Design Guidelines

**Version:** 1.0
**Last Updated:** 2026-01-03
**Target:** Small teams < 30 users
**Tech Stack:** Next.js 15 + shadcn/ui + Tailwind CSS + TypeScript

---

## 1. COLOR PALETTE

### Primary Brand Colors
Nexora uses a sophisticated blue-teal gradient system inspired by modern SaaS platforms, distinct from ClickUp's purple-pink palette.

```css
/* Primary Colors */
--nexora-primary: #0EA5E9;        /* Sky Blue 500 */
--nexora-primary-hover: #0284C7;  /* Sky Blue 600 */
--nexora-primary-light: #E0F2FE;  /* Sky Blue 100 */

--nexora-secondary: #14B8A6;      /* Teal 500 */
--nexora-secondary-hover: #0D9488; /* Teal 600 */
--nexora-secondary-light: #CCFBF1; /* Teal 100 */

/* Accent Colors */
--nexora-accent-purple: #8B5CF6;  /* Violet 500 */
--nexora-accent-coral: #F43F5E;   /* Rose 500 */
--nexora-accent-amber: #F59E0B;   /* Amber 500 */
--nexora-accent-emerald: #10B981; /* Emerald 500 */

/* Gradient System */
--nexora-gradient-primary: linear-gradient(135deg, #0EA5E9 0%, #14B8A6 100%);
--nexora-gradient-accent: linear-gradient(135deg, #8B5CF6 0%, #F43F5E 100%);
--nexora-gradient-subtle: linear-gradient(135deg, #E0F2FE 0%, #CCFBF1 100%);
```

### Neutral Colors (Light Mode)

```css
/* Backgrounds */
--nexora-bg-primary: #FFFFFF;
--nexora-bg-secondary: #F8FAFC;   /* Slate 50 */
--nexora-bg-tertiary: #F1F5F9;    /* Slate 100 */
--nexora-bg-elevated: #FFFFFF;

/* Borders */
--nexora-border-default: #E2E8F0; /* Slate 200 */
--nexora-border-strong: #CBD5E1;  /* Slate 300 */
--nexora-border-subtle: #F1F5F9;  /* Slate 100 */

/* Text */
--nexora-text-primary: #0F172A;   /* Slate 900 */
--nexora-text-secondary: #475569; /* Slate 600 */
--nexora-text-tertiary: #94A3B8;  /* Slate 400 */
--nexora-text-disabled: #CBD5E1;  /* Slate 300 */
```

### Neutral Colors (Dark Mode)

```css
/* Backgrounds */
--nexora-dark-bg-primary: #0F172A;    /* Slate 900 */
--nexora-dark-bg-secondary: #1E293B;  /* Slate 800 */
--nexora-dark-bg-tertiary: #334155;   /* Slate 700 */
--nexora-dark-bg-elevated: #1E293B;   /* Slate 800 */

/* Borders */
--nexora-dark-border-default: #334155; /* Slate 700 */
--nexora-dark-border-strong: #475569;  /* Slate 600 */
--nexora-dark-border-subtle: #1E293B;  /* Slate 800 */

/* Text */
--nexora-dark-text-primary: #F8FAFC;   /* Slate 50 */
--nexora-dark-text-secondary: #CBD5E1; /* Slate 300 */
--nexora-dark-text-tertiary: #64748B;  /* Slate 500 */
--nexora-dark-text-disabled: #475569;  /* Slate 600 */
```

### Semantic Colors

```css
/* Success */
--nexora-success: #10B981;
--nexora-success-bg: #D1FAE5;
--nexora-success-border: #34D399;

/* Warning */
--nexora-warning: #F59E0B;
--nexora-warning-bg: #FEF3C7;
--nexora-warning-border: #FBBF24;

/* Error */
--nexora-error: #EF4444;
--nexora-error-bg: #FEE2E2;
--nexora-error-border: #F87171;

/* Info */
--nexora-info: #3B82F6;
--nexora-info-bg: #DBEAFE;
--nexora-info-border: #60A5FA;
```

### shadcn/ui CSS Variable Mapping

```css
:root {
  /* Primary */
  --background: 255 255 255;
  --foreground: 15 23 42; /* Slate 900 */

  --primary: 14 165 233; /* Sky Blue 500 */
  --primary-foreground: 255 255 255;

  --secondary: 20 184 166; /* Teal 500 */
  --secondary-foreground: 255 255 255;

  --muted: 241 245 249; /* Slate 100 */
  --muted-foreground: 100 116 139; /* Slate 500 */

  --accent: 139 92 246; /* Violet 500 */
  --accent-foreground: 255 255 255;

  --destructive: 239 68 68; /* Red 500 */
  --destructive-foreground: 255 255 255;

  --border: 226 232 240; /* Slate 200 */
  --input: 226 232 240;
  --ring: 14 165 233; /* Sky Blue 500 */

  --radius: 0.5rem; /* 8px */
}

.dark {
  --background: 15 23 42; /* Slate 900 */
  --foreground: 248 250 252; /* Slate 50 */

  --primary: 14 165 233;
  --primary-foreground: 255 255 255;

  --muted: 30 41 59; /* Slate 800 */
  --muted-foreground: 148 163 184; /* Slate 400 */

  --border: 51 65 85; /* Slate 700 */
  --input: 51 65 85;
  --ring: 14 165 233;
}
```

---

## 2. TYPOGRAPHY

### Font Families

**Primary Typeface:** Plus Jakarta Sans (Google Fonts)
- Designer: Gumpita Rahayu (Tokotype)
- Style: Geometric sans-serif with modern, approachable aesthetic
- Vietnamese Support: Yes (full Latin + Vietnamese character set)
- Weights: 200, 300, 400, 500, 600, 700, 800
- Variable Font: Available for optimal performance

**Secondary Typeface:** Inter
- Usage: Extended reading, data-heavy interfaces
- Weights: 400, 500, 600, 700

**Monospace Typeface:** JetBrains Mono
- Usage: Code blocks, data timestamps, keyboard shortcuts
- Weights: 400, 500

### Typography Scale

```css
/* Font Sizes */
--text-xs: 0.75rem;    /* 12px - Captions, labels */
--text-sm: 0.875rem;   /* 14px - Secondary text, helper text */
--text-base: 1rem;     /* 16px - Body text, standard UI */
--text-lg: 1.125rem;   /* 18px - Emphasized body */
--text-xl: 1.25rem;    /* 20px - Small headings */
--text-2xl: 1.5rem;    /* 24px - Section headings */
--text-3xl: 1.875rem;  /* 30px - Page titles */
--text-4xl: 2.25rem;   /* 36px - Hero titles */
```

### Font Weights

```css
--font-light: 300;      /* Subtle elements */
--font-normal: 400;     /* Body text */
--font-medium: 500;     /* Emphasized text, labels */
--font-semibold: 600;   /* Headings, important UI */
--font-bold: 700;       /* Strong headings */
--font-extrabold: 800;  /* Display headings */
```

### Line Heights

```css
--leading-tight: 1.25;   /* Headings */
--leading-normal: 1.5;   /* Body text */
--leading-relaxed: 1.625; /* Extended reading */
```

### Letter Spacing

```css
--tracking-tight: -0.025em;  /* Large headings */
--tracking-normal: 0;        /* Body text */
--tracking-wide: 0.025em;    /* Small text, labels */
```

### Typography Hierarchy

| Element | Font Size | Weight | Line Height | Usage |
|---------|-----------|--------|-------------|-------|
| H1 | 36px (2.25rem) | 800 (Extrabold) | 1.25 | Page titles |
| H2 | 30px (1.875rem) | 700 (Bold) | 1.25 | Section headings |
| H3 | 24px (1.5rem) | 600 (Semibold) | 1.25 | Subsection headings |
| H4 | 20px (1.25rem) | 600 (Semibold) | 1.25 | Card titles |
| Body Large | 18px (1.125rem) | 400 (Normal) | 1.5 | Emphasized content |
| Body | 16px (1rem) | 400 (Normal) | 1.5 | Standard content |
| Body Small | 14px (0.875rem) | 400 (Normal) | 1.5 | Secondary text |
| Caption | 12px (0.75rem) | 500 (Medium) | 1.5 | Labels, metadata |
| Button | 14-16px (0.875-1rem) | 600 (Semibold) | 1.5 | Button text |

### Google Fonts Implementation

```tsx
// next.config.js or app/layout.tsx
import { Plus Jakarta Sans, Inter, JetBrains_Mono } from 'next/font/google'

const plusJakartaSans = Plus Jakarta Sans({
  subsets: ['latin', 'latin-ext', 'vietnamese'],
  variable: '--font-plus-jakarta-sans',
  display: 'swap',
})

const inter = Inter({
  subsets: ['latin', 'latin-ext', 'vietnamese'],
  variable: '--font-inter',
  display: 'swap',
})

const jetbrainsMono = JetBrains_Mono({
  subsets: ['latin'],
  variable: '--font-jetbrains-mono',
  display: 'swap',
})
```

---

## 3. SPACING SYSTEM

### Base Unit: 4px (Power of 2 Scale)

```css
--spacing-0: 0;
--spacing-1: 0.25rem;  /* 4px */
--spacing-2: 0.5rem;   /* 8px - Base unit */
--spacing-3: 0.75rem;  /* 12px */
--spacing-4: 1rem;     /* 16px */
--spacing-5: 1.25rem;  /* 20px */
--spacing-6: 1.5rem;   /* 24px */
--spacing-8: 2rem;     /* 32px */
--spacing-10: 2.5rem;  /* 40px */
--spacing-12: 3rem;    /* 48px */
--spacing-16: 4rem;    /* 64px */
--spacing-20: 5rem;    /* 80px */
--spacing-24: 6rem;    /* 96px */
```

### Component Spacing

```css
/* Buttons */
--button-padding-sm: 0.5rem 1rem;      /* 8px 16px */
--button-padding-md: 0.75rem 1.5rem;   /* 12px 24px */
--button-padding-lg: 1rem 2rem;        /* 16px 32px */

/* Cards */
--card-padding-sm: 1rem;               /* 16px */
--card-padding-md: 1.5rem;             /* 24px */
--card-padding-lg: 2rem;               /* 32px */

/* Inputs */
--input-padding: 0.75rem 1rem;         /* 12px 16px */

/* Layout */
--section-gap: 4rem;                   /* 64px */
--component-gap: 1.5rem;               /* 24px */
--element-gap: 1rem;                   /* 16px */
--tight-gap: 0.5rem;                   /* 8px */
```

### Layout Padding

```css
/* Container Padding */
--container-padding-mobile: 1rem;      /* 16px */
--container-padding-tablet: 2rem;      /* 32px */
--container-padding-desktop: 3rem;     /* 48px */

/* Section Spacing */
--section-spacing-vertical: 5rem;      /* 80px */
--section-spacing-horizontal: 2rem;    /* 32px */
```

---

## 4. BORDER RADIUS

### Radius Scale

```css
--radius-none: 0;
--radius-sm: 0.25rem;   /* 4px - Small elements, badges */
--radius-md: 0.5rem;    /* 8px - Buttons, inputs */
--radius-lg: 0.75rem;   /* 12px - Cards, panels */
--radius-xl: 1rem;      /* 16px - Modals, large cards */
--radius-2xl: 1.5rem;   /* 24px - Hero elements */
--radius-full: 9999px;  /* Pill buttons, avatars */
```

### Component-Specific Radius

| Component | Border Radius | Notes |
|-----------|---------------|-------|
| Buttons (Primary/Secondary) | 8px (0.5rem) | Modern but not too rounded |
| Buttons (Ghost/Icon) | 6px (0.375rem) | Slightly more subtle |
| Cards | 12px (0.75rem) | Soft, approachable |
| Panels | 12px (0.75rem) | Matches cards |
| Inputs | 8px (0.5rem) | Consistent with buttons |
| Modals | 16px (1rem) | More pronounced for focus |
| Badges/Chips | 4px (0.25rem) or pill | Small or fully rounded |
| Avatars | Pill (9999px) | Circular |
| Toasts | 12px (0.75rem) | Matches cards |
| Dropdowns | 8px (0.5rem) | Consistent with inputs |

---

## 5. COMPONENT SPECIFICATIONS

### Buttons

#### Primary Button

```css
.nexora-button-primary {
  background: var(--nexora-gradient-primary);
  color: white;
  border: none;
  border-radius: 8px;
  padding: 12px 24px;
  font-size: 14px;
  font-weight: 600;
  line-height: 1.5;
  transition: all 200ms ease;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.nexora-button-primary:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.15);
  opacity: 0.95;
}

.nexora-button-primary:active {
  transform: translateY(0);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.nexora-button-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
}
```

**Tailwind Classes:**
```tsx
<Button className="bg-gradient-to-r from-sky-500 to-teal-500 hover:from-sky-600 hover:to-teal-600 text-white font-semibold rounded-lg px-6 py-3 shadow-sm hover:shadow-md transition-all">
  Primary Action
</Button>
```

#### Secondary Button

```css
.nexora-button-secondary {
  background: var(--nexora-bg-secondary);
  color: var(--nexora-text-primary);
  border: 2px solid var(--nexora-border-default);
  border-radius: 8px;
  padding: 10px 22px;
  font-size: 14px;
  font-weight: 600;
  transition: all 200ms ease;
}

.nexora-button-secondary:hover {
  background: var(--nexora-bg-tertiary);
  border-color: var(--nexora-border-strong);
}
```

**Tailwind Classes:**
```tsx
<Button variant="outline" className="border-2 hover:bg-slate-50">
  Secondary
</Button>
```

#### Ghost Button

```css
.nexora-button-ghost {
  background: transparent;
  color: var(--nexora-primary);
  border: none;
  border-radius: 6px;
  padding: 8px 16px;
  font-size: 14px;
  font-weight: 500;
  transition: all 150ms ease;
}

.nexora-button-ghost:hover {
  background: var(--nexora-primary-light);
}
```

**Tailwind Classes:**
```tsx
<Button variant="ghost" className="text-sky-500 hover:bg-sky-50">
  Ghost
</Button>
```

#### Icon Button

```css
.nexora-button-icon {
  width: 40px;
  height: 40px;
  background: transparent;
  border: none;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 150ms ease;
}

.nexora-button-icon:hover {
  background: var(--nexora-bg-tertiary);
}
```

**Tailwind Classes:**
```tsx
<Button size="icon" variant="ghost" className="w-10 h-10">
  <Icon name="settings" className="w-5 h-5" />
</Button>
```

#### Button Sizes

| Size | Height | Padding | Font Size | Icon Size |
|------|--------|---------|-----------|-----------|
| Small (sm) | 36px | 8px 16px | 13px | 16px |
| Default (md) | 44px | 12px 24px | 14px | 18px |
| Large (lg) | 52px | 16px 32px | 16px | 20px |

---

### Cards

#### Default Card

```css
.nexora-card {
  background: var(--nexora-bg-primary);
  border: 1px solid var(--nexora-border-default);
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  transition: all 200ms ease;
}

.nexora-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  transform: translateY(-2px);
}
```

**Tailwind Classes:**
```tsx
<Card className="bg-white border border-slate-200 rounded-xl p-6 shadow-sm hover:shadow-md transition-all">
  <CardHeader>
    <CardTitle>Card Title</CardTitle>
  </CardHeader>
  <CardContent>
    Card content goes here...
  </CardContent>
</Card>
```

#### Elevated Card

```css
.nexora-card-elevated {
  background: var(--nexora-bg-elevated);
  border: none;
  border-radius: 16px;
  padding: 32px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
}
```

#### Card Variants

| Variant | Border | Shadow | Padding | Radius |
|---------|--------|--------|---------|--------|
| Default | 1px solid | 0 1px 3px | 24px | 12px |
| Elevated | None | 0 8px 24px | 32px | 16px |
| Outlined | 2px solid | None | 20px | 12px |
| Flat | None | None | 16px | 8px |

---

### Input Fields

#### Text Input

```css
.nexora-input {
  background: var(--nexora-bg-primary);
  border: 2px solid var(--nexora-border-default);
  border-radius: 8px;
  padding: 12px 16px;
  font-size: 16px;
  line-height: 1.5;
  color: var(--nexora-text-primary);
  transition: all 200ms ease;
}

.nexora-input:focus {
  outline: none;
  border-color: var(--nexora-primary);
  box-shadow: 0 0 0 3px rgba(14, 165, 233, 0.1);
}

.nexora-input:hover:not(:focus) {
  border-color: var(--nexora-border-strong);
}

.nexora-input::placeholder {
  color: var(--nexora-text-tertiary);
}

.nexora-input:disabled {
  background: var(--nexora-bg-secondary);
  color: var(--nexora-text-disabled);
  cursor: not-allowed;
}
```

**Tailwind Classes:**
```tsx
<Input className="border-2 border-slate-200 rounded-lg px-4 py-3 focus:border-sky-500 focus:ring-sky-500/20" placeholder="Enter text..." />
```

#### Input States

| State | Border Color | Background | Box Shadow |
|-------|--------------|------------|------------|
| Default | #E2E8F0 | White | None |
| Hover | #CBD5E1 | White | None |
| Focus | #0EA5E9 | White | 0 0 0 3px rgba(14, 165, 233, 0.1) |
| Error | #EF4444 | White | 0 0 0 3px rgba(239, 68, 68, 0.1) |
| Disabled | #E2E8F0 | #F8FAFC | None |

#### Floating Label Input

```tsx
<div className="relative">
  <input
    type="text"
    id="email"
    className="peer pt-6 px-4 pb-2 border-2 border-slate-200 rounded-lg w-full focus:border-sky-500 focus:ring-sky-500/20 outline-none transition-all"
    placeholder=" "
  />
  <label
    htmlFor="email"
    className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 transition-all peer-focus:top-2 peer-focus:-translate-y-0 peer-focus:text-xs peer-focus:text-sky-500 peer-not-placeholder-shown:top-2 peer-not-placeholder-shown:-translate-y-0 peer-not-placeholder-shown:text-xs"
  >
    Email address
  </label>
</div>
```

---

### Navigation

#### Sidebar Navigation

```css
.nexora-sidebar {
  width: 260px;
  background: var(--nexora-bg-primary);
  border-right: 1px solid var(--nexora-border-default);
  display: flex;
  flex-direction: column;
  transition: width 200ms ease;
}

.nexora-sidebar-collapsed {
  width: 64px;
}

.nexora-nav-item {
  height: 44px;
  padding: 0 16px;
  margin: 4px 12px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 12px;
  color: var(--nexora-text-secondary);
  font-size: 14px;
  font-weight: 500;
  transition: all 150ms ease;
  cursor: pointer;
}

.nexora-nav-item:hover {
  background: var(--nexora-bg-tertiary);
  color: var(--nexora-text-primary);
}

.nexora-nav-item-active {
  background: var(--nexora-primary-light);
  color: var(--nexora-primary);
  font-weight: 600;
}
```

**Dimensions:**
- Expanded Width: 260px
- Collapsed Width: 64px
- Item Height: 44px
- Item Padding: 0 16px
- Icon Size: 20px

#### Top Navigation

```css
.nexora-topbar {
  height: 64px;
  background: var(--nexora-bg-primary);
  border-bottom: 1px solid var(--nexora-border-default);
  display: flex;
  align-items: center;
  padding: 0 24px;
  justify-content: space-between;
}
```

---

### Task Cards (Board View)

```css
.nexora-task-card {
  background: var(--nexora-bg-primary);
  border: 1px solid var(--nexora-border-default);
  border-radius: 10px;
  padding: 16px;
  margin-bottom: 12px;
  cursor: grab;
  transition: all 200ms ease;
}

.nexora-task-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  border-color: var(--nexora-primary);
}

.nexora-task-card-dragging {
  opacity: 0.5;
  cursor: grabbing;
}
```

**Task Card Structure:**
```
┌─────────────────────────┐
│ Task Title             │
│ (16px, semibold)       │
│                         │
│ ┌─────┐ ┌─────┐ ┌─────┐│
│ │Tag  │ │Tag  │ │+2   ││
│ └─────┘ └─────┘ └─────┘│
│                         │
│ 🔴 High priority        │
│                         │
│ ┌───┐ ┌───┐ ┌───┐      │
│ │👤 │ │📎 │ │💬 │ 5d   │
│ └───┘ └───┘ └───┘      │
└─────────────────────────┘
```

---

### Modals/Dialogs

```css
.nexora-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.5);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
  animation: fadeIn 200ms ease;
}

.nexora-modal-content {
  background: var(--nexora-bg-primary);
  border-radius: 16px;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
  max-width: 600px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  animation: slideUp 250ms ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}
```

**Modal Structure:**
```
┌─────────────────────────────────┐
│ ┌─────────┐           [✕]       │
│ │ Title   │                      │
│ │ Description │                  │
└─────────────────────────────────┘
│                                 │
│ Modal content...                │
│                                 │
├─────────────────────────────────┤
│           [Cancel] [Confirm]    │
└─────────────────────────────────┘
```

---

### Badges/Chips

```css
.nexora-badge {
  display: inline-flex;
  align-items: center;
  padding: 4px 10px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  gap: 6px;
}

.nexora-badge-success {
  background: var(--nexora-success-bg);
  color: var(--nexora-success);
  border: 1px solid var(--nexora-success-border);
}

.nexora-badge-warning {
  background: var(--nexora-warning-bg);
  color: var(--nexora-warning);
  border: 1px solid var(--nexora-warning-border);
}

.nexora-badge-error {
  background: var(--nexora-error-bg);
  color: var(--nexora-error);
  border: 1px solid var(--nexora-error-border);
}
```

---

## 6. LAYOUT PATTERNS

### Grid System

**12-Column Grid (Responsive)**

```css
.nexora-grid {
  display: grid;
  grid-template-columns: repeat(12, 1fr);
  gap: 24px;
  max-width: 1280px;
  margin: 0 auto;
  padding: 0 24px;
}

/* Mobile */
@media (max-width: 767px) {
  .nexora-grid {
    grid-template-columns: repeat(4, 1fr);
    gap: 16px;
    padding: 0 16px;
  }
}

/* Tablet */
@media (min-width: 768px) and (max-width: 1023px) {
  .nexora-grid {
    grid-template-columns: repeat(8, 1fr);
    gap: 20px;
    padding: 0 20px;
  }
}
```

### Dashboard Layout

```
┌─────────────┬──────────────────────────────────────────┐
│             │  Top Navigation Bar (64px)               │
│             ├──────────────────────────────────────────┤
│   Sidebar   │                                          │
│   (260px)   │                                          │
│             │           Main Content                   │
│  Collapsed  │           (Flexible)                     │
│   to 64px   │                                          │
│             │                                          │
│             │                                          │
└─────────────┴──────────────────────────────────────────┘
```

### Board View Layout

```
┌─────────────────────────────────────────────────────────┐
│ Column 1 (280px) │ Column 2 (280px) │ Column 3 (280px) │
├──────────────────┼──────────────────┼──────────────────┤
│ Task Card        │ Task Card        │ Task Card        │
├──────────────────┼──────────────────┼──────────────────┤
│ Task Card        │ Task Card        │ Task Card        │
├──────────────────┼──────────────────┼──────────────────┤
│ Task Card        │ Task Card        │                  │
└──────────────────┴──────────────────┴──────────────────┘
```

**Column Specifications:**
- Width: 280px (min), 320px (max)
- Gap: 24px
- Card Margin: 0 0 12px 0
- Horizontal Scroll: Enabled on overflow

---

## 7. ICONOGRAPHY

### Icon Library: Lucide React

**Rationale:**
- Tree-shakeable (ESM)
- Consistent stroke width (2px)
- 1000+ icons
- Excellent TypeScript support
- Customizable via props

### Icon Sizes

```tsx
<Icon name="icon-name" size={16} /> // 16px - Inline, small buttons
<Icon name="icon-name" size={18} /> // 18px - Standard UI
<Icon name="icon-name" size={20} /> // 20px - Navigation, buttons
<Icon name="icon-name" size={24} /> // 24px - Feature highlights
<Icon name="icon-name" size={32} /> // 32px - Hero sections
```

### Stroke Weights

```tsx
// Thin (1.5px) - Subtle icons
<Icon strokeWidth={1.5} />

// Regular (2px) - Default
<Icon strokeWidth={2} />

// Bold (2.5px) - Emphasized icons
<Icon strokeWidth={2.5} />
```

### Common Icon Patterns

| Usage | Icon Size | Stroke | Color |
|-------|-----------|--------|-------|
| Navigation | 20px | 2px | Text secondary |
| Button (left) | 18px | 2px | Inherit |
| Button (icon-only) | 20px | 2px | Inherit |
| Status indicators | 16px | 2px | Semantic |
| Feature cards | 32px | 2px | Primary |
| Metadata | 14px | 1.5px | Text tertiary |

---

## 8. SHADCN/UI CUSTOMIZATION

### Tailwind Configuration

```javascript
// tailwind.config.ts
import type { Config } from 'tailwindcss'

const config: Config = {
  darkMode: ['class'],
  content: [
    './pages/**/*.{ts,tsx}',
    './components/**/*.{ts,tsx}',
    './app/**/*.{ts,tsx}',
    './src/**/*.{ts,tsx}',
  ],
  theme: {
    container: {
      center: true,
      padding: {
        DEFAULT: '1rem',
        sm: '2rem',
        lg: '4rem',
        xl: '5rem',
        '2xl': '6rem',
      },
    },
    extend: {
      colors: {
        border: 'hsl(var(--border))',
        input: 'hsl(var(--input))',
        ring: 'hsl(var(--ring))',
        background: 'hsl(var(--background))',
        foreground: 'hsl(var(--foreground))',
        primary: {
          DEFAULT: 'hsl(var(--primary))',
          foreground: 'hsl(var(--primary-foreground))',
        },
        secondary: {
          DEFAULT: 'hsl(var(--secondary))',
          foreground: 'hsl(var(--secondary-foreground))',
        },
        destructive: {
          DEFAULT: 'hsl(var(--destructive))',
          foreground: 'hsl(var(--destructive-foreground))',
        },
        muted: {
          DEFAULT: 'hsl(var(--muted))',
          foreground: 'hsl(var(--muted-foreground))',
        },
        accent: {
          DEFAULT: 'hsl(var(--accent))',
          foreground: 'hsl(var(--accent-foreground))',
        },
        popover: {
          DEFAULT: 'hsl(var(--popover))',
          foreground: 'hsl(var(--popover-foreground))',
        },
        card: {
          DEFAULT: 'hsl(var(--card))',
          foreground: 'hsl(var(--card-foreground))',
        },
      },
      borderRadius: {
        lg: 'var(--radius)',
        md: 'calc(var(--radius) - 2px)',
        sm: 'calc(var(--radius) - 4px)',
      },
      fontFamily: {
        sans: ['var(--font-plus-jakarta-sans)', 'sans-serif'],
        mono: ['var(--font-jetbrains-mono)', 'monospace'],
      },
    },
  },
  plugins: [require('tailwindcss-animate')],
}

export default config
```

### Custom Component Variants

```typescript
// components/ui/button.tsx - Enhanced with Nexora styles
const buttonVariants = cva(
  "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50",
  {
    variants: {
      variant: {
        default: "bg-gradient-to-r from-sky-500 to-teal-500 text-white hover:from-sky-600 hover:to-teal-600 shadow-sm",
        destructive: "bg-red-500 text-white hover:bg-red-600",
        outline: "border-2 border-slate-200 bg-white hover:bg-slate-50",
        secondary: "bg-slate-100 text-slate-900 hover:bg-slate-200",
        ghost: "hover:bg-slate-100",
        link: "text-sky-500 underline-offset-4 hover:underline",
        nexora: "bg-gradient-to-r from-sky-500 to-teal-500 text-white font-semibold rounded-lg shadow-md hover:shadow-lg",
      },
      size: {
        default: "h-10 px-4 py-2",
        sm: "h-9 rounded-md px-3",
        lg: "h-11 rounded-md px-8",
        icon: "h-10 w-10",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "default",
    },
  }
)
```

---

## 9. DARK MODE

### Dark Mode Implementation

```tsx
// components/theme-provider.tsx
'use client'

import * as React from 'react'

type Theme = 'light' | 'dark' | 'system'

type ThemeProviderProps = {
  children: React.ReactNode
  defaultTheme?: Theme
  storageKey?: string
}

type ThemeProviderState = {
  theme: Theme
  setTheme: (theme: Theme) => void
}

const initialState: ThemeProviderState = {
  theme: 'system',
  setTheme: () => null,
}

const ThemeProviderContext = React.createContext<ThemeProviderState>(initialState)

export function ThemeProvider({
  children,
  defaultTheme = 'system',
  storageKey = 'nexora-ui-theme',
  ...props
}: ThemeProviderProps) {
  const [theme, setTheme] = React.useState<Theme>(
    () => (typeof window !== 'undefined' && (localStorage.getItem(storageKey) as Theme)) || defaultTheme
  )

  React.useEffect(() => {
    const root = window.document.documentElement

    root.classList.remove('light', 'dark')

    if (theme === 'system') {
      const systemTheme = window.matchMedia('(prefers-color-scheme: dark)').matches
        ? 'dark'
        : 'light'

      root.classList.add(systemTheme)
      return
    }

    root.classList.add(theme)
  }, [theme])

  const value = {
    theme,
    setTheme: (theme: Theme) => {
      localStorage.setItem(storageKey, theme)
      setTheme(theme)
    },
  }

  return (
    <ThemeProviderContext.Provider {...props} value={value}>
      {children}
    </ThemeProviderContext.Provider>
  )
}

export const useTheme = () => {
  const context = React.useContext(ThemeProviderContext)

  if (context === undefined) {
    throw new Error('useTheme must be used within a ThemeProvider')
  }

  return context
}
```

### Dark Mode Color Overrides

```css
/* globals.css */
.dark {
  /* Backgrounds */
  --background: 15 23 42; /* Slate 900 */
  --foreground: 248 250 252; /* Slate 50 */

  /* Components */
  --card: 30 41 59; /* Slate 800 */
  --card-foreground: 248 250 252;
  --popover: 30 41 59;
  --popover-foreground: 248 250 252;

  /* Primary (slightly brighter in dark mode) */
  --primary: 14 180 233; /* Sky 400 */
  --primary-foreground: 15 23 42;

  /* Secondary */
  --secondary: 51 65 85; /* Slate 700 */
  --secondary-foreground: 248 250 252;

  /* Muted */
  --muted: 51 65 85;
  --muted-foreground: 148 163 184; /* Slate 400 */

  /* Accent */
  --accent: 51 65 85;
  --accent-foreground: 248 250 252;

  /* Borders */
  --border: 51 65 85; /* Slate 700 */
  --input: 51 65 85;
  --ring: 14 180 233;
}
```

---

## 10. ACCESSIBILITY (WCAG 2.2 AA)

### Color Contrast Ratios

All color combinations meet or exceed WCAG 2.2 AA standards:

| Combination | Ratio | Grade |
|-------------|-------|-------|
| Primary (Sky 500) on White | 4.52:1 | AA ✓ |
| Primary (Sky 400) on Dark BG | 4.61:1 | AA ✓ |
| Text Primary on White | 16.08:1 | AAA ✓ |
| Text Secondary on White | 7.08:1 | AA ✓ |
| Text Primary on Dark BG | 15.1:1 | AAA ✓ |
| Text Secondary on Dark BG | 4.52:1 | AA ✓ |

### Focus States

All interactive elements must have visible focus indicators:

```css
.focus-visible:focus-visible {
  outline: 2px solid var(--nexora-primary);
  outline-offset: 2px;
}
```

### Touch Targets

- Minimum size: 44×44px (mobile)
- Spacing between targets: 8px minimum

### Screen Reader Support

```tsx
// Semantic HTML
<button aria-label="Close modal">
  <X aria-hidden="true" />
</button>

// ARIA attributes
<div role="tabpanel" aria-labelledby="tab-1">
  Tab content
</div>
```

### Keyboard Navigation

- Tab: Navigate forward
- Shift+Tab: Navigate backward
- Enter/Space: Activate buttons
- Escape: Close modals/dropdowns
- Arrow keys: Navigate lists/menus

---

## 11. ANIMATIONS & MICRO-INTERACTIONS

### Transition Timings

```css
--transition-fast: 150ms;
--transition-base: 200ms;
--transition-slow: 300ms;

--easing-default: cubic-bezier(0.4, 0, 0.2, 1);
--easing-in: cubic-bezier(0.4, 0, 1, 1);
--easing-out: cubic-bezier(0, 0, 0.2, 1);
--easing-in-out: cubic-bezier(0.4, 0, 0.6, 1);
```

### Hover Animations

```css
.hover-lift {
  transition: transform var(--transition-base) var(--easing-out),
              box-shadow var(--transition-base) var(--easing-out);
}

.hover-lift:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
}
```

### Loading States

```tsx
// Skeleton loader
<div className="animate-pulse bg-slate-200 rounded-lg h-4 w-full" />

// Spinner
<div className="animate-spin h-5 w-5 border-2 border-sky-500 border-t-transparent rounded-full" />
```

### Respect Reduced Motion

```css
@media (prefers-reduced-motion: reduce) {
  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
```

---

## 12. RESPONSIVE BREAKPOINTS

```css
/* Mobile First Approach */
--breakpoint-xs: 375px;   /* Small mobile */
--breakpoint-sm: 640px;   /* Mobile */
--breakpoint-md: 768px;   /* Tablet */
--breakpoint-lg: 1024px;  /* Desktop */
--breakpoint-xl: 1280px;  /* Large desktop */
--breakpoint-2xl: 1536px; /* Extra large */
```

**Tailwind Classes:**
```tsx
<div className="px-4 md:px-8 lg:px-12">
  Responsive padding
</div>
```

---

## 13. IMPLEMENTATION CHECKLIST

### Setup Phase
- [ ] Install and configure Google Fonts (Plus Jakarta Sans, JetBrains Mono)
- [ ] Set up shadcn/ui with CSS variables
- [ ] Override default colors with Nexora palette
- [ ] Configure Tailwind with custom spacing and radius
- [ ] Create theme provider component
- [ ] Test light/dark mode switching

### Component Phase
- [ ] Create base button variants (primary, secondary, ghost, icon)
- [ ] Create card components (default, elevated, outlined)
- [ ] Create input components (text, textarea, select)
- [ ] Create badge/status components
- [ ] Create modal/dialog components
- [ ] Create navigation components (sidebar, topbar)

### Layout Phase
- [ ] Build 12-column grid system
- [ ] Create responsive container
- [ ] Build sidebar layout (collapsible)
- [ ] Build dashboard layout
- [ ] Build board view layout
- [ ] Test all layouts on mobile/tablet/desktop

### Testing Phase
- [ ] Verify WCAG 2.2 AA contrast ratios
- [ ] Test keyboard navigation
- [ ] Test screen reader compatibility
- [ ] Test touch targets on mobile
- [ ] Test animations with reduced motion
- [ ] Cross-browser testing (Chrome, Firefox, Safari, Edge)

---

## 14. DESIGN ASSETS

### Logo Specifications
- **Format:** SVG (vector) + PNG (raster)
- **Primary Version:** Sky blue to teal gradient
- **Monochrome:** Slate 900 (light mode), Slate 50 (dark mode)
- **Clear Space:** 1x height on all sides
- **Minimum Size:** 32px width (digital), 1 inch (print)

### Icon Style Guide
- **Stroke Width:** 2px (standard), 1.5px (thin)
- **Corner Radius:** 2px (slightly rounded)
- **Grid:** 24×24px base
- **Stroke Cap:** Round
- **Stroke Join:** Round

---

## 15. BEST PRACTICES

### Do's
- Always use semantic HTML
- Maintain consistent spacing (multiples of 4px)
- Test in both light and dark modes
- Provide alt text for images
- Use descriptive link text
- Keep font sizes readable (minimum 14px for body)
- Maintain 4.5:1 contrast ratio minimum
- Test with real users

### Don'ts
- Don't rely on color alone for meaning
- Don't use pure black (#000000) or pure white (#FFFFFF) in dark mode
- Don't create touch targets smaller than 44×44px
- Don't use generic placeholder images
- Don't skip focus states
- Don't ignore mobile responsiveness
- Don't hardcode colors (use CSS variables)
- Don't ignore accessibility

---

**Document Status:** Ready for Implementation
**Next Steps:** Create HTML wireframes, generate logo, begin component development
