import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { Telemetry } from '../models/telemetry';

@Injectable({
  providedIn: 'root'
})
export class TelemetrySignalRService {
  private connection: HubConnection | null = null;
  private telemetrySubject = new Subject<Telemetry>();
  telemetry$ = this.telemetrySubject.asObservable();

  constructor() {
    console.log('🚗 Initializing SignalR Service');
    this.connection = new HubConnectionBuilder()
      .withUrl('/telemetryhub')
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveTelemetry', (data: Telemetry) => {
      console.log('🦠 Raw SignalR Data:', JSON.stringify(data, null, 2));
      this.telemetrySubject.next(data);
    });

    this.connection.onclose(error => {
      console.error('🛑 SignalR Connection Closed:', error);
    });

    this.connection.onreconnecting(error => {
      console.warn('🔄 SignalR Reconnecting:', error);
    });

    this.connection.onreconnected(connectionId => {
      console.log('✅ SignalR Reconnected:', connectionId);
    });

    this.startConnection();
  }

  private async startConnection() {
    if (!this.connection) return;
    try {
      await this.connection.start();
      console.log('🚖🚖 SignalR Connected to /telemetryhub');
    } catch (error) {
      console.error('❌ SignalR Connection Failed:', error);
      setTimeout(() => this.startConnection(), 5000);
    }
  }
}