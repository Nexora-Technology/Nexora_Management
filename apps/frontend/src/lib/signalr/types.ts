// SignalR message types

export interface TaskUpdatedMessage {
  taskId: string;
  projectId: string;
  type: 'created' | 'updated' | 'deleted';
  updatedBy: string;
  timestamp: string;
  data?: unknown;
}

export interface UserPresenceMessage {
  userId: string;
  userName: string;
  workspaceId: string;
  connectionId?: string;
  isOnline: boolean;
  lastSeen: string;
}

export interface NotificationMessage {
  notificationId: string;
  userId: string;
  type: string;
  title: string;
  message?: string;
  actionUrl?: string;
  createdAt: string;
}

export interface TypingIndicatorMessage {
  userId: string;
  userName: string;
  taskId: string;
  isTyping: boolean;
  timestamp: string;
}

export type ConnectionState =
  | 'disconnected'
  | 'connecting'
  | 'connected'
  | 'reconnecting'
  | 'disconnecting';
