export type TaskStatus = "todo" | "inProgress" | "complete" | "overdue"
export type TaskPriority = "urgent" | "high" | "medium" | "low"
export type TaskType = "epic" | "story" | "subtask"
export type ViewMode = "list" | "board" | "calendar" | "gantt"

export interface Task {
  id: string
  title: string
  description?: string
  status: TaskStatus
  priority: TaskPriority
  taskType: TaskType
  // Hierarchy fields - 3-level hierarchy: Epic → Story → Subtask
  parentTaskId?: string | null  // Generic parent reference
  epicId?: string | null        // For stories referencing their epic
  storyId?: string | null       // For subtasks referencing their story
  assignee?: {
    id: string
    name: string
    avatar?: string
  }
  dueDate?: string
  startDate?: string
  estimatedHours?: number
  commentCount: number
  attachmentCount: number
  projectId: string  // Deprecated: Use listId instead
  listId?: string    // NEW: Reference to TaskList (replaces projectId)
  spaceId?: string   // NEW: Reference to Space
  folderId?: string  // NEW: Reference to Folder (optional)
  createdAt: string
  updatedAt: string
}

export interface TaskFilter {
  status?: TaskStatus | "all"
  priority?: TaskPriority | "all"
  search?: string
}
