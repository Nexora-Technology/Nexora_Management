import apiClient from "@/lib/api-client";
import {
  GoalPeriod,
  Objective,
  KeyResult,
  ObjectiveTreeNode,
  ProgressDashboard,
  PagedResponse,
  CreatePeriodRequest,
  UpdatePeriodRequest,
  CreateObjectiveRequest,
  UpdateObjectiveRequest,
  CreateKeyResultRequest,
  UpdateKeyResultRequest,
  ObjectiveFilters,
  DashboardFilters,
} from "./types";

const GOALS_BASE = "/api/goals";

/**
 * Goals API Client
 * Provides methods for interacting with the Goal Tracking & OKRs backend
 */
export const goalsApi = {
  // ==================== PERIODS ====================

  /**
   * Create a new goal period
   */
  createPeriod: async (data: CreatePeriodRequest): Promise<GoalPeriod> => {
    const response = await apiClient.post<GoalPeriod>(
      `${GOALS_BASE}/periods`,
      data
    );
    return response.data;
  },

  /**
   * Get all periods for a workspace
   */
  getPeriods: async (
    workspaceId: string,
    status?: string
  ): Promise<GoalPeriod[]> => {
    const params = new URLSearchParams();
    params.append("workspaceId", workspaceId);
    if (status) params.append("status", status);

    const response = await apiClient.get<GoalPeriod[]>(
      `${GOALS_BASE}/periods?${params.toString()}`
    );
    return response.data;
  },

  /**
   * Update a goal period
   */
  updatePeriod: async (
    periodId: string,
    data: UpdatePeriodRequest
  ): Promise<GoalPeriod> => {
    const response = await apiClient.put<GoalPeriod>(
      `${GOALS_BASE}/periods/${periodId}`,
      data
    );
    return response.data;
  },

  /**
   * Delete a goal period
   */
  deletePeriod: async (periodId: string): Promise<void> => {
    await apiClient.delete(`${GOALS_BASE}/periods/${periodId}`);
  },

  // ==================== OBJECTIVES ====================

  /**
   * Create a new objective
   */
  createObjective: async (data: CreateObjectiveRequest): Promise<Objective> => {
    const response = await apiClient.post<Objective>(
      `${GOALS_BASE}/objectives`,
      data
    );
    return response.data;
  },

  /**
   * Get objectives with pagination and filters
   */
  getObjectives: async (
    workspaceId: string,
    filters: ObjectiveFilters = {}
  ): Promise<PagedResponse<Objective>> => {
    const params = new URLSearchParams();
    params.append("workspaceId", workspaceId);
    if (filters.periodId) params.append("periodId", filters.periodId);
    if (filters.parentObjectiveId !== undefined)
      params.append("parentObjectiveId", filters.parentObjectiveId || "");
    if (filters.status) params.append("status", filters.status);
    params.append("page", String(filters.page || 1));
    params.append("pageSize", String(filters.pageSize || 20));

    const response = await apiClient.get<PagedResponse<Objective>>(
      `${GOALS_BASE}/objectives?${params.toString()}`
    );
    return response.data;
  },

  /**
   * Get objective tree (hierarchical view)
   */
  getObjectiveTree: async (
    workspaceId: string,
    periodId?: string
  ): Promise<ObjectiveTreeNode[]> => {
    const params = new URLSearchParams();
    params.append("workspaceId", workspaceId);
    if (periodId) params.append("periodId", periodId);

    const response = await apiClient.get<ObjectiveTreeNode[]>(
      `${GOALS_BASE}/objectives/tree?${params.toString()}`
    );
    return response.data;
  },

  /**
   * Update an objective
   */
  updateObjective: async (
    objectiveId: string,
    data: UpdateObjectiveRequest
  ): Promise<Objective> => {
    const response = await apiClient.put<Objective>(
      `${GOALS_BASE}/objectives/${objectiveId}`,
      data
    );
    return response.data;
  },

  /**
   * Delete an objective
   */
  deleteObjective: async (objectiveId: string): Promise<void> => {
    await apiClient.delete(`${GOALS_BASE}/objectives/${objectiveId}`);
  },

  // ==================== KEY RESULTS ====================

  /**
   * Create a new key result
   */
  createKeyResult: async (data: CreateKeyResultRequest): Promise<KeyResult> => {
    const response = await apiClient.post<KeyResult>(
      `${GOALS_BASE}/keyresults`,
      data
    );
    return response.data;
  },

  /**
   * Update a key result
   */
  updateKeyResult: async (
    keyResultId: string,
    data: UpdateKeyResultRequest
  ): Promise<KeyResult> => {
    const response = await apiClient.put<KeyResult>(
      `${GOALS_BASE}/keyresults/${keyResultId}`,
      data
    );
    return response.data;
  },

  /**
   * Delete a key result
   */
  deleteKeyResult: async (keyResultId: string): Promise<void> => {
    await apiClient.delete(`${GOALS_BASE}/keyresults/${keyResultId}`);
  },

  // ==================== DASHBOARD ====================

  /**
   * Get progress dashboard statistics
   */
  getProgressDashboard: async (
    workspaceId: string,
    filters: DashboardFilters = {}
  ): Promise<ProgressDashboard> => {
    const params = new URLSearchParams();
    params.append("workspaceId", workspaceId);
    if (filters.periodId) params.append("periodId", filters.periodId);

    const response = await apiClient.get<ProgressDashboard>(
      `${GOALS_BASE}/dashboard?${params.toString()}`
    );
    return response.data;
  },
};
