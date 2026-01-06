# Implementation Plan: Fix Kanban Board Drag-and-Drop

**Date**: 2025-01-07
**Component**: TaskBoard Kanban Board
**Priority**: P1 (High)
**Effort**: 3-4 hours
**Status**: Ready for Implementation

---

## Executive Summary

Created focused implementation plan to fix critical drag-and-drop bug preventing users from moving tasks between Kanban columns. Root cause: collision detection conflict between task cards and column containers.

**Plan Location**: `/apps/frontend/plans/260107-0032-fix-kanban-dnd/plan.md`

---

## Problem

Users cannot drag tasks between columns (To Do → In Progress → Complete → Overdue). Task cards are detected as drop targets instead of column containers due to `closestCenter` collision detection prioritizing nearby task cards over parent column containers.

---

## Solution

**Primary Fix**: Replace `closestCenter` with `pointerWithin` collision detection
- Only considers droppables under cursor
- Eliminates false positives from adjacent task cards
- 30-minute implementation

**Secondary Fix**: Add `isDragging` state tracking
- Prevents useEffect race conditions during drag
- Ensures drag operations complete before state sync
- 45-minute implementation

**Fallback**: Custom collision detection function (if needed)
- Explicitly prioritizes columns over tasks
- 1-hour implementation

---

## Implementation Phases

### Phase 1: Update Collision Detection (30 min)
Import `pointerWithin` from @dnd-kit/core, replace `collisionDetection={closestCenter}` with `collisionDetection={pointerWithin}` on line 199.

### Phase 2: Add Drag State Tracking (45 min)
Add `isDragging` state, set/clear in drag handlers, update useEffect to skip sync during active drag.

### Phase 3: Verify Drag Logic (30 min)
Review handleDragEnd logic, confirm column detection and task reorder paths work correctly.

### Phase 4: Testing (1h 15min)
15 test scenarios covering:
- Column-to-column drag (5 cases)
- Within-column reorder (4 cases)
- Edge cases (6 cases)
- Visual verification (4 cases)

### Phase 5: Fallback (1h, if needed)
Custom collision detection that explicitly filters for column droppables first.

---

## Success Criteria

✅ Tasks draggable between all 4 columns
✅ Column hover visual feedback
✅ Status updates after drop
✅ No flicker/reversion
✅ Reorder within column works
✅ Keyboard operations unaffected
✅ All 15 test cases pass

---

## Files Modified

**Single File**: `apps/frontend/src/components/tasks/task-board.tsx`

**Lines Changed**:
- Line 12: Import `pointerWithin`
- Line 91: Add `isDragging` state
- Line 94-96: Update useEffect
- Line 143-149: Update handleDragStart
- Line 151: Update handleDragEnd
- Line 199: Update DndContext collisionDetection

**Total LOC Modified**: ~15 lines across 6 sections

---

## Risk Assessment

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| pointerWithin insufficient | Low (20%) | Medium | Fallback ready |
| useEffect race condition | Medium (40%) | High | isDragging prevents |
| Performance degradation | Low (10%) | Low | pointerWithin lighter |
| Accessibility regression | Low (5%) | Medium | Keyboard unaffected |

---

## Timeline

| Phase | Duration |
|-------|----------|
| Phase 1-3 (Implementation) | 1h 45min |
| Phase 4 (Testing) | 1h 15min |
| Phase 5 (Fallback, if needed) | 1h |
| **Total (worst case)** | **4h** |
| **Total (expected)** | **3h** |

---

## Dependencies

✅ All dependencies installed:
- @dnd-kit/core ^6.3.1
- React 19.1.0

**No new dependencies required.**

---

## Unresolved Questions

1. Mobile touch drag behavior?
2. Drag confirmation dialog needed?
3. Immediate persist vs. save button?
4. Network failure handling after drag?

---

## Next Steps

1. Implement Phase 1-3 (collision detection + drag state)
2. Run Phase 4 test suite
3. If tests fail → Implement Phase 5 fallback
4. Deploy to staging for validation
5. Production release

---

## References

- **Plan**: `/apps/frontend/plans/260107-0032-fix-kanban-dnd/plan.md`
- **Component**: `/apps/frontend/src/components/tasks/task-board.tsx`
- **Analysis**: `/apps/frontend/plans/reports/scout-external-260105-0147-task-components-exploration.md`
- **@dnd-kit Docs**: https://docs.dndkit.com/api-documentation/context-provider/collision-detection
