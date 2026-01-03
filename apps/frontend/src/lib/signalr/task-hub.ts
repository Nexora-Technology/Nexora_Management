import { SignalRConnection } from './signalr-connection';
import type { TaskUpdatedMessage } from './types';

export class TaskHubConnection extends SignalRConnection {
  constructor(accessToken: string) {
    super({
      url: `${process.env.NEXT_PUBLIC_API_URL}/hubs/tasks`,
      accessToken,
      automaticReconnect: true,
    });
  }

  async joinProject(projectId: string): Promise<void> {
    await this.invoke('JoinProject', projectId);
  }

  async leaveProject(projectId: string): Promise<void> {
    await this.invoke('LeaveProject', projectId);
  }

  onTaskUpdated(callback: (message: TaskUpdatedMessage) => void): void {
    this.on('TaskUpdated', (...args: unknown[]) => {
      if (args[0]) {
        callback(args[0] as TaskUpdatedMessage);
      }
    });
  }

  onTaskCreated(callback: (message: TaskUpdatedMessage) => void): void {
    this.on('TaskCreated', (...args: unknown[]) => {
      if (args[0]) {
        callback(args[0] as TaskUpdatedMessage);
      }
    });
  }

  onTaskDeleted(callback: (message: TaskUpdatedMessage) => void): void {
    this.on('TaskDeleted', (...args: unknown[]) => {
      if (args[0]) {
        callback(args[0] as TaskUpdatedMessage);
      }
    });
  }

  onStatusChanged(callback: (message: TaskUpdatedMessage) => void): void {
    this.on('StatusChanged', (...args: unknown[]) => {
      if (args[0]) {
        callback(args[0] as TaskUpdatedMessage);
      }
    });
  }
}
