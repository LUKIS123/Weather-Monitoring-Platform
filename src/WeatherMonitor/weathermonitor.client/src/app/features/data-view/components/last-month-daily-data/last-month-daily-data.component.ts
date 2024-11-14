import { Component, computed, inject, input, Signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ChartsConfigService } from '../../../../shared/services/charts-config.service';
import { GetWeatherDataLastMonthResponse } from '../../models/get-weather-last-month-response';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { DailyTwoSeriesChartComponent } from '../../../../shared/components/daily-two-series-chart/daily-two-series-chart.component';

@Component({
  selector: 'app-last-month-daily-data',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    DailyTwoSeriesChartComponent,
  ],
  templateUrl: './last-month-daily-data.component.html',
})
export class LastMonthDailyDataComponent {
  private readonly translateService = inject(TranslateService);
  private readonly chartsConfigService = inject(ChartsConfigService);
  data = input.required<GetWeatherDataLastMonthResponse>();
  dataType = input.required<'weather' | 'pollution'>();

  startDate = computed(() => {
    const start = this.data().startDate;
    if (start) {
      return new Date(start).setMinutes(0, 0, 0);
    }
    return new Date().setMinutes(0, 0, 0);
  });
  endDate = computed(() => {
    const end = this.data().endDate;
    if (end) {
      return new Date(end).setMinutes(0, 0, 0);
    }
    return new Date().setMinutes(0, 0, 0);
  });
  chartName = 'DataVisualisation.Last30Days';

  /*
   * Temperature data
   */
  temperatureData: Signal<{
    dayTime: [number, number | null][];
    nightTime: [number, number | null][];
  }> = computed(() => {
    const data = this.data();
    const dayTime = data?.dayTimeData
      ? data.dayTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgTemperature ?? null,
            ] as [number, number | null]
        )
      : [];
    const nightTime = data?.nightTimeData
      ? data.nightTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgTemperature ?? null,
            ] as [number, number | null]
        )
      : [];
    return { dayTime, nightTime };
  });
  temperatureDataDisplayName = this.translateService.instant(
    'DataVisualisation.Temperature.DayAndNight'
  );
  temperatureFirstSeriesName = this.translateService.instant(
    'DataVisualisation.Temperature.Day'
  );
  temperatureSecondSeriesName = this.translateService.instant(
    'DataVisualisation.Temperature.Night'
  );
  temperatureUnit = this.chartsConfigService.temperatureChartOptions.unit;
  temperatureColors = ['#ff5733', '#c0392b'];

  /*
   * Humidity data
   */
  humidityData: Signal<{
    dayTime: [number, number | null][];
    nightTime: [number, number | null][];
  }> = computed(() => {
    const data = this.data();
    const dayTime = data?.dayTimeData
      ? data.dayTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgHumidity ?? null,
            ] as [number, number | null]
        )
      : [];
    const nightTime = data?.nightTimeData
      ? data.nightTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgHumidity ?? null,
            ] as [number, number | null]
        )
      : [];
    return { dayTime, nightTime };
  });
  humidityDataDisplayName = this.translateService.instant(
    'DataVisualisation.Humidity.DayAndNight'
  );
  humidityFirstSeriesName = this.translateService.instant(
    'DataVisualisation.Humidity.Day'
  );
  humiditySecondSeriesName = this.translateService.instant(
    'DataVisualisation.Humidity.Night'
  );
  humidityUnit = this.chartsConfigService.humidityChartOptions.unit;
  humidityColors = ['#3498db', '#2e86c1'];

  /*
   * Pressure data
   */
  pressureData: Signal<{
    dayTime: [number, number | null][];
    nightTime: [number, number | null][];
  }> = computed(() => {
    const data = this.data();
    const dayTime = data?.dayTimeData
      ? data.dayTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgAirPressure ? +(x.avgAirPressure / 100).toFixed(2) : null,
            ] as [number, number | null]
        )
      : [];
    const nightTime = data?.nightTimeData
      ? data.nightTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgAirPressure ? +(x.avgAirPressure / 100).toFixed(2) : null,
            ] as [number, number | null]
        )
      : [];
    return { dayTime, nightTime };
  });
  pressureDataDisplayName = this.translateService.instant(
    'DataVisualisation.Pressure.DayAndNight'
  );
  pressureFirstSeriesName = this.translateService.instant(
    'DataVisualisation.Pressure.Day'
  );
  pressureSecondSeriesName = this.translateService.instant(
    'DataVisualisation.Pressure.Night'
  );
  pressureUnit = this.chartsConfigService.pressureChartOptions.unit;
  pressureColors = ['#8e44ad', '#5b2c6f'];

  /*
   * Pollution data
   */
  pm1_0Data: Signal<{
    dayTime: [number, number | null][];
    nightTime: [number, number | null][];
  }> = computed(() => {
    const data = this.data();
    const dayTime = data?.dayTimeData
      ? data.dayTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgPM1_0 ?? null,
            ] as [number, number | null]
        )
      : [];
    const nightTime = data?.nightTimeData
      ? data.nightTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgPM1_0 ?? null,
            ] as [number, number | null]
        )
      : [];
    return { dayTime, nightTime };
  });
  pm1_0DataDisplayName = this.translateService.instant(
    'DataVisualisation.PM1_0.DayAndNight'
  );
  pm1_0FirstSeriesName = this.translateService.instant(
    'DataVisualisation.PM1_0.Day'
  );
  pm1_0SecondSeriesName = this.translateService.instant(
    'DataVisualisation.PM1_0.Night'
  );
  pm1_0Unit = this.chartsConfigService.pm1_0ChartOptions.unit;
  pm1_0Colors = ['#a8e6a1', '#7cb980'];

  pm2_5Data: Signal<{
    dayTime: [number, number | null][];
    nightTime: [number, number | null][];
  }> = computed(() => {
    const data = this.data();
    const dayTime = data?.dayTimeData
      ? data.dayTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgPM2_5 ?? null,
            ] as [number, number | null]
        )
      : [];
    const nightTime = data?.nightTimeData
      ? data.nightTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgPM2_5 ?? null,
            ] as [number, number | null]
        )
      : [];
    return { dayTime, nightTime };
  });
  pm2_5DataDisplayName = this.translateService.instant(
    'DataVisualisation.PM2_5.DayAndNight'
  );
  pm2_5FirstSeriesName = this.translateService.instant(
    'DataVisualisation.PM2_5.Day'
  );
  pm2_5SecondSeriesName = this.translateService.instant(
    'DataVisualisation.PM2_5.Night'
  );
  pm2_5Unit = this.chartsConfigService.pm2_5ChartOptions.unit;
  pm2_5Colors = ['#4caf50', '#388e3c'];

  pm10Data: Signal<{
    dayTime: [number, number | null][];
    nightTime: [number, number | null][];
  }> = computed(() => {
    const data = this.data();
    const dayTime = data?.dayTimeData
      ? data.dayTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgPM10 ?? null,
            ] as [number, number | null]
        )
      : [];
    const nightTime = data?.nightTimeData
      ? data.nightTimeData.map(
          (x) =>
            [
              x.date ? new Date(x.date).getTime() : new Date().getTime(),
              x.avgPM10 ?? null,
            ] as [number, number | null]
        )
      : [];
    return { dayTime, nightTime };
  });
  pm10DataDisplayName = this.translateService.instant(
    'DataVisualisation.PM10.DayAndNight'
  );
  pm10FirstSeriesName = this.translateService.instant(
    'DataVisualisation.PM10.Day'
  );
  pm10SecondSeriesName = this.translateService.instant(
    'DataVisualisation.PM10.Night'
  );
  pm10Unit = this.chartsConfigService.pm10ChartOptions.unit;
  pm10Colors = ['#1b5e20', '#0d3d14'];
}
