# Nexora Management Platform - Tech Stack

**Last Updated:** 2026-01-03
**Target Scale:** <30 users
**Architecture:** Modular Monolith

---

## Overview

ClickUp-like project management platform with AI capabilities, built for rapid deployment using Docker Compose.

---

## Backend Stack

### Core Framework
- **.NET**: ASP.NET Core 10 (LTS)
- **API Pattern**: Minimal APIs (not MVC Controllers)
- **Architecture**: Clean Architecture with CQRS + MediatR
- **ORM**: Entity Framework Core 10
- **Database**: PostgreSQL 16+

### Key Libraries
| Component | Library | Purpose |
|-----------|---------|---------|
| Authentication | Microsoft.AspNetCore.Authentication.JwtBearer | JWT auth with refresh tokens |
| Authorization | Microsoft.AspNetCore.Authorization | RBAC + Resource-based permissions |
| Validation | FluentValidation | Request validation |
| Real-time | Microsoft.AspNetCore.SignalR | WebSocket collaboration |
| Background Jobs | Hangfire | Job scheduling, migrations |
| Caching | Microsoft.Extensions.Caching.StackExchangeRedis | Redis integration |
| Logging | Serilog | Structured logging |
| API Docs | Swashbuckle.AspNetCore | Swagger/OpenAPI |
| Rate Limiting | AspNetCoreRateLimit | API protection |

### Security
- **Authentication**: JWT with refresh token rotation
- **Authorization**: RBAC (Owner, Admin, Member, Guest) + Resource-level permissions
- **Data Isolation**: PostgreSQL Row-Level Security (RLS)
- **API Security**: Rate limiting, CORS, helmet headers

---

## Frontend Stack

### Core Framework
- **Framework**: Next.js 15 (App Router)
- **Language**: TypeScript 5.8+
- **State Management**:
  - Server state: React Query (TanStack Query)
  - Client state: Zustand
  - Form state: React Hook Form
- **Styling**: Tailwind CSS

### UI Libraries
| Component | Library | Purpose |
|-----------|---------|---------|
| UI Components | shadcn/ui | Accessible, customizable components |
| Icons | Lucide React | Lightweight icon library |
| Forms | React Hook Form + Zod | Form validation |
| Data Table | TanStack Table | Advanced filtering, sorting |
| Real-time | @microsoft/signalr | SignalR client |
| Charts | Recharts | Analytics dashboards |
| Rich Text Editor | Tiptap | Collaborative docs |
| Drag & Drop | @dnd-kit | Board view, task reordering |

### Developer Experience
- **Linting**: ESLint + TypeScript ESLint
- **Formatting**: Prettier
- **Build Tool**: Turbopack (Next.js built-in)
- **Monorepo**: Turborepo (if needed for scale)

---

## Database

### PostgreSQL 16+
- **Schema Design**: 12 core tables (Users, Workspaces, Projects, Tasks, Comments, Attachments, etc.)
- **Flexible Data**: JSONB columns for custom fields, settings
- **Performance**:
  - Recursive CTEs for task hierarchies
  - GIN indexes for JSONB
  - Materialized views for dashboard queries
- **Security**: Row-Level Security (RLS) for multi-tenant isolation
- **Monitoring**: pg_stat_statements for query performance

### Redis
- **Use Cases**:
  - Distributed caching (Cache-Aside pattern)
  - Session storage
  - SignalR backplane (if multi-server)
  - Rate limiting counters

---

## AI Integration

### Google Gemini API
- **Models**:
  - **Gemini 2.5 Pro**: Complex reasoning, planning ($1.25/1M tokens)
  - **Gemini 2.5 Flash**: Fast, cost-effective ($0.075/1M tokens)
- **Features**:
  - Task suggestions & prioritization
  - Smart summarization (tasks, projects, comments)
  - AI chatbot with RAG (1M token context)
  - Subtask generation from parent tasks
  - Sentiment analysis on team communications
  - Content generation assistance
- **Architecture**: Backend proxy → Gemini API (never expose API keys in frontend)
- **Cost Optimization**:
  - Context caching for repeated operations (90% savings)
  - Smart routing: Flash → Pro based on complexity
  - Estimated cost: $1-10/month for 1,000 operations

---

## DevOps & Infrastructure

### Container Orchestration
- **Runtime**: Docker Compose (production-ready)
- **Containers**:
  - .NET Backend API
  - Next.js Frontend + Nginx
  - PostgreSQL 16
  - Redis 7
  - Traefik (reverse proxy + SSL)

### Reverse Proxy & SSL
- **Traefik**: Auto-discovery, Let's Encrypt SSL
- **Networks**: Isolated (proxy + internal)
- **Security**: HTTP-to-HTTPS redirect, rate limiting

### Configuration
- **Environment**: Docker Secrets for sensitive data
- **Environments**: `.dev.yml`, `.prod.yml`, `.staging.yml`
- **Health Checks**: All services (30s intervals, 3 retries)

### Deployment Strategy
- **CI/CD**: GitHub Actions
- **Zero-downtime**: `docker compose pull && docker compose up -d`
- **Backups**: Automated pg_dump cron container
- **Monitoring**: Serilog + SEQ (optional)

---

## File Storage

### Recommended: Cloud Storage (Production)
- **Azure Blob Storage** (if using Azure)
- **AWS S3** (if using AWS)
- **MinIO** (self-hosted S3-compatible)
- **Access**: Signed URLs with expiration

### Alternative: Local Storage (Development)
- **Path**: `/var/www/uploads`
- **Access**: Static file middleware with auth check

---

## Monitoring & Observability

### Application Monitoring
- **Logging**: Serilog (file + console)
- **Metrics**: OpenTelemetry (optional)
- **Health Checks**: ASP.NET Core Health Checks UI
- **Performance**: MiniProfiler (dev only)

### Database Monitoring
- **PostgreSQL**: pg_stat_statements
- **Redis**: INFO command monitoring
- **Slow Queries**: log_min_duration_statement = 100ms

---

## Development Tools

### IDE & Editor
- **Recommended**: Visual Studio 2022 / JetBrains Rider
- **Alternative**: VS Code with C# Dev Kit

### Version Control
- **Git**: GitHub / GitLab / Azure DevOps
- **Branch Strategy**: GitFlow or trunk-based

### API Testing
- **Swagger UI**: Built-in at `/swagger`
- **Postman/Insomnia**: For manual testing
- **RestClient**: VS Code extension

---

## Migration Strategy

### Jira Data Center
- **Format**: ZIP backup (entities.xml, activeobjects.xml)
- **Parser**: Custom `System.Xml.Linq` parser
- **Migration Tool**: Separate console application
- **Validation**: Pre/post-migration checks

### Phased Approach
1. **Phase 1**: Users, projects, basic tasks
2. **Phase 2**: Comments, attachments, custom fields
3. **Phase 3**: Workflows, sprints, advanced features

---

## Security Best Practices

### Application Security
- ✅ Non-root containers (UID 1001)
- ✅ Read-only filesystems (where possible)
- ✅ Alpine base images (minimal attack surface)
- ✅ Secrets via Docker Secrets (not env vars)
- ✅ Rate limiting at proxy + app level
- ✅ HTTPS only (Let's Encrypt)

### Code Security
- ✅ Input validation (FluentValidation)
- ✅ SQL injection prevention (EF Core parameterized queries)
- ✅ XSS protection (Next.js automatic escaping)
- ✅ CSRF tokens (automatic in .NET)
- ✅ Content Security Policy (CSP) headers

### Dependencies
- ✅ Dependabot for vulnerability alerts
- ✅ Trivy scanning in CI/CD
- ✅ Regular package updates

---

## Performance Optimization

### Backend
- **Caching**: Redis for frequently accessed data
- **Database**:
  - Connection pooling (default: 100)
  - AsNoTracking() for read-only queries
  - Compiled queries for hot paths
- **API**: Response compression (gzip/brotli)
- **Background**: Hangfire for async operations

### Frontend
- **Code Splitting**: Next.js automatic route splitting
- **Image Optimization**: next/image automatic optimization
- **Bundle Size**: Webpack analyzer (monitor)
- **CDN**: Static assets via CDN (production)

---

## Timeline Estimate

### MVP (Core Features Only)
- **Duration**: 8-12 weeks
- **Features**: Tasks, Projects, Board View, Basic Auth, Jira Migration

### Full Platform (All Features)
- **Duration**: 16-24 weeks (4-6 months)
- **Features**: All ClickUp features + AI + Advanced Dashboards

### Deployment
- **Docker Compose Setup**: 1 week
- **Production Hardening**: 1-2 weeks

---

## Next Steps

1. ✅ Tech stack defined
2. ⏳ Create detailed implementation plan
3. ⏳ Design wireframes & UI/UX
4. ⏳ Build core features
5. ⏳ Deploy with Docker Compose

---

## Open Questions

1. **Multi-tenancy**: App-level isolation vs PostgreSQL RLS?
2. **Search**: PostgreSQL full-text search vs Elasticsearch?
3. **File Storage**: Cloud (S3/Azure) vs local MinIO?
4. **Mobile**: React Native app or PWA only?
5. **Backup**: Managed database service vs self-hosted with cron backups?

These will be resolved during implementation planning based on infrastructure preferences.
