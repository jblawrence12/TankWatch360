export interface Telemetry {
  id: number;
  timestamp: string;
  level: number;
  temperature: number;
  radiation: number;
  pressure: number;
  tankId: number;
  tankName: string;
  lat: number;
  lng: number;
}
