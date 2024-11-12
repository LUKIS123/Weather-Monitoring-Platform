import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, Signal } from '@angular/core';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ChartsConfigService } from '../../../../shared/services/charts-config.service';
import { GetWeatherDataLastDayResponse } from '../../models/get-weather-last-day-response';
import { DailyChartComponent } from '../../../../shared/components/daily-chart/daily-chart.component';

@Component({
  selector: 'app-last-day-hourly-data',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule, DailyChartComponent],
  templateUrl: './last-day-hourly-data.component.html',
})
export class LastDayHourlyDataComponent {
  private readonly translateService = inject(TranslateService);
  private readonly chartsConfigService = inject(ChartsConfigService);
  data = input.required<GetWeatherDataLastDayResponse>();
  dataType = input.required<'weather' | 'pollution'>();

  startDate = computed(() => {
    const end = this.endDate();
    if (this.data().currentDate) {
      return new Date(end - 23 * 60 * 60 * 1000).setMinutes(0, 0, 0);
    }
    return new Date().setMinutes(0, 0, 0) - 23 * 60 * 60 * 1000;
  });
  endDate = computed(() => {
    const currentDate = this.data().currentDate;
    if (currentDate) {
      return new Date(currentDate).setMinutes(0, 0, 0);
    }
    return new Date().setMinutes(0, 0, 0);
  });
  chartName = computed(() => 'DataVisualisation.Last24Hours');

  /*
   * Temperature data
   */
  temperatureData: Signal<[number, number | null][]> = computed(() => {
    const data = this.data();
    return data?.hourlyData
      ? data.hourlyData.map((x) => [
          x.hourlyTimeStamp
            ? new Date(x.hourlyTimeStamp).getTime()
            : new Date().getTime(),
          x.avgTemperature ?? null,
        ])
      : [];
  });
  temperatureSeriesName = computed(() =>
    this.translateService.instant(
      this.chartsConfigService.temperatureChartOptions.seriesName
    )
  );
  temperatureUnit = computed(
    () => this.chartsConfigService.temperatureChartOptions.unit
  );
  temperatureColor = computed(
    () => this.chartsConfigService.temperatureChartOptions.color
  );

  /*
   * Humidity data
   */
  humidityData: Signal<[number, number | null][]> = computed(() => {
    const data = this.data();
    return data?.hourlyData
      ? data.hourlyData.map((x) => [
          x.hourlyTimeStamp
            ? new Date(x.hourlyTimeStamp).getTime()
            : new Date().getTime(),
          x.avgHumidity ?? null,
        ])
      : [];
  });
  humiditySeriesName = computed(() =>
    this.translateService.instant(
      this.chartsConfigService.humidityChartOptions.seriesName
    )
  );
  humidityUnit = computed(
    () => this.chartsConfigService.humidityChartOptions.unit
  );
  humidityColor = computed(
    () => this.chartsConfigService.humidityChartOptions.color
  );

  /*
   * Pressure data
   */
  pressureData: Signal<[number, number | null][]> = computed(() => {
    const data = this.data();
    return data?.hourlyData
      ? data.hourlyData.map((x) => [
          x.hourlyTimeStamp
            ? new Date(x.hourlyTimeStamp).getTime()
            : new Date().getTime(),
          x.avgAirPressure ? +(x.avgAirPressure / 100).toFixed(2) : null,
        ])
      : [];
  });
  pressureSeriesName = computed(() =>
    this.translateService.instant(
      this.chartsConfigService.pressureChartOptions.seriesName
    )
  );
  pressureUnit = computed(
    () => this.chartsConfigService.pressureChartOptions.unit
  );
  pressureColor = computed(
    () => this.chartsConfigService.pressureChartOptions.color
  );

  /*
   * Pollution data
   */
  pm1_0Data: Signal<[number, number | null][]> = computed(() => {
    const data = this.data();
    return data?.hourlyData
      ? data.hourlyData.map((x) => [
          x.hourlyTimeStamp
            ? new Date(x.hourlyTimeStamp).getTime()
            : new Date().getTime(),
          x.avgPM1_0 ?? null,
        ])
      : [];
  });
  pm1_0SeriesName = computed(() =>
    this.translateService.instant(
      this.chartsConfigService.pm1_0ChartOptions.seriesName
    )
  );
  pm1_0Color = computed(() => this.chartsConfigService.pm1_0ChartOptions.color);
  pm1_0Unit = computed(() => this.chartsConfigService.pm1_0ChartOptions.unit);

  pm2_5Data: Signal<[number, number | null][]> = computed(() => {
    const data = this.data();
    return data?.hourlyData
      ? data.hourlyData.map((x) => [
          x.hourlyTimeStamp
            ? new Date(x.hourlyTimeStamp).getTime()
            : new Date().getTime(),
          x.avgPM2_5 ?? null,
        ])
      : [];
  });
  pm2_5SeriesName = computed(() =>
    this.translateService.instant(
      this.chartsConfigService.pm2_5ChartOptions.seriesName
    )
  );
  pm2_5Color = computed(() => this.chartsConfigService.pm2_5ChartOptions.color);
  pm2_5Unit = computed(() => this.chartsConfigService.pm2_5ChartOptions.unit);

  pm10Data: Signal<[number, number | null][]> = computed(() => {
    const data = this.data();
    return data?.hourlyData
      ? data.hourlyData.map((x) => [
          x.hourlyTimeStamp
            ? new Date(x.hourlyTimeStamp).getTime()
            : new Date().getTime(),
          x.avgPM10 ?? null,
        ])
      : [];
  });
  pm10SeriesName = computed(() =>
    this.translateService.instant(
      this.chartsConfigService.pm10ChartOptions.seriesName
    )
  );
  pm10Color = computed(() => this.chartsConfigService.pm10ChartOptions.color);
  pm10Unit = computed(() => this.chartsConfigService.pm10ChartOptions.unit);
}
