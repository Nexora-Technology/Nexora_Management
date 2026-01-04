export type TaskStatus = "todo" | "inProgress" | "complete" | "overdue"
export type TaskPriority = "urgent" | "high" | "medium" | "low"

export interface Task {
  id: string
  title: string
  description?: string
  status: TaskStatus
  priority: TaskPriority
  assignee?: {
    id: string
    name: string
    avatar?: string
  }
  dueDate?: string
  commentCount: number
  attachmentCount: number
  projectId: string
  createdAt: string
  updatedAt: string
}

export interface TaskFilter {
  status?: TaskStatus | "all"
  priority?: TaskPriority | "all"
  search?: string
}
