import { SignalRConnection } from './signalr-connection';
import type { NotificationMessage } from './types';

export class NotificationHubConnection extends SignalRConnection {
  constructor(accessToken: string) {
    super({
      url: `${process.env.NEXT_PUBLIC_API_URL}/hubs/notifications`,
      accessToken,
      automaticReconnect: true,
    });
  }

  async joinUserNotifications(): Promise<void> {
    await this.invoke('JoinUserNotifications');
  }

  async markNotificationRead(notificationId: string): Promise<void> {
    await this.invoke('MarkNotificationRead', notificationId);
  }

  async markAllNotificationsRead(): Promise<void> {
    await this.invoke('MarkAllNotificationsRead');
  }

  onNotificationReceived(callback: (message: NotificationMessage) => void): void {
    this.on('NotificationReceived', (...args: unknown[]) => {
      if (args[0]) {
        callback(args[0] as NotificationMessage);
      }
    });
  }
}
