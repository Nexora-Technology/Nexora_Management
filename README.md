# Nexora Management

A powerful, ClickUp-inspired project management platform built with modern technologies. Nexora provides teams with a comprehensive solution for task management, collaboration, and productivity tracking.

## Tech Stack

### Frontend

- **Next.js 15** - React framework with App Router
- **TypeScript** - Type-safe development
- **Tailwind CSS** - Utility-first styling
- **shadcn/ui** - High-quality React components
- **Zustand** - Lightweight state management
- **React Query** - Data fetching and caching
- **SignalR** - Real-time communication

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
- [x] Task management (CRUD, hierarchy, multiple views)
- [x] Real-time updates via SignalR
- [x] File attachments
- [x] Comments and collaboration
- [ ] Document & Wiki system (60% complete)
- [ ] Advanced filtering and search
- [ ] Mobile responsive design
- [ ] Performance optimization
- [ ] Deployment to production

## Current Phase: Document & Wiki System (Phase 07 - 60% Complete)

### Backend Implementation ✅

- 4 Domain entities: Page, PageVersion, PageCollaborator, PageComment
- 6 CQRS Commands: CreatePage, UpdatePage, DeletePage, ToggleFavorite, MovePage, RestorePageVersion
- 4 CQRS Queries: GetPageById, GetPageTree, GetPageHistory, SearchPages
- 10 API endpoints at `/api/documents`
- Unique slug generation and auto-versioning

### Frontend Implementation ✅

- TipTap rich text editor with toolbar
- Page tree component with search
- Page list with favorites and recent views
- Version history component
- 7 document components exported

### Pending ⏳

- Database migration application
- Route integration
- Backend API integration
- Collaboration UI
- Testing

## Support

For questions, issues, or suggestions:

- Open an issue on [GitHub](https://github.com/Nexora-Technology/Nexora_Management/issues)
- Check existing documentation
- Contact the maintainers

---

Built with ❤️ using .NET 9.0, Next.js 15, and modern web technologies.
