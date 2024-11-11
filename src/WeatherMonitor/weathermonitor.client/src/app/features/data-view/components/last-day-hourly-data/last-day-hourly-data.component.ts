import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, Signal } from '@angular/core';
import { DailyChartComponent } from '../../../../shared/components/daily-chart/daily-chart.component';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { GetWeatherDataLastDayResponse } from '../../models/get-weather-last-day-response';
import { ChartsConfigService } from '../../../../shared/services/charts-config.service';

@Component({
  selector: 'app-last-day-hourly-data',
  standalone: true,
  imports: [CommonModule, DailyChartComponent, MaterialModule, TranslateModule],
  templateUrl: './last-day-hourly-data.component.html',
})
export class LastDayHourlyDataComponent {
  private readonly translateService = inject(TranslateService);
  private readonly chartsConfigService = inject(ChartsConfigService);

  data = input.required<GetWeatherDataLastDayResponse>();
  chartInput: Signal<[number, number | null][]> = computed(() => {
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
  seriesName = computed(() =>
    this.translateService.instant(
      this.chartsConfigService.temperatureChartOptions.seriesName
    )
  );
  unit = computed(() => this.chartsConfigService.temperatureChartOptions.unit);
  chartName = computed(() => 'DataVisualisation.Last24Hours');
  color = computed(
    () => this.chartsConfigService.temperatureChartOptions.color
  );
}
