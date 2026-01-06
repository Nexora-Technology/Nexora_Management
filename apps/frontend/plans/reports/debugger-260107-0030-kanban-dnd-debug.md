# Debug Report: Kanban Board Drag-and-Drop Issue

**Date:** 2026-01-07
**Issue:** Tasks snap back to original column after drop
**File:** `/apps/frontend/src/components/tasks/task-board.tsx`

## Executive Summary

Task drag-and-drop partially works - visual feedback (purple highlight) shows when hovering over columns, but tasks snap back to original column after drop. Root cause identified: **collision detection conflict between SortableContext and DroppableBoardColumn**.

## Root Cause Analysis

### Problem 1: Collision Detection Priority (PRIMARY ISSUE)

**The issue:** When using `closestCenter` collision detection with both `SortableContext` (tasks) and `useDroppable` (columns), the draggable task cards **collide with other task cards before detecting the column**.

**Code location:** `task-board.tsx:199`
```tsx
<DndContext
  sensors={sensors}
  collisionDetection={closestCenter}  // ❌ Problem here
  onDragStart={handleDragStart}
  onDragEnd={handleDragEnd}
>
```

**Why it breaks:**
1. Tasks are inside `SortableContext` with IDs like `"epic-1"`, `"story-1"`, etc.
2. Columns use `useDroppable` with IDs like `"column-todo"`, `"column-inProgress"`, etc.
3. When dragging a task over a column, `closestCenter` finds the **closest center among all droppables**
4. Task cards in the column are closer than the column itself
5. `over.id` becomes the ID of another task card, NOT the column
6. `handleDragEnd` line 166 check fails: `overId.toString().startsWith("column-")` is FALSE
7. Line 181-193 reordering logic runs instead, but only reorders within same column
8. Task snaps back because position change is reverted by useEffect (lines 94-96)

**Evidence:**
- DroppableBoardColumn `useDroppable` is configured correctly (line 47-52)
- Task cards use `useSortable` correctly (draggable-task-card.tsx:42-48)
- Both have `data.type` set but `closestCenter` ignores it
- Highlight works because `isOver` in DroppableBoardColumn uses its own collision detection

### Problem 2: useEffect State Reversion

**Code location:** `task-board.tsx:94-96`
```tsx
React.useEffect(() => {
  setTaskList(tasks)
}, [tasks])
```

**Issue:** When parent re-renders with unchanged `tasks` prop, `useEffect` runs and resets local state, undoing the drag operation.

**Why it happens:**
1. `onTaskStatusChange` is called (line 174)
2. Parent's `handleTaskStatusChange` updates parent state (page.tsx:52-60)
3. Parent re-renders with new `tasks` prop
4. But before that render completes, `useEffect` runs with old `tasks` reference
5. Local `taskList` is reset to old values
6. Task snaps back visually

## Technical Analysis

### Collision Detection Flow

**Expected flow:**
```
Drag task → Hover over column → over.id = "column-inProgress" → Status updates → Task moves
```

**Actual flow:**
```
Drag task → Hover over column → over.id = "story-2" (another task) → Reordering logic → No change → Task snaps back
```

### State Update Timing

```
Timeline:
T1: User drops task on "column-inProgress"
T2: handleDragEnd executes with over.id = "story-2"
T3: Reordering logic runs (no actual change)
T4: Local taskList updated (no status change)
T5: Parent onTaskStatusChange NOT called (because column not detected)
T6: useEffect runs, resets taskList from tasks prop
T7: Task snaps back to original position
```

## Solutions

### Solution 1: Custom Collision Detection (RECOMMENDED)

Use custom collision detection that prioritizes columns over tasks:

```tsx
import { pointerWithin, rectIntersection } from "@dnd-kit/core"

const collisionDetectionStrategy = useCallback((args: CollisionDetectionArguments) => {
  // First, check if we're over a column (pointer within column bounds)
  const pointerCollisions = pointerWithin(args)
  const columnCollisions = pointerCollisions.filter(
    collision => collision.id.toString().startsWith("column-")
  )

  if (columnCollisions.length > 0) {
    return columnCollisions
  }

  // If not over a column, use default closestCenter for task reordering
  return closestCenter(args)
}, [])
```

**Pros:**
- Columns are always detected first when hovering
- Task reordering still works
- Minimal code change

**Cons:**
- Need to handle edge cases (empty columns, small columns)

### Solution 2: Use rectIntersection Collision Detection

Replace `closestCenter` with `rectIntersection`:

```tsx
import { rectIntersection } from "@dnd-kit/core"

<DndContext
  sensors={sensors}
  collisionDetection={rectIntersection}  // Intersection-based detection
  onDragStart={handleDragStart}
  onDragEnd={handleDragEnd}
>
```

**Pros:**
- Simple change
- Columns are large targets, easily intersected
- Works well for drop zones

**Cons:**
- May be less precise for task reordering
- Multiple intersections possible (need to pick closest)

### Solution 3: Disable Sortable During Column Change

Temporarily disable sortable when dragging over columns:

```tsx
const handleDragMove = useCallback((event: DragMoveEvent) => {
  const { over } = event
  if (over?.id.toString().startsWith("column-")) {
    // Disable sortable reordering
    setSortableEnabled(false)
  } else {
    setSortableEnabled(true)
  }
}, [])
```

**Pros:**
- Clear intent
- No collision detection changes

**Cons:**
- More complex state management
- May cause flickering

### Solution 4: Fix useEffect Dependencies (ALSO NEEDED)

Prevent useEffect from reverting state unnecessarily:

```tsx
// Track if we're currently dragging
const [isDragging, setIsDragging] = useState(false)

const handleDragStart = useCallback((event: DragStartEvent) => {
  setIsDragging(true)
  // ... rest of logic
}, [])

const handleDragEnd = useCallback((event: DragEndEvent) => {
  setIsDragging(false)
  // ... rest of logic
}, [])

// Only update local state if not dragging
React.useEffect(() => {
  if (!isDragging) {
    setTaskList(tasks)
  }
}, [tasks, isDragging])
```

**Pros:**
- Prevents state reversion during drag
- Simple fix

**Cons:**
- Adds more state
- Need to ensure cleanup on drag cancel

## Recommended Fix

**Apply both Solution 1 (custom collision detection) AND Solution 4 (fix useEffect):**

1. Custom collision detection prioritizes columns
2. useEffect prevents state reversion during drag
3. Task reordering still works within columns
4. Status changes persist correctly

**Files to modify:**
- `/apps/frontend/src/components/tasks/task-board.tsx`
  - Add custom collision detection
  - Add `isDragging` state
  - Fix useEffect dependencies
  - Update handleDragStart/handleDragEnd

## Testing Checklist

After fix:
- [ ] Drag task from "todo" to "inProgress" → Task moves
- [ ] Drag task from "inProgress" to "complete" → Task moves
- [ ] Drag task from "complete" back to "todo" → Task moves
- [ ] Reorder tasks within same column → Works
- [ ] Drop task on empty column → Works
- [ ] Visual feedback (purple highlight) shows correctly
- [ ] Task status persists after drop
- [ ] Parent component receives status change callback
- [ ] No console errors

## Unresolved Questions

1. Should we use `pointerWithin` or custom rect-based collision for column detection?
2. Do we need to handle `DragCancelEvent` to clean up `isDragging` state?
3. Should empty columns have minimum height to ensure they're droppable?
4. Is there a performance impact from custom collision detection on large task lists?

## Sources

- [Collision detection algorithms](https://docs.dndkit.com/api-documentation/context-provider/collision-detection-algorithms)
- [Sortable preset documentation](https://docs.dndkit.com/presets/sortable)
- [Collision detection in sortable multi-containers issue](https://github.com/clauderic/dnd-kit/issues/73)
- [Canceling drag operation with closestCenter](https://github.com/clauderic/dnd-kit/discussions/210)
