---
title: "Fix Kanban Board Drag-and-Drop Between Columns"
description: "Implement custom collision detection to fix drag-and-drop issue where tasks cannot be moved between columns"
status: in_progress
priority: P1
effort: 3h
branch: main
tags: [bug-fix, dnd-kit, kanban, collision-detection]
created: 2025-01-07
---

## Problem Statement

**Issue**: Tasks cannot be dragged between Kanban columns (To Do → In Progress → Complete → Overdue)

**Root Cause**: Collision detection conflict - `closestCenter` finds the closest droppable among ALL items (tasks + columns). Task cards are closer than the column container, so it detects a task instead of the column.

**Impact**: Users cannot change task status by dragging between columns, breaking core Kanban functionality.

---

## Implementation Progress

✅ **Phase 1 Complete** - Updated collision detection from `closestCenter` to `pointerWithin`
✅ **Phase 2 Complete** - Added `isDragging` state tracking to prevent useEffect race conditions
⏳ **Phase 3 Pending** - Verify drag logic handles column detection correctly
⏳ **Phase 4 Pending** - Testing (15 scenarios)

---

## Solution Architecture

**Strategy**: Custom collision detection that prioritizes column containers over task cards

### Technical Approach

1. **Pointer-Within Detection**: Use `pointerWithin` collision detection from @dnd-kit/core
   - Only detects droppables under the pointer
   - More precise than `closestCenter` for nested droppables

2. **Drag State Tracking**: Add `isDragging` state to prevent useEffect race conditions
   - Prevents state sync during active drag operations
   - Ensures drag operations complete before external state updates

3. **Custom Collision Logic** (if pointerWithin insufficient):
   - Custom collision detection function
   - Prioritize column droppables over task droppables
   - Fall back to closestCenter for task reordering within same column

---

## Implementation Plan

### Phase 1: Update Collision Detection (30 min)

**File**: `apps/frontend/src/components/tasks/task-board.tsx`

**Changes**:
1. Import `pointerWithin` from `@dnd-kit/core` (line 12)
2. Replace `collisionDetection={closestCenter}` with `collisionDetection={pointerWithin}` (line 199)

**Code**:
```typescript
// Line 12 - Update import
import {
  // ... existing imports
  pointerWithin,  // ADD THIS
} from "@dnd-kit/core"

// Line 199 - Update DndContext
<DndContext
  sensors={sensors}
  collisionDetection={pointerWithin}  // CHANGE FROM closestCenter
  onDragStart={handleDragStart}
  onDragEnd={handleDragEnd}
>
```

**Rationale**: `pointerWithin` only considers droppables under the cursor, eliminating false positives from nearby task cards.

---

### Phase 2: Add Drag State Tracking (45 min)

**File**: `apps/frontend/src/components/tasks/task-board.tsx`

**Changes**:
1. Add `isDragging` state (line 91)
2. Set `isDragging` in `handleDragStart` (line 143)
3. Clear `isDragging` in `handleDragEnd` (line 151)
4. Update useEffect to skip sync when dragging (line 94)

**Code**:
```typescript
// Line 91 - Add state
export const TaskBoard = memo(function TaskBoard({ ... }: TaskBoardProps) {
  const [activeTask, setActiveTask] = useState<Task | null>(null)
  const [taskList, setTaskList] = useState<Task[]>(tasks)
  const [isDragging, setIsDragging] = useState(false)  // ADD THIS

  // Lines 94-96 - Update useEffect
  React.useEffect(() => {
    if (isDragging) return  // ADD THIS - Skip sync during drag
    setTaskList(tasks)
  }, [tasks, isDragging])  // ADD isDragging TO DEPS

  // Lines 143-149 - Update handleDragStart
  const handleDragStart = useCallback((event: DragStartEvent) => {
    const { active } = event
    const task = taskList.find((t) => t.id === active.id)
    if (task) {
      setActiveTask(task)
      setIsDragging(true)  // ADD THIS
    }
  }, [taskList])

  // Lines 151-154 - Update handleDragEnd
  const handleDragEnd = useCallback((event: DragEndEvent) => {
    const { active, over } = event
    setActiveTask(null)
    setIsDragging(false)  // ADD THIS
    // ... rest of handler
  }, [taskList, onTaskStatusChange, onTaskReorder])
```

**Rationale**: Prevents useEffect from reverting drag changes before drag operation completes.

---

### Phase 3: Verify Drag Logic (30 min)

**File**: `apps/frontend/src/components/tasks/task-board.tsx`

**Review**:
1. Check `handleDragEnd` logic (lines 151-194)
2. Verify column detection: `overId.toString().startsWith("column-")` (line 166)
3. Verify task reorder logic (lines 181-193)
4. Confirm state updates occur correctly

**Expected Behavior**:
- Drag task over column → Status changes
- Drag task over another task → Reorder within column
- No flickering or state reversion

---

### Phase 4: Testing (1h 15min)

**Test Cases**:

1. **Column-to-Column Drag** (5 scenarios)
   - To Do → In Progress
   - In Progress → Complete
   - Complete → Overdue
   - Overdue → To Do
   - All 4 columns cycle

2. **Within-Column Reorder** (4 scenarios)
   - Reorder in To Do column
   - Reorder in In Progress column
   - Reorder in Complete column
   - Reorder in Overdue column

3. **Edge Cases** (6 scenarios)
   - Empty column drag
   - Drag to same column
   - Rapid column changes
   - Drag and drop outside board
   - Keyboard drag operations
   - Mobile touch drag

4. **Visual Verification** (4 scenarios)
   - Drag overlay visible
   - Column highlight on hover
   - No flicker during drag
   - Smooth animations

---

### Phase 5: Fallback Collision Detection (if needed) (1h)

**Condition**: If `pointerWithin` doesn't resolve the issue

**Implementation**:
```typescript
// Add custom collision detection function
import { CollisionDetection, pointerWithin } from "@dnd-kit/core"

const customCollisionDetection: CollisionDetection = (args) => {
  // Check if pointer is over a column first
  const pointerCollisions = pointerWithin(args)

  // Filter for column droppables only
  const columnCollisions = pointerCollisions.filter(collision =>
    collision.id.toString().startsWith("column-")
  )

  // If over a column, prioritize it
  if (columnCollisions.length > 0) {
    return columnCollisions
  }

  // Fall back to all pointer collisions
  return pointerCollisions
}

// Use in DndContext
<DndContext
  sensors={sensors}
  collisionDetection={customCollisionDetection}
  onDragStart={handleDragStart}
  onDragEnd={handleDragEnd}
>
```

**Rationale**: Explicitly prioritizes column droppables over task droppables when both are under pointer.

---

## Success Criteria

✅ Tasks can be dragged between all 4 columns
✅ Visual feedback shows column being hovered
✅ Task status updates correctly after drop
✅ No state reversion or flickering during drag
✅ Reorder within same column still works
✅ Keyboard navigation unaffected
✅ All 15 test cases pass

---

## Risk Assessment

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| pointerWithin insufficient | Low (20%) | Medium | Have fallback custom collision ready |
| useEffect race condition | Medium (40%) | High | isDragging state prevents this |
| Performance degradation | Low (10%) | Low | pointerWithin is lighter than closestCenter |
| Accessibility regression | Low (5%) | Medium | Keyboard drag unaffected |

---

## Dependencies

**Required**:
- @dnd-kit/core ^6.3.1 (✅ already installed)
- React 19.1.0 (✅ already installed)

**None Required** - All dependencies already in place.

---

## Timeline

| Phase | Duration | Owner |
|-------|----------|-------|
| Phase 1: Update collision detection | 30 min | Frontend Dev |
| Phase 2: Add drag state tracking | 45 min | Frontend Dev |
| Phase 3: Verify drag logic | 30 min | Frontend Dev |
| Phase 4: Testing | 1h 15min | QA |
| Phase 5: Fallback (if needed) | 1h | Frontend Dev |
| **Total (with fallback)** | **4h** | - |
| **Total (without fallback)** | **3h** | - |

---

## Implementation Order

1. **Phase 1** → Update collision detection
2. **Phase 2** → Add drag state tracking
3. **Phase 3** → Verify drag logic
4. **Phase 4** → Test all scenarios
5. **Phase 5** → Only if Phases 1-4 fail

**Stop Condition**: If Phase 4 tests pass, stop. Do not implement Phase 5.

---

## Post-Implementation

**Verification Steps**:
1. Run `npm run dev` in `/apps/frontend`
2. Navigate to Tasks board view
3. Execute all 15 test cases
4. Check browser console for errors
5. Verify network requests for status changes

**Rollback Plan**:
- Revert `task-board.tsx` to previous version
- Switch back to `closestCenter` collision detection
- Remove `isDragging` state

**Documentation Updates**:
- Update `task-components-exploration.md` with collision detection details
- Add drag-and-drop behavior to component documentation

---

## Unresolved Questions

1. Should drag-and-drop be disabled on mobile touch devices?
2. Should column-to-column drag trigger a confirmation dialog?
3. Should drag operations persist immediately or wait for "Save" button?
4. What happens if network request fails after drag operation?

---

## Next Steps

**Immediate**:
1. Implement Phase 1-3
2. Run Phase 4 tests
3. Deploy to staging environment

**Future**:
1. Add drag analytics (which columns most used)
2. Implement drag undo/redo
3. Add drag animations customization
4. Consider drag-and-drop for subtasks (when feature added)

---

## References

- **@dnd-kit docs**: https://docs.dndkit.com/
- **Collision detection guide**: https://docs.dndkit.com/api-documentation/context-provider/collision-detection
- **Current implementation**: `apps/frontend/src/components/tasks/task-board.tsx` (lines 1-304)
- **Related report**: `apps/frontend/plans/reports/scout-external-260105-0147-task-components-exploration.md`
