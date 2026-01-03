import { SignalRConnection } from './signalr-connection';
import type { UserPresenceMessage, TypingIndicatorMessage } from './types';

export class PresenceHubConnection extends SignalRConnection {
  constructor(accessToken: string) {
    super({
      url: `${process.env.NEXT_PUBLIC_API_URL}/hubs/presence`,
      accessToken,
      automaticReconnect: true,
    });
  }

  async joinWorkspace(workspaceId: string): Promise<void> {
    await this.invoke('JoinWorkspace', workspaceId);
  }

  async leaveWorkspace(workspaceId: string): Promise<void> {
    await this.invoke('LeaveWorkspace', workspaceId);
  }

  async updateLastSeen(): Promise<void> {
    await this.invoke('UpdateLastSeen');
  }

  async startTyping(taskId: string): Promise<void> {
    await this.invoke('StartTyping', taskId);
  }

  async stopTyping(taskId: string): Promise<void> {
    await this.invoke('StopTyping', taskId);
  }

  onUserJoined(callback: (message: UserPresenceMessage) => void): void {
    this.on('UserJoined', (...args: unknown[]) => {
      if (args[0]) {
        callback(args[0] as UserPresenceMessage);
      }
    });
  }

  onUserLeft(callback: (message: UserPresenceMessage) => void): void {
    this.on('UserLeft', (...args: unknown[]) => {
      if (args[0]) {
        callback(args[0] as UserPresenceMessage);
      }
    });
  }

  onUserTyping(callback: (message: TypingIndicatorMessage) => void): void {
    this.on('UserTyping', (...args: unknown[]) => {
      if (args[0]) {
        callback(args[0] as TypingIndicatorMessage);
      }
    });
  }

  async joinViewing(taskId: string): Promise<void> {
    await this.invoke('JoinViewing', taskId);
  }

  async leaveViewing(taskId: string): Promise<void> {
    await this.invoke('LeaveViewing', taskId);
  }

  async sendTypingIndicator(taskId: string, isTyping: boolean): Promise<void> {
    await this.invoke('SendTypingIndicator', taskId, isTyping);
  }
}
