import { useEffect, useRef, useState, useCallback } from 'react';
import { TaskHubConnection } from '@/lib/signalr/task-hub';
import type { TaskUpdatedMessage, ConnectionState } from '@/lib/signalr/types';
import { useAuth } from '@/features/auth/providers/auth-provider';

export function useTaskHub(projectId?: string) {
  const { token } = useAuth();
  const hubRef = useRef<TaskHubConnection | null>(null);
  const [connectionState, setConnectionState] = useState<ConnectionState>('disconnected');
  const [currentProject, setCurrentProject] = useState<string | undefined>(projectId);

  const startConnection = useCallback(async () => {
    if (!token || hubRef.current) return;

    try {
      const hub = new TaskHubConnection(token);
      hubRef.current = hub;

      await hub.start();
      setConnectionState('connected');

      if (currentProject) {
        await hub.joinProject(currentProject);
      }
    } catch (error) {
      console.error('Failed to connect to TaskHub:', error);
      setConnectionState('disconnected');
    }
  }, [token, currentProject]);

  const stopConnection = useCallback(async () => {
    if (hubRef.current) {
      try {
        if (currentProject) {
          await hubRef.current.leaveProject(currentProject);
        }
        await hubRef.current.stop();
        hubRef.current = null;
        setConnectionState('disconnected');
      } catch (error) {
        console.error('Error disconnecting TaskHub:', error);
      }
    }
  }, [currentProject]);

  const joinProject = useCallback(async (newProjectId: string) => {
    if (!hubRef.current || !newProjectId) return;

    try {
      if (currentProject && currentProject !== newProjectId) {
        await hubRef.current.leaveProject(currentProject);
      }
      await hubRef.current.joinProject(newProjectId);
      setCurrentProject(newProjectId);
    } catch (error) {
      console.error('Error joining project:', error);
    }
  }, [currentProject]);

  const onTaskUpdated = useCallback((callback: (message: TaskUpdatedMessage) => void) => {
    hubRef.current?.onTaskUpdated(callback);
  }, []);

  const onTaskCreated = useCallback((callback: (message: TaskUpdatedMessage) => void) => {
    hubRef.current?.onTaskCreated(callback);
  }, []);

  const onTaskDeleted = useCallback((callback: (message: TaskUpdatedMessage) => void) => {
    hubRef.current?.onTaskDeleted(callback);
  }, []);

  const onStatusChanged = useCallback((callback: (message: TaskUpdatedMessage) => void) => {
    hubRef.current?.onStatusChanged(callback);
  }, []);

  // Manage connection lifecycle
  useEffect(() => {
    startConnection();

    return () => {
      stopConnection();
    };
  }, [startConnection, stopConnection]);

  // Handle projectId changes
  useEffect(() => {
    if (projectId && projectId !== currentProject && connectionState === 'connected') {
      joinProject(projectId);
    }
  }, [projectId, currentProject, connectionState, joinProject]);

  return {
    connectionState,
    isConnected: connectionState === 'connected',
    hub: hubRef.current,
    joinProject,
    leaveProject: stopConnection,
    listeners: {
      onTaskUpdated,
      onTaskCreated,
      onTaskDeleted,
      onTaskStatusChanged: onStatusChanged,
      onCommentAdded: useCallback((_callback: (message: Record<string, unknown>) => void) => {
        // TODO: Implement when comment hub is ready
        console.warn('onCommentAdded not yet implemented');
      }, []),
      onCommentUpdated: useCallback((_callback: (message: Record<string, unknown>) => void) => {
        // TODO: Implement when comment hub is ready
        console.warn('onCommentUpdated not yet implemented');
      }, []),
      onCommentDeleted: useCallback((_callback: (message: Record<string, unknown>) => void) => {
        // TODO: Implement when comment hub is ready
        console.warn('onCommentDeleted not yet implemented');
      }, []),
      onAttachmentUploaded: useCallback((_callback: (message: Record<string, unknown>) => void) => {
        // TODO: Implement when attachment hub is ready
        console.warn('onAttachmentUploaded not yet implemented');
      }, []),
      onAttachmentDeleted: useCallback((_callback: (message: Record<string, unknown>) => void) => {
        // TODO: Implement when attachment hub is ready
        console.warn('onAttachmentDeleted not yet implemented');
      }, []),
    },
  };
}
