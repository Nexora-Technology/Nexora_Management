"use client"

import * as React from "react"
import { memo } from "react"
import { useSortable } from "@dnd-kit/sortable"
import { CSS } from "@dnd-kit/utilities"
import { TaskCard } from "./task-card"
import { Task } from "./types"
import { cn } from "@/lib/utils"

interface DraggableTaskCardProps {
  task: Task
  onClick?: () => void
  disabled?: boolean
  className?: string
}

/**
 * Draggable wrapper for TaskCard using @dnd-kit/sortable
 *
 * Features:
 * - Smooth drag transitions (200ms)
 * - Scale and opacity changes during drag
 * - Entire card is draggable (no drag handle needed)
 * - Keyboard accessible
 * - Maintains TaskCard functionality
 * - ClickUp-style visual feedback
 */
export const DraggableTaskCard = memo(function DraggableTaskCard({
  task,
  onClick,
  disabled = false,
  className,
}: DraggableTaskCardProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({
    id: task.id,
    disabled,
    data: {
      type: "task",
      task,
    },
  })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  }

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={cn(
        // Dragging state - ClickUp style
        isDragging && [
          "opacity-50",
          "scale-105",
          "shadow-lg",
          "ring-2",
          "ring-primary",
          "ring-offset-2",
          "dark:ring-offset-gray-900",
        ],
        // Smooth transitions
        "transition-transform duration-200",
        "transition-opacity duration-200",
        "transition-shadow duration-200",
        className
      )}
    >
      {/* Inner div handles drag, TaskCard handles click */}
      <div {...attributes} {...listeners} className="cursor-grab active:cursor-grabbing">
        <TaskCard
          task={task}
          onClick={onClick}
        />
      </div>
    </div>
  )
}, (prevProps, nextProps) => {
  return (
    prevProps.task.id === nextProps.task.id &&
    prevProps.task.status === nextProps.task.status &&
    prevProps.disabled === nextProps.disabled &&
    prevProps.onClick === nextProps.onClick &&
    prevProps.className === nextProps.className
  )
})
