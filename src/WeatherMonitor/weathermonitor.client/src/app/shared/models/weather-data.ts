export interface WeatherSensorData {
  temperature: number;
  humidity: number;
  airPressure: number;
  altitude: number;
  pM10?: number | null;
  pM1_0?: number | null;
  pM2_5?: number | null;
}
