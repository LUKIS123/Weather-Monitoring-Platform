export interface GetWeatherDataLastDayResponse {
  currentDate: Date;
  hourlyData: LastDayHourlyData[];
}

export interface LastDayHourlyData {
  hourlyTimeStamp: Date;
  avgTemperature?: number;
  avgHumidity?: number;
  avgAirPressure?: number;
  avgPM1_0?: number;
  avgPM2_5?: number;
  avgPM10?: number;
}
