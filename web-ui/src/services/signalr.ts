import { API_BASE_URL } from '@/api/config';
import * as signalR from '@microsoft/signalr';

export interface ImportReport {
  jobId: string;
  succeededCount: number;
  failedCount: number;
  rowResults: ImportRowResult[];
}

export interface ImportRowResult {
  rowNumber: number;
  name: string;
  description: string;
  category: string;
  price: string;
  quantity: string;
  supplier: string;
  lowStockAlertThreshold: number;
  isValid: boolean;
  errors: string[];
}

export interface ImportSummary {
  jobId: string;
  totalImported: number;
  totalFailed: number;
  completedAt: string;
}

type ConnectionState = 'disconnected' | 'connecting' | 'connected' | 'reconnecting';

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private connectionState: ConnectionState = 'disconnected';
  private listeners: Map<string, Set<(...args: unknown[]) => void>> = new Map();
  private stateListeners: Set<(state: ConnectionState) => void> = new Set();
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;

  async connect(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    if (this.connection?.state === signalR.HubConnectionState.Connecting) {
      return;
    }

    this.updateState('connecting');

    const hubUrl = `${API_BASE_URL.replace(/\/api\/?$/, '')}/hubs/import-status`;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => {
          return localStorage.getItem('access_token') || '';
        },
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.previousRetryCount >= this.maxReconnectAttempts) {
            return null; // Stop reconnecting
          }
          // Exponential backoff: 1s, 2s, 4s, 8s, 16s
          return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 16000);
        },
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Setup event forwarding
    this.connection.on('OnPreviewReady', (report: ImportReport) => {
      console.log('OnPreviewReady', report);
      this.emit('OnPreviewReady', report);
    });

    this.connection.on('OnImportCompleted', (summary: ImportSummary) => {
      this.emit('OnImportCompleted', summary);
    });

    this.connection.on('OnJobFailed', (payload: { jobId: string; message: string }) => {
      console.log('OnJobFailed', payload);
      this.emit('OnJobFailed', payload.jobId, payload.message);
    });

    this.connection.on('OnProgress', (progress: { jobId: string; percentage: number; message: string }) => {
      console.log('OnProgress', progress);
      this.emit('OnProgress', progress);
    });

    // Connection state handlers
    this.connection.onreconnecting(() => {
      this.updateState('reconnecting');
      this.reconnectAttempts++;
    });

    this.connection.onreconnected(() => {
      this.updateState('connected');
      this.reconnectAttempts = 0;
    });

    this.connection.onclose(() => {
      this.updateState('disconnected');
    });

    try {
      await this.connection.start();
      this.updateState('connected');
      this.reconnectAttempts = 0;
    } catch (error) {
      this.updateState('disconnected');
      console.error('SignalR connection failed:', error);
      throw error;
    }
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      this.updateState('disconnected');
    }
  }

  private updateState(state: ConnectionState): void {
    this.connectionState = state;
    this.stateListeners.forEach((listener) => listener(state));
  }

  getState(): ConnectionState {
    return this.connectionState;
  }

  onStateChange(callback: (state: ConnectionState) => void): () => void {
    this.stateListeners.add(callback);
    return () => this.stateListeners.delete(callback);
  }

  on<T extends unknown[]>(event: string, callback: (...args: T) => void): () => void {
    if (!this.listeners.has(event)) {
      this.listeners.set(event, new Set());
    }
    this.listeners.get(event)!.add(callback as (...args: unknown[]) => void);

    return () => {
      this.listeners.get(event)?.delete(callback as (...args: unknown[]) => void);
    };
  }

  private emit(event: string, ...args: unknown[]): void {
    this.listeners.get(event)?.forEach((callback) => callback(...args));
  }

  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }
}

// Singleton instance
export const signalRService = new SignalRService();
