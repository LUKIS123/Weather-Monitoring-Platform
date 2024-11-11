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

  public readonly pollutionChartOptions: ChartOptions = {
    color: '#7dcea0',
    seriesName: 'Shared.Pollution',
    unit: 'µg/m³',
  } as ChartOptions;
}
