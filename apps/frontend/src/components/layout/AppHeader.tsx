"use client";

import { useEffect, useState } from "react";
import { NotificationCenter, type Notification } from "@/features/notifications/NotificationCenter";
import { useNotificationHub } from "@/hooks/signalr/useNotificationHub";

export function AppHeader() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);

  const {
    isConnected,
    listeners
  } = useNotificationHub();

  useEffect(() => {
    // TODO: Fetch initial notifications from API
    // fetchNotifications().then(setNotifications);
  }, []);

  // Register NotificationReceived listener
  useEffect(() => {
    const unsubscribe = listeners.onNotificationReceived((message) => {
      const newNotification: Notification = {
        id: message.notificationId,
        type: message.type,
        title: message.title,
        message: message.message,
        actionUrl: message.actionUrl,
        isRead: false,
        createdAt: new Date(message.createdAt)
      };

      // Add notification to list
      setNotifications((prev) => [newNotification, ...prev]);

      // Increment unread count
      setUnreadCount((prev) => prev + 1);

      // Show toast notification (already handled by useNotificationHub)
    });

    return unsubscribe;
  }, [listeners]);

  const handleMarkAsRead = (id: string) => {
    setNotifications((prev) =>
      prev.map((notification) =>
        notification.id === id ? { ...notification, isRead: true } : notification
      )
    );
    setUnreadCount((prev) => Math.max(0, prev - 1));

    // TODO: Call API to mark as read
    // markNotificationAsRead(id);
  };

  const handleMarkAllAsRead = () => {
    setNotifications((prev) =>
      prev.map((notification) => ({ ...notification, isRead: true }))
    );
    setUnreadCount(0);

    // TODO: Call API to mark all as read
    // markAllNotificationsAsRead();
  };

  const handleDelete = (id: string) => {
    setNotifications((prev) => prev.filter((notification) => notification.id !== id));
    if (!notifications.find((n) => n.id === id)?.isRead) {
      setUnreadCount((prev) => Math.max(0, prev - 1));
    }

    // TODO: Call API to delete notification
    // deleteNotification(id);
  };

  return (
    <header className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container flex h-16 items-center justify-between">
        <div className="flex items-center gap-8">
          <div className="flex items-center gap-2">
            <span className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              Nexora
            </span>
          </div>

          {/* Navigation links would go here */}
          <nav className="hidden md:flex items-center gap-6">
            <a href="/projects" className="text-sm font-medium text-muted-foreground hover:text-foreground">
              Projects
            </a>
            <a href="/tasks" className="text-sm font-medium text-muted-foreground hover:text-foreground">
              My Tasks
            </a>
            <a href="/team" className="text-sm font-medium text-muted-foreground hover:text-foreground">
              Team
            </a>
          </nav>
        </div>

        <div className="flex items-center gap-4">
          {/* Connection status indicator */}
          <div className="hidden sm:flex items-center gap-2">
            <div className={`h-2 w-2 rounded-full ${isConnected ? "bg-green-500" : "bg-red-500"}`} />
            <span className="text-xs text-muted-foreground">
              {isConnected ? "Connected" : "Connecting..."}
            </span>
          </div>

          {/* Notification Center */}
          <NotificationCenter
            notifications={notifications}
            unreadCount={unreadCount}
            onMarkAsRead={handleMarkAsRead}
            onMarkAllAsRead={handleMarkAllAsRead}
            onDelete={handleDelete}
          />

          {/* User menu would go here */}
          <div className="h-8 w-8 rounded-full bg-gradient-to-r from-blue-600 to-purple-600 flex items-center justify-center text-white font-medium text-sm cursor-pointer">
            JD
          </div>
        </div>
      </div>
    </header>
  );
}
