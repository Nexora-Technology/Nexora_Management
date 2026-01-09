import { apiClient } from '../api-client';

export interface DashboardDto {
  id: string;
  workspaceId: string;
  name: string;
  layout: string | null;
  createdBy: string;
  isTemplate: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateDashboardRequest {
  workspaceId: string;
  name: string;
  layout?: string;
  isTemplate?: boolean;
}

export interface UpdateDashboardRequest {
  name?: string;
  layout?: string;
}

export const dashboardService = {
  async getDashboards(workspaceId: string): Promise<DashboardDto[]> {
    const response = await apiClient.get('/api/dashboards', {
      params: { workspaceId }
    });
    return response.data;
  },

  async getDashboard(id: string): Promise<DashboardDto> {
    const response = await apiClient.get(`/api/dashboards/${id}`);
    return response.data;
  },

  async createDashboard(request: CreateDashboardRequest): Promise<DashboardDto> {
    const response = await apiClient.post('/api/dashboards', request);
    return response.data;
  },

  async updateDashboard(id: string, request: UpdateDashboardRequest): Promise<DashboardDto> {
    const response = await apiClient.put(`/api/dashboards/${id}`, request);
    return response.data;
  },

  async deleteDashboard(id: string): Promise<void> {
    await apiClient.delete(`/api/dashboards/${id}`);
  }
};
