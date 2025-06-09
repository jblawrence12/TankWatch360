import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import type { Telemetry } from '../models/telemetry';

@Injectable({
  providedIn: 'root'
})
export class TelemetrySignalRService {
  private hubConnection: signalR.HubConnection | null = null;
  public telemetry$ = new BehaviorSubject<Telemetry | null>(null);

  constructor() {
    this.startConnection();
  }

  private startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/telemetryhub') // proxy handles localhost:5048
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('✅ SignalR Connected to /telemetryhub'))
      .catch(err => console.error('❌ SignalR Connection Error:', err));

    this.hubConnection.on('ReceiveTelemetry', (data: Telemetry) => {
      console.log('📡 SignalR Received Telemetry:', data);
      this.telemetry$.next(data);
    });

    this.hubConnection.onclose(err => {
      console.warn('🔌 SignalR Connection Closed:', err);
    });
  }
}
