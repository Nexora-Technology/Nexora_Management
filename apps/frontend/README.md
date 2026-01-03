# Nexora Management Platform - Frontend

> Modern team management and task coordination platform built with Next.js 15

## Tech Stack

- **Framework**: Next.js 15 (App Router)
- **React Version**: React 19
- **Language**: TypeScript
- **Styling**: Tailwind CSS + shadcn/ui
- **State Management**: Zustand
- **Data Fetching**: TanStack React Query
- **Real-time**: SignalR
- **Forms**: React Hook Form + Zod
- **HTTP Client**: Axios
- **Fonts**: Plus Jakarta Sans (primary), JetBrains Mono (code)

## Getting Started

### Prerequisites

- Node.js 18+ installed
- Backend API running on `http://localhost:5000` (see `../backend/README.md`)

### Installation

```bash
# Install dependencies
npm install
```

### Environment Variables

Create a `.env.local` file in the root directory:

```env
# API Configuration
NEXT_PUBLIC_API_URL=http://localhost:5000

# App Configuration
NEXT_PUBLIC_APP_NAME=Nexora Management Platform
NEXT_PUBLIC_APP_URL=http://localhost:3000
```

### Development

```bash
# Start development server (runs on port 3000)
npm run dev
```

Visit [http://localhost:3000](http://localhost:3000) to view the application.

### Build

```bash
# Create production build
npm run build

# Start production server
npm start
```

## Available Scripts

| Script | Description |
|--------|-------------|
| `npm run dev` | Start development server with Turbopack |
| `npm run build` | Create optimized production build |
| `npm start` | Start production server |
| `npm run lint` | Run ESLint for code linting |
| `npm run typecheck` | Run TypeScript type checking |

## Project Structure

```
apps/frontend/
├── src/
│   ├── app/                    # Next.js App Router pages
│   │   ├── layout.tsx         # Root layout with providers
│   │   ├── page.tsx           # Home page
│   │   └── globals.css        # Global styles & CSS variables
│   ├── components/            # React components
│   │   └── ui/               # shadcn/ui components
│   ├── lib/                  # Utility libraries
│   │   ├── api-client.ts     # Axios configuration
│   │   ├── providers.tsx     # React Query & app providers
│   │   └── utils.ts          # Utility functions (cn helper)
│   └── hooks/                # Custom React hooks (to be added)
├── public/                   # Static assets
├── .env.local               # Environment variables (gitignored)
├── components.json          # shadcn/ui configuration
├── tailwind.config.ts       # Tailwind CSS configuration
├── tsconfig.json           # TypeScript configuration
└── next.config.ts          # Next.js configuration
```

## Design System

### Colors

- **Primary**: Sky Blue (#0EA5E9)
- **Secondary**: Teal (#14B8A6)
- **Accent**: Violet (#8B5CF6)
- **Neutral**: Slate scale

### Typography

- **Primary Font**: Plus Jakarta Sans (weights 200-800)
- **Monospace Font**: JetBrains Mono
- **Vietnamese Support**: Fully supported

### Components

Built with shadcn/ui - fully accessible, customizable, and themeable components.

```tsx
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";

<Button className="bg-gradient-to-r from-sky-500 to-teal-500">
  Get Started
</Button>
```

## Key Features Implemented

- ✅ Next.js 15 with App Router and Turbopack
- ✅ TypeScript strict mode
- ✅ Tailwind CSS with Nexora theme
- ✅ shadcn/ui component library
- ✅ React Query for data fetching
- ✅ Zustand for state management
- ✅ SignalR for real-time updates
- ✅ React Hook Form + Zod validation
- ✅ Plus Jakarta Sans font integration
- ✅ Dark mode support
- ✅ Environment configuration

## API Integration

The frontend uses Axios for HTTP requests with automatic token handling:

```typescript
import apiClient from "@/lib/api-client";

// GET request
const response = await apiClient.get("/api/tasks");

// POST request
const response = await apiClient.post("/api/tasks", {
  title: "New Task",
  status: "todo",
});
```

## State Management

### React Query (Server State)

```typescript
import { useQuery } from "@tanstack/react-query";

function TasksList() {
  const { data, isLoading } = useQuery({
    queryKey: ["tasks"],
    queryFn: () => apiClient.get("/api/tasks").then(res => res.data),
  });
}
```

### Zustand (Client State)

```typescript
import { create } from "zustand";

const useTaskStore = create((set) => ({
  selectedTask: null,
  setSelectedTask: (task) => set({ selectedTask: task }),
}));
```

## Code Quality

- **Linting**: ESLint with Next.js config
- **Type Checking**: TypeScript strict mode
- **Formatting**: Prettier (recommended)
- **Import Order**: `@/` alias for absolute imports

## Browser Support

- Chrome (last 2 versions)
- Firefox (last 2 versions)
- Safari (last 2 versions)
- Edge (last 2 versions)

## Performance

- **Turbopack**: Enabled for fast development builds
- **Image Optimization**: Next.js Image component
- **Font Optimization**: next/font for automatic font optimization
- **Code Splitting**: Automatic with App Router

## Deployment

### Vercel (Recommended)

```bash
# Install Vercel CLI
npm i -g vercel

# Deploy
vercel
```

### Docker

```bash
# Build image
docker build -t nexora-frontend .

# Run container
docker run -p 3000:3000 nexora-frontend
```

### Manual Deployment

```bash
npm run build
npm start
```

## Troubleshooting

### Port Already in Use

```bash
# Kill process on port 3000
npx kill-port 3000
```

### Module Not Found

```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
```

### Build Errors

```bash
# Clear Next.js cache
rm -rf .next
npm run build
```

## Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Make your changes
3. Run tests: `npm run lint && npm run typecheck`
4. Commit changes: `git commit -m "Add your feature"`
5. Push: `git push origin feature/your-feature`

## Resources

- [Next.js Documentation](https://nextjs.org/docs)
- [shadcn/ui Components](https://ui.shadcn.com)
- [Tailwind CSS](https://tailwindcss.com/docs)
- [React Query Docs](https://tanstack.com/query/latest)
- [Zustand Docs](https://zustand-demo.pmnd.rs)

## License

MIT License - see LICENSE file for details
