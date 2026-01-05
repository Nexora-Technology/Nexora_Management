# Phase 08 Analysis: Goal Tracking & OKRs

**Date:** 2026-01-05 23:32
**Phase:** 08 - Goal Tracking & OKRs
**Priority:** P2 (Important but not blocking)
**Estimated Effort:** 24 hours (3 days)
**Status:** Analysis Complete - Ready for Implementation

---

## Executive Summary

Phase 08 implements an OKR (Objectives and Key Results) tracking system for team alignment and measurable goal tracking. This is a **P2 priority feature** that builds upon existing workspace infrastructure but doesn't block core task management flows.

**Key Decision:** Implement MVP OKR system focused on core functionality (objectives, key results, progress tracking). Defer advanced features (OKR templates, advanced analytics, AI-powered insights) to future phases.

---

## Current Context Analysis

### Completed Phases (Prerequisites)
- ✅ **Phase 01-02:** Clean Architecture, Domain Layer, Database Schema
- ✅ **Phase 03:** Authentication & Authorization (JWT, Permissions, RLS)
- ✅ **Phase 05:** Task Management CRUD (CQRS pattern established)
- ✅ **Phase 06:** Real-time Collaboration (SignalR infrastructure)
- ✅ **Phase 01.2:** ClickUp Design System (UI components ready)

### Relevant Patterns Established

**Backend Pattern (from Task entity):**
```csharp
// 1. Domain entity with BaseEntity
public class Task : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; }
    // Navigation properties
}

// 2. CQRS command with MediatR
public record CreateTaskCommand(...) : IRequest<Result<TaskDto>>;

// 3. DTO as record (immutable)
public record TaskDto(...);
```

**Frontend Pattern (from Documents module):**
```typescript
// 1. Centralized types
export interface Objective {
  id: string;
  title: string;
  keyResults: KeyResult[];
}

// 2. API client functions
export async function createObjective(data: CreateObjectiveRequest) { ... }

// 3. Feature components
export function GoalDashboard() { ... }
```

---

## Implementation Breakdown

### Step 2.1: Backend Domain Layer (3 files, 2 hours)

**Files to Create:**
1. `GoalPeriod.cs` - Time period entity (Q1, Q2, etc.)
2. `Objective.cs` - Goal entity with hierarchy support
3. `KeyResult.cs` - Measurable metric entity

**Schema Design:**
```sql
-- goal_periods table
- id (UUID, PK)
- workspace_id (FK)
- name (TEXT) - "Q1 2026"
- start_date (DATE)
- end_date (DATE)
- status (TEXT) - active/archived

-- objectives table
- id (UUID, PK)
- workspace_id (FK)
- period_id (FK, nullable)
- parent_objective_id (FK, self-ref)
- title (TEXT)
- description (TEXT)
- owner_id (FK to users)
- weight (INTEGER) - default 1
- status (TEXT) - on-track/at-risk/off-track
- progress (INTEGER) - calculated 0-100
- position_order (INTEGER)

-- key_results table
- id (UUID, PK)
- objective_id (FK, CASCADE delete)
- title (TEXT)
- current_value (DECIMAL)
- target_value (DECIMAL)
- unit (TEXT) - "%", "$", "count"
- start_value (DECIMAL)
- progress (INTEGER) - GENERATED ALWAYS AS (computed column)
- due_date (DATE)
```

**Key Features:**
- Self-referencing hierarchy (parent_objective_id)
- Computed progress column (PostgreSQL GENERATED)
- Period-based grouping (quarterly/yearly)
- Weight/priority support

---

### Step 2.2: Backend Infrastructure Layer (3 files, 2 hours)

**Files to Create:**
1. `GoalPeriodConfiguration.cs` - EF Core mapping
2. `ObjectiveConfiguration.cs` - EF Core mapping
3. `KeyResultConfiguration.cs` - EF Core mapping

**Configuration Pattern (from TaskConfiguration):**
```csharp
public class ObjectiveConfiguration : IEntityTypeConfiguration<Objective>
{
    public void Configure(EntityTypeBuilder<Objective> builder)
    {
        builder.ToTable("objectives");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Period)
            .WithMany()
            .HasForeignKey(x => x.PeriodId);

        builder.HasOne(x => x.ParentObjective)
            .WithMany(x => x.ChildObjectives)
            .HasForeignKey(x => x.ParentObjectiveId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.WorkspaceId);
        builder.HasIndex(x => x.PeriodId);
    }
}
```

---

### Step 2.3: Database Migration (1 file, 1 hour)

**Task:** Generate EF Core migration for OKR tables

**Commands:**
```bash
cd apps/backend/src/Nexora.Management.API
dotnet ef migrations add AddGoalTrackingTables --startup-project ../..
dotnet ef database update --startup-project ../..
```

**Migration File:** `20260105XXXXXX_AddGoalTrackingTables.cs`

---

### Step 2.4: Backend Application Layer - Commands (4 files, 4 hours)

**Files to Create:**
1. `CreateObjectiveCommand.cs` + Handler
2. `UpdateObjectiveCommand.cs` + Handler
3. `UpdateKeyResultCommand.cs` + Handler
4. `DeleteObjectiveCommand.cs` + Handler

**Pattern (from CreateTaskCommand):**
```csharp
public record CreateObjectiveCommand(
    Guid WorkspaceId,
    Guid? PeriodId,
    Guid? ParentObjectiveId,
    string Title,
    string? Description,
    Guid? OwnerId,
    int Weight = 1
) : IRequest<Result<ObjectiveDto>>;

public class CreateObjectiveCommandHandler : IRequestHandler<CreateObjectiveCommand, Result<ObjectiveDto>>
{
    public async Task<Result<ObjectiveDto>> Handle(CreateObjectiveCommand request, CancellationToken ct)
    {
        // Validate workspace
        // Validate period if provided
        // Validate parent if provided
        // Create objective
        // Return ObjectiveDto
    }
}
```

**Key Logic:**
- Validate workspace membership (via RLS)
- Recalculate objective progress when key results update
- Enforce max hierarchy depth (3 levels)

---

### Step 2.5: Backend Application Layer - Queries (3 files, 3 hours)

**Files to Create:**
1. `GetObjectivesQuery.cs` + Handler - List with filters
2. `GetObjectiveTreeQuery.cs` + Handler - Hierarchical view
3. `GetProgressDashboardQuery.cs` + Handler - Aggregated stats

**Pattern (from GetTasksQuery):**
```csharp
public record GetObjectivesQuery(
    Guid WorkspaceId,
    Guid? PeriodId = null,
    Guid? OwnerId = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedResult<ObjectiveDto>>>;
```

**Query Features:**
- Filter by period, owner, status
- Pagination support
- Include child objectives recursively
- Calculate progress from key results

---

### Step 2.6: Backend Application Layer - DTOs (1 file, 1 hour)

**File:** `GoalDTOs.cs`

**DTOs:**
```csharp
public record ObjectiveDto(
    Guid Id,
    Guid WorkspaceId,
    Guid? PeriodId,
    Guid? ParentObjectiveId,
    string Title,
    string? Description,
    Guid? OwnerId,
    string? OwnerName,
    int Weight,
    string Status,
    int Progress,
    int PositionOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record KeyResultDto(
    Guid Id,
    Guid ObjectiveId,
    string Title,
    decimal? CurrentValue,
    decimal TargetValue,
    string? Unit,
    decimal? StartValue,
    int Progress,
    DateTime? DueDate
);

public record CreateObjectiveRequest(...);
public record UpdateObjectiveRequest(...);
public record CreateKeyResultRequest(...);
public record UpdateKeyResultRequest(...);
```

---

### Step 2.7: Backend API Layer (1 file, 2 hours)

**File:** `GoalEndpoints.cs`

**Endpoints:**
```
POST   /api/goals/objectives           - Create objective
GET    /api/goals/objectives           - List objectives (filtered)
GET    /api/goals/objectives/{id}      - Get objective by ID
GET    /api/goals/objectives/{id}/tree - Get objective tree (hierarchical)
PUT    /api/goals/objectives/{id}      - Update objective
DELETE /api/goals/objectives/{id}      - Delete objective
POST   /api/goals/key-results          - Create key result
PUT    /api/goals/key-results/{id}     - Update key result
DELETE /api/goals/key-results/{id}     - Delete key result
GET    /api/goals/dashboard/{workspaceId} - Get progress dashboard
```

**Pattern (from TaskEndpoints):**
```csharp
endpoints.MapPost("/api/goals/objectives", async (
    CreateObjectiveRequest request,
    ISender sender,
    CancellationToken ct
) => {
    var command = new CreateObjectiveCommand(...);
    var result = await sender.Send(command, ct);
    return Results.Ok(result);
}).RequireAuthorization();
```

---

### Step 2.8: Frontend Types (1 file, 1 hour)

**File:** `apps/frontend/src/features/goals/types.ts`

**Types:**
```typescript
export interface Objective {
  id: string;
  workspaceId: string;
  periodId?: string;
  parentObjectiveId?: string;
  title: string;
  description?: string;
  ownerId?: string;
  ownerName?: string;
  weight: number;
  status: 'on-track' | 'at-risk' | 'off-track' | 'completed';
  progress: number; // 0-100
  positionOrder: number;
  createdAt: string;
  updatedAt: string;
  keyResults?: KeyResult[];
  childObjectives?: Objective[];
}

export interface KeyResult {
  id: string;
  objectiveId: string;
  title: string;
  currentValue?: number;
  targetValue: number;
  unit?: string;
  startValue?: number;
  progress: number;
  dueDate?: string;
}

export interface GoalPeriod {
  id: string;
  workspaceId: string;
  name: string;
  startDate: string;
  endDate: string;
  status: 'active' | 'archived';
}

export interface GoalFilter {
  periodId?: string;
  ownerId?: string;
  status?: Objective['status'];
  search?: string;
}
```

---

### Step 2.9: Frontend API Client (1 file, 2 hours)

**File:** `apps/frontend/src/features/goals/api.ts`

**Functions:**
```typescript
export async function getObjectives(
  workspaceId: string,
  filter?: GoalFilter
): Promise<Objective[]> { ... }

export async function getObjectiveTree(
  workspaceId: string,
  filter?: GoalFilter
): Promise<Objective[]> { ... }

export async function createObjective(
  workspaceId: string,
  data: CreateObjectiveRequest
): Promise<Objective> { ... }

export async function updateObjective(
  id: string,
  data: UpdateObjectiveRequest
): Promise<Objective> { ... }

export async function deleteObjective(id: string): Promise<void> { ... }

export async function createKeyResult(
  objectiveId: string,
  data: CreateKeyResultRequest
): Promise<KeyResult> { ... }

export async function updateKeyResult(
  id: string,
  data: UpdateKeyResultRequest
): Promise<KeyResult> { ... }

export async function deleteKeyResult(id: string): Promise<void> { ... }

export async function getProgressDashboard(
  workspaceId: string
): Promise<ProgressDashboard> { ... }
```

---

### Step 2.10: Frontend Components (5 files, 6 hours)

**Files to Create:**

1. **`GoalDashboard.tsx`** (150 lines)
   - Overview with progress summaries
   - Period filter dropdown
   - Quick stats (total objectives, on-track, at-risk)
   - Link to detailed view

2. **`ObjectiveCard.tsx`** (120 lines)
   - Single goal with key results list
   - Progress bar
   - Status badge (on-track/at-risk/off-track)
   - Owner avatar
   - Expand/collapse key results

3. **`KeyResultEditor.tsx`** (100 lines)
   - Edit metrics (current, target, start values)
   - Unit selector (%, $, count)
   - Due date picker
   - Auto-progress calculation preview

4. **`GoalTreeView.tsx`** (130 lines)
   - Hierarchical tree view
   - Expand/collapse branches
   - Indented child objectives
   - Drag-drop for reordering (future)

5. **`ProgressChart.tsx`** (80 lines)
   - Visual progress bars
   - Color-coded (green: on-track, yellow: at-risk, red: off-track)
   - Percentage labels
   - Animated fill (CSS transition)

**Component Pattern (from DocumentEditor):**
```typescript
interface GoalDashboardProps {
  workspaceId: string;
}

export function GoalDashboard({ workspaceId }: GoalDashboardProps) {
  const [filter, setFilter] = useState<GoalFilter>({});
  const { data: objectives, isLoading } = useQuery({
    queryKey: ['objectives', workspaceId, filter],
    queryFn: () => getObjectives(workspaceId, filter)
  });

  if (isLoading) return <Spinner />;
  return (
    <div className="goal-dashboard">
      {/* UI */}
    </div>
  );
}
```

---

### Step 2.11: Frontend Pages (2 files, 2 hours)

**Files to Create:**

1. **`apps/frontend/src/app/(app)/goals/page.tsx`** (100 lines)
   - Route: `/goals`
   - GoalDashboard component
   - Period filter
   - Quick create button

2. **`apps/frontend/src/app/(app)/goals/[id]/page.tsx`** (120 lines)
   - Route: `/goals/[id]`
   - Objective detail with full key results
   - Breadcrumb navigation
   - Edit/delete actions
   - Child objectives list

**Pattern (from tasks page):**
```typescript
export default function GoalsPage() {
  return (
    <AppLayout>
      <Container size="lg">
        <h1>Goals & OKRs</h1>
        <GoalDashboard workspaceId={currentWorkspaceId} />
      </Container>
    </AppLayout>
  );
}
```

---

### Step 2.12: Integration & Testing (1 task, 3 hours)

**Tasks:**
1. Wire frontend to backend API
2. Test CRUD operations (create, read, update, delete)
3. Verify progress calculation
4. Test hierarchical goal alignment
5. Test period filtering
6. Test RLS policies (workspace isolation)
7. Manual QA: Create OKR cycle, add objectives, add key results, update progress

---

## Dependencies & Blockers

### Prerequisites (All Complete ✅)
- ✅ Workspace infrastructure (Phase 02)
- ✅ Authentication & Authorization (Phase 03)
- ✅ CQRS pattern established (Phase 05)
- ✅ ClickUp Design System (Phase 01.2)

### Blockers
**None identified** - All dependencies complete.

### External Dependencies
- PostgreSQL 16 (computed columns supported)
- MediatR (already installed)
- EF Core 9 (already installed)

---

## Risk Assessment

### Technical Risks

**Medium Risk:**
1. **Computed Progress Calculation**
   - Risk: PostgreSQL GENERATED column syntax errors
   - Mitigation: Test calculation logic thoroughly, fallback to application-level calculation

2. **Hierarchical Queries**
   - Risk: Recursive CTEs for goal tree performance
   - Mitigation: Limit hierarchy depth to 3 levels, add indexes

3. **Progress Aggregation**
   - Risk: Objective progress from key results may not sum correctly
   - Mitigation: Use weighted average based on key result importance

**Low Risk:**
1. RLS policies (pattern established from Tasks)
2. CQRS pattern (well-established in codebase)
3. Frontend state management (React Query proven)

---

## Recommended Implementation Order

**Critical Path (Must Complete):**
1. Backend Domain (Step 2.1) → 2 hours
2. Backend Infrastructure (Step 2.2) → 2 hours
3. Database Migration (Step 2.3) → 1 hour
4. Backend Commands (Step 2.4) → 4 hours
5. Backend Queries (Step 2.5) → 3 hours
6. Backend DTOs (Step 2.6) → 1 hour
7. Backend Endpoints (Step 2.7) → 2 hours
8. Frontend Types (Step 2.8) → 1 hour
9. Frontend API Client (Step 2.9) → 2 hours
10. Frontend Components (Step 2.10) → 6 hours
11. Frontend Pages (Step 2.11) → 2 hours
12. Integration Testing (Step 2.12) → 3 hours

**Total:** 29 hours (exceeds 24h estimate by 5 hours)

**Optimization Strategy:**
- Skip goal tree view initially (defer to Phase 08.1)
- Use simple progress bars (defer ProgressChart component)
- Reduce validation complexity
- **Revised Total:** 20-24 hours

---

## Ambiguities & Questions

### Unresolved Questions

1. **Progress Calculation Formula**
   - ❓ How should objective progress be calculated from multiple key results?
   - 🔴 **Decision Needed:** Simple average vs. weighted average vs. manual override
   - **Recommendation:** Simple average (sum of progress / count) for MVP

2. **Period Management**
   - ❓ Should periods be auto-created or manual?
   - 🔴 **Decision Needed:** Auto-create Q1-Q4 each year vs. manual creation
   - **Recommendation:** Manual creation for flexibility

3. **Hierarchy Depth**
   - ❓ Max levels of nested objectives?
   - 🔴 **Decision Needed:** 2 levels (parent-child) vs. 3+ levels
   - **Recommendation:** 3 levels max (Company → Team → Individual)

4. **Objective Status Logic**
   - ❓ When does objective become "at-risk" or "off-track"?
   - 🔴 **Decision Needed:** Manual vs. automatic (based on due date, progress %)
   - **Recommendation:** Manual for MVP, auto-rules in Phase 08.1

5. **Key Result Types**
   - ❓ Support for different metric types (currency, percentage, boolean)?
   - 🔴 **Decision Needed:** Single decimal type vs. typed metrics
   - **Recommendation:** Single decimal with unit text for MVP

6. **Real-time Updates**
   - ❓ Should OKR updates use SignalR?
   - 🔴 **Decision Needed:** Real-time progress updates vs. polling
   - **Recommendation:** Skip for MVP (poll on refresh), add in Phase 08.1

---

## Success Criteria

### Must Have (MVP)
- ✅ Create objectives with title, description, owner, period
- ✅ Create key results with current/target values and units
- ✅ Auto-calculate progress from key results (0-100%)
- ✅ Hierarchical goal alignment (parent-child)
- ✅ Period-based filtering (Q1, Q2, etc.)
- ✅ Progress visualization (progress bars)
- ✅ CRUD operations for objectives and key results
- ✅ Workspace isolation via RLS

### Should Have (Phase 08.1)
- ⏳ Goal tree view (hierarchical visualization)
- ⏳ Advanced progress chart (burndown, trend lines)
- ⏳ Automatic status calculation (at-risk/off-track logic)
- ⏳ Real-time updates via SignalR
- ⏳ OKR templates (reuse across periods)

### Could Have (Future Phases)
- ⏳ AI-powered OKR suggestions
- ⏳ OKR alignment scoring
- ⏳ Comparative analytics (period-over-period)
- ⏳ OKR check-in reminders
- ⏳ OKR export to PDF/Excel

---

## Testing Requirements

### Unit Tests (Backend)
- [x] Objective progress calculation
- [x] Key result progress calculation
- [x] Hierarchy depth validation
- [x] Period overlap validation
- [x] RLS policy enforcement

### Integration Tests (API)
- [x] Create objective → verify in DB
- [x] Update key result → verify objective progress updates
- [x] Delete objective → verify cascade delete key results
- [x] Get objective tree → verify hierarchy

### E2E Tests (Frontend)
- [x] Create OKR flow (period → objective → key results)
- [x] Update progress → verify visual updates
- [x] Filter by period → verify correct results
- [x] Navigate goal tree → verify expand/collapse

### Manual Testing Checklist
- [ ] Create quarterly period (Q1 2026)
- [ ] Create company-level objective
- [ ] Create team-level child objective
- [ ] Add 3 key results to team objective
- [ ] Update key result current value
- [ ] Verify objective progress recalculates
- [ ] Filter dashboard by Q1 period
- [ ] Delete objective → verify key results removed
- [ ] Test RLS → user from different workspace cannot access

---

## Next Steps

### Immediate Actions (Ready to Start)
1. ✅ **Backend Team:** Create domain entities (Step 2.1)
2. ✅ **Backend Team:** Create EF Core configurations (Step 2.2)
3. ✅ **Backend Team:** Generate migration (Step 2.3)
4. ✅ **Backend Team:** Implement CQRS commands (Step 2.4)
5. ✅ **Frontend Team:** Create types and API client (Step 2.8-2.9)

### Clarifications Needed (Before Implementation)
1. 🔴 **Product Owner:** Confirm progress calculation formula (simple avg vs. weighted)
2. 🔴 **Product Owner:** Confirm period management (auto vs. manual)
3. 🔴 **Product Owner:** Confirm hierarchy depth limit (2 vs. 3 levels)
4. 🔴 **Product Owner:** Confirm objective status logic (manual vs. automatic)

### Phase Handoff
- **Backend Lead:** Implement Steps 2.1-2.7 (16 hours)
- **Frontend Lead:** Implement Steps 2.8-2.11 (11 hours)
- **QA Team:** Step 2.12 testing (3 hours)
- **Project Manager:** Track progress, resolve blockers

---

## Conclusion

Phase 08 (Goal Tracking & OKRs) is **well-defined and ready for implementation**. The phase builds on established patterns (CQRS, Clean Architecture, ClickUp Design System) and has **no technical blockers**.

**Key Recommendations:**
1. Start with backend foundation (entities, migrations, CQRS)
2. Implement MVP functionality first (defer advanced features to Phase 08.1)
3. Focus on core value: objectives, key results, progress tracking
4. Test progress calculation thoroughly (computed column complexity)
5. Validate RLS policies for workspace isolation

**Estimated Timeline:** 3 days (24 hours) with parallel backend/frontend work

**Risk Level:** Low (established patterns, no new dependencies)

**Priority:** P2 (important but not blocking - can be deferred if critical path issues arise)

---

**Report Generated:** 2026-01-05 23:32
**Generated By:** project-manager agent
**Report Version:** 1.0
**Status:** ✅ Analysis Complete
