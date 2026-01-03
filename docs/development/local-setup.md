# Local Development Setup Guide

This guide will help you set up your local development environment for Nexora Management.

## Prerequisites

Before starting, ensure you have the following installed:

### Required Software

1. **Docker Desktop** (v24.0+)
   - [Download for macOS](https://www.docker.com/products/docker-desktop/)
   - [Download for Windows](https://www.docker.com/products/docker-desktop/)
   - [Download for Linux](https://docs.docker.com/engine/install/)

2. **.NET 9.0 SDK**
   - [Download for macOS/Windows/Linux](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Verify installation: `dotnet --version`

3. **Node.js 20+**
   - [Download Node.js](https://nodejs.org/)
   - Recommended: Use [nvm](https://github.com/nvm-sh/nvm) (macOS/Linux) or [nvm-windows](https://github.com/coreybutler/nvm-windows) to manage Node versions
   - Verify installation: `node --version` and `npm --version`

### Recommended Tools

- **Visual Studio Code** with extensions:
  - C# Dev Kit
  - ESLint
  - Prettier
  - Docker
  - GitLens
  - Thunder Client (for API testing)

- **Postman** or **Thunder Client** for API testing

- **pgAdmin** or **DBeaver** for database management

## Quick Start (Docker)

The fastest way to get started is using Docker Compose:

```bash
# Clone the repository
git clone <repository-url>
cd Nexora_Management

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

**Access Points:**
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- PostgreSQL: localhost:5432
- PostgreSQL Admin: http://localhost:5050 (if pgAdmin enabled)

## Manual Development Setup

If you prefer to run services directly (without Docker), follow these steps:

### 1. Clone and Install Dependencies

```bash
# Clone repository
git clone <repository-url>
cd Nexora_Management

# Install root dependencies
npm install

# Install backend dependencies (handled by dotnet restore)
# Install frontend dependencies (handled later)
```

### 2. Database Setup

**Option A: Using Docker (Recommended)**

```bash
# Start PostgreSQL only
docker-compose up -d postgres

# Wait for PostgreSQL to be ready
docker-compose logs -f postgres
```

**Option B: Local PostgreSQL Installation**

If you have PostgreSQL installed locally:

```bash
# Create database
createdb nexora

# Or use psql
psql -U postgres
CREATE DATABASE nexora;
\q
```

### 3. Backend Setup

```bash
cd apps/backend

# Restore dependencies
dotnet restore

# Update appsettings.json with your connection string
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=nexora;Username=postgres;Password=postgres"
  }
}

# Run database migrations
dotnet ef database update --project src/Nexora.Management.Infrastructure

# (Optional) Seed sample data
dotnet run --project src/Nexora.Management.Infrastructure/ --seed-data

# Start the backend API
dotnet run --project src/Nexora.Management.Api

# Backend will be available at http://localhost:5000
```

**Verify Backend:**
```bash
curl http://localhost:5000/health
# Expected: {"status":"healthy"}
```

### 4. Frontend Setup

```bash
cd apps/frontend

# Install dependencies
npm install

# Copy environment file
cp .env.example .env.local

# Update .env.local with your API URL
NEXT_PUBLIC_API_URL=http://localhost:5000

# Start development server
npm run dev

# Frontend will be available at http://localhost:3000
```

**Verify Frontend:**
- Open http://localhost:3000 in your browser
- Should see the application landing page

## Running Tests

### Backend Tests

```bash
cd apps/backend

# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/Nexora.Management.UnitTests

# Run specific test
dotnet test --filter "FullyQualifiedName~TaskServiceTests"
```

### Frontend Tests

```bash
cd apps/frontend

# Run tests
npm test

# Run tests in watch mode
npm test -- --watch

# Run tests with coverage
npm test -- --coverage
```

### Run All Tests (from root)

```bash
# Using Turborepo
npm test

# Or run specific package
cd apps/backend && dotnet test
cd apps/frontend && npm test
```

## Development Workflow

### 1. Start All Services

```bash
# Using Turborepo (from root)
npm run dev

# This will start:
# - Backend API on port 5000
# - Frontend on port 3000
# - Database (if using Docker)
```

### 2. Make Changes

- **Backend:** Edit files in `apps/backend/src/`
  - Changes auto-compile on save (Hot Reload)
  - API automatically restarts

- **Frontend:** Edit files in `apps/frontend/`
  - Next.js Fast Refresh enabled
  - Changes appear instantly

### 3. Run Linting

```bash
# Lint all packages
npm run lint

# Lint specific package
cd apps/frontend && npm run lint
```

### 4. Format Code

```bash
# Format all code
npm run format

# Check formatting
npm run format:check
```

## Troubleshooting

### Backend Issues

**Port 5000 already in use:**
```bash
# Find and kill process
# macOS/Linux
lsof -ti:5000 | xargs kill -9

# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

**Database connection errors:**
```bash
# Verify PostgreSQL is running
docker-compose ps postgres

# Check database logs
docker-compose logs postgres

# Test connection
psql -h localhost -U postgres -d nexora
```

**Migration errors:**
```bash
# Reset database (WARNING: Deletes all data)
dotnet ef database drop --force
dotnet ef database update
```

### Frontend Issues

**Port 3000 already in use:**
```bash
# Kill process
npx kill-port 3000

# Or use different port
npm run dev -- -p 3001
```

**Module not found errors:**
```bash
# Clear cache and reinstall
cd apps/frontend
rm -rf node_modules package-lock.json .next
npm install
```

**Environment variables not working:**
```bash
# Verify .env.local exists
ls -la .env.local

# Restart dev server after changing .env.local
```

### Docker Issues

**Containers not starting:**
```bash
# Rebuild containers
docker-compose down
docker-compose up --build

# Remove all containers and volumes
docker-compose down -v
docker-compose up --build
```

**Out of memory:**
```bash
# Increase Docker memory limit in Docker Desktop settings
# Recommended: 4GB+ for development
```

**Permission errors (Linux):**
```bash
# Fix file permissions
sudo chown -R $USER:$USER .
```

### Git Hooks Issues

**Pre-commit hook failing:**
```bash
# Check what's failing
npx lint-staged

# Format files manually
npm run format

# Or bypass (not recommended)
git commit --no-verify -m "..."
```

## IDE Configuration

### Visual Studio Code

Recommended workspace settings (`.vscode/settings.json`):

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": "explicit"
  },
  "files.exclude": {
    "**/.next": true,
    "**/node_modules": true,
    "**/bin": true,
    "**/obj": true
  },
  "omnisharp.enableRoslynAnalyzers": true,
  "omnisharp.enableEditorConfigSupport": true,
  "dotnet.defaultSolution": "apps/backend/Nexora.Management.sln"
}
```

### Rider / Visual Studio

1. Open `apps/backend/Nexora.Management.sln`
2. Set `src/Nexora.Management.Api` as startup project
3. Configure launch settings for F5 debugging

### VS Code Tasks

Example `.vscode/tasks.json`:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Start Backend",
      "command": "dotnet run --project apps/backend/src/Nexora.Management.Api",
      "group": "build",
      "isBackground": true
    },
    {
      "label": "Start Frontend",
      "command": "cd apps/frontend && npm run dev",
      "group": "build",
      "isBackground": true
    }
  ]
}
```

## Performance Tips

### Backend

- Use `dotnet watch` for faster development:
  ```bash
  dotnet watch --project src/Nexora.Management.Api
  ```

- Enable Hot Reload in Visual Studio or VS Code

### Frontend

- Use Turborepo caching for faster builds:
  ```bash
  npm run dev -- --force
  ```

- Disable source maps in production for faster builds

### Database

- Use connection pooling (configured by default)

- Add indexes for frequently queried fields

- Consider read replicas for reporting queries

## Next Steps

After completing setup:

1. Read [Architecture Decisions](../adr/001-architecture-decisions.md)
2. Review [Code Standards](../code-standards.md)
3. Check [Contributing Guidelines](../../CONTRIBUTING.md)
4. Explore the codebase starting with:
   - Backend: `apps/backend/src/Core/Entities/`
   - Frontend: `apps/frontend/app/`

## Getting Help

If you encounter issues not covered here:

1. Check [GitHub Issues](../../issues)
2. Ask in [Discussions](../../discussions)
3. Contact maintainers

Happy coding! 🚀
