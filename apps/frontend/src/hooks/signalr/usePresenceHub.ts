import { useEffect, useRef, useState, useCallback } from 'react';
import { PresenceHubConnection } from '@/lib/signalr/presence-hub';
import type { UserPresenceMessage, TypingIndicatorMessage, ConnectionState } from '@/lib/signalr/types';
import { useAuth } from '@/features/auth/providers/auth-provider';

interface OnlineUser {
  userId: string;
  userName: string;
  userEmail?: string;
  workspaceId: string;
  lastSeen: Date;
  currentView?: {
    entityType: 'project' | 'task';
    entityId: string;
    viewType: 'board' | 'list' | 'task';
  };
}

export function usePresenceHub(workspaceId?: string) {
  const { token } = useAuth();
  const hubRef = useRef<PresenceHubConnection | null>(null);
  const [connectionState, setConnectionState] = useState<ConnectionState>('disconnected');
  const [onlineUsers, setOnlineUsers] = useState<Map<string, OnlineUser>>(new Map());
  const [typingUsers, setTypingUsers] = useState<Map<string, TypingIndicatorMessage>>(new Map());

  const startConnection = useCallback(async () => {
    if (!token || hubRef.current) return;

    try {
      const hub = new PresenceHubConnection(token);
      hubRef.current = hub;

      // Set up event handlers
      hub.onUserJoined((message) => {
        setOnlineUsers((prev) => {
          const next = new Map(prev);
          next.set(message.userId, {
            userId: message.userId,
            userName: message.userName,
            workspaceId: message.workspaceId,
            lastSeen: new Date(message.lastSeen),
          });
          return next;
        });
      });

      hub.onUserLeft((message) => {
        setOnlineUsers((prev) => {
          const next = new Map(prev);
          next.delete(message.userId);
          return next;
        });
      });

      hub.onUserTyping((message) => {
        if (!message.isTyping) {
          setTypingUsers((prev) => {
            const next = new Map(prev);
            next.delete(message.userId);
            return next;
          });
          return;
        }

        setTypingUsers((prev) => {
          const next = new Map(prev);
          next.set(message.userId, message);
          // Auto-remove after 3 seconds
          setTimeout(() => {
            setTypingUsers((current) => {
              const updated = new Map(current);
              updated.delete(message.userId);
              return updated;
            });
          }, 3000);
          return next;
        });
      });

      await hub.start();
      setConnectionState('connected');

      if (workspaceId) {
        await hub.joinWorkspace(workspaceId);
      }
    } catch (error) {
      console.error('Failed to connect to PresenceHub:', error);
      setConnectionState('disconnected');
    }
  }, [token, workspaceId]);

  const stopConnection = useCallback(async () => {
    if (hubRef.current) {
      try {
        if (workspaceId) {
          await hubRef.current.leaveWorkspace(workspaceId);
        }
        await hubRef.current.stop();
        hubRef.current = null;
        setConnectionState('disconnected');
      } catch (error) {
        console.error('Error disconnecting PresenceHub:', error);
      }
    }
  }, [workspaceId]);

  const isUserOnline = useCallback(
    (userId: string) => {
      const user = onlineUsers.get(userId);
      if (!user) return false;
      // Consider user online if seen in last 5 minutes
      const fiveMinutesAgo = new Date(Date.now() - 5 * 60 * 1000);
      return user.lastSeen > fiveMinutesAgo;
    },
    [onlineUsers]
  );

  const joinViewing = useCallback(async (entityId: string, viewType?: 'board' | 'list' | 'task') => {
    if (!hubRef.current) return;
    try {
      await hubRef.current.joinViewing(entityId);
    } catch (error) {
      console.error('Error joining viewing:', error);
    }
  }, []);

  const leaveViewing = useCallback(async (entityId: string, viewType?: 'board' | 'list' | 'task') => {
    if (!hubRef.current) return;
    try {
      await hubRef.current.leaveViewing(entityId);
    } catch (error) {
      console.error('Error leaving viewing:', error);
    }
  }, []);

  const trackTyping = useCallback(async (taskId: string) => {
    if (!hubRef.current) return;
    try {
      await hubRef.current.startTyping(taskId);
      // Auto-clear typing after 3 seconds
      setTimeout(() => {
        hubRef.current?.stopTyping(taskId);
      }, 3000);
    } catch (error) {
      console.error('Error tracking typing:', error);
    }
  }, []);

  // Manage connection lifecycle
  useEffect(() => {
    startConnection();

    return () => {
      stopConnection();
    };
  }, [startConnection, stopConnection]);

  // Update last seen every 2 minutes
  useEffect(() => {
    if (connectionState !== 'connected') return;

    const interval = setInterval(() => {
      hubRef.current?.updateLastSeen();
    }, 2 * 60 * 1000);

    return () => clearInterval(interval);
  }, [connectionState]);

  return {
    connectionState,
    isConnected: connectionState === 'connected',
    hub: hubRef.current,
    onlineUsers: Array.from(onlineUsers.values()),
    typingUsers: Array.from(typingUsers.values()),
    isUserOnline,
    joinViewing,
    leaveViewing,
    trackTyping,
  };
}
