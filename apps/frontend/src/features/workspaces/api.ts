import apiClient from '@/lib/api-client';
import type {
  Workspace,
  CreateWorkspaceRequest,
  UpdateWorkspaceRequest,
} from './types';

// Workspaces API
export const getWorkspaceById = (id: string) =>
  apiClient.get<Workspace>(`/api/workspaces/${id}`);

export const getWorkspacesByUser = () =>
  apiClient.get<Workspace[]>('/api/workspaces');

export const createWorkspace = (data: CreateWorkspaceRequest) =>
  apiClient.post<Workspace>('/api/workspaces', data);

export const updateWorkspace = (id: string, data: UpdateWorkspaceRequest) =>
  apiClient.put<Workspace>(`/api/workspaces/${id}`, data);

export const deleteWorkspace = (id: string) =>
  apiClient.delete(`/api/workspaces/${id}`);

export const switchWorkspace = (workspaceId: string) =>
  apiClient.post(`/api/workspaces/${workspaceId}/switch`, {});

// Export as a grouped object for convenience
export const workspacesApi = {
  getWorkspaceById,
  getWorkspacesByUser,
  createWorkspace,
  updateWorkspace,
  deleteWorkspace,
  switchWorkspace,
};
