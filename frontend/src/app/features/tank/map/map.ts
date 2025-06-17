import { Component, Input, OnDestroy } from '@angular/core';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import * as L from 'leaflet';
import type { Telemetry } from '../../../models/telemetry';

function zoneColour(radiation: number): string {
  return radiation <= 2   ? 'green'
       : radiation <= 3.5 ? 'orange'
       :                   'red';
}

@Component({
  selector: 'app-tank-map',
  standalone: true,
  imports: [LeafletModule],
  template: `
    <div leaflet
         [leafletOptions]="options"
         (leafletMapReady)="onMapReady($event)"
         class="leaflet-root">
    </div>
  `,
  styles: [`
    :host { display: block; height: 100%; width: 100%; }
    .leaflet-root { height: 100%; width: 100%; }
  `]
})
export class TankMap implements OnDestroy {
  options: L.MapOptions = {
    center: [46.550, -119.550],
    zoom: 12,
    zoomControl: false,
    dragging: false,
    scrollWheelZoom: false,
    doubleClickZoom: false,
    boxZoom: false,
    keyboard: false,
    touchZoom: false,
    attributionControl: false,
    layers: [
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 })
    ]
  };

  private map?: L.Map;
  private markerById: Record<number, L.CircleMarker> = {};
  private queue: Telemetry[] = [];

  @Input() set telemetry(t: Telemetry | null) {
    console.log('📥 TankMap received telemetry:', t);
    if (!t) return;

    if (!this.map) {
      this.queue.push(t);
      return;
    }

    this.renderReading(t);
  }

  onMapReady(map: L.Map): void {
    this.map = map;
    setTimeout(() => map.invalidateSize(), 0);

    this.queue.forEach(r => this.renderReading(r));
    this.queue.length = 0;

    // Hardcoded test circle
    L.circleMarker([46.55, -119.55], {
      radius: 10,
      color: 'blue'
    }).addTo(this.map).bindTooltip('Hardcoded Marker');
  }

  private renderReading(t: Telemetry): void {
    const lat = (t as any).lat ?? (t as any).latitude ?? 0;
    const lng = (t as any).lng ?? (t as any).longitude ?? 0;
    const tankId = (t as any).tankId ?? (t as any).id ?? -1;
    const tankName = (t as any).tankName ?? `Tank ${tankId}`;

    if (!this.map || isNaN(lat) || isNaN(lng)) {
      console.warn('❌ Invalid telemetry position:', t);
      return;
    }

    const pos = L.latLng(lat, lng);
    const colour = zoneColour(t.radiation);

    console.log(`🌀 Drawing marker at ${pos} with color ${colour}`);

    let marker = this.markerById[tankId];
    if (marker) {
      marker.setLatLng(pos)
            .setStyle({ fillColor: colour })
            .setTooltipContent(this.tooltip(tankName, t));
    } else {
      marker = L.circleMarker(pos, {
        radius: 8,
        weight: 1,
        color: '#000',
        fillColor: colour,
        fillOpacity: 0.85
      }).bindTooltip(this.tooltip(tankName, t))
        .addTo(this.map);

      this.markerById[tankId] = marker;
    }
  }

  private tooltip(name: string, t: Telemetry): string {
    return `${name}<br>
            Rad ${t.radiation.toFixed(2)} mSv<br>
            Lvl ${t.level.toFixed(1)} %`;
  }

  ngOnDestroy(): void {
    this.map?.remove();
    this.map = undefined;
  }
}
