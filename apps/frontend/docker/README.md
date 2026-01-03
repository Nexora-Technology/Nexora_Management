# Docker Development Environment

Quick Start:
```bash
docker compose -f docker/docker-compose.yml up -d
```

Services:
- Frontend: http://localhost:3000
- Backend: http://localhost:5000
- PostgreSQL: localhost:5432 (user: nexora, pass: nexora_dev, db: nexora_dev)
- Redis: localhost:6379 (password: nexora_dev)

Development mode (hot reload):
```bash
docker compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml up -d
```

Stop services:
```bash
docker compose -f docker/docker-compose.yml down
```
