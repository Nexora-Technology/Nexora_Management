# Nexora Management Backend

.NET 9.0 Web API following Clean Architecture principles for the Nexora Management System.

## Project Structure

```
src/
├── Nexora.Management.API/           # Presentation layer - Web API
├── Nexora.Management.Application/    # Application layer - Business logic, DTOs, MediatR
├── Nexora.Management.Domain/         # Domain layer - Entities, interfaces
└── Nexora.Management.Infrastructure/ # Infrastructure layer - DB, external services

tests/
└── Nexora.Management.Tests/         # Unit and integration tests
```

## Prerequisites

- .NET 9.0 SDK
- PostgreSQL 15+
- Redis 7+ (optional, for caching)
- Docker & Docker Compose (recommended for local development)

## Getting Started

### 1. Install Dependencies

```bash
cd apps/backend
dotnet restore
```

### 2. Configure Database

Update connection string in `src/Nexora.Management.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=nexora_management;Username=your_user;Password=your_password"
  }
}
```

Or use environment variables:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=nexora_management;Username=your_user;Password=your_password"
```

### 3. Run Database Migrations

```bash
cd src/Nexora.Management.API
dotnet ef database update
```

### 4. Run the API

```bash
dotnet run --project src/Nexora.Management.API
```

The API will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: https://localhost:5001 (root URL)

### 5. Using Docker

```bash
# Start PostgreSQL and Redis
docker-compose up -d

# Run the API
dotnet run --project src/Nexora.Management.API
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/Nexora.Management.Tests
```

## Configuration

### Environment Variables

Key environment variables (see `appsettings.json` for defaults):

- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `Redis__ConnectionString` - Redis connection string (optional)
- `ASPNETCORE_ENVIRONMENT` - Environment (Development, Production)
- `ASPNETCORE_URLS` - URLs to bind to

### CORS Configuration

Configure allowed origins in `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": [ "http://localhost:3000" ]
  }
}
```

## API Documentation

- Swagger UI is available at the root URL when running in Development mode
- OpenAPI specification: `/swagger/v1/swagger.json`

## Key Technologies

- **.NET 9.0** - Latest .NET platform
- **ASP.NET Core** - Web framework
- **Entity Framework Core 9** - ORM
- **PostgreSQL** - Primary database
- **Redis** - Caching layer
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Swagger** - API documentation

## Development Guidelines

### Clean Architecture Layers

1. **Domain Layer** - Core business logic, no dependencies
2. **Application Layer** - Use cases, DTOs, interfaces
3. **Infrastructure Layer** - External concerns (DB, services)
4. **API Layer** - Presentation, HTTP, routing

### Code Style

- Follow C# coding conventions
- Use XML comments for public APIs
- Enable nullable reference types
- Use pattern matching where appropriate
- Keep methods small and focused

### Adding New Features

1. Define domain entities in `Domain` layer
2. Create DTOs in `Application/Common`
3. Implement commands/queries with MediatR
4. Add validators using FluentValidation
5. Implement database access in `Infrastructure` layer
6. Create controllers/endpoints in `API` layer

## Troubleshooting

### Database Connection Issues

```bash
# Check PostgreSQL is running
docker ps | grep postgres

# Check connection string
echo $ConnectionStrings__DefaultConnection
```

### Port Already in Use

```bash
# Find process using port 5000
lsof -i :5000

# Kill process
kill -9 <PID>
```

### Migration Issues

```bash
# Create new migration
dotnet ef migrations add MigrationName --project src/Nexora.Management.Infrastructure

# Rollback migration
dotnet ef database update PreviousMigration
```

## Build & Deployment

```bash
# Build for release
dotnet build -c Release

# Publish self-contained
dotnet publish -c Release -o ./publish --self-contained

# Run published app
cd publish
./Nexora.Management.API
```

## Health Check

```bash
curl http://localhost:5000/health
```

Response:
```json
{
  "status": "healthy",
  "timestamp": "2026-01-03T10:30:00Z",
  "version": "1.0.0"
}
```

## License

Proprietary - Nexora Management System
