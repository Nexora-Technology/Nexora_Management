# Frontend-Backend API Connection Issues - Analysis Report

**Report ID:** debugger-260107-2317-api-connection-issues
**Date:** 2026-01-07
**Severity:** HIGH
**Status:** Analysis Complete

---

## Executive Summary

Critical configuration mismatches found between frontend and backend API connection settings across multiple configuration files. The frontend is attempting to connect to port 5000, but the backend is exposed on port 5001, causing connection failures.

**Root Cause:** Inconsistent `NEXT_PUBLIC_API_URL` configuration across Docker Compose files, documentation, and source code.

**Impact:** Frontend cannot communicate with backend API, breaking all API-dependent functionality.

**Priority:** CRITICAL - Immediate fix required for application functionality.

---

## Current State Analysis

### Container Configuration

**Backend Container:**
- Container port: `8080` (internal)
- Host port mapping: `5001:8080`
- Status: ✅ Healthy
- Health check: `http://localhost:8080/health` (internal)
- External access: `http://localhost:5001`

**Frontend Container:**
- Container port: `3000` (internal)
- Host port mapping: `3000:3000`
- Status: ⚠️ Unhealthy
- Environment variable: `NEXT_PUBLIC_API_URL=http://localhost:5000` ❌

### Configuration File Audit

#### ✅ CORRECT Configurations

1. **`docker/docker-compose.yml` (Production):**
   ```yaml
   backend:
     ports:
       - "5001:8080"  ✅ CORRECT
   frontend:
     environment:
       NEXT_PUBLIC_API_URL: http://backend:8080  ✅ CORRECT (Docker network)
   ```

#### ❌ INCORRECT Configurations

1. **`docker/docker-compose.override.yml` (Development):**
   ```yaml
   frontend:
     environment:
       NEXT_PUBLIC_API_URL: http://localhost:5000  ❌ WRONG PORT
   ```
   **Issue:** Should be `http://localhost:5001` or `http://backend:8080`

2. **`apps/frontend/.env.local` (Local Development):**
   ```bash
   NEXT_PUBLIC_API_URL=http://localhost:5000  ❌ WRONG PORT
   ```
   **Issue:** Should be `http://localhost:5001`

3. **`apps/frontend/src/lib/api-client.ts` (Fallback):**
   ```typescript
   const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
   ```
   **Issue:** Default fallback should be `http://localhost:5001`

4. **Documentation Inconsistencies:**
   - `README.md`: `NEXT_PUBLIC_API_URL=http://localhost:5000` ❌
   - `docs/development/local-setup.md`: `http://localhost:5000` ❌
   - `docs/deployment-guide.md`: `http://localhost:5001` ✅ (CORRECT)

---

## Technical Analysis

### Network Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      Host Machine                            │
│                                                              │
│  ┌────────────────────────────────────────────────────┐    │
│  │              Docker Network (nexora-network)        │    │
│  │                                                      │    │
│  │  ┌──────────────┐         ┌──────────────┐         │    │
│  │  │   Frontend   │────────▶│   Backend    │         │    │
│  │  │   Container  │ :3000   │  Container   │ :8080   │    │
│  │  └──────────────┘         └──────────────┘         │    │
│  │         │                         │                 │    │
│  └─────────┼─────────────────────────┼─────────────────┘    │
│            │ 3000                    │ 8080                 │
│            ▼                         ▼                      │
│       Port Mapping              Port Mapping               │
│       3000:3000                 5001:8080                  │
│            │                         │                      │
│            ▼                         ▼                      │
│     http://localhost:3000    http://localhost:5001         │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Connection Scenarios

#### Scenario 1: Docker Container-to-Container (Production)
**Configuration:** `docker-compose.yml`
- Frontend env: `NEXT_PUBLIC_API_URL=http://backend:8080`
- ✅ **CORRECT** - Uses Docker network service name
- Backend accessible at: `http://backend:8080` (internal DNS)

#### Scenario 2: Docker Development with Override
**Configuration:** `docker-compose.yml` + `docker-compose.override.yml`
- Frontend env: `NEXT_PUBLIC_API_URL=http://localhost:5000`
- ❌ **WRONG** - Port 5000 doesn't exist
- Should be: `http://backend:8080` (Docker network) OR `http://localhost:5001` (host access)

#### Scenario 3: Local Development (Frontend on Host)
**Configuration:** `.env.local`
- Frontend env: `NEXT_PUBLIC_API_URL=http://localhost:5000`
- ❌ **WRONG** - Backend container exposed on 5001
- Should be: `http://localhost:5001`

#### Scenario 4: Build-Time Configuration
**Configuration:** Next.js build process
- Build time: Environment variables baked into build
- Runtime: Server-side uses process.env, client-side uses window.env
- ❌ **ISSUE** - Wrong port baked into build

---

## Root Cause Identification

### Primary Issues

1. **Port Mismatch:**
   - Backend exposed: `5001:8080`
   - Frontend configured: `http://localhost:5000`
   - Gap: Port 5000 vs 5001

2. **Configuration Hierarchy Confusion:**
   - Docker Compose base: ✅ Correct (`http://backend:8080`)
   - Docker Compose override: ❌ Wrong (`http://localhost:5000`)
   - Override takes precedence in development

3. **Documentation Out of Sync:**
   - Multiple docs reference port 5000
   - Only deployment-guide.md correctly references 5001
   - Creates developer confusion

4. **Missing Environment-Specific Configs:**
   - No `.env.development`, `.env.production` files
   - Single `.env.local` used for all scenarios
   - No build-time vs runtime distinction

---

## Recommended Fix Strategy

### Option A: Unified Port Configuration (RECOMMENDED)

**Approach:** Standardize all configurations to use correct ports

**Changes Required:**

1. **`docker/docker-compose.override.yml`:**
   ```yaml
   frontend:
     environment:
       - NEXT_PUBLIC_API_URL=http://backend:8080  # Docker network
   ```

2. **`apps/frontend/.env.local`:**
   ```bash
   NEXT_PUBLIC_API_URL=http://localhost:5001  # Host access to backend container
   ```

3. **`apps/frontend/src/lib/api-client.ts`:**
   ```typescript
   const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5001";
   ```

4. **`README.md`:**
   ```markdown
   # Backend API: http://localhost:5001
   NEXT_PUBLIC_API_URL=http://localhost:5001
   ```

5. **`docs/development/local-setup.md`:**
   ```markdown
   # Backend API: http://localhost:5001
   NEXT_PUBLIC_API_URL=http://localhost:5001
   ```

**Pros:**
- Clear, consistent configuration
- Matches actual backend port mapping
- Works across all scenarios

**Cons:**
- Requires multiple file updates
- Documentation changes needed

---

### Option B: Docker Network Only (ALTERNATIVE)

**Approach:** Always use Docker network service names

**Changes Required:**

1. **`docker/docker-compose.override.yml`:**
   ```yaml
   frontend:
     environment:
       - NEXT_PUBLIC_API_URL=http://backend:8080
   ```

2. **`apps/frontend/.env.local`:**
   ```bash
   NEXT_PUBLIC_API_URL=http://backend:8080
   ```

3. Add `extra_hosts` to docker-compose for local dev:
   ```yaml
   frontend:
     extra_hosts:
       - "backend:host-gateway"
   ```

**Pros:**
- Consistent with production
- No port numbers in config
- More resilient to port changes

**Cons:**
- Requires extra_hosts configuration
- More complex for local development
- Doesn't work when frontend runs on host

---

### Option C: Environment-Specific Configs (ROBUST)

**Approach:** Separate configs for each environment

**Changes Required:**

1. **Create `apps/frontend/.env.development`:**
   ```bash
   NEXT_PUBLIC_API_URL=http://localhost:5001
   ```

2. **Create `apps/frontend/.env.production`:**
   ```bash
   NEXT_PUBLIC_API_URL=http://backend:8080
   ```

3. **Update `next.config.ts`:**
   ```typescript
   const nextConfig: NextConfig = {
     output: 'standalone',
     env: {
       NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL,
     },
   };
   ```

4. **Update Dockerfiles to use appropriate env files:**

**Pros:**
- Clear separation of concerns
- Each environment optimized
- Follows Next.js best practices

**Cons:**
- More files to maintain
- Requires build process changes
- Higher complexity

---

## Implementation Plan

### Phase 1: Immediate Fix (Option A)

**Priority:** CRITICAL
**Timeline:** Immediate
**Changes:**

1. Update `docker/docker-compose.override.yml`:
   - Change `NEXT_PUBLIC_API_URL` to `http://backend:8080`

2. Update `apps/frontend/.env.local`:
   - Change `NEXT_PUBLIC_API_URL` to `http://localhost:5001`

3. Update `apps/frontend/src/lib/api-client.ts`:
   - Change default fallback to `http://localhost:5001`

4. Rebuild and restart:
   ```bash
   docker-compose down
   docker-compose build frontend
   docker-compose up -d
   ```

5. Verify connection:
   ```bash
   curl http://localhost:3000  # Frontend
   curl http://localhost:5001/health  # Backend
   ```

### Phase 2: Documentation Update

**Priority:** HIGH
**Timeline:** Within 24 hours

1. Update `README.md` with correct ports
2. Update `docs/development/local-setup.md`
3. Add architecture diagram to docs
4. Update troubleshooting sections

### Phase 3: Long-Term Improvement (Option C)

**Priority:** MEDIUM
**Timeline:** Next sprint

1. Implement environment-specific configs
2. Add configuration validation
3. Update CI/CD pipelines
4. Add integration tests for API connectivity

---

## Testing Strategy

### Pre-Deployment Testing

1. **Docker Network Test:**
   ```bash
   docker exec nexora-frontend wget -qO- http://backend:8080/health
   ```

2. **Host Access Test:**
   ```bash
   curl http://localhost:5001/health
   ```

3. **Frontend API Client Test:**
   - Check browser DevTools Network tab
   - Verify API requests reach correct endpoint
   - Check SignalR hub connections

### Post-Deployment Verification

1. **Health Checks:**
   - Backend: `http://localhost:5001/health` ✅
   - Frontend: `http://localhost:3000` ✅
   - Docker health status: `docker ps`

2. **Functional Tests:**
   - Login flow
   - API calls from browser
   - SignalR connections
   - WebSocket connections

3. **Container Logs:**
   ```bash
   docker logs nexora-frontend --tail 50
   docker logs nexora-backend --tail 50
   ```

---

## Prevention Measures

### Configuration Validation

1. **Add pre-commit hook:**
   ```bash
   # Validate NEXT_PUBLIC_API_URL consistency
   .claude/hooks/validate-api-url.sh
   ```

2. **Add CI/CD check:**
   ```yaml
   - name: Validate API Configuration
     run: |
       # Check port consistency across files
   ```

3. **Add integration test:**
   ```typescript
   // tests/integration/api-connection.test.ts
   describe('API Connection', () => {
     it('should connect to backend', async () => {
       const response = await fetch('/api/health');
       expect(response.ok).toBe(true);
     });
   });
   ```

### Documentation Standards

1. **Single Source of Truth:**
   - Maintain port configuration in one location
   - Reference from other docs
   - Use includes/cross-references

2. **Configuration Template:**
   ```markdown
   ## Current Port Configuration
   - Backend: 5001 (host), 8080 (container)
   - Frontend: 3000
   - PostgreSQL: 5432
   - Redis: 6379
   ```

3. **Change Management:**
   - Update docs when changing ports
   - Port change requires doc review
   - Include port config in release notes

---

## Unresolved Questions

1. **Why was port 5000 originally chosen?**
   - Historical context needed
   - May conflict with macOS AirPlay Receiver

2. **Are there other services using port 5000?**
   - Need to check system port usage
   - Consider port conflicts

3. **Should we standardize on different ports?**
   - Backend: 5000 → 5001 ✅ DONE
   - Consider frontend: 3000 → 3000 (no change)
   - Document port selection rationale

4. **How to handle future port changes?**
   - Need port change procedure
   - Impact analysis checklist
   - Rollback plan

5. **Should we use environment variables everywhere?**
   - Consider moving all config to env vars
   - Reduce hardcoded values
   - Improve portability

---

## Conclusion

The API connection issue stems from inconsistent port configuration across multiple files. The backend correctly exposes port 5001, but the frontend is configured to use port 5000.

**Immediate Action Required:**
1. Update `docker/docker-compose.override.yml` to use `http://backend:8080`
2. Update `.env.local` to use `http://localhost:5001`
3. Update `api-client.ts` fallback to `http://localhost:5001`
4. Rebuild and restart containers

**Long-term Improvements:**
1. Implement environment-specific configurations
2. Add configuration validation
3. Update all documentation
4. Establish change management process

---

## Appendix

### File Changes Checklist

- [ ] `docker/docker-compose.override.yml` - Change NEXT_PUBLIC_API_URL
- [ ] `apps/frontend/.env.local` - Change NEXT_PUBLIC_API_URL
- [ ] `apps/frontend/src/lib/api-client.ts` - Change default URL
- [ ] `README.md` - Update port references
- [ ] `docs/development/local-setup.md` - Update port references
- [ ] `docs/deployment-guide.md` - Verify consistency
- [ ] Add port configuration to architecture docs

### Related Reports

- `plans/reports/code-reviewer-260107-2302-docker-compose-testing.md`
- `plans/reports/debugger-260105-0155-sidebar-not-visible.md`

### Next Steps

1. **Immediate:** Implement Option A fixes
2. **Short-term:** Update documentation
3. **Medium-term:** Implement Option C (environment-specific configs)
4. **Long-term:** Add configuration validation and testing

---

**Report Generated:** 2026-01-07 23:17:00 UTC
**Analyst:** Debugger Subagent
**Status:** ✅ Analysis Complete - Awaiting Implementation
