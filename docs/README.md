# Documentation Index

**Last Updated:** 2026-01-03
**Version:** Phase 01 Complete

## Quick Links

- **New Developers:** Start with [Local Setup Guide](development/local-setup.md)
- **Infrastructure Setup:** [Infrastructure Setup](infrastructure-setup.md)
- **Code Standards:** [Development Standards](development-standards.md)
- **Deployment:** [Deployment Guide](deployment-guide.md)

## Documentation Structure

```
docs/
├── README.md (this file)
├── project-overview-pdr.md       # Project vision and requirements
├── infrastructure-setup.md        # NEW: Monorepo, Docker, CI/CD
├── development-standards.md       # NEW: Code quality and workflows
├── deployment-guide.md            # NEW: Build, run, troubleshooting
├── code-standards.md              # Legacy: Refer to development-standards.md
├── system-architecture.md         # Clean Architecture overview
├── codebase-summary.md            # Quick codebase reference
├── authorization-implementation.md # NEW: Phase 03 Authorization guide
├── design-guidelines.md           # UI/UX standards
├── project-roadmap.md             # Development phases
├── development/
│   └── local-setup.md            # Local development setup
├── adr/
│   └── 001-architecture-decisions.md
└── research/                      # Research documents
```

## Getting Started

### For New Developers

1. **Setup Your Environment**
   - Read: [Local Setup Guide](development/local-setup.md)
   - Install: Docker, .NET 9.0, Node.js 20+
   - Run: `docker-compose up -d`

2. **Understand the Architecture**
   - Read: [System Architecture](system-architecture.md)
   - Read: [Codebase Summary](codebase-summary.md)
   - Explore: Domain entities and Clean Architecture layers

3. **Follow Development Standards**
   - Read: [Development Standards](development-standards.md)
   - Configure: IDE and pre-commit hooks
   - Learn: Git workflow and commit conventions

4. **Start Contributing**
   - Read: [Contributing Guidelines](../CONTRIBUTING.md)
   - Pick: An issue from GitHub
   - Create: Feature branch and start coding

### For Infrastructure Setup

**Infrastructure Setup Guide** covers:

- Monorepo structure with Turborepo
- Docker Compose configuration
- CI/CD pipelines (GitHub Actions)
- Development tooling (Prettier, ESLint, Husky)
- Git hooks and pre-commit checks

**Link:** [infrastructure-setup.md](infrastructure-setup.md)

### For Deployment

**Deployment Guide** covers:

- Local development setup (Docker vs manual)
- Docker deployment and container management
- Environment configuration
- Build and run instructions
- Troubleshooting common issues

**Link:** [deployment-guide.md](deployment-guide.md)

## Core Documentation

### Architecture

- **[System Architecture](system-architecture.md)**
  - Clean Architecture layers (Domain, Infrastructure, Application, API)
  - Entity relationships and database schema
  - Row-Level Security (RLS) implementation
  - Permission-based authorization system
  - Workspace Authorization Middleware
  - Technology stack justification

- **[Codebase Summary](codebase-summary.md)**
  - Quick reference for all components
  - Entity hierarchy and relationships
  - File structure and organization
  - Configuration files
  - Phase completion status

- **[Authorization Implementation Guide](authorization-implementation.md)**
  - Phase 03 authentication & authorization details
  - Permission-based access control
  - Row-Level Security (RLS) setup
  - Workspace Authorization Middleware
  - Permission system and seeding
  - Best practices and troubleshooting

### Development Standards

- **[Development Standards](development-standards.md)**
  - Code formatting rules (Prettier, dotnet format)
  - Linting configuration (ESLint, StyleCop)
  - Pre-commit hooks (Husky, lint-staged)
  - Git workflow and commit conventions
  - Code quality standards (TypeScript, C#)
  - Testing standards
  - Documentation standards

### Project Planning

- **[Project Overview & PDR](project-overview-pdr.md)**
  - Project vision and objectives
  - Product Development Requirements (PDR)
  - Feature requirements
  - Success metrics

- **[Design Guidelines](design-guidelines.md)**
  - UI/UX design principles
  - Component design patterns
  - Accessibility standards
  - Design tokens and theming

- **[Project Roadmap](project-roadmap.md)**
  - Development phases (Phase 01-12)
  - Timeline and milestones
  - Feature delivery schedule

### Architecture Decisions

- **[ADR 001: Architecture Decisions](adr/001-architecture-decisions.md)**
  - Technology stack choices
  - Architecture pattern decisions
  - Database design decisions
  - Historical context and rationale

## Research Documents

The `docs/research/` directory contains research and investigation documents:

- [Docker Compose Deployment Research](research/docker-compose-deployment-research-2026-01-03.md)
- [ClickUp-like Platform Research](research-clickup-like-project-management-platform-2026-01-03.md)
- [Design Trends 2025](design-trends-2025-research.md)
- Various design system and feature research documents

## Phase Completion Status

### Phase 01: Infrastructure & Setup ✅

**Completed:**

- [x] Repository initialization
- [x] Monorepo structure (Turborepo)
- [x] Docker Compose configuration
- [x] CI/CD pipelines (GitHub Actions)
- [x] Development tooling setup
- [x] Code formatting and linting
- [x] Pre-commit hooks
- [x] Infrastructure documentation
- [x] Development standards documentation

**Documentation Deliverables:**

- [infrastructure-setup.md](infrastructure-setup.md)
- [development-standards.md](-development-standards.md)
- [deployment-guide.md](deployment-guide.md)

### Phase 02: Domain & Database ✅

**Completed:**

- [x] Clean Architecture structure
- [x] Domain entities (14 models)
- [x] EF Core configurations
- [x] Database migrations
- [x] Row-Level Security (RLS)
- [x] Role and permission seeding

**Documentation Deliverables:**

- [system-architecture.md](system-architecture.md)
- [codebase-summary.md](codebase-summary.md)

### Phase 03: Authentication & Authorization ✅

**Completed:**

- [x] JWT-based authentication with access/refresh tokens
- [x] Permission-based authorization with dynamic policy provider
- [x] Workspace Authorization Middleware for RLS user context
- [x] Raw SQL execution methods for authorization queries
- [x] RequirePermission attribute for endpoint-level authorization

**Documentation Deliverables:**

- [authorization-implementation.md](authorization-implementation.md) - Comprehensive guide
- Updated [system-architecture.md](system-architecture.md) with authorization details
- Updated [codebase-summary.md](codebase-summary.md) with new components
- Updated [development-standards.md](development-standards.md) with authorization standards

### Phase 04-12: In Progress

See [project-roadmap.md](project-roadmap.md) for upcoming phases.

## Quick Reference

### Common Commands

```bash
# Start all services
docker-compose up -d

# Run tests
npm test

# Lint code
npm run lint

# Format code
npm run format

# Build all packages
npm run build

# Run pre-commit hooks manually
npx lint-staged
```

### Important Files

- **Root Config:**
  - `turbo.json` - Turborepo configuration
  - `package.json` - Root scripts
  - `.gitignore` - Git ignore rules
  - `.prettierrc` - Formatting rules
  - `.lintstagedrc.json` - Pre-commit hooks

- **Infrastructure:**
  - `docker/docker-compose.yml` - Container orchestration
  - `Dockerfile.backend` - Backend container
  - `Dockerfile.frontend` - Frontend container
  - `.github/workflows/` - CI/CD pipelines

- **Development:**
  - `.eslintrc.js` - ESLint rules
  - `.husky/pre-commit` - Git hooks

### Access Points (Local Development)

- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger
- PostgreSQL: localhost:5432
- Redis: localhost:6379

## Contributing to Documentation

When making changes to the codebase:

1. **Update relevant documentation** if you change:
   - Architecture or structure
   - API endpoints
   - Configuration
   - Development workflow
   - Deployment process

2. **Follow documentation standards:**
   - Use clear, concise language
   - Include code examples
   - Update "Last Updated" date
   - Keep version in sync with project phases

3. **Documentation review:**
   - Technical accuracy
   - Completeness
   - Clarity for new developers
   - Consistency with other docs

## Need Help?

- **Setup Issues:** Check [deployment-guide.md](deployment-guide.md) troubleshooting section
- **Code Standards:** See [development-standards.md](development-standards.md)
- **Architecture Questions:** Review [system-architecture.md](system-architecture.md)
- **General Issues:** Check [GitHub Issues](../issues) or [Discussions](../discussions)

---

**Maintained By:** Development Team
**Documentation Version:** 1.0

For the latest updates, always refer to the project README: [../README.md](../README.md)
