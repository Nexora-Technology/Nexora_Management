"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Bell, Trash2, Check, CheckCheck } from "lucide-react";
import { cn } from "@/lib/utils";

export interface Notification {
  id: string;
  type: string;
  title: string;
  message?: string;
  actionUrl?: string;
  isRead: boolean;
  createdAt: Date;
}

interface NotificationCenterProps {
  notifications: Notification[];
  unreadCount: number;
  onMarkAsRead?: (id: string) => void;
  onMarkAllAsRead?: () => void;
  onDelete?: (id: string) => void;
  className?: string;
}

export function NotificationCenter({
  notifications,
  unreadCount,
  onMarkAsRead,
  onMarkAllAsRead,
  onDelete,
  className = ""
}: NotificationCenterProps) {
  const [open, setOpen] = useState(false);

  const handleNotificationClick = (notification: Notification) => {
    if (!notification.isRead && onMarkAsRead) {
      onMarkAsRead(notification.id);
    }
    if (notification.actionUrl) {
      window.location.href = notification.actionUrl;
    }
    setOpen(false);
  };

  const getNotificationIcon = (type: string) => {
    switch (type) {
      case "task_assigned":
        return "📋";
      case "comment_mentioned":
        return "💬";
      case "status_changed":
        return "🔄";
      case "due_date_reminder":
        return "⏰";
      case "project_invitation":
        return "✉️";
      default:
        return "🔔";
    }
  };

  const timeAgo = (date: Date) => {
    const seconds = Math.floor((new Date().getTime() - new Date(date).getTime()) / 1000);

    if (seconds < 60) return "just now";
    if (seconds < 3600) return `${Math.floor(seconds / 60)}m ago`;
    if (seconds < 86400) return `${Math.floor(seconds / 3600)}h ago`;
    return `${Math.floor(seconds / 86400)}d ago`;
  };

  return (
    <DropdownMenu open={open} onOpenChange={setOpen}>
      <DropdownMenuTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          className={cn("relative", className)}
        >
          <Bell className="h-5 w-5" />
          {unreadCount > 0 && (
            <Badge
              variant="destructive"
              className="absolute -top-1 -right-1 h-5 w-5 rounded-full p-0 flex items-center justify-center text-xs"
            >
              {unreadCount > 9 ? "9+" : unreadCount}
            </Badge>
          )}
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-80">
        <DropdownMenuLabel className="flex items-center justify-between">
          <span>Notifications</span>
          {unreadCount > 0 && (
            <Button
              variant="ghost"
              size="sm"
              onClick={onMarkAllAsRead}
              className="h-auto p-1 text-xs"
            >
              Mark all read
            </Button>
          )}
        </DropdownMenuLabel>
        <DropdownMenuSeparator />
        <ScrollArea className="h-96">
          <DropdownMenuGroup>
            {notifications.length === 0 ? (
              <div className="p-4 text-center text-muted-foreground">
                No notifications
              </div>
            ) : (
              notifications.map((notification) => (
                <DropdownMenuItem
                  key={notification.id}
                  className={cn(
                    "flex flex-col items-start gap-1 p-3 cursor-pointer",
                    !notification.isRead && "bg-accent/50"
                  )}
                  onClick={() => handleNotificationClick(notification)}
                >
                  <div className="flex items-start gap-2 w-full">
                    <span className="text-lg">
                      {getNotificationIcon(notification.type)}
                    </span>
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center justify-between gap-2">
                        <p className="font-medium text-sm truncate">
                          {notification.title}
                        </p>
                        {!notification.isRead && (
                          <div className="h-2 w-2 rounded-full bg-blue-500 flex-shrink-0" />
                        )}
                      </div>
                      {notification.message && (
                        <p className="text-xs text-muted-foreground line-clamp-2">
                          {notification.message}
                        </p>
                      )}
                      <p className="text-xs text-muted-foreground mt-1">
                        {timeAgo(notification.createdAt)}
                      </p>
                    </div>
                  </div>
                </DropdownMenuItem>
              ))
            )}
          </DropdownMenuGroup>
        </ScrollArea>
        {notifications.length > 0 && (
          <>
            <DropdownMenuSeparator />
            <div className="p-2">
              <Button
                variant="ghost"
                size="sm"
                onClick={onMarkAllAsRead}
                className="w-full justify-start"
              >
                <CheckCheck className="mr-2 h-4 w-4" />
                Mark all as read
              </Button>
            </div>
          </>
        )}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
