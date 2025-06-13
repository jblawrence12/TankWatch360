import { Component, Input } from '@angular/core';
import { LeafletModule }    from '@asymmetrik/ngx-leaflet';
import * as L from 'leaflet';
import type { Telemetry }   from '../../../models/telemetry';

/** Decide the colour for a tank based on its radiation reading */
function zoneColour(radiation: number): string {
  if (radiation <= 2)       return 'green';   // normal
  if (radiation <= 3.5)     return 'orange';  // warning
  return 'red';                               // critical
}


@Component({
  selector: 'app-tank-map',
  standalone: true,

  /* 👇  ADD LeafletModule here */
  imports: [LeafletModule],

  template: `
    <div leaflet
         [leafletOptions]="options"
         (leafletMapReady)="onReady($event)"
         class="leaflet-root">
    </div>
  `,
  styles: [
    ':host {display:block;height:100%;width:100%;}',
    '.leaflet-root {height:100%;width:100%;}'
  ]
})
export class TankMap {

  /* fixed, non-interactive Hanford view */
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
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
                  { maxZoom: 19 })
    ]
  };

  private map!: L.Map;
/** keep one marker per tank so we can update colour/position */
  private markerById: Record<number, L.CircleMarker> = {};

  onReady(m: L.Map): void {
    this.map = m;
    /* let Angular finish layout, then resize Leaflet */
    setTimeout(() => this.map.invalidateSize(), 0);
  }

  /* live updates later */
@Input() set telemetry(data: Telemetry | null) {
  if (!data || !this.map) return;

  const colour = zoneColour(data.radiation);
  const latlng = L.latLng(data.lat, data.lng);

  const existing = this.markerById[data.tankId];

  if (existing) {
    // update position, colour, and tooltip
    existing
      .setLatLng(latlng)
      .setStyle({ fillColor: colour })
      .setTooltipContent(
        `${data.tankName}<br>` +
        `Rad ${data.radiation.toFixed(2)} mSv<br>` +
        `Lvl ${data.level.toFixed(1)} %`
      );
  } else {
    // create first marker for this tank
    const marker = L.circleMarker(latlng, {
      radius: 8,
      weight: 1,
      color: '#000',
      fillColor: colour,
      fillOpacity: 0.85
    })
    .bindTooltip(
      `${data.tankName}<br>` +
      `Rad ${data.radiation.toFixed(2)} mSv<br>` +
      `Lvl ${data.level.toFixed(1)} %`
    )
    .addTo(this.map);

    this.markerById[data.tankId] = marker;
  }
}


  
}
