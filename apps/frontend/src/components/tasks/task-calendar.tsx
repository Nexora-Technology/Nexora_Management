"use client"

import * as React from "react"
import { memo, useState, useMemo } from "react"
import { ChevronLeft, ChevronRight } from "lucide-react"
import { Task } from "./types"
import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"
import { STATUS_BADGE_VARIANTS } from "./constants"
import { Button } from "@/components/ui/button"

interface TaskCalendarProps {
  tasks: Task[]
  onTaskClick?: (task: Task) => void
  className?: string
}

export const TaskCalendar = memo(function TaskCalendar({ tasks, onTaskClick, className }: TaskCalendarProps) {
  const [currentDate, setCurrentDate] = useState(new Date())

  const daysInMonth = useMemo(() => {
    const year = currentDate.getFullYear()
    const month = currentDate.getMonth()
    const firstDay = new Date(year, month, 1)
    const lastDay = new Date(year, month + 1, 0)
    const daysInMonth = lastDay.getDate()
    const startingDayOfWeek = firstDay.getDay()

    const days: Array<Date | null> = []

    // Add empty cells for days before the first day of the month
    for (let i = 0; i < startingDayOfWeek; i++) {
      days.push(null)
    }

    // Add all days of the month
    for (let day = 1; day <= daysInMonth; day++) {
      days.push(new Date(year, month, day))
    }

    return days
  }, [currentDate])

  const tasksByDate = useMemo(() => {
    const map = new Map<string, Task[]>()
    for (const task of tasks) {
      if (task.dueDate) {
        const dateKey = new Date(task.dueDate).toDateString()
        if (!map.has(dateKey)) {
          map.set(dateKey, [])
        }
        map.get(dateKey)!.push(task)
      }
    }
    return map
  }, [tasks])

  const handlePrevMonth = () => {
    setCurrentDate(new Date(currentDate.getFullYear(), currentDate.getMonth() - 1))
  }

  const handleNextMonth = () => {
    setCurrentDate(new Date(currentDate.getFullYear(), currentDate.getMonth() + 1))
  }

  const handleToday = () => {
    setCurrentDate(new Date())
  }

  const monthName = currentDate.toLocaleDateString("en-US", { month: "long", year: "numeric" })
  const today = new Date().toDateString()

  const weekDays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"]

  return (
    <div className={cn("bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 p-6", className)}>
      {/* Header */}
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-semibold text-gray-900 dark:text-white">{monthName}</h2>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="sm" onClick={handleToday}>
            Today
          </Button>
          <Button variant="ghost" size="icon" onClick={handlePrevMonth}>
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <Button variant="ghost" size="icon" onClick={handleNextMonth}>
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {/* Calendar Grid */}
      <div className="grid grid-cols-7 gap-1">
        {/* Weekday Headers */}
        {weekDays.map((day) => (
          <div key={day} className="text-center text-sm font-medium text-gray-500 dark:text-gray-400 py-2">
            {day}
          </div>
        ))}

        {/* Calendar Days */}
        {daysInMonth.map((date, index) => {
          if (!date) {
            return <div key={`empty-${index}`} className="h-24" />
          }

          const dateKey = date.toDateString()
          const dayTasks = tasksByDate.get(dateKey) || []
          const isToday = dateKey === today

          return (
            <div
              key={dateKey}
              className={cn(
                "h-24 border border-gray-200 dark:border-gray-700 rounded-md p-2 overflow-hidden",
                "hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-colors",
                isToday && "ring-2 ring-primary ring-offset-2"
              )}
            >
              <div className={cn(
                "text-sm font-medium mb-1",
                isToday ? "text-primary" : "text-gray-900 dark:text-white"
              )}>
                {date.getDate()}
              </div>

              <div className="space-y-1">
                {dayTasks.slice(0, 3).map((task) => (
                  <div
                    key={task.id}
                    onClick={() => onTaskClick?.(task)}
                    className="text-xs p-1 rounded cursor-pointer hover:opacity-80 transition-opacity"
                    role="button"
                    tabIndex={0}
                    onKeyDown={(e) => {
                      if (e.key === "Enter" || e.key === " ") {
                        e.preventDefault()
                        onTaskClick?.(task)
                      }
                    }}
                  >
                    <div className="truncate font-medium text-gray-900 dark:text-white">
                      {task.title}
                    </div>
                  </div>
                ))}

                {dayTasks.length > 3 && (
                  <div className="text-xs text-gray-500 dark:text-gray-400 text-center">
                    +{dayTasks.length - 3} more
                  </div>
                )}
              </div>
            </div>
          )
        })}
      </div>

      {/* Legend */}
      <div className="mt-6 flex items-center gap-4 text-sm text-gray-600 dark:text-gray-400">
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 rounded-full bg-primary" />
          <span>Today</span>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-3 h-3 border border-gray-300 dark:border-gray-600 rounded" />
          <span>Task due date</span>
        </div>
      </div>
    </div>
  )
}, (prevProps, nextProps) => {
  return (
    prevProps.tasks.length === nextProps.tasks.length &&
    prevProps.tasks.every((t, i) => t.id === nextProps.tasks[i]?.id) &&
    prevProps.tasks.every((t, i) => t.dueDate === nextProps.tasks[i]?.dueDate) &&
    prevProps.onTaskClick === nextProps.onTaskClick &&
    prevProps.className === nextProps.className
  )
})
