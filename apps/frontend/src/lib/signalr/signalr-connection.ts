import * as signalR from '@microsoft/signalr';
import type { ConnectionState } from './types';

export interface SignalRConnectionOptions {
  url: string;
  accessToken?: string;
  automaticReconnect?: boolean;
  onReconnecting?: (error?: Error) => void;
  onReconnected?: (connectionId?: string) => void;
  onClose?: (error?: Error) => void;
}

export class SignalRConnection {
  private connection: signalR.HubConnection | null = null;
  private options: SignalRConnectionOptions;
  private state: ConnectionState = 'disconnected';

  constructor(options: SignalRConnectionOptions) {
    this.options = options;
  }

  async start(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    this.state = 'connecting';

    try {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(this.options.url, {
          accessTokenFactory: () => this.options.accessToken || '',
          skipNegotiation: false,
          withCredentials: true,
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext) => {
            // Exponential backoff: 0s, 2s, 10s, 30s, then 60s
            const retries = retryContext.previousRetryCount || 0;
            return Math.min(1000 * Math.pow(2, retries), 60000);
          },
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();

      this.connection.onreconnecting((error) => {
        this.state = 'reconnecting';
        this.options.onReconnecting?.(error);
      });

      this.connection.onreconnected((connectionId) => {
        this.state = 'connected';
        this.options.onReconnected?.(connectionId);
      });

      this.connection.onclose((error) => {
        this.state = 'disconnected';
        this.options.onClose?.(error);
      });

      await this.connection.start();
      this.state = 'connected';
    } catch (error) {
      this.state = 'disconnected';
      throw error;
    }
  }

  async stop(): Promise<void> {
    if (this.connection) {
      this.state = 'disconnecting';
      try {
        await this.connection.stop();
        this.state = 'disconnected';
      } catch (error) {
        this.state = 'disconnected';
        throw error;
      }
    }
  }

  on(methodName: string, callback: (...args: unknown[]) => void): void {
    this.connection?.on(methodName, callback);
  }

  off(methodName: string, callback?: (...args: unknown[]) => void): void {
    if (callback) {
      this.connection?.off(methodName, callback);
    } else {
      this.connection?.off(methodName);
    }
  }

  async invoke(methodName: string, ...args: unknown[]): Promise<unknown> {
    if (!this.connection) {
      throw new Error('Connection not established');
    }
    return this.connection.invoke(methodName, ...args);
  }

  getState(): ConnectionState {
    return this.state;
  }

  getConnectionId(): string | undefined {
    return this.connection?.connectionId ?? undefined;
  }
}
