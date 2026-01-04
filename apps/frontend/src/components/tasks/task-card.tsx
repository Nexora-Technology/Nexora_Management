"use client"

import * as React from "react"
import { memo } from "react"
import { GripVertical, MessageSquare, Paperclip } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { cn } from "@/lib/utils"
import { Task } from "./types"
import { PRIORITY_COLORS, STATUS_LABELS, STATUS_BADGE_VARIANTS } from "./constants"

interface TaskCardProps {
  task: Task
  onClick?: () => void
  className?: string
}

export const TaskCard = memo(function TaskCard({ task, onClick, className }: TaskCardProps) {
  return (
    <div
      onClick={onClick}
      onKeyDown={(e) => {
        if (e.key === "Enter" || e.key === " ") {
          e.preventDefault()
          onClick?.()
        }
      }}
      role="button"
      tabIndex={0}
      className={cn(
        "group bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700",
        "p-4 hover:shadow-md transition-all duration-200 cursor-pointer",
        "focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2",
        "animate-fade-in",
        className
      )}
    >
      {/* Header */}
      <div className="flex items-start gap-3 mb-3">
        {/* Drag Handle */}
        <button
          className="flex-shrink-0 opacity-0 group-hover:opacity-100 transition-opacity mt-1"
          onClick={(e) => e.stopPropagation()}
          aria-label="Drag to reorder task"
          type="button"
        >
          <GripVertical className="h-4 w-4 text-gray-400" />
        </button>

        {/* Content */}
        <div className="flex-1 min-w-0">
          <h4 className="text-sm font-medium text-gray-900 dark:text-white truncate">
            {task.title}
          </h4>
        </div>

        {/* Priority Dot */}
        <div
          className={cn(
            "flex-shrink-0 w-2 h-2 rounded-full",
            PRIORITY_COLORS[task.priority]
          )}
          title={task.priority}
        />
      </div>

      {/* Footer */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          {/* Status Badge */}
          <Badge status={STATUS_BADGE_VARIANTS[task.status]} size="sm">
            {STATUS_LABELS[task.status]}
          </Badge>

          {/* Assignee */}
          {task.assignee && (
            <Avatar className="h-6 w-6">
              {task.assignee.avatar && (
                <AvatarImage src={task.assignee.avatar} alt={task.assignee.name} />
              )}
              <AvatarFallback name={task.assignee.name} />
            </Avatar>
          )}
        </div>

        {/* Meta */}
        <div className="flex items-center gap-3 text-xs text-gray-500 dark:text-gray-400">
          {task.commentCount > 0 && (
            <div className="flex items-center gap-1">
              <MessageSquare className="h-3.5 w-3.5" />
              <span>{task.commentCount}</span>
            </div>
          )}
          {task.attachmentCount > 0 && (
            <div className="flex items-center gap-1">
              <Paperclip className="h-3.5 w-3.5" />
              <span>{task.attachmentCount}</span>
            </div>
          )}
        </div>
      </div>
    </div>
  )
})
