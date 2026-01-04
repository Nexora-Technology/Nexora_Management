# Infrastructure Setup

**Last Updated:** 2026-01-04
**Version:** Phase 06 Complete (Real-time Collaboration)

## Overview

Phase 01 established the complete infrastructure foundation for Nexora Management platform, including monorepo structure, containerization, CI/CD pipelines, and development tooling.

## Repository Structure

### Monorepo Layout

```
Nexora_Management/
├── .github/
│   └── workflows/              # CI/CD pipelines
│       ├── pr-checks.yml       # Pull request validation
│       └── build.yml           # Main branch builds
├── .husky/                     # Git hooks
│   └── pre-commit              # Lint-staged runner
├── apps/
│   ├── backend/                # .NET 9.0 Web API
│   │   ├── src/
│   │   │   ├── Core/           # Domain entities
│   │   │   ├── Application/    # Business logic
│   │   │   ├── Infrastructure/ # Data access
│   │   │   └── API/            # Controllers
│   │   └── tests/              # Unit/integration tests
│   └── frontend/               # Next.js 15 application
│       ├── src/
│       │   ├── app/            # App Router pages (⚠️ NOT `app/` at root)
│       │   ├── components/     # React components
│       │   ├── features/       # Feature modules
│       │   ├── lib/            # Utilities
│       │   └── hooks/          # Custom hooks
│       ├── public/             # Static assets
│       ├── tailwind.config.ts  # Tailwind CSS v3.4.0 config
│       ├── postcss.config.mjs  # PostCSS v3 config
│       └── next.config.ts      # Next.js config (standalone mode)
├── docker/                     # Docker configurations
│   ├── docker-compose.yml      # Production compose
│   ├── docker-compose.override.yml # Development overrides
│   └── postgres-init/          # Database initialization scripts
├── docs/                       # Documentation
├── Dockerfile.backend          # Backend container image
├── Dockerfile.frontend         # Frontend container image
├── package.json                # Root monorepo config
├── turbo.json                  # Turborepo configuration
├── .gitignore                  # Git ignore rules
├── .eslintrc.js                # ESLint configuration
├── .prettierrc                 # Prettier formatting rules
├── .lintstagedrc.json          # Lint-staged configuration
└── .dockerignore               # Docker ignore rules
```

### Monorepo Management

**Tool:** Turborepo 2.7+

**Configuration:** `turbo.json`

```json
{
  "$schema": "https://turbo.build/schema.json",
  "tasks": {
    "build": {
      "dependsOn": ["^build"],
      "outputs": ["dist/**", ".next/**", "bin/**"]
    },
    "dev": {
      "cache": false,
      "persistent": true
    },
    "lint": {
      "outputs": []
    },
    "test": {
      "dependsOn": ["^build"],
      "outputs": []
    }
  }
}
```

**Root Scripts:**

- `npm run dev` - Start all services in development mode
- `npm run build` - Build all packages
- `npm run test` - Run all tests
- `npm run lint` - Lint all packages
- `npm run format` - Format all code with Prettier

## Development Environment Setup

### Prerequisites

**Required Software:**

- Docker Desktop 4.0+ (or Docker Engine)
- Docker Compose 2.0+
- .NET 9.0 SDK
- Node.js 20+
- npm 10+
- Git 2.30+

**Verify Installation:**

```bash
docker --version
docker-compose --version
dotnet --version
node --version
npm --version
git --version
```

### Initial Setup

**1. Clone Repository:**

```bash
git clone <repository-url>
cd Nexora_Management
```

**2. Install Dependencies:**

```bash
# Install root dependencies (Turborepo, tooling)
npm install

# Install frontend dependencies
cd apps/frontend
npm install
cd ../..

# Install backend dependencies (handled by dotnet restore)
cd apps/backend
dotnet restore
```

**3. Setup Git Hooks:**

```bash
# Husky setup (runs automatically via npm install)
npx husky install
```

**4. Configure Environment:**

```bash
# Backend configuration
cp apps/frontend/.env.example apps/frontend/.env

# Edit .env with your settings
nano apps/frontend/.env
```

### IDE Setup

**Visual Studio Code (Recommended):**

Extensions:

- C# Dev Kit
- .NET Install Tool
- ESLint
- Prettier
- Docker
- GitLens

**Visual Studio 2022 (Optional):**

- Load `apps/backend/Nexora.Management.sln`
- Recommended for backend development

**JetBrains Rider (Optional):**

- Alternative C# IDE with excellent .NET support

## Docker Compose Services

### Service Architecture

**Location:** `docker/docker-compose.yml`

**Services:**

1. **postgres** - PostgreSQL 16 database
2. **redis** - Redis 7 cache
3. **backend** - .NET 9.0 Web API
4. **frontend** - Next.js 15 application

### PostgreSQL Configuration

**Image:** `postgres:16-alpine`

**Environment Variables:**

```yaml
POSTGRES_USER: nexora
POSTGRES_PASSWORD: nexora_dev
POSTGRES_DB: nexora_dev
```

**Ports:**

- `5432:5432` (Host:Container)

**Volumes:**

- `postgres-data:/var/lib/postgresql/data` - Data persistence
- `./postgres-init:/docker-entrypoint-initdb.d` - Init scripts

**Health Check:**

```yaml
test: ['CMD-SHELL', 'pg_isready -U nexora -d nexora_dev']
interval: 10s
timeout: 5s
retries: 5
```

### Redis Configuration

**Image:** `redis:7-alpine`

**Command:** `redis-server --requirepass nexora_dev`

**Ports:**

- `6379:6379`

**Health Check:**

```yaml
test: ['CMD', 'redis-cli', '--raw', 'incr', 'ping']
interval: 10s
timeout: 3s
retries: 5
```

### Backend Configuration

**Build Context:** Repository root
**Dockerfile:** `Dockerfile.backend`

**Environment:**

```yaml
ASPNETCORE_ENVIRONMENT: Development
ASPNETCORE_URLS: http://+:8080
ConnectionStrings__PostgreSQL: 'Host=postgres;Port=5432;Database=nexora_dev;Username=nexora;Password=nexora_dev'
ConnectionStrings__Redis: 'redis:6379,password=nexora_dev'
```

**Ports:**

- `5001:8080` (Host:Container)

**Dependencies:**

- postgres (health check must pass)
- redis (health check must pass)

### Frontend Configuration

**Build Context:** Repository root
**Dockerfile:** `Dockerfile.frontend`

**Environment:**

```yaml
NODE_ENV: production
NEXT_PUBLIC_API_URL: http://backend:8080
```

**Ports:**

- `3000:3000`

**Dependencies:**

- backend (service must be started)

### Networking

**Network:** `nexora-network` (bridge driver)

All services communicate via this network. Service discovery uses Docker Compose service names (e.g., `backend`, `postgres`).

### Development Overrides

**Location:** `docker/docker-compose.override.yml`

Overrides for development:

- Volume mounts for hot reloading
- Additional debugging ports
- Development-specific environment variables

**Usage:**

```bash
# Production configuration
docker-compose -f docker/docker-compose.yml up

# Development configuration (auto-applied)
docker-compose up
```

## CI/CD Pipeline Configuration

### GitHub Actions Workflows

**Location:** `.github/workflows/`

#### PR Checks Workflow

**File:** `.github/workflows/pr-checks.yml`

**Trigger:**

```yaml
on:
  pull_request:
    branches: [main]
```

**Jobs:**

**1. Backend Tests:**

```yaml
runs-on: ubuntu-latest
steps:
  - Checkout code
  - Setup .NET 9.0
  - Restore dependencies
  - Build solution
  - Run tests with coverage
  - Upload test results artifact
```

**2. Frontend Tests:**

```yaml
runs-on: ubuntu-latest
steps:
  - Checkout code
  - Setup Node.js 20
  - Install dependencies (npm ci)
  - Run ESLint
  - Type check
  - Build application
```

**3. Docker Build Validation:**

```yaml
runs-on: ubuntu-latest
steps:
  - Checkout code
  - Build backend Docker image
  - Build frontend Docker image
  - Validate docker-compose configuration
```

#### Build Workflow

**File:** `.github/workflows/build.yml`

**Trigger:**

```yaml
on:
  push:
    branches: [main]
```

**Jobs:**

**Test and Build:**

```yaml
runs-on: ubuntu-latest
jobs:
  backend-test-and-build:
    steps:
      - Checkout code
      - Setup .NET 9.0
      - dotnet restore
      - dotnet build
      - dotnet test

  frontend-test-and-build:
    steps:
      - Checkout code
      - Setup Node.js 20
      - npm ci
      - npx turbo run test --filter=@nexora/frontend
      - npx turbo run build --filter=@nexora/frontend
      - Upload build artifacts
```

**Notes:**

- Uses `npx turbo` to run locally installed Turborepo
- No Docker build step (Dockerfiles not yet implemented)
- package-lock.json is committed for reproducible installs

### CI/CD Best Practices

**Branch Protection Rules:**

- Require PR reviews before merge
- Require status checks to pass
- Block force pushes
- Require up-to-date branch before merge

**Required Checks:**

- Backend tests
- Frontend tests
- Docker build validation

**Secret Management:**

- Store sensitive data in GitHub Secrets
- Never commit `.env` files
- Use environment-specific configs

## Development Tooling Setup

### Code Formatting

**Tool:** Prettier 3.0+

**Configuration:** `.prettierrc`

```json
{
  "semi": true,
  "singleQuote": true,
  "tabWidth": 2,
  "trailingComma": "es5",
  "printWidth": 100,
  "arrowParens": "always",
  "endOfLine": "lf"
}
```

**Usage:**

```bash
# Format all files
npm run format

# Check formatting
npm run format:check

# Format specific file
npx prettier --write apps/frontend/src/app/page.tsx
```

**Ignored Files:** `.prettierignore`

### Linting

**Frontend:** ESLint

**Configuration:** `.eslintrc.js`

```javascript
module.exports = {
  root: true,
  parser: '@typescript-eslint/parser',
  plugins: ['@typescript-eslint'],
  extends: ['eslint:recommended', '@typescript-eslint/recommended'],
  // ... rules
};
```

**Usage:**

```bash
# Lint all packages
npm run lint

# Lint frontend
cd apps/frontend
npm run lint

# Auto-fix issues
npm run lint -- --fix
```

**Backend:** dotnet format (built-in)

```bash
cd apps/backend
dotnet format
```

### Pre-commit Hooks

**Tool:** Husky + lint-staged

**Hook:** `.husky/pre-commit`

```bash
#!/bin/sh
. "$(dirname "$0")/_/husky.sh"

npx lint-staged
```

**Configuration:** `.lintstagedrc.json`

```json
{
  "*.{cs,csproj}": ["dotnet format --include"],
  "*.{ts,tsx,js,jsx}": ["eslint --fix", "prettier --write"],
  "*.{json,md}": ["prettier --write"]
}
```

**Behavior:**

- Runs automatically on `git commit`
- Only lints staged files
- Blocks commit if linting fails
- Auto-fixes issues where possible

**Bypass (Not Recommended):**

```bash
git commit --no-verify -m "message"
```

### Git Configuration

**Git Ignore:** `.gitignore`

**Key Patterns:**

```gitignore
# Dependencies
node_modules/
**/bin/
**/obj/
**/.vs/

# Build outputs
dist/
.next/
out/

# Environment
.env
.env.local
.env.*.local

# IDE
.vscode/
.idea/
*.swp
*.swo

# OS
.DS_Store
Thumbs.db

# Docker
.dockerignore

# Logs
logs/
*.log
```

**Commit Message Convention:**

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

**Types:**

- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `style:` - Code style changes (formatting)
- `refactor:` - Code refactoring
- `test:` - Adding or updating tests
- `chore:` - Maintenance tasks
- `perf:` - Performance improvements

**Examples:**

```bash
git commit -m "feat: add user authentication"
git commit -m "fix: resolve issue with task creation"
git commit -m "docs: update API documentation"
git commit -m "refactor: simplify workspace management logic"
```

### Docker Tooling

**Dockerfile:** `Dockerfile.backend`

**Multi-stage Build:**

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["apps/backend/Nexora.Management.API/Nexora.Management.API.csproj", "API/"]
RUN dotnet restore "API/API.csproj"
COPY apps/backend/ .
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexora.Management.API.dll"]
```

**Dockerfile:** `Dockerfile.frontend`

**Multi-stage Build with Standalone Output:**

```dockerfile
# Dependencies stage
FROM node:20-alpine AS deps
WORKDIR /app
COPY package*.json ./
COPY apps/frontend/package*.json ./apps/frontend/
COPY packages/*/package*.json ./packages/*/
RUN npm ci

# Builder stage
FROM node:20-alpine AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .
WORKDIR /app/apps/frontend
RUN npm run build

# Runner stage
FROM node:20-alpine AS runner
WORKDIR /app
RUN addgroup -g 1001 -S nodejs && adduser -S nextjs -u 1001
ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

COPY --from=builder /app/apps/frontend/public ./apps/frontend/public
COPY --from=builder --chown=nextjs:nodejs /app/apps/frontend/.next/static ./apps/frontend/.next/static
COPY --from=builder --chown=nextjs:nodejs /app/apps/frontend/.next/server ./apps/frontend/.next/server
COPY --from=builder --chown=nextjs:nodejs /app/apps/frontend/.next/standalone ./

USER nextjs
EXPOSE 3000
CMD ["node", "apps/frontend/server.js"]
```

### Frontend Build Configuration

**⚠️ CRITICAL:** Next.js App Router requires specific directory structure and configuration.

**Directory Structure:**

```
apps/frontend/
├── src/
│   └── app/              # ✅ App Router pages here
└── app/                  # ❌ NEVER create empty directory at root
```

**Issue:** Empty `apps/frontend/app` directory blocks Next.js from detecting `src/app`, causing 404 errors.

**Solution:** Only use `src/app` for App Router pages.

**Tailwind CSS Configuration:**

**File:** `apps/frontend/tailwind.config.ts`

```typescript
import type { Config } from 'tailwindcss';
import tailwindcssAnimate from 'tailwindcss-animate';

const config: Config = {
  darkMode: 'class',
  content: [
    './src/pages/**/*.{js,ts,jsx,tsx,mdx}',
    './src/components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/**/*.{js,ts,jsx,tsx,mdx}',
    './src/features/**/*.{js,ts,jsx,tsx,mdx}', // Required for feature modules
  ],
  theme: {
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
        // ... more shadcn/ui colors
      },
    },
  },
  plugins: [tailwindcssAnimate],
};

export default config;
```

**PostCSS Configuration:**

**File:** `apps/frontend/postcss.config.mjs`

```javascript
const config = {
  plugins: {
    tailwindcss: {}, // v3 format (NOT "@tailwindcss/postcss")
    autoprefixer: {},
  },
};

export default config;
```

**⚠️ IMPORTANT:** Project uses Tailwind CSS v3.4.0, NOT v4.

**Dependencies:**

```json
{
  "devDependencies": {
    "tailwindcss": "^3.4.0",
    "postcss": "^8.4.0",
    "autoprefixer": "^10.4.0",
    "tailwindcss-animate": "^1.0.7"
  }
}
```

**Build Verification:**

```bash
cd apps/frontend
npm run build

# Should output:
# Route (app)                                 Size  First Load JS
# ┌ ○ /                                    X kB          XX kB
# ├ ○ /dashboard                           X kB          XX kB
# └ ƒ /projects/[id]                        XX kB         XXX kB
```

If build shows `Route (pages)` instead of `Route (app)`, App Router is not being detected.

**Docker Ignore:** `.dockerignore`

```
**/bin/
**/obj/
**/.vs/
**/node_modules/
**/.next/
.git/
README.md
```

**Usage:**

```bash
# Build backend image
docker build -f Dockerfile.backend -t nexora-backend:latest .

# Build frontend image
docker build -f Dockerfile.frontend -t nexora-frontend:latest .

# Run with docker-compose
docker-compose up -d

# View logs
docker-compose logs -f backend

# Stop services
docker-compose down

# Remove volumes
docker-compose down -v
```

## Troubleshooting

### Docker Issues

**Port Already in Use:**

```bash
# Find process using port
lsof -i :3000
lsof -i :5001

# Kill process
kill -9 <PID>

# Or use npx kill-port
npx kill-port 3000
```

**Container Health Check Failures:**

```bash
# Check container health
docker-compose ps

# View health check logs
docker inspect nexora-postgres | grep -A 10 Health

# Restart containers
docker-compose restart

# Rebuild from scratch
docker-compose down -v
docker-compose up --build
```

**Volume Issues:**

```bash
# Remove all volumes
docker-compose down -v

# Remove specific volume
docker volume rm nexora-management_postgres-data
```

### Dependency Issues

**.NET Restore Failures:**

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore with verbose output
dotnet restore --verbosity detailed
```

**npm Install Failures:**

```bash
# Clear npm cache
npm cache clean --force

# Remove node_modules and reinstall
rm -rf node_modules apps/frontend/node_modules
rm package-lock.json apps/frontend/package-lock.json
npm install
```

### Git Hook Issues

**Husky Not Working:**

```bash
# Reinstall husky
npm run prepare

# Or manually
npx husky install
```

**Pre-commit Hook Permission Denied:**

```bash
# Make hook executable
chmod +x .husky/pre-commit
```

### Environment Issues

**Wrong .NET Version:**

```bash
# Check installed versions
dotnet --list-sdks

# Install .NET 9.0
# Visit: https://dotnet.microsoft.com/download/dotnet/9.0
```

**Wrong Node Version:**

```bash
# Check version
node --version

# Use nvm to switch
nvm install 20
nvm use 20
```

## Best Practices

### Development Workflow

1. **Always pull latest changes before starting work**
2. **Create feature branches from main**
3. **Run linter before committing**
4. **Write tests for new functionality**
5. **Keep commits atomic and focused**
6. **Ensure all tests pass before pushing**
7. **Update documentation with code changes**

### Docker Usage

1. **Use docker-compose for local development**
2. **Leverage volume mounts for hot reloading**
3. **Don't commit production environment variables**
4. **Use specific image tags in production**
5. **Regularly update base images**

### Performance

1. **Use Turborepo caching for faster builds**
2. **Run only necessary services during development**
3. **Use Docker layer caching effectively**
4. **Minimize node_modules in images**

---

**Documentation Version:** 1.0
**Maintained By:** Development Team
