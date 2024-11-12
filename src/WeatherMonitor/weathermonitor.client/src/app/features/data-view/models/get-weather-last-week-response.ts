export interface GetWeatherDataLastWeekResponse {
  startDateTime: Date;
  endDateTime: Date;
  lastWeekHourlyData: LastWeekHourlyData[];
}

export interface LastWeekHourlyData {
  hourDateTime: Date;
  avgTemperature?: number;
  avgHumidity?: number;
  avgAirPressure?: number;
  avgPM1_0?: number;
  avgPM2_5?: number;
  avgPM10?: number;
}
