import type { Telemetry } from './telemetry';

export interface TelemetryZone extends Telemetry {
  levelZone: Zone;
  temperatureZone: Zone;
  radiationZone: Zone;
  pressureZone: Zone;
}

type Zone = 'green' | 'yellow' | 'red';
