'use client';

import * as React from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { workspacesApi } from './api';
import type { Workspace, CreateWorkspaceRequest } from './types';

interface WorkspaceContextType {
  // State
  currentWorkspace: Workspace | null;
  workspaces: Workspace[];
  isLoading: boolean;
  error: Error | null;

  // Actions
  setCurrentWorkspace: (workspace: Workspace | null) => void;
  switchWorkspace: (workspaceId: string) => Promise<void>;
  createWorkspace: (data: CreateWorkspaceRequest) => Promise<Workspace>;
  refetchWorkspaces: () => void;
}

const WorkspaceContext = React.createContext<WorkspaceContextType | undefined>(undefined);

interface WorkspaceProviderProps {
  children: React.ReactNode;
}

export function WorkspaceProvider({ children }: WorkspaceProviderProps) {
  const queryClient = useQueryClient();
  const [currentWorkspaceId, setCurrentWorkspaceId] = React.useState<string | null>(() => {
    // Load from localStorage on mount
    if (typeof window !== 'undefined') {
      const stored = localStorage.getItem('current_workspace_id');
      // Basic validation: ensure it's a non-empty string
      if (stored && stored.trim().length > 0) {
        return stored;
      }
    }
    return null;
  });

  // Fetch all workspaces for the current user
  const {
    data: workspaces = [],
    isLoading,
    error,
    refetch,
  } = useQuery({
    queryKey: ['workspaces'],
    queryFn: async () => {
      const response = await workspacesApi.getWorkspacesByUser();
      return response.data;
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
  });

  // Derive current workspace from workspaces list
  const currentWorkspace = React.useMemo(() => {
    if (!currentWorkspaceId || workspaces.length === 0) return null;

    // Find workspace by ID
    const workspace = workspaces.find((w) => w.id === currentWorkspaceId);
    if (workspace) return workspace;

    // If current workspace ID not found, fall back to default workspace
    const defaultWorkspace = workspaces.find((w) => w.isDefault);
    if (defaultWorkspace) {
      setCurrentWorkspaceId(defaultWorkspace.id);
      return defaultWorkspace;
    }

    // If no default, return first workspace
    if (workspaces.length > 0) {
      setCurrentWorkspaceId(workspaces[0].id);
      return workspaces[0];
    }

    return null;
  }, [currentWorkspaceId, workspaces]);

  // Set current workspace
  const setCurrentWorkspace = React.useCallback((workspace: Workspace | null) => {
    if (workspace) {
      setCurrentWorkspaceId(workspace.id);
      if (typeof window !== 'undefined') {
        localStorage.setItem('current_workspace_id', workspace.id);
      }
    } else {
      setCurrentWorkspaceId(null);
      if (typeof window !== 'undefined') {
        localStorage.removeItem('current_workspace_id');
      }
    }
  }, []);

  // Switch workspace mutation
  const switchWorkspaceMutation = useMutation({
    mutationFn: async (workspaceId: string) => {
      await workspacesApi.switchWorkspace(workspaceId);
    },
    onSuccess: (_, workspaceId) => {
      const workspace = workspaces.find((w) => w.id === workspaceId);
      if (workspace) {
        setCurrentWorkspace(workspace);
      }
      // Invalidate all queries that depend on workspace
      queryClient.invalidateQueries({ queryKey: ['spaces'] });
      queryClient.invalidateQueries({ queryKey: ['folders'] });
      queryClient.invalidateQueries({ queryKey: ['tasklists'] });
    },
  });

  // Create workspace mutation
  const createWorkspaceMutation = useMutation({
    mutationFn: async (data: CreateWorkspaceRequest) => {
      const response = await workspacesApi.createWorkspace(data);
      return response.data;
    },
    onSuccess: (newWorkspace) => {
      // Invalidate workspaces query to refetch
      queryClient.invalidateQueries({ queryKey: ['workspaces'] });
      // Switch to new workspace
      setCurrentWorkspace(newWorkspace);
    },
  });

  const value: WorkspaceContextType = {
    currentWorkspace,
    workspaces,
    isLoading,
    error,
    setCurrentWorkspace,
    switchWorkspace: (workspaceId: string) => switchWorkspaceMutation.mutateAsync(workspaceId),
    createWorkspace: (data: CreateWorkspaceRequest) => createWorkspaceMutation.mutateAsync(data),
    refetchWorkspaces: () => refetch(),
  };

  return <WorkspaceContext value={value}>{children}</WorkspaceContext>;
}

export function useWorkspace(): WorkspaceContextType {
  const context = React.useContext(WorkspaceContext);
  if (context === undefined) {
    throw new Error('useWorkspace must be used within a WorkspaceProvider');
  }
  return context;
}
