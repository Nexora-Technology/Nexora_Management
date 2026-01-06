"use client"

import * as React from "react"
import { memo, useCallback, useMemo, useState } from "react"
import {
  DndContext,
  DragEndEvent,
  DragOverlay,
  DragStartEvent,
  KeyboardSensor,
  PointerSensor,
  pointerWithin,
  useSensor,
  useSensors,
  useDroppable,
} from "@dnd-kit/core"
import {
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable"
import { Task } from "./types"
import { TaskCard } from "./task-card"
import { DraggableTaskCard } from "./draggable-task-card"
import { BoardLayout, BoardColumn } from "@/components/layout/board-layout"
import { cn } from "@/lib/utils"

interface TaskBoardProps {
  tasks: Task[]
  onTaskClick?: (task: Task) => void
  onTaskStatusChange?: (taskId: string, newStatus: Task["status"]) => void
  onTaskReorder?: (tasks: Task[]) => void
  className?: string
}

/**
 * DroppableBoardColumn - BoardColumn that accepts dropped tasks
 */
interface DroppableBoardColumnProps {
  id: string
  title: string
  count: number
  children: React.ReactNode
}

function DroppableBoardColumn({ id, title, count, children }: DroppableBoardColumnProps) {
  const { setNodeRef, isOver } = useDroppable({
    id: `column-${id}`,
    data: {
      type: "column",
      status: id,
    },
  })

  return (
    <div
      ref={setNodeRef}
      className={cn(
        "w-[280px] flex-shrink-0 snap-start",
        isOver && "bg-primary/5 rounded-lg transition-colors"
      )}
    >
      {/* Column Header */}
      <div className="mb-3 flex items-center justify-between px-1">
        <h3 className="text-sm font-semibold text-gray-900 dark:text-white">
          {title}
        </h3>
        {count !== undefined && (
          <span className="text-xs text-gray-500 dark:text-gray-400">
            {count}
          </span>
        )}
      </div>

      {/* Column Content */}
      <div className="space-y-2">
        {children}
      </div>
    </div>
  )
}

export const TaskBoard = memo(function TaskBoard({
  tasks,
  onTaskClick,
  onTaskStatusChange,
  onTaskReorder,
  className,
}: TaskBoardProps) {
  const [activeTask, setActiveTask] = useState<Task | null>(null)
  const [taskList, setTaskList] = useState<Task[]>(tasks)
  const [isDragging, setIsDragging] = useState(false)

  // Update local state when tasks prop changes
  React.useEffect(() => {
    if (isDragging) return
    setTaskList(tasks)
  }, [tasks, isDragging])

  const tasksByStatus = React.useMemo(() => {
    const result: { todo: Task[]; inProgress: Task[]; complete: Task[]; overdue: Task[] } = {
      todo: [],
      inProgress: [],
      complete: [],
      overdue: [],
    }
    for (const task of taskList) {
      if (task.status === "todo") result.todo.push(task)
      else if (task.status === "inProgress") result.inProgress.push(task)
      else if (task.status === "complete") result.complete.push(task)
      else if (task.status === "overdue") result.overdue.push(task)
    }
    return result
  }, [taskList])

  const columnIds = React.useMemo(() => {
    return {
      todo: tasksByStatus.todo.map((t) => t.id),
      inProgress: tasksByStatus.inProgress.map((t) => t.id),
      complete: tasksByStatus.complete.map((t) => t.id),
      overdue: tasksByStatus.overdue.map((t) => t.id),
    }
  }, [tasksByStatus])

  const handleCardClick = useCallback((task: Task) => {
    onTaskClick?.(task)
  }, [onTaskClick])

  const createClickHandler = useCallback((task: Task) => () => handleCardClick(task), [handleCardClick])

  const totalTasks = useMemo(() => taskList.length, [taskList])

  // Configure sensors for drag and drop
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8, // 8px movement required to start dragging
      },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  const handleDragStart = useCallback((event: DragStartEvent) => {
    const { active } = event
    const task = taskList.find((t) => t.id === active.id)
    if (task) {
      setActiveTask(task)
      setIsDragging(true)
    }
  }, [taskList])

  const handleDragEnd = useCallback((event: DragEndEvent) => {
    const { active, over } = event
    setActiveTask(null)
    setIsDragging(false)

    if (!over) return

    const activeId = active.id as string
    const overId = over.id as string

    // Find the active task
    const activeTask = taskList.find((t) => t.id === activeId)
    if (!activeTask) return

    // Check if dropped over a column (status change)
    // Column IDs are prefixed with "column-"
    if (overId.toString().startsWith("column-")) {
      const newStatus = overId.toString().replace("column-", "") as Task["status"]
      if (activeTask.status !== newStatus) {
        // Update task status
        const updatedTasks = taskList.map((t) =>
          t.id === activeId ? { ...t, status: newStatus } : t
        )
        setTaskList(updatedTasks)
        onTaskStatusChange?.(activeId, newStatus)
        onTaskReorder?.(updatedTasks)
      }
      return
    }

    // Check if reordered within the same column (dropped on another task)
    if (activeId !== overId) {
      const activeIndex = taskList.findIndex((t) => t.id === activeId)
      const overIndex = taskList.findIndex((t) => t.id === overId)

      if (activeIndex !== -1 && overIndex !== -1) {
        const newTasks = [...taskList]
        const [removed] = newTasks.splice(activeIndex, 1)
        newTasks.splice(overIndex, 0, removed)

        setTaskList(newTasks)
        onTaskReorder?.(newTasks)
      }
    }
  }, [taskList, onTaskStatusChange, onTaskReorder])

  return (
    <DndContext
      sensors={sensors}
      collisionDetection={pointerWithin}
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
    >
      <BoardLayout className={className}>
        <div aria-live="polite" aria-atomic="true" className="sr-only">
          {totalTasks} tasks loaded
        </div>

        <DroppableBoardColumn
          id="todo"
          title="To Do"
          count={tasksByStatus.todo.length}
        >
          <SortableContext
            items={columnIds.todo}
            strategy={verticalListSortingStrategy}
          >
            {tasksByStatus.todo.map((task) => (
              <DraggableTaskCard
                key={task.id}
                task={task}
                onClick={createClickHandler(task)}
              />
            ))}
          </SortableContext>
        </DroppableBoardColumn>

        <DroppableBoardColumn
          id="inProgress"
          title="In Progress"
          count={tasksByStatus.inProgress.length}
        >
          <SortableContext
            items={columnIds.inProgress}
            strategy={verticalListSortingStrategy}
          >
            {tasksByStatus.inProgress.map((task) => (
              <DraggableTaskCard
                key={task.id}
                task={task}
                onClick={createClickHandler(task)}
              />
            ))}
          </SortableContext>
        </DroppableBoardColumn>

        <DroppableBoardColumn
          id="complete"
          title="Complete"
          count={tasksByStatus.complete.length}
        >
          <SortableContext
            items={columnIds.complete}
            strategy={verticalListSortingStrategy}
          >
            {tasksByStatus.complete.map((task) => (
              <DraggableTaskCard
                key={task.id}
                task={task}
                onClick={createClickHandler(task)}
              />
            ))}
          </SortableContext>
        </DroppableBoardColumn>

        <DroppableBoardColumn
          id="overdue"
          title="Overdue"
          count={tasksByStatus.overdue.length}
        >
          <SortableContext
            items={columnIds.overdue}
            strategy={verticalListSortingStrategy}
          >
            {tasksByStatus.overdue.map((task) => (
              <DraggableTaskCard
                key={task.id}
                task={task}
                onClick={createClickHandler(task)}
              />
            ))}
          </SortableContext>
        </DroppableBoardColumn>
      </BoardLayout>

      {/* Drag Overlay - Visual feedback during drag */}
      <DragOverlay>
        {activeTask ? (
          <div className="rotate-3 shadow-2xl">
            <TaskCard task={activeTask} />
          </div>
        ) : null}
      </DragOverlay>
    </DndContext>
  )
}, (prevProps, nextProps) => {
  return (
    prevProps.tasks.length === nextProps.tasks.length &&
    prevProps.tasks.every((t, i) => t.id === nextProps.tasks[i]?.id) &&
    prevProps.tasks.every((t, i) => t.status === nextProps.tasks[i]?.status) &&
    prevProps.onTaskClick === nextProps.onTaskClick &&
    prevProps.className === nextProps.className
  )
})
