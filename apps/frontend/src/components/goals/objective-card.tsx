"use client"

import * as React from "react"
import { memo } from "react"
import { Target, TrendingUp, AlertCircle, CheckCircle2, User, ChevronRight } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { Progress } from "@/components/ui/progress"
import { cn } from "@/lib/utils"
import { Objective, KeyResult } from "@/features/goals/types"

interface ObjectiveCardProps {
  objective: Objective
  keyResults?: KeyResult[]
  ownerName?: string
  onClick?: () => void
  className?: string
  showSubObjectives?: boolean
  subObjectiveCount?: number
}

const STATUS_CONFIG = {
  "on-track": {
    icon: TrendingUp,
    color: "text-success",
    bgColor: "bg-success/10",
    borderColor: "border-success/20",
    label: "On Track"
  },
  "at-risk": {
    icon: AlertCircle,
    color: "text-warning",
    bgColor: "bg-warning/10",
    borderColor: "border-warning/20",
    label: "At Risk"
  },
  "off-track": {
    icon: AlertCircle,
    color: "text-error",
    bgColor: "bg-error/10",
    borderColor: "border-error/20",
    label: "Off Track"
  },
  "completed": {
    icon: CheckCircle2,
    color: "text-primary",
    bgColor: "bg-primary/10",
    borderColor: "border-primary/20",
    label: "Completed"
  }
}

/**
 * ObjectiveCard Component
 *
 * Displays an objective with progress, status badge, and key results summary.
 * Supports hierarchical display with sub-objective indicator.
 */
export const ObjectiveCard = memo(function ObjectiveCard({
  objective,
  keyResults = [],
  ownerName,
  onClick,
  className,
  showSubObjectives = false,
  subObjectiveCount = 0
}: ObjectiveCardProps) {
  const statusConfig = STATUS_CONFIG[objective.status as keyof typeof STATUS_CONFIG]
  const StatusIcon = statusConfig.icon

  const progressColor =
    objective.progress >= 80 ? "bg-success" :
    objective.progress >= 50 ? "bg-warning" :
    "bg-error"

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
        "group bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700",
        "p-5 hover:shadow-lg transition-all duration-200 cursor-pointer",
        "focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2",
        "animate-fade-in",
        statusConfig.borderColor,
        className
      )}
    >
      {/* Header: Title + Status */}
      <div className="flex items-start justify-between gap-3 mb-3">
        <div className="flex items-start gap-2 flex-1 min-w-0">
          <Target className="w-5 h-5 text-primary mt-0.5 flex-shrink-0" />
          <h3 className="font-semibold text-gray-900 dark:text-white truncate">
            {objective.title}
          </h3>
        </div>

        <div className={cn(
          "flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium",
          statusConfig.bgColor,
          statusConfig.color
        )}>
          <StatusIcon className="w-3.5 h-3.5" />
          <span>{statusConfig.label}</span>
        </div>
      </div>

      {/* Description */}
      {objective.description && (
        <p className="text-sm text-gray-600 dark:text-gray-400 mb-3 line-clamp-2">
          {objective.description}
        </p>
      )}

      {/* Progress Bar */}
      <div className="mb-3">
        <div className="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400 mb-1.5">
          <span>Progress</span>
          <span className="font-medium">{objective.progress}%</span>
        </div>
        <Progress
          value={objective.progress}
          className="h-2"
          indicatorClassName={progressColor}
        />
      </div>

      {/* Footer: Owner + Key Results + Sub-objectives */}
      <div className="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400">
        <div className="flex items-center gap-3">
          {/* Owner */}
          {ownerName && (
            <div className="flex items-center gap-1.5">
              <User className="w-3.5 h-3.5" />
              <span className="truncate max-w-[100px]">{ownerName}</span>
            </div>
          )}

          {/* Key Results Count */}
          <div className="flex items-center gap-1.5">
            <Target className="w-3.5 h-3.5" />
            <span>{keyResults.length} KR{keyResults.length !== 1 ? "s" : ""}</span>
          </div>
        </div>

        {/* Sub-objectives Indicator */}
        {showSubObjectives && subObjectiveCount > 0 && (
          <div className="flex items-center gap-1 text-primary">
            <span>{subObjectiveCount} sub-objective{subObjectiveCount !== 1 ? "s" : ""}</span>
            <ChevronRight className="w-4 h-4" />
          </div>
        )}
      </div>
    </div>
  )
})
