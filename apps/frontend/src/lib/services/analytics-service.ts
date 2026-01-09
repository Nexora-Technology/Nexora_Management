import { apiClient } from '../api-client';

export interface DashboardStatsDto {
  totalTasks: number;
  completedTasks: number;
  inProgressTasks: number;
  overdueTasks: number;
  totalProjects: number;
  activeProjects: number;
  completionPercentage: number;
  totalTeamMembers: number;
  activeMembers: number;
}

export interface ProjectProgressDto {
  projectId: string;
  projectName: string;
  totalTasks: number;
  completedTasks: number;
  inProgressTasks: number;
  completionPercentage: number;
  color: string | null;
}

export interface TeamWorkloadDto {
  userId: string;
  userName: string;
  userAvatar: string | null;
  assignedTasks: number;
  completedTasks: number;
  inProgressTasks: number;
  completionRate: number;
  totalHours: number;
}

export const analyticsService = {
  async getDashboardStats(workspaceId: string): Promise<DashboardStatsDto> {
    const response = await apiClient.get(`/api/analytics/dashboard/${workspaceId}`);
    return response.data;
  },

  async getProjectProgress(workspaceId: string): Promise<ProjectProgressDto[]> {
    const response = await apiClient.get(`/api/analytics/project/${workspaceId}/progress`);
    return response.data;
  },

  async getTeamWorkload(workspaceId: string): Promise<TeamWorkloadDto[]> {
    const response = await apiClient.get(`/api/analytics/team/${workspaceId}/workload`);
    return response.data;
  }
};
