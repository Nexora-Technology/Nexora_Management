# Docker Actions for GitHub Actions

Docker container patterns and optimization for GitHub Actions.

## Docker Action Structure

```yaml
# action.yml
name: 'My Action'
description: 'Custom Docker action'
runs:
  using: 'docker'
  image: 'Dockerfile'
  entrypoint: 'entrypoint.sh'
```

```dockerfile
# Dockerfile
FROM node:20-alpine
COPY entrypoint.sh /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
```

## Building in Workflows

```yaml
- name: Build Docker image
  run: docker build -t myapp .

- name: Run container
  run: docker run myapp npm test
```

## Docker Compose Integration

```yaml
- name: Start services
  run: docker compose up -d postgres redis

- name: Run tests
  run: docker compose run --rm app npm test

- name: Stop services
  if: always()
  run: docker compose down
```

## Caching Docker Layers

```yaml
- name: Cache Docker layers
  uses: actions/cache@v4
  with:
    path: /tmp/.buildxcache
    key: ${{ runner.os }}-buildx-${{ github.sha }}
    restore-keys: |
      ${{ runner.os }}-buildx-
```

## Multi-stage Builds

```dockerfile
# Build stage
FROM node:20-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Production stage
FROM node:20-alpine AS runner
WORKDIR /app
COPY --from=builder /app/.next ./
CMD ["npm", "start"]
```

## Common Patterns

### Turborepo Build
```dockerfile
# Copy root package files
COPY package*.json ./
COPY apps/frontend/package*.json ./apps/frontend/

# Install with turborepo
RUN npm ci

# Build specific package
RUN npx turbo run build --filter=@nexora/frontend
```

### .NET Build
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["*.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "App.dll"]
```

## Optimization Tips

1. Use specific image tags (not `latest`)
2. Combine RUN commands to reduce layers
3. Use `.dockerignore` to exclude unnecessary files
4. Leverage BuildKit cache mounts
5. Use multi-stage builds for smaller final images
