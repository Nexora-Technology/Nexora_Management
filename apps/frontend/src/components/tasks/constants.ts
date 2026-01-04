import type { TaskPriority, TaskStatus } from "./types"

/**
 * Priority color mappings for consistent styling across components
 */
export const PRIORITY_COLORS: Record<TaskPriority, string> = {
  urgent: "bg-red-500",
  high: "bg-orange-500",
  medium: "bg-yellow-500",
  low: "bg-blue-500",
}

/**
 * Status label mappings for consistent display text
 */
export const STATUS_LABELS: Record<TaskStatus, string> = {
  todo: "To Do",
  inProgress: "In Progress",
  complete: "Complete",
  overdue: "Overdue",
}

/**
 * Status to badge variant mapping
 */
export const STATUS_BADGE_VARIANTS: Record<
  TaskStatus,
  "complete" | "inProgress" | "overdue" | "neutral"
> = {
  todo: "neutral",
  inProgress: "inProgress",
  complete: "complete",
  overdue: "overdue",
}
