# Docker Development Environment

This directory contains Docker Compose configuration for local development of the Nexora Management application.

## Services

The stack includes the following services:

- **PostgreSQL 16** - Database (port 5432)
- **Redis 7** - Cache and message broker (port 6379)
- **Backend API** - .NET 9.0 API (port 5000)
- **Frontend** - Next.js application (port 3000)

## Quick Start

### Prerequisites

- Docker Desktop or Docker Engine installed
- Docker Compose plugin available

### Start All Services

```bash
docker compose -f docker/docker-compose.yml up -d
```

This will:
1. Build Docker images for backend and frontend
2. Start PostgreSQL and Redis
3. Start backend API (waits for DB)
4. Start frontend (waits for backend)

### With Development Override

For development with hot reload:

```bash
docker compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml up -d
```

## Service URLs

Once started, services are available at:

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **API Swagger**: http://localhost:5000/swagger
- **PostgreSQL**: localhost:5432
  - User: `nexora`
  - Password: `nexora_dev`
  - Database: `nexora_dev`
- **Redis**: localhost:6379
  - Password: `nexora_dev`

## Common Commands

### View Logs

```bash
# All services
docker compose -f docker/docker-compose.yml logs -f

# Specific service
docker compose -f docker/docker-compose.yml logs -f backend
docker compose -f docker/docker-compose.yml logs -f frontend
```

### Stop Services

```bash
docker compose -f docker/docker-compose.yml down
```

### Stop and Remove Volumes

```bash
docker compose -f docker/docker-compose.yml down -v
```

### Rebuild Services

```bash
docker compose -f docker/docker-compose.yml up -d --build
```

### Restart Specific Service

```bash
docker compose -f docker/docker-compose.yml restart backend
```

### Execute Commands in Container

```bash
# Backend shell
docker compose -f docker/docker-compose.yml exec backend sh

# PostgreSQL CLI
docker compose -f docker/docker-compose.yml exec postgres psql -U nexora -d nexora_dev

# Redis CLI
docker compose -f docker/docker-compose.yml exec redis redis-cli -a nexora_dev
```

## Development Workflow

### Backend Development

1. Make changes to backend code
2. Backend service uses volume mounts in override file
3. For changes to take effect, restart backend:
   ```bash
   docker compose -f docker/docker-compose.yml restart backend
   ```

### Frontend Development

1. Make changes to frontend code
2. Next.js hot reload will pick up changes automatically
3. Volume mounts ensure changes reflect in container

### Database Migrations

Migrations should be applied via backend:

```bash
docker compose -f docker/docker-compose.yml exec backend dotnet ef database update
```

## Troubleshooting

### Service Won't Start

1. Check logs: `docker compose -f docker/docker-compose.yml logs [service-name]`
2. Verify ports aren't already in use
3. Ensure Docker has enough resources allocated

### Database Connection Issues

1. Check PostgreSQL is healthy:
   ```bash
   docker compose -f docker/docker-compose.yml ps postgres
   ```
2. Verify connection string in docker-compose.yml

### Volume Issues

To reset all data:
```bash
docker compose -f docker/docker-compose.yml down -v
docker compose -f docker/docker-compose.yml up -d
```

## Production Deployment

For production, do NOT use docker-compose.override.yml. Ensure:
- Environment variables override defaults
- Secrets are properly managed
- Resource limits are configured
- Proper logging and monitoring

## File Structure

```
docker/
├── docker-compose.yml           # Main compose file
├── docker-compose.override.yml  # Development overrides
├── postgres-init/              # Database initialization scripts
│   └── 01-init.sql
└── README.md                   # This file

Root:
├── Dockerfile.backend          # .NET API image
├── Dockerfile.frontend         # Next.js image
└── .dockerignore              # Docker build exclusions
```

## Health Checks

All services include health checks:
- PostgreSQL: pg_isready every 10s
- Redis: PING every 10s
- Backend: HTTP /health every 30s
- Frontend: HTTP /api/health every 30s

Check health status:
```bash
docker compose -f docker/docker-compose.yml ps
```
