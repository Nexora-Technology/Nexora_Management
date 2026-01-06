"use client"

import * as React from "react"
import { memo, useMemo } from "react"
import { Task } from "./types"
import { cn } from "@/lib/utils"
import { Badge } from "@/components/ui/badge"
import { PRIORITY_COLORS } from "./constants"

interface TaskGanttProps {
  tasks: Task[]
  onTaskClick?: (task: Task) => void
  className?: string
}

export const TaskGantt = memo(function TaskGantt({ tasks, onTaskClick, className }: TaskGanttProps) {
  // Calculate date range for the Gantt chart
  const { startDate, endDate, totalDays } = useMemo(() => {
    const dates = tasks.flatMap(t => [
      t.startDate ? new Date(t.startDate).getTime() : Infinity,
      t.dueDate ? new Date(t.dueDate).getTime() : -Infinity
    ]).filter(d => d !== Infinity && d !== -Infinity)

    if (dates.length === 0) {
      const now = new Date()
      const start = new Date(now.getFullYear(), now.getMonth(), 1)
      const end = new Date(now.getFullYear(), now.getMonth() + 2, 0)
      return { startDate: start, endDate: end, totalDays: 61 }
    }

    const minDate = new Date(Math.min(...dates))
    const maxDate = new Date(Math.max(...dates))

    // Add buffer days
    const startDate = new Date(minDate)
    startDate.setDate(startDate.getDate() - 7)
    const endDate = new Date(maxDate)
    endDate.setDate(endDate.getDate() + 7)

    const totalDays = Math.ceil((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24))

    return { startDate, endDate, totalDays }
  }, [tasks])

  // Generate timeline headers
  const timelineHeaders = useMemo(() => {
    const headers: Array<{ date: Date; label: string; isMonthStart: boolean }> = []
    const currentDate = new Date(startDate)

    for (let i = 0; i < totalDays; i++) {
      const isMonthStart = currentDate.getDate() === 1
      headers.push({
        date: new Date(currentDate),
        label: currentDate.getDate().toString(),
        isMonthStart
      })
      currentDate.setDate(currentDate.getDate() + 1)
    }

    return headers
  }, [startDate, totalDays])

  // Calculate task bar position and width
  const getTaskBarStyle = (task: Task) => {
    if (!task.startDate || !task.dueDate) {
      return null
    }

    const taskStart = new Date(task.startDate)
    const taskEnd = new Date(task.dueDate)

    const startOffset = Math.max(0, (taskStart.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24))
    const duration = Math.max(1, (taskEnd.getTime() - taskStart.getTime()) / (1000 * 60 * 60 * 24))

    const left = (startOffset / totalDays) * 100
    const width = (duration / totalDays) * 100

    return { left: `${left}%`, width: `${width}%` }
  }

  return (
    <div className={cn("bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 overflow-hidden", className)}>
      {/* Timeline Header */}
      <div className="border-b border-gray-200 dark:border-gray-700">
        <div className="flex">
          {/* Task Name Column Header */}
          <div className="w-64 flex-shrink-0 p-4 bg-gray-50 dark:bg-gray-900 border-r border-gray-200 dark:border-gray-700">
            <span className="text-sm font-semibold text-gray-900 dark:text-white">Task Name</span>
          </div>

          {/* Timeline Header */}
          <div className="flex-1 overflow-x-auto">
            <div className="flex" style={{ width: `${Math.max(100, totalDays * 40)}px` }}>
              {timelineHeaders.map((header, index) => (
                <div
                  key={index}
                  className={cn(
                    "flex-shrink-0 text-center py-2 border-l border-gray-200 dark:border-gray-700",
                    header.isMonthStart && "bg-gray-50 dark:bg-gray-900"
                  )}
                  style={{ width: "40px" }}
                >
                  <span className={cn(
                    "text-xs",
                    header.isMonthStart ? "font-semibold text-gray-900 dark:text-white" : "text-gray-500 dark:text-gray-400"
                  )}>
                    {header.label}
                  </span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* Task Rows */}
      <div className="divide-y divide-gray-200 dark:divide-gray-700">
        {tasks.map((task) => {
          const barStyle = getTaskBarStyle(task)

          return (
            <div key={task.id} className="flex hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-colors">
              {/* Task Info */}
              <div className="w-64 flex-shrink-0 p-4 border-r border-gray-200 dark:border-gray-700">
                <div className="flex items-center gap-2 mb-1">
                  <button
                    onClick={() => onTaskClick?.(task)}
                    className="flex-1 text-left text-sm font-medium text-gray-900 dark:text-white hover:text-primary dark:hover:text-primary transition-colors truncate"
                  >
                    {task.title}
                  </button>
                  <Badge status={PRIORITY_COLORS[task.priority] ? "overdue" : "neutral"} size="sm">
                    {task.priority}
                  </Badge>
                </div>
                <div className="flex items-center gap-2 text-xs text-gray-500 dark:text-gray-400">
                  {task.taskType && (
                    <span className="capitalize">{task.taskType}</span>
                  )}
                  {task.dueDate && (
                    <>
                      <span>•</span>
                      <span>{new Date(task.dueDate).toLocaleDateString()}</span>
                    </>
                  )}
                </div>
              </div>

              {/* Gantt Bar */}
              <div className="flex-1 relative p-4">
                <div className="relative" style={{ width: `${Math.max(100, totalDays * 40)}px`, height: "40px" }}>
                  {/* Grid lines */}
                  {timelineHeaders.map((_, index) => (
                    <div
                      key={index}
                      className="absolute top-0 bottom-0 border-l border-gray-100 dark:border-gray-800"
                      style={{ left: `${(index / totalDays) * 100}%`, width: "40px" }}
                    />
                  ))}

                  {/* Task Bar */}
                  {barStyle && (
                    <div
                      onClick={() => onTaskClick?.(task)}
                      className={cn(
                        "absolute top-1/2 -translate-y-1/2 h-8 rounded-md cursor-pointer",
                        "hover:opacity-90 transition-opacity",
                        "bg-gradient-to-r from-primary to-primary-hover",
                        "shadow-sm"
                      )}
                      style={barStyle}
                      role="button"
                      tabIndex={0}
                      onKeyDown={(e) => {
                        if (e.key === "Enter" || e.key === " ") {
                          e.preventDefault()
                          onTaskClick?.(task)
                        }
                      }}
                    >
                      <div className="h-full px-2 flex items-center">
                        <span className="text-xs font-medium text-white truncate">
                          {task.title}
                        </span>
                      </div>
                    </div>
                  )}

                  {/* Duration info */}
                  {task.startDate && task.dueDate && (
                    <div className="absolute top-full left-0 mt-1 text-xs text-gray-500 dark:text-gray-400">
                      {Math.ceil((new Date(task.dueDate).getTime() - new Date(task.startDate).getTime()) / (1000 * 60 * 60 * 24))}d
                    </div>
                  )}
                </div>
              </div>
            </div>
          )
        })}
      </div>

      {/* Empty State */}
      {tasks.length === 0 && (
        <div className="p-12 text-center text-gray-500 dark:text-gray-400">
          <p className="text-lg font-medium mb-2">No tasks to display</p>
          <p className="text-sm">Tasks with start and due dates will appear in the Gantt chart</p>
        </div>
      )}

      {/* Legend */}
      <div className="border-t border-gray-200 dark:border-gray-700 p-4 bg-gray-50 dark:bg-gray-900">
        <div className="flex items-center gap-6 text-sm text-gray-600 dark:text-gray-400">
          <div className="flex items-center gap-2">
            <div className="w-16 h-4 rounded bg-gradient-to-r from-primary to-primary-hover" />
            <span>Task duration</span>
          </div>
          <div className="flex items-center gap-2">
            <div className="w-16 h-4 rounded border border-dashed border-gray-400" />
            <span>Planned</span>
          </div>
        </div>
      </div>
    </div>
  )
}, (prevProps, nextProps) => {
  return (
    prevProps.tasks.length === nextProps.tasks.length &&
    prevProps.tasks.every((t, i) => t.id === nextProps.tasks[i]?.id) &&
    prevProps.tasks.every((t, i) => t.startDate === nextProps.tasks[i]?.startDate) &&
    prevProps.tasks.every((t, i) => t.dueDate === nextProps.tasks[i]?.dueDate) &&
    prevProps.onTaskClick === nextProps.onTaskClick &&
    prevProps.className === nextProps.className
  )
})
