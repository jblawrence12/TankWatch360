// map-page.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule }        from '@angular/common';
import { RouterModule }        from '@angular/router';
import { MatToolbarModule }    from '@angular/material/toolbar';
import { MatIconModule }       from '@angular/material/icon';   // 👈 NEW
import { TankMap }             from '../map/map';
import { TelemetrySignalRService } from '../../../services/telemetry-signalr.service';
import type { Telemetry }      from '../../../models/telemetry';

@Component({
  selector: 'app-map-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,        // for routerLink
    MatToolbarModule,
    MatIconModule,       // ← now mat-icon is recognised
    TankMap
  ],
  templateUrl: './map-page.html',
  styleUrls: ['./map-page.scss']
})
export class MapPage implements OnInit {
  lastTelemetry: Telemetry | null = null;
  constructor(private s: TelemetrySignalRService) {}

  ngOnInit(): void {
    this.s.telemetry$.subscribe(t => (this.lastTelemetry = t));
  }
}
