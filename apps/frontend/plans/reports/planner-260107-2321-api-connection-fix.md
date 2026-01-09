# Planner Report: API Connection Port Mismatch Fix

**Date:** 2026-01-07 23:21
**Issue:** Frontend-backend API connection failures due to port mismatch
**Severity:** P1 - Critical (blocking all API communication)
**Status:** Plan created, ready for implementation

---

## Summary

Root cause: Port mismatch across 3 configuration files causing frontend to call port 5000 while backend listens on port 5001.

**Scope:**
- 3 file changes required
- 2 environment-specific configurations
- 1 service restart
- 5 verification tests

**Estimated Effort:** 1 hour

---

## Plan Created

**Location:** `plans/260107-2321-api-connection-fix/plan.md`

**Key Changes:**

1. `docker/docker-compose.override.yml` (Line 26)
   - Change: `http://localhost:5000` → `http://backend:8080`
   - Context: Docker internal network

2. `apps/frontend/.env.local`
   - Change: `http://localhost:5000` → `http://localhost:5001`
   - Context: Local development

3. `apps/frontend/src/lib/api-client.ts` (Line 3)
   - Change: Fallback URL `http://localhost:5000` → `http://localhost:5001`
   - Context: Build-time default

---

## Implementation Phases

### Phase 1-3: File Updates (15 min)
- Update Docker environment variable
- Update local .env file
- Update API client fallback

### Phase 4: Service Restart (10 min)
- Rebuild and restart Docker services

### Phase 5: Verification (15 min)
- Container connectivity test
- Host machine test
- Browser API call test
- Console error check

---

## Testing Approach

**Layer 1: Container-to-Container**
```bash
docker exec nexora-frontend wget -qO- http://backend:8080/health
```

**Layer 2: Host-to-Container**
```bash
curl http://localhost:5001/health
```

**Layer 3: Browser-to-API**
- DevTools Network tab verification
- Console error inspection

---

## Rollback Strategy

All changes tracked in Git:
```bash
git checkout HEAD -- docker/docker-compose.override.yml \
                          apps/frontend/.env.local \
                          apps/frontend/src/lib/api-client.ts
```

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Cache issues | Clear `.next`, rebuild containers |
| CORS errors | Verify backend CORS configuration |
| Port conflicts | Check port availability before start |
| Env var loading | Verify with `docker compose config` |

---

## Success Criteria

- ✅ All 3 files updated
- ✅ Docker services healthy
- ✅ Container connectivity working
- ✅ Browser API calls successful
- ✅ No console errors
- ✅ Health check returns 200 OK

---

## Next Steps

1. Review plan at `plans/260107-2321-api-connection-fix/plan.md`
2. Approve plan for implementation
3. Execute phases 1-5 sequentially
4. Run verification matrix
5. Update documentation

---

## Unresolved Questions

1. Backend health check endpoint path?
2. CORS configuration for Docker + localhost?
3. Production API URLs for future deployment?

---

**Report Generated:** 2026-01-07 23:21
**Planner:** planner subagent
**Plan Status:** Ready for implementation
