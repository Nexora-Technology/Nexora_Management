---
title: "Fix Frontend-Backend API Connection Port Mismatch"
description: "Resolve port mismatch between frontend (5000) and backend (5001) causing API connection failures"
status: pending
priority: P1
effort: 1h
branch: main
tags: [bugfix, api, docker, configuration]
created: 2026-01-07
---

## Executive Summary

**Issue:** Frontend configured for port 5000, backend runs on port 5001
**Impact:** API calls failing with connection refused errors
**Root Cause:** Port mismatch across 3 configuration files
**Fix:** Update all configurations to use consistent port 5001

---

## Current State Analysis

### Port Mappings

| Component | Current Port | Expected Port | Status |
|-----------|--------------|---------------|--------|
| Backend (host) | 5001 | 5001 | ✅ Correct |
| Backend (container) | 8080 | 8080 | ✅ Correct |
| Frontend (host) | 3000 | 3000 | ✅ Correct |
| Frontend API URL | 5000 | 5001 | ❌ **MISMATCH** |

### Configuration Issues

1. **docker-compose.override.yml** (Line 26)
   ```yaml
   NEXT_PUBLIC_API_URL: http://localhost:5000  # ❌ Wrong port
   ```

2. **apps/frontend/.env.local**
   ```env
   NEXT_PUBLIC_API_URL=http://localhost:5000   # ❌ Wrong port
   ```

3. **apps/frontend/src/lib/api-client.ts** (Line 3)
   ```typescript
   const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
   # ❌ Wrong fallback
   ```

---

## Target Configuration

### Environment-Specific URLs

| Environment | API URL | Network Context |
|-------------|---------|-----------------|
| **Docker** | `http://backend:8080` | Docker internal network |
| **Local Dev** | `http://localhost:5001` | Host machine loopback |
| **Fallback** | `http://localhost:5001` | Build-time default |

---

## Implementation Plan

### Phase 1: Docker Configuration (5 min)

**File:** `docker/docker-compose.override.yml`

**Change:** Line 26
```yaml
# Before
NEXT_PUBLIC_API_URL: http://localhost:5000

# After
NEXT_PUBLIC_API_URL: http://backend:8080
```

**Rationale:**
- Containers communicate via Docker network
- Service name `backend` resolves to backend container
- Port 8080 is internal container port (mapped 5001:8080 externally)

---

### Phase 2: Local Development Environment (5 min)

**File:** `apps/frontend/.env.local`

**Change:** API URL port
```env
# Before
NEXT_PUBLIC_API_URL=http://localhost:5000

# After
NEXT_PUBLIC_API_URL=http://localhost:5001
```

**Rationale:**
- Local development runs directly on host machine
- Backend accessible via localhost:5001 (external port mapping)
- Matches docker-compose.yml port configuration: `"5001:8080"`

---

### Phase 3: API Client Fallback (5 min)

**File:** `apps/frontend/src/lib/api-client.ts`

**Change:** Default fallback URL
```typescript
// Before
const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

// After
const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5001";
```

**Rationale:**
- Provides build-time fallback if env var missing
- Matches local development port
- Defensive programming for missing configuration

---

### Phase 4: Service Restart (10 min)

**Steps:**
1. Stop all services: `docker compose down`
2. Rebuild frontend (env change): `docker compose build frontend`
3. Start all services: `docker compose up -d`
4. Wait for health checks: `docker compose ps`

**Commands:**
```bash
cd /Users/nhatduyfirst/Documents/Projects/Nexora_Management/docker
docker compose down
docker compose build frontend
docker compose up -d
```

---

### Phase 5: Verification (15 min)

#### 5.1 Container Connectivity Test
```bash
# Test from frontend container
docker exec nexora-frontend wget -qO- http://backend:8080/health || echo "FAIL"

# Expected: HTTP 200 or health check JSON
```

#### 5.2 Host Machine Test
```bash
# Test from host
curl http://localhost:5001/health || echo "FAIL"

# Expected: HTTP 200 or health check JSON
```

#### 5.3 Frontend Browser Test
1. Open http://localhost:3000
2. Open DevTools → Network tab
3. Trigger API call (login, fetch tasks, etc.)
4. Verify requests go to correct URL:
   - Docker: `http://backend:8080/api/*`
   - Local: `http://localhost:5001/api/*`

#### 5.4 Console Error Check
- Check browser console for CORS errors
- Check for connection refused errors
- Verify API responses return data

---

## File Change Checklist

### Files to Modify
- [ ] `docker/docker-compose.override.yml` - Line 26 (Docker env)
- [ ] `apps/frontend/.env.local` - API URL (local dev)
- [ ] `apps/frontend/src/lib/api-client.ts` - Line 3 (fallback)

### Files to Verify
- [ ] `docker/docker-compose.yml` - Port mapping (5001:8080)
- [ ] `apps/frontend/next.config.ts` - No API proxy conflicts
- [ ] `apps/frontend/src/lib/api-client.ts` - Interceptor logic

---

## Testing Matrix

| Scenario | API URL | Expected Result | Test Command |
|----------|---------|-----------------|--------------|
| Docker container → Backend | `http://backend:8080` | 200 OK | `docker exec nexora-frontend wget http://backend:8080/health` |
| Local dev → Backend | `http://localhost:5001` | 200 OK | `curl http://localhost:5001/health` |
| Browser → API | `http://localhost:3000` | No CORS errors | Manual DevTools check |
| Production build | Build-time fallback | Uses 5001 | `npm run build && npm start` |

---

## Rollback Procedures

### If Docker Environment Fails
```bash
# Revert docker-compose.override.yml
git checkout docker/docker-compose.override.yml
docker compose down
docker compose up -d
```

### If Local Dev Fails
```bash
# Revert .env.local
git checkout apps/frontend/.env.local
npm run dev
```

### If Build Fails
```bash
# Revert api-client.ts
git checkout apps/frontend/src/lib/api-client.ts
rm -rf .next
npm run build
```

### Complete Rollback
```bash
git checkout HEAD -- docker/docker-compose.override.yml \
                          apps/frontend/.env.local \
                          apps/frontend/src/lib/api-client.ts
docker compose down
docker compose up -d --force-recreate
```

---

## Success Criteria

- [ ] All 3 configuration files updated
- [ ] Docker services start without errors
- [ ] Container-to-container connectivity works
- [ ] Browser can access API without CORS errors
- [ ] API calls return valid data (not connection errors)
- [ ] Console shows no `ECONNREFUSED` errors
- [ ] Health check endpoint responds 200 OK

---

## Risk Assessment

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Breaking existing Docker setup | Low | High | Git rollback, backup compose files |
| CORS configuration issue | Low | Medium | Verify backend CORS allows frontend origin |
| Environment var not loading | Low | Medium | Test with `docker compose config` |
| Port 5001 already in use | Low | Low | Check with `lsof -i :5001` |
| Cache issues (.next, node_modules) | Medium | Low | Clear caches before testing |

---

## Troubleshooting Guide

### Issue: Still getting connection errors
**Steps:**
1. Clear Next.js cache: `rm -rf .next`
2. Rebuild container: `docker compose build --no-cache frontend`
3. Check environment inside container:
   ```bash
   docker exec nexora-frontend env | grep API_URL
   ```
4. Verify backend is running:
   ```bash
   docker compose ps backend
   ```

### Issue: CORS errors persist
**Check:**
1. Backend CORS configuration allows `http://localhost:3000`
2. Backend CORS allows `http://frontend:3000` (Docker)
3. Verify backend logs for CORS rejections

### Issue: Works in Docker, not local dev
**Cause:** `.env.local` not updated or not loaded
**Fix:**
```bash
# Verify .env.local content
cat apps/frontend/.env.local | grep API_URL
# Restart dev server
npm run dev
```

---

## Post-Implementation Tasks

### Documentation Updates
1. Update `README.md` with correct port references
2. Add troubleshooting section to `docs/deployment-guide.md`
3. Document environment-specific configurations

### Future Prevention
1. Add `.env.example` template file
2. Create pre-commit hook to validate port consistency
3. Add port validation to docker-compose health checks
4. Document port architecture in `docs/system-architecture.md`

---

## Unresolved Questions

1. **Backend CORS config:** Need to verify backend allows both Docker and localhost origins
2. **Health check endpoint:** Does backend have `/health` or `/api/health` endpoint?
3. **Production URLs:** What are production API URLs for staging/production environments?
4. **SSL/HTTPS:** When will SSL be required? How will it affect local dev?

---

## References

- Debugger Report: `plans/reports/debugger-260105-0155-sidebar-not-visible.md`
- Docker Compose: `docker/docker-compose.yml`
- Backend API: `../apps/backend/src/Nexora.Management.API/`
- Frontend Config: `apps/frontend/next.config.ts`

---

**Created:** 2026-01-07
**Planner:** planner subagent
**Status:** Ready for implementation
**Estimated Effort:** 1 hour (including testing)
