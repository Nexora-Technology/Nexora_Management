"use client"

import * as React from "react"
import { memo } from "react"
import { Task } from "./types"
import { cn } from "@/lib/utils"
import { PRIORITY_COLORS, STATUS_LABELS } from "./constants"

interface TaskRowProps {
  task: Task
  isSelected?: boolean
  onSelect?: (id: string) => void
}

export const TaskRow = memo(function TaskRow({ task, isSelected, onSelect }: TaskRowProps) {
  return (
    <tr
      className={cn(
        "border-b border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-800/50 transition-colors",
        isSelected && "bg-primary/5"
      )}
    >
      <td className="w-12 px-4">
        <input
          type="checkbox"
          checked={isSelected}
          onChange={() => onSelect?.(task.id)}
          className="h-4 w-4 rounded border-gray-300 text-primary focus:ring-primary"
        />
      </td>
      <td className="px-4 py-3">
        <div className="text-sm font-medium text-gray-900 dark:text-white">
          {task.title}
        </div>
      </td>
      <td className="px-4 py-3">
        <span className="text-xs text-gray-500 dark:text-gray-400">
          {STATUS_LABELS[task.status]}
        </span>
      </td>
      <td className="px-4 py-3">
        <span className={cn(
          "inline-block w-2 h-2 rounded-full",
          PRIORITY_COLORS[task.priority]
        )} />
      </td>
      <td className="px-4 py-3">
        {task.assignee && (
          <div className="text-sm text-gray-600 dark:text-gray-400">
            {task.assignee.name}
          </div>
        )}
      </td>
      <td className="px-4 py-3">
        {task.dueDate && (
          <span className="text-sm text-gray-600 dark:text-gray-400">
            {new Date(task.dueDate).toLocaleDateString()}
          </span>
        )}
      </td>
    </tr>
  )
}, (prevProps, nextProps) => {
  return (
    prevProps.task.id === nextProps.task.id &&
    prevProps.task.title === nextProps.task.title &&
    prevProps.task.status === nextProps.task.status &&
    prevProps.isSelected === nextProps.isSelected &&
    prevProps.onSelect === nextProps.onSelect
  )
})
