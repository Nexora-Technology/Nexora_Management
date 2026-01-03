export interface Task {
  id: string;
  projectId: string;
  parentTaskId: string | null;
  title: string;
  description: string | null;
  statusId: string;
  priority: string;
  assigneeId: string | null;
  dueDate: string | null;
  startDate: string | null;
  estimatedHours: number | null;
  positionOrder: number;
  createdBy: string;
  createdAt: string;
  updatedAt: string;
}

export interface TaskStatus {
  id: string;
  projectId: string;
  name: string;
  color: string | null;
  orderIndex: number;
  type: string;
}

export interface TaskFilters {
  statusId?: string;
  assigneeId?: string;
  search?: string;
  parentTaskId?: string | null;
}

export interface TaskSort {
  field: "title" | "status" | "assignee" | "dueDate" | "priority" | "createdAt";
  direction: "asc" | "desc";
}
