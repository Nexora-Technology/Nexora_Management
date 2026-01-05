/**
 * Goal Tracking & OKRs Types
 * Phase 08: Goal Tracking & OKRs
 */

// ==================== Domain Types ====================

export interface GoalPeriod {
  id: string;
  workspaceId: string;
  name: string;
  startDate: string;
  endDate: string;
  status: "active" | "archived";
  createdAt: string;
  updatedAt: string;
}

export interface Objective {
  id: string;
  workspaceId: string;
  periodId: string | null;
  parentObjectiveId: string | null;
  title: string;
  description: string | null;
  ownerId: string | null;
  weight: number;
  status: "on-track" | "at-risk" | "off-track" | "completed";
  progress: number;
  positionOrder: number;
  createdAt: string;
  updatedAt: string;
}

export interface KeyResult {
  id: string;
  objectiveId: string;
  title: string;
  metricType: "number" | "percentage" | "currency";
  currentValue: number;
  targetValue: number;
  unit: string;
  dueDate: string | null;
  progress: number;
  weight: number;
  createdAt: string;
  updatedAt: string;
}

// ==================== Tree & Dashboard Types ====================

export interface ObjectiveTreeNode extends Objective {
  ownerName: string | null;
  ownerEmail: string | null;
  keyResults: KeyResult[];
  subObjectives: ObjectiveTreeNode[];
}

export interface StatusBreakdown {
  status: string;
  count: number;
  percentage: number;
}

export interface ObjectiveSummary {
  id: string;
  title: string;
  status: string;
  progress: number;
  keyResultCount: number;
  ownerId: string | null;
}

export interface ProgressDashboard {
  totalObjectives: number;
  averageProgress: number;
  totalKeyResults: number;
  completedKeyResults: number;
  statusBreakdown: StatusBreakdown[];
  topObjectives: ObjectiveSummary[];
  bottomObjectives: ObjectiveSummary[];
}

export interface PagedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// ==================== Request/Response Types ====================

export interface CreatePeriodRequest {
  workspaceId: string;
  name: string;
  startDate: string;
  endDate: string;
}

export interface UpdatePeriodRequest {
  name?: string;
  startDate?: string;
  endDate?: string;
  status?: string;
}

export interface CreateObjectiveRequest {
  workspaceId: string;
  periodId?: string;
  parentObjectiveId?: string;
  title: string;
  description?: string;
  ownerId?: string;
  weight?: number;
}

export interface UpdateObjectiveRequest {
  title?: string;
  description?: string;
  ownerId?: string;
  weight?: number;
  status?: string;
  positionOrder?: number;
}

export interface CreateKeyResultRequest {
  objectiveId: string;
  title: string;
  metricType: string;
  currentValue: number;
  targetValue: number;
  unit: string;
  dueDate?: string;
  weight?: number;
}

export interface UpdateKeyResultRequest {
  title?: string;
  currentValue?: number;
  targetValue?: number;
  dueDate?: string;
  weight?: number;
}

// ==================== Filter & Query Types ====================

export interface ObjectiveFilters {
  periodId?: string;
  parentObjectiveId?: string | null;
  status?: string;
  page?: number;
  pageSize?: number;
}

export interface DashboardFilters {
  periodId?: string;
}

// ==================== UI State Types ====================

export interface GoalsUIState {
  selectedPeriodId: string | null;
  selectedObjectiveId: string | null;
  viewMode: "tree" | "list" | "dashboard";
  filters: ObjectiveFilters;
}
