import { Injectable } from '@angular/core';
import { ChartOptions } from '../models/chart-options';

@Injectable({
  providedIn: 'root',
})
export class ChartsConfigService {
  public readonly temperatureChartOptions: ChartOptions = {
    color: '#ff5733',
    seriesName: 'Shared.Temperature',
    unit: '°C',
  } as ChartOptions;

  public readonly humidityChartOptions: ChartOptions = {
    color: '#33a2ff',
    seriesName: 'Shared.Humidity',
    unit: '%',
  } as ChartOptions;

  public readonly pressureChartOptions: ChartOptions = {
    color: '#8e44ad',
    seriesName: 'Shared.Pressure',
    unit: 'hPa',
  } as ChartOptions;

  public readonly pm1_0ChartOptions: ChartOptions = {
    color: '#a8e6a1',
    seriesName: 'Shared.PM1_0',
    unit: 'µg/m³',
  } as ChartOptions;

  public readonly pm2_5ChartOptions: ChartOptions = {
    color: '#4caf50',
    seriesName: 'Shared.PM2_5',
    unit: 'µg/m³',
  } as ChartOptions;

  public readonly pm10ChartOptions: ChartOptions = {
    color: '#1b5e20',
    seriesName: 'Shared.PM10',
    unit: 'µg/m³',
  } as ChartOptions;
}
