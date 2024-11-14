export interface GetWeatherDataLastMonthResponse {
  startDate: Date;
  endDate: Date;
  dayTimeData: LastMonthDailyData[];
  nightTimeData: LastMonthDailyData[];
}

export interface LastMonthDailyData {
  date: Date;
  avgTemperature?: number;
  avgHumidity?: number;
  avgAirPressure?: number;
  avgPM1_0?: number;
  avgPM2_5?: number;
  avgPM10?: number;
}
