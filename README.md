# Nexora Management

A powerful, ClickUp-inspired project management platform built with modern technologies. Nexora provides teams with a comprehensive solution for task management, collaboration, and productivity tracking.

## Tech Stack

### Frontend

- **Next.js 15** - React framework with App Router
- **TypeScript** - Type-safe development
- **Tailwind CSS** - Utility-first styling
- **shadcn/ui** - High-quality React components (18 components integrated)
- **Zustand** - Lightweight state management
- **React Query** - Data fetching and caching (@tanstack/react-table)
- **SignalR** - Real-time communication (@microsoft/signalr)
- **@dnd-kit** - Drag and drop functionality (core, modifiers, sortable, utilities)

### Backend

- **.NET 9.0** - Modern, high-performance framework
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 9** - ORM and data access
- **SignalR** - Real-time WebSocket communication
- **PostgreSQL** - Primary database with Row-Level Security
- **JWT Authentication** - Secure token-based auth

### DevOps & Tooling

- **Docker & Docker Compose** - Container orchestration
- **Turborepo** - Monorepo build system
- **GitHub Actions** - CI/CD pipelines
- **Husky + lint-staged** - Git hooks and pre-commit checks
- **Prettier + ESLint** - Code quality and formatting

## Quick Start

### Prerequisites

- Docker and Docker Compose
- .NET 9.0 SDK
- Node.js 20+
- npm 10+

### Using Docker Compose (Recommended)

```bash
# Clone the repository
git clone https://github.com/Nexora-Technology/Nexora_Management.git
cd Nexora_Management

# Start all services
docker-compose up -d

# Access the application
# Frontend: http://localhost:3000
# Backend API: http://localhost:5000
# PostgreSQL: localhost:5432
```

### Development Setup

#### Backend Development

```bash
cd apps/backend

# Restore dependencies
dotnet restore

# Run migrations
dotnet ef database update

# Start the backend
dotnet run --project src/Nexora.Management.Api

# API will be available at http://localhost:5000
```

#### Frontend Development

```bash
cd apps/frontend

# Install dependencies
npm install

# Start development server
npm run dev

# App will be available at http://localhost:3000
```

## Project Structure

```
Nexora_Management/
├── .github/
│   └── workflows/          # CI/CD pipelines
│       ├── pr-checks.yml
│       └── build.yml
├── apps/
│   ├── backend/            # .NET 9.0 Web API
│   │   ├── src/
│   │   │   ├── Core/       # Domain entities and interfaces
│   │   │   ├── Application/# Application logic and use cases
│   │   │   ├── Infrastructure/# Data access and external services
│   │   │   └── API/        # Controllers and endpoints
│   │   └── tests/          # Unit and integration tests
│   └── frontend/           # Next.js 15 application
│       ├── app/            # App Router pages
│       ├── components/     # Reusable React components
│       ├── lib/            # Utilities and configurations
│       └── public/         # Static assets
├── docs/                   # Documentation
│   ├── adr/               # Architecture Decision Records
│   └── development/       # Development guides
├── .husky/                # Git hooks
├── docker-compose.yml     # Multi-container orchestration
├── package.json           # Root package.json (monorepo scripts)
├── turbo.json             # Turborepo configuration
└── README.md
```

## Development Workflow

1. **Create a feature branch**: `git checkout -b feature/your-feature`
2. **Make changes**: Follow coding standards and commit conventions
3. **Test locally**: Run `npm test` and `npm run lint`
4. **Create PR**: Target the `main` branch
5. **CI checks**: PR checks run automatically
6. **Code review**: Address feedback
7. **Merge**: After approval and passing checks

## Available Scripts

At the root level (monorepo):

```bash
npm run dev          # Start all services in development mode
npm run build        # Build all packages
npm run test         # Run all tests
npm run lint         # Lint all packages
npm run format       # Format all code with Prettier
npm run format:check # Check code formatting
```

## Testing

### Backend Tests

```bash
cd apps/backend
dotnet test
```

### Frontend Tests

```bash
cd apps/frontend
npm test
```

### All Tests (from root)

```bash
npm test
```

## Contributing

We welcome contributions! Please read our [Contributing Guidelines](CONTRIBUTING.md) to understand our development workflow, coding standards, and PR process.

Key points:

- Follow Clean Architecture principles
- Write tests for new features
- Ensure all CI checks pass
- Update documentation as needed
- Use conventional commit messages

## Code of Conduct

Be respectful, inclusive, and constructive. We're all working together to build something great.

## Documentation

### Getting Started

- [Project Overview](docs/project-overview-pdr.md)
- [Local Setup Guide](docs/development/local-setup.md)

### Infrastructure & Development

- [Infrastructure Setup](docs/infrastructure-setup.md) - Monorepo structure, Docker, CI/CD
- [Development Standards](docs/development-standards.md) - Code formatting, linting, workflows
- [Deployment Guide](docs/deployment-guide.md) - Build, run, and troubleshooting

### Architecture

- [System Architecture](docs/system-architecture.md) - Clean Architecture layers
- [Codebase Summary](docs/codebase-summary.md) - Quick reference
- [Architecture Decisions](docs/adr/001-architecture-decisions.md)

### Project Planning

- [Code Standards](docs/code-standards.md)
- [Design Guidelines](docs/design-guidelines.md)
- [Project Roadmap](docs/project-roadmap.md)

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Roadmap

- [x] Project setup and architecture
- [x] Authentication & authorization (JWT, permissions, RLS)
- [x] Core workspace functionality
- [x] Task management (CRUD, hierarchy, multiple views, drag-and-drop)
- [x] Real-time updates via SignalR
- [x] File attachments
- [x] Comments and collaboration
- [x] Document & Wiki system (100% complete)
- [x] Goal tracking & OKRs (100% complete)
- [ ] Advanced filtering and search
- [ ] Mobile responsive design
- [ ] Performance optimization
- [ ] Deployment to production

## Current Phase: Goal Tracking & OKRs (Phase 08 - Complete) ✅

### Phase 08 Achievements ✅

- **Goal Tracking System:**
  - GoalPeriod entities for time-based tracking (Q1, Q2, FY, etc.)
  - Objective entities with hierarchical structure (3 levels max)
  - KeyResult entities with measurable metrics (number, percentage, currency)
  - Weighted average progress calculation
  - Auto-status calculation (on-track/at-risk/off-track)
  - Progress dashboard with analytics

- **Frontend Components:**
  - ObjectiveCard component for displaying objectives
  - KeyResultEditor for editing key results
  - ProgressDashboard for visual analytics
  - ObjectiveTree for hierarchical view
  - Goals pages (list and detail views)

- **API Endpoints:**
  - 12 goal tracking endpoints (periods, objectives, key results, dashboard)
  - RESTful API design with CQRS pattern
  - Proper error handling and validation

- **Database:**
  - 3 new tables (goal_periods, objectives, key_results)
  - 8 indexes for performance
  - Foreign key relationships for data integrity

### Recent Improvements (January 2026) ✅

- **Drag and Drop Functionality:**
  - Fixed Kanban board drag and drop
  - Tasks can be dragged anywhere on the card
  - Tasks can be dragged between columns to change status
  - Added @dnd-kit/core 6.3.1, @dnd-kit/modifiers 9.0.0, @dnd-kit/sortable 10.0.0, @dnd-kit/utilities 3.2.2

### Next Phase 📋

**Phase 09:** Time Tracking

- Time entry entities and tables
- Timer functionality
- Time reports and analytics

### Previous Achievements ✅

**Phase 05B (ClickUp Design System Polish):** Complete ✅

- Documentation: 5 components with JSDoc
- Component usage guide
- Sidebar integration via route group layout

**Phase 05A (Performance & Accessibility):** Complete ✅

- 75% reduction in unnecessary re-renders
- Single-pass algorithm (O(n) complexity)
- aria-live regions (WCAG 2.1 AA compliant)
- ARIA labels for interactive elements
- Code Review: 8.5/10

### Build Status ✅

- TypeScript compilation: Passed
- Backend: 0 errors, 24 warnings (pre-existing)
- Frontend: 0 TypeScript errors
- Code review: 8.5/10
- Commit: Latest (2026-01-06)

## Support

For questions, issues, or suggestions:

- Open an issue on [GitHub](https://github.com/Nexora-Technology/Nexora_Management/issues)
- Check existing documentation
- Contact the maintainers

---

Built with ❤️ using .NET 9.0, Next.js 15, and modern web technologies.
