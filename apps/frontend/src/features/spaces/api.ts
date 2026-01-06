import apiClient from '@/lib/api-client';
import type {
  Space,
  CreateSpaceRequest,
  UpdateSpaceRequest,
  Folder,
  CreateFolderRequest,
  UpdateFolderRequest,
  UpdateFolderPositionRequest,
  TaskList,
  CreateTaskListRequest,
  UpdateTaskListRequest,
  UpdateTaskListPositionRequest,
} from './types';

// Spaces API
export const getSpaceById = (id: string) =>
  apiClient.get<Space>(`/api/spaces/${id}`);

export const getSpacesByWorkspace = (workspaceId: string) =>
  apiClient.get<Space[]>('/api/spaces', { params: { workspaceId } });

export const createSpace = (data: CreateSpaceRequest) =>
  apiClient.post<Space>('/api/spaces', data);

export const updateSpace = (id: string, data: UpdateSpaceRequest) =>
  apiClient.put<Space>(`/api/spaces/${id}`, data);

export const deleteSpace = (id: string) =>
  apiClient.delete(`/api/spaces/${id}`);

// Folders API
export const getFolderById = (id: string) =>
  apiClient.get<Folder>(`/api/folders/${id}`);

export const getFoldersBySpace = (spaceId: string) =>
  apiClient.get<Folder[]>(`/api/spaces/${spaceId}/folders`);

export const createFolder = (data: CreateFolderRequest) =>
  apiClient.post<Folder>('/api/folders', data);

export const updateFolder = (id: string, data: UpdateFolderRequest) =>
  apiClient.put<Folder>(`/api/folders/${id}`, data);

export const updateFolderPosition = (id: string, data: UpdateFolderPositionRequest) =>
  apiClient.patch<Folder>(`/api/folders/${id}/position`, data);

export const deleteFolder = (id: string) =>
  apiClient.delete(`/api/folders/${id}`);

// TaskLists API (Note: endpoint is /api/tasklists)
export const getTaskListById = (id: string) =>
  apiClient.get<TaskList>(`/api/tasklists/${id}`);

export const getTaskLists = (spaceId?: string, folderId?: string) =>
  apiClient.get<TaskList[]>('/api/tasklists', { params: { spaceId, folderId } });

export const createTaskList = (data: CreateTaskListRequest) =>
  apiClient.post<TaskList>('/api/tasklists', data);

export const updateTaskList = (id: string, data: UpdateTaskListRequest) =>
  apiClient.put<TaskList>(`/api/tasklists/${id}`, data);

export const updateTaskListPosition = (id: string, data: UpdateTaskListPositionRequest) =>
  apiClient.patch<TaskList>(`/api/tasklists/${id}/position`, data);

export const deleteTaskList = (id: string) =>
  apiClient.delete(`/api/tasklists/${id}`);

// Export as a grouped object for convenience
export const spacesApi = {
  // Spaces
  getSpaceById,
  getSpacesByWorkspace,
  createSpace,
  updateSpace,
  deleteSpace,
  // Folders
  getFolderById,
  getFoldersBySpace,
  createFolder,
  updateFolder,
  updateFolderPosition,
  deleteFolder,
  // TaskLists
  getTaskListById,
  getTaskLists,
  createTaskList,
  updateTaskList,
  updateTaskListPosition,
  deleteTaskList,
};
