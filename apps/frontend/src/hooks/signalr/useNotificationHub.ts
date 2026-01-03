import { useEffect, useRef, useState, useCallback } from 'react';
import { NotificationHubConnection } from '@/lib/signalr/notification-hub';
import type { NotificationMessage, ConnectionState } from '@/lib/signalr/types';
import { useAuth } from '@/features/auth/providers/auth-provider';
import toast from 'react-hot-toast';

export function useNotificationHub() {
  const { token } = useAuth();
  const hubRef = useRef<NotificationHubConnection | null>(null);
  const [connectionState, setConnectionState] = useState<ConnectionState>('disconnected');
  const [notifications, setNotifications] = useState<NotificationMessage[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);

  const startConnection = useCallback(async () => {
    if (!token || hubRef.current) return;

    try {
      const hub = new NotificationHubConnection(token);
      hubRef.current = hub;

      hub.onNotificationReceived((message) => {
        setNotifications((prev) => [message, ...prev]);
        setUnreadCount((prev) => prev + 1);

        // Show toast notification
        toast.success(`${message.title}: ${message.message}`, {
          duration: 5000,
        });
      });

      await hub.start();
      setConnectionState('connected');

      await hub.joinUserNotifications();
    } catch (error) {
      console.error('Failed to connect to NotificationHub:', error);
      setConnectionState('disconnected');
    }
  }, [token]);

  const stopConnection = useCallback(async () => {
    if (hubRef.current) {
      try {
        await hubRef.current.stop();
        hubRef.current = null;
        setConnectionState('disconnected');
      } catch (error) {
        console.error('Error disconnecting NotificationHub:', error);
      }
    }
  }, []);

  const markAsRead = useCallback(
    async (notificationId: string) => {
      await hubRef.current?.markNotificationRead(notificationId);
      setNotifications((prev) =>
        prev.map((n) =>
          n.notificationId === notificationId
            ? { ...n, readAt: new Date().toISOString() }
            : n
        )
      );
      setUnreadCount((prev) => Math.max(0, prev - 1));
    },
    []
  );

  const markAllAsRead = useCallback(async () => {
    await hubRef.current?.markAllNotificationsRead();
    setNotifications((prev) =>
      prev.map((n) => ({ ...n, readAt: new Date().toISOString() }))
    );
    setUnreadCount(0);
  }, []);

  // Manage connection lifecycle
  useEffect(() => {
    startConnection();

    return () => {
      stopConnection();
    };
  }, [startConnection, stopConnection]);

  const onNotificationReceived = useCallback((callback: (message: NotificationMessage) => void) => {
    hubRef.current?.onNotificationReceived(callback);
  }, []);

  return {
    connectionState,
    isConnected: connectionState === 'connected',
    hub: hubRef.current,
    notifications,
    unreadCount,
    markAsRead,
    markAllAsRead,
    listeners: {
      onNotificationReceived,
    },
  };
}
