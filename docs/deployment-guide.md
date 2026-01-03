# Deployment Guide

**Last Updated:** 2026-01-03
**Version:** Phase 03 Complete (Authentication)

## Overview

This guide provides comprehensive instructions for deploying and running the Nexora Management platform in various environments, including local development, staging, and production.

## Table of Contents

- [Local Development Setup](#local-development-setup)
- [Docker Deployment](#docker-deployment)
- [Environment Configuration](#environment-configuration)
- [Build and Run Instructions](#build-and-run-instructions)
- [Troubleshooting](#troubleshooting)

## Local Development Setup

### Prerequisites Checklist

Before starting, ensure you have the following installed:

- [ ] Docker Desktop 4.0+ or Docker Engine
- [ ] Docker Compose 2.0+
- [ ] .NET 9.0 SDK
- [ ] Node.js 20+
- [ ] npm 10+
- [ ] Git 2.30+
- [ ] VS Code or Visual Studio 2022 (optional)

### Quick Start (Docker Compose)

**Recommended for most developers**

**1. Clone Repository:**

```bash
git clone <repository-url>
cd Nexora_Management
```

**2. Start All Services:**

```bash
docker-compose up -d
```

**3. Verify Services:**

```bash
# Check all containers are running
docker-compose ps

# View logs
docker-compose logs -f

# Check specific service
docker-compose logs backend
```

**4. Access Applications:**

- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger
- PostgreSQL: localhost:5432
- Redis: localhost:6379

**5. Stop Services:**

```bash
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

### Manual Development Setup

**For developers who need more control**

**1. Clone and Install:**

```bash
git clone <repository-url>
cd Nexora_Management

# Install root dependencies
npm install

# Install frontend dependencies
cd apps/frontend
npm install
cd ../..

# Install backend dependencies
cd apps/backend
dotnet restore
```

**2. Setup PostgreSQL:**

**Option A: Use Docker (Recommended)**

```bash
docker run -d \
  --name nexora-postgres \
  -e POSTGRES_USER=nexora \
  -e POSTGRES_PASSWORD=nexora_dev \
  -e POSTGRES_DB=nexora_dev \
  -p 5432:5432 \
  postgres:16-alpine
```

**Option B: Install Locally**

- Download and install PostgreSQL 16 from https://www.postgresql.org/download/
- Create database and user using pgAdmin or psql

**3. Setup Redis:**

**Option A: Use Docker (Recommended)**

```bash
docker run -d \
  --name nexora-redis \
  -p 6379:6379 \
  redis:7-alpine
```

**Option B: Install Locally**

- Download and install Redis from https://redis.io/download

**4. Configure Backend:**

```bash
cd apps/backend/src/Nexora.Management.API

# Create appsettings.Development.json
cat > appsettings.Development.json << EOF
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=nexora_dev;Username=nexora;Password=nexora_dev"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
EOF
```

**5. Run Migrations:**

```bash
cd apps/backend
dotnet ef database update --project src/Nexora.Management.API
```

**6. Start Backend:**

```bash
cd apps/backend
dotnet run --project src/Nexora.Management.API

# API available at http://localhost:5000
# Swagger at http://localhost:5000/swagger
```

**7. Configure Frontend:**

```bash
cd apps/frontend

# Create .env.local
cat > .env.local << EOF
NEXT_PUBLIC_API_URL=http://localhost:5000
EOF
```

**8. Start Frontend:**

```bash
cd apps/frontend
npm run dev

# App available at http://localhost:3000
```

## Docker Deployment

### Building Images

**Build Individual Images:**

```bash
# Backend
docker build -f Dockerfile.backend -t nexora-backend:latest .

# Frontend
docker build -f Dockerfile.frontend -t nexora-frontend:latest .
```

**Build with Specific Tags:**

```bash
docker build -f Dockerfile.backend -t nexora-backend:v1.0.0 -t nexora-backend:latest .
```

### Running with Docker Compose

**Production Mode:**

```bash
docker-compose -f docker/docker-compose.yml up -d
```

**Development Mode (with overrides):**

```bash
docker-compose up -d
```

**Specific Services:**

```bash
# Start only database services
docker-compose up -d postgres redis

# Start only backend
docker-compose up -d backend

# Start all services
docker-compose up -d
```

### Container Management

**View Logs:**

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend

# Last 100 lines
docker-compose logs --tail=100 backend
```

**Execute Commands in Container:**

```bash
# Backend container
docker-compose exec backend bash
docker-compose exec backend dotnet --version

# Frontend container
docker-compose exec frontend sh
docker-compose exec frontend npm --version

# PostgreSQL
docker-compose exec postgres psql -U nexora -d nexora_dev

# Redis
docker-compose exec redis redis-cli -a nexora_dev
```

**Restart Services:**

```bash
# Restart all
docker-compose restart

# Restart specific service
docker-compose restart backend
```

**Update Running Containers:**

```bash
# Rebuild and restart
docker-compose up -d --build

# Force rebuild without cache
docker-compose build --no-cache
docker-compose up -d
```

### Docker Compose Override

**File:** `docker/docker-compose.override.yml`

Used for development-specific overrides:

```yaml
version: '3.8'

services:
  backend:
    volumes:
      - ./apps/backend:/app:cached # Hot reloading
    environment:
      - ASPNETCORE_ENVIRONMENT=Development

  frontend:
    volumes:
      - ./apps/frontend:/app:cached # Hot reloading
      - /app/node_modules # Prevent override
    environment:
      - NODE_ENV=development
    command: npm run dev
```

## Environment Configuration

### Backend Configuration

**File:** `apps/backend/src/Nexora.Management.API/appsettings.json`

**Production Settings:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Port=5432;Database=nexora_prod;Username=nexora;Password=${DB_PASSWORD}"
  },
  "Cors": {
    "AllowedOrigins": ["https://nexora.example.com"]
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning"
    }
  },
  "Jwt": {
    "SecretKey": "${JWT_SECRET}",
    "Issuer": "nexora",
    "Audience": "nexora-api",
    "ExpiryMinutes": 60
  }
}
```

**Environment Variables:**

- `DB_PASSWORD` - Database password
- `JWT__Secret` - JWT signing secret (use `__` for nested config)
- `JWT__Issuer` - JWT issuer claim
- `JWT__Audience` - JWT audience claim
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ASPNETCORE_URLS` - Binding URLs

**Important:** For JWT settings in production, use environment variables with double underscore notation for nested configuration (e.g., `JWT__Secret` maps to `Jwt:Secret`).

### Frontend Configuration

**File:** `apps/frontend/.env.production`

```bash
NEXT_PUBLIC_API_URL=https://api.nexora.example.com
NEXT_PUBLIC_APP_NAME=Nexora Management
NEXT_PUBLIC_APP_VERSION=1.0.0
```

**Environment Variables:**

- `NEXT_PUBLIC_API_URL` - Backend API URL (public)
- `NODE_ENV` - Node environment (production)
- Private variables (server-only) can be added without `NEXT_PUBLIC_` prefix

### Docker Compose Environment

**File:** `docker/.env` (gitignored)

```bash
# PostgreSQL
POSTGRES_USER=nexora
POSTGRES_PASSWORD=your_secure_password_here
POSTGRES_DB=nexora_prod

# Redis
REDIS_PASSWORD=your_redis_password_here

# Backend
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__PostgreSQL=Host=postgres;Port=5432;Database=nexora_prod;Username=nexora;Password=${POSTGRES_PASSWORD}
ConnectionStrings__Redis=redis:6379,password=${REDIS_PASSWORD}

# Frontend
NODE_ENV=production
NEXT_PUBLIC_API_URL=https://api.nexora.example.com
```

## Build and Run Instructions

### Development Build

**Frontend:**

```bash
cd apps/frontend

# Development mode (hot reloading)
npm run dev

# Production build (local testing)
npm run build
npm start
```

**Backend:**

```bash
cd apps/backend

# Development mode
dotnet run --project src/Nexora.Management.API

# Build for release
dotnet build src/Nexora.Management.API --configuration Release

# Run release build
dotnet run --project src/Nexora.Management.API --configuration Release
```

### Production Build

**Frontend:**

```bash
cd apps/frontend

# Install dependencies
npm ci

# Build optimized production bundle
npm run build

# Output: .next/ directory
```

**Backend:**

```bash
cd apps/backend

# Restore dependencies
dotnet restore

# Publish release
dotnet publish src/Nexora.Management.API -c Release -o ./publish

# Run published application
cd publish
dotnet Nexora.Management.API.dll
```

### Using Turborepo

**Build All Packages:**

```bash
# Root directory
npm run build

# Or using turbo directly
turbo run build
```

**Run All Tests:**

```bash
npm run test
```

**Lint All Packages:**

```bash
npm run lint
```

### Database Migrations

**Create Migration:**

```bash
cd apps/backend
dotnet ef migrations add MigrationName --project src/Nexora.Management.API
```

**Apply Migrations:**

```bash
dotnet ef database update --project src/Nexora.Management.API
```

**Rollback Migration:**

```bash
dotnet ef database update PreviousMigrationName --project src/Nexora.Management.API
```

**Generate SQL Script:**

```bash
dotnet ef migrations script --project src/Nexora.Management.API --output migration.sql
```

## Troubleshooting

### Common Issues

#### Port Already in Use

**Symptom:** Error "bind: address already in use"

**Solutions:**

```bash
# Find process using port
lsof -i :3000  # Frontend
lsof -i :5000  # Backend
lsof -i :5432  # PostgreSQL

# Kill process
kill -9 <PID>

# Or use kill-port (npm package)
npx kill-port 3000
```

#### Docker Container Health Check Failing

**Symptom:** Container keeps restarting

**Diagnosis:**

```bash
# Check container status
docker-compose ps

# View health check details
docker inspect nexora-postgres | grep -A 10 Health

# Check logs
docker-compose logs postgres
```

**Solutions:**

```bash
# Restart containers
docker-compose restart

# Rebuild from scratch
docker-compose down -v
docker-compose up --build

# Check for port conflicts
docker ps
```

#### Database Connection Issues

**Symptom:** "Cannot connect to database server"

**Solutions:**

**1. Verify PostgreSQL is running:**

```bash
docker-compose ps postgres

# Or if running standalone
docker ps | grep postgres
```

**2. Check connection string:**

```bash
# Test connection
docker-compose exec postgres psql -U nexora -d nexora_dev -c "SELECT 1"
```

**3. Check network:**

```bash
# Verify backend can reach postgres
docker-compose exec backend ping postgres
```

**4. Review logs:**

```bash
docker-compose logs postgres
docker-compose logs backend
```

#### Backend Not Starting

**Symptom:** Backend exits immediately or crashes

**Diagnosis:**

```bash
# Check logs
docker-compose logs backend

# Or if running locally
cd apps/backend
dotnet run --project src/Nexora.Management.API
```

**Common Causes:**

1. **Missing connection string:**
   - Verify `appsettings.json` has ConnectionStrings
   - Check environment variables

2. **Database not ready:**
   - Ensure PostgreSQL is healthy
   - Check `depends_on` conditions

3. **Port conflict:**
   - Change port in docker-compose.yml
   - Or stop conflicting service

4. **Migration not applied:**
   ```bash
   cd apps/backend
   dotnet ef database update
   ```

#### Frontend Build Errors

**Symptom:** Build fails with TypeScript/ESLint errors

**Solutions:**

**1. Clear cache and reinstall:**

```bash
cd apps/frontend
rm -rf node_modules .next
rm package-lock.json
npm install
```

**2. Fix linting issues:**

```bash
npm run lint -- --fix
npm run format
```

**3. Check TypeScript errors:**

```bash
npm run type-check
```

#### Hot Reloading Not Working

**Symptom:** Changes not reflected in browser

**Solutions:**

**Frontend:**

```bash
# Ensure using dev mode
npm run dev

# Check for .env.local changes (requires restart)
# Clear cache
rm -rf .next
npm run dev
```

**Backend:**

```bash
# In development mode, use hot reload
dotnet watch --project src/Nexora.Management.API
```

#### npm Install Fails

**Symptom:** "ENOSPC", "EACCES", or network errors

**Solutions:**

```bash
# Clear npm cache
npm cache clean --force

# Increase file watchers (Linux/Mac)
echo fs.inotify.max_user_watches=524288 | sudo tee -a /etc/sysctl.conf
sudo sysctl -p

# Use legacy peer deps
npm install --legacy-peer-deps

# Or use yarn
yarn install
```

#### dotnet Restore Fails

**Symptom:** "Unable to load the service index"

**Solutions:**

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore with verbose output
dotnet restore --verbosity detailed

# Check NuGet.config
cat ~/.nuget/NuGet/NuGet.Config

# Use alternative package source
dotnet restore --source https://api.nuget.org/v3/index.json
```

### Performance Issues

#### Slow Build Times

**Solutions:**

```bash
# Use Turborepo caching
npm run build

# Use Docker buildkit
export DOCKER_BUILDKIT=1
docker-compose build

# Parallel builds in docker-compose
docker-compose build --parallel
```

#### High Memory Usage

**Solutions:**

```bash
# Limit container resources
docker-compose up -d --scale backend=2

# Or update docker-compose.yml
services:
  backend:
    deploy:
      resources:
        limits:
          memory: 512M
```

### Debug Mode

#### Enable Debug Logging

**Backend:**

```json
// appsettings.Development.json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

**Frontend:**

```bash
# .env.local
DEBUG=nexora:*
```

#### Attach Debugger

**Backend (VS Code):**

```json
// .vscode/launch.json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/apps/backend/src/Nexora.Management.API/bin/Debug/net9.0/Nexora.Management.API.dll",
      "args": [],
      "cwd": "${workspaceFolder}/apps/backend/src/Nexora.Management.API",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      }
    }
  ]
}
```

**Frontend:**

```bash
# Run with inspect flag
NODE_OPTIONS='--inspect' npm run dev

# Then attach debugger in VS Code or Chrome DevTools
```

## Production Deployment Checklist

### Pre-Deployment

- [ ] All environment variables configured (especially JWT settings)
- [ ] Database migrations applied
- [ ] SSL certificates configured
- [ ] CORS settings updated for production domain
- [ ] Logging and monitoring configured
- [ ] Backup strategy in place
- [ ] Health check endpoints configured
- [ ] Secret management configured (JWT secret must be secure)
- [ ] CDN configured for static assets
- [ ] Database connection pooling optimized

**Security Note:** Ensure JWT secret is at least 32 characters and stored securely in production (never commit to git). Use environment variables or secret management services.

### Post-Deployment

- [ ] Verify all services are healthy
- [ ] Test critical user flows
- [ ] Check application logs for errors
- [ ] Monitor resource usage
- [ ] Verify backup automation
- [ ] Test rollback procedure

---

**Documentation Version:** 1.0
**Maintained By:** Development Team
