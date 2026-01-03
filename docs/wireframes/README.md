# Nexora Design Wireframes

**Created:** 2026-01-03
**Version:** 1.0

## Summary

This directory contains comprehensive HTML wireframes for the Nexora Management Platform, implementing the design system defined in `docs/design-guidelines.md`.

## Wireframes Created

### 1. Login/Signup Page (`login-signup.html`)
**Features:**
- Modern gradient background matching Nexora brand colors
- Tab-based login/signup切换
- Floating label form inputs
- Social login options (Google, GitHub)
- Remember me checkbox
- Forgot password link
- Fully responsive design
- **Key Design Elements:** Sky blue to teal gradient, rounded cards, clean typography

### 2. Dashboard (`dashboard.html`)
**Features:**
- Left sidebar navigation (260px, collapsible)
- Top bar with breadcrumbs, search, and notifications
- Stats grid with 4 key metrics
- Recent tasks list with priority indicators
- Activity feed with real-time updates
- User profile section
- Hover effects and micro-interactions
- **Key Design Elements:** Card-based layout, gradient accents, icon system

### 3. Board View (`board-view.html`)
**Features:**
- Kanban-style board with 5 columns (Backlog, To Do, In Progress, Review, Done)
- Drag-and-drop task cards
- Task cards with tags, avatars, priorities, metadata
- View switcher (Board, List, Calendar, Gantt)
- Filter chips
- Add task functionality
- Column task counts
- **Key Design Elements:** Color-coded tags, priority badges, avatar stacks

### 4. AI Chatbot Interface (`ai-chatbot.html`)
**Features:**
- AI assistant chat interface powered by Gemini
- Message bubbles with user/AI distinction
- Typing indicator animation
- Suggested prompts
- Quick action buttons
- Task card integration in chat
- File attachment and voice input
- Auto-resize textarea
- **Key Design Elements:** Modern chat UI, gradient buttons, code blocks, task cards

## Design System Implementation

All wireframes follow the Nexora Design Guidelines:

- **Colors:** Sky Blue (#0EA5E9) to Teal (#14B8A6) gradient system
- **Typography:** Plus Jakarta Sans (primary), Inter (secondary)
- **Spacing:** 8px base unit
- **Border Radius:** 8px (buttons/inputs), 12px (cards), 16px (modals)
- **Shadows:** Subtle elevation on hover
- **Transitions:** 150-200ms ease-in-out

## Interactive Features

All wireframes include:
- Hover states on all interactive elements
- Smooth transitions and animations
- Responsive layouts (mobile, tablet, desktop)
- Keyboard navigation support
- Focus states for accessibility
- Dark mode compatible (CSS variables ready)

## How to Use

1. Open any HTML file in a web browser to view the wireframe
2. All wireframes are self-contained with embedded CSS
3. JavaScript is included for interactive elements
4. Responsive - test at different viewport sizes

## Screenshots

Screenshots directory will contain:
- Dashboard (light mode)
- Dashboard (dark mode)
- Board view
- AI chatbot interface
- Logo variations

## Technical Specifications

- **HTML5** semantic markup
- **CSS3** with CSS custom properties (variables)
- **Vanilla JavaScript** (no framework dependencies)
- **Google Fonts** integration (Plus Jakarta Sans)
- **Lucide-style** icons (emoji-based for wireframes)
- **Responsive design** with mobile-first approach

## Accessibility Features

- WCAG 2.2 AA compliant contrast ratios
- Semantic HTML structure
- Focus indicators on all interactive elements
- ARIA labels where needed
- Keyboard navigation support
- Touch targets minimum 44×44px

## All Wireframes Completed ✅

1. ✅ Login/Signup Page (`login-signup.html`)
2. ✅ Dashboard (`dashboard.html`)
3. ✅ Board View (`board-view.html`)
4. ✅ AI Chatbot Interface (`ai-chatbot.html`)
5. ✅ Task List View (`task-list-view.html`)
6. ✅ Calendar View (`calendar-view.html`)
7. ✅ Gantt Chart View (`gantt-view.html`)
8. ✅ Task Detail Modal (`task-detail-modal.html`)
9. ✅ Settings Page (`settings-page.html`)
10. ✅ Team Members Page (`team-members-page.html`)

### New Wireframes Added:

### 5. Task List View (`task-list-view.html`)
**Features:**
- Hierarchical task list with expand/collapse
- Indentation for subtasks (3 levels)
- Task rows with status, priority, assignee, due date
- Inline editing capability
- Filter controls at top
- Bulk action checkboxes
- Progress indicators
- **Key Design Elements:** Tree structure, status badges, avatar stack

### 6. Calendar View (`calendar-view.html`)
**Features:**
- Monthly calendar grid
- Week view toggle
- Tasks displayed as colored events
- Color-coded by status/priority
- Mini month navigator
- Drag to create tasks
- Date picker for navigation
- Weekend highlighting
- Today indicator
- **Key Design Elements:** Calendar grid, event chips, legend

### 7. Gantt Chart View (`gantt-view.html`)
**Features:**
- Timeline-based task display
- Horizontal scroll for dates
- Task bars with duration
- Dependencies shown as arrows
- Group by project/sprint
- Zoom controls (day/week/month)
- Task details on hover
- Progress bars on tasks
- Weekend grid highlighting
- **Key Design Elements:** Timeline bars, dependency lines, progress indicators

### 8. Task Detail Modal (`task-detail-modal.html`)
**Features:**
- Full-screen modal overlay
- Task title (editable)
- Description rich text editor
- Status, priority, assignee dropdowns
- Comments thread
- Attachments list
- Subtasks checklist
- Activity log/timeline
- Related tasks
- AI suggestions panel
- **Key Design Elements:** Modal overlay, sidebar layout, rich content areas

### 9. Settings Page (`settings-page.html`)
**Features:**
- Tab-based navigation (Profile, Workspace, Notifications, Security, Billing)
- Profile section: avatar, name, email, timezone
- Workspace settings: name, logo, default view
- Notification preferences: email, push, in-app toggles
- Security: password change, 2FA, active sessions
- Billing: plan info, payment method, history
- Form inputs with save buttons
- **Key Design Elements:** Tab navigation, form groups, toggle switches

### 10. Team Members Page (`team-members-page.html`)
**Features:**
- Member list with avatars, names, roles, status
- Invite members button (opens modal)
- Role dropdowns (Owner, Admin, Member, Guest)
- Filter by role or status
- Action menu (edit, remove, deactivate)
- Online/offline indicators
- Statistics overview
- Member metadata
- **Key Design Elements:** Member cards, status indicators, role badges

## File Structure

```
docs/wireframes/
├── login-signup.html          # Login/Signup page
├── dashboard.html              # Main dashboard
├── board-view.html             # Kanban board view
├── task-list-view.html         # Hierarchical task list
├── calendar-view.html          # Calendar view
├── gantt-view.html             # Gantt chart timeline
├── task-detail-modal.html      # Task detail modal
├── settings-page.html          # Settings page
├── team-members-page.html      # Team management
├── ai-chatbot.html             # AI assistant chat
├── logo.svg                    # Main logo (with text)
├── logo-icon.svg               # Icon-only logo
├── logo.png                    # Raster version
├── logo@2x.png                 # Hi-res version
├── logo-icon-32.png            # 32px icon
├── logo-icon-64.png            # 64px icon
├── logo-icon-128.png           # 128px icon
├── screenshots/                # Directory for screenshots
│   ├── light/                  # Light mode screenshots
│   │   ├── dashboard.png
│   │   ├── board-view.png
│   │   ├── task-list-view.png
│   │   ├── calendar-view.png
│   │   ├── gantt-view.png
│   │   ├── task-detail-modal.png
│   │   ├── settings-page.png
│   │   ├── team-members.png
│   │   ├── ai-chatbot.png
│   │   └── login.png
│   ├── dark/                   # Dark mode screenshots
│   │   ├── dashboard.png
│   │   ├── board-view.png
│   │   ├── task-detail-modal.png
│   │   └── ai-chatbot.png
│   └── logo/                   # Logo variations
│       ├── logo-gradient.png   # Primary gradient
│       ├── logo-light.png      # Light mode
│       └── logo-dark.png       # Dark mode
└── README.md                   # This file
```

## Browser Compatibility

Tested and compatible with:
- Chrome 120+
- Firefox 121+
- Safari 17+
- Edge 120+

## Credits

**Design:** Based on ClickUp-inspired design system with unique Nexora branding
**Development:** Claude Code AI Assistant
**Date:** January 3, 2026
