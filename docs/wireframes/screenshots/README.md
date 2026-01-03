# Screenshot Guide

This directory contains screenshots of all Nexora wireframes in both light and dark modes.

## 📸 How to Take Screenshots

### Method 1: Browser DevTools (Recommended)
1. Open the wireframe HTML file in Chrome/Edge
2. Open DevTools: F12 or Cmd+Option+I (Mac) / Ctrl+Shift+I (Windows)
3. Press Cmd+Shift+P (Mac) or Ctrl+Shift+P (Windows)
4. Type "screenshot" and select "Capture node screenshot"
5. Click on the main content area to capture
6. Save to appropriate subdirectory

### Method 2: Operating System Tools

**macOS:**
- Full screen: Cmd + Shift + 3
- Selection: Cmd + Shift + 4
- Browser window: Cmd + Shift + 4, then Space

**Windows:**
- Snipping Tool: Win + Shift + S
- Full screen: PrtScn (Print Screen)
- Browser window: Alt + PrtScn

**Linux:**
- GNOME: PrtScn or Shift + PrtScn
- Custom: Use `gnome-screenshot` or ` spectacle`

### Method 3: Online Screenshot Tools
- https://www.screely.com/ - Clean browser screenshots
- https://screenshot.guru/ - Quick online screenshots
- https://www.site-shot.com/ - Full page screenshots

## 🎨 Recommended Screenshot Settings

### Desktop Screenshots (Light Mode)
- **Size:** 1440×900px or 1920×1080px
- **Browser:** Chrome or Firefox
- **Zoom:** 100%
- **Files to capture:**
  1. dashboard.html
  2. board-view.html
  3. task-list-view.html
  4. calendar-view.html
  5. gantt-view.html
  6. task-detail-modal.html
  7. settings-page.html
  8. team-members-page.html
  9. ai-chatbot.html
  10. login-signup.html

### Dark Mode Screenshots
**Note:** Current wireframes use light mode by default. To capture dark mode:

1. Add dark mode CSS to the `<style>` section:
```css
:root {
  --bg-primary: #0F172A;
  --bg-secondary: #1E293B;
  --bg-tertiary: #334155;
  --border-default: #334155;
  --text-primary: #F8FAFC;
  --text-secondary: #CBD5E1;
  --text-tertiary: #64748B;
}
```

2. Capture these wireframes in dark mode:
   - dashboard.html
   - board-view.html
   - task-detail-modal.html
   - ai-chatbot.html

### Logo Screenshots
1. Open any wireframe and capture the logo from the sidebar
2. Use a transparent background
3. Save in multiple sizes:
   - 32×32px (favicon)
   - 64×64px (small icon)
   - 128×128px (medium icon)
   - 256×256px (large icon)

## 📁 File Naming Convention

```
screenshots/
├── light/
│   ├── dashboard.png
│   ├── board-view.png
│   ├── task-list-view.png
│   ├── calendar-view.png
│   ├── gantt-view.png
│   ├── task-detail-modal.png
│   ├── settings-page.png
│   ├── team-members.png
│   ├── ai-chatbot.png
│   └── login.png
├── dark/
│   ├── dashboard.png
│   ├── board-view.png
│   ├── task-detail-modal.png
│   └── ai-chatbot.png
└── logo/
    ├── logo-32.png
    ├── logo-64.png
    ├── logo-128.png
    └── logo-256.png
```

## ✅ Quality Checklist

Before saving screenshots, ensure:
- [ ] Full page is visible (no horizontal scroll)
- [ ] No DevTools are visible
- [ ] No mouse cursor in the screenshot
- [ ] All images/icons are loaded
- [ ] Resolution is at least 1440px wide
- [ ] File size is reasonable (< 500KB)
- [ ] PNG format for quality
- [ ] Descriptive filename

## 🚀 Quick Start

To screenshot all wireframes quickly:

1. **Light Mode:**
   ```bash
   # Open each file in Chrome at 1440×900 and take screenshots
   open -a "Google Chrome" --args --window-size=1440,900 dashboard.html
   ```

2. **Dark Mode:**
   - Add dark mode CSS variables to each file
   - Refresh and capture
   - Revert changes

3. **Batch Script (macOS):**
   ```bash
   for file in *.html; do
     open -a "Google Chrome" "$file"
     echo "Take screenshot of $file, then press Enter"
     read
   done
   ```

## 📊 Current Status

- [ ] Light mode screenshots (10/10)
- [ ] Dark mode screenshots (0/4)
- [ ] Logo variations (0/4)

**Last Updated:** 2026-01-03
