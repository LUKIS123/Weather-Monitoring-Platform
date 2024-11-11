import { CommonModule } from '@angular/common';
import { Component, computed, input, Signal } from '@angular/core';
import { DailyChartComponent } from '../../../../shared/components/daily-chart/daily-chart.component';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule } from '@ngx-translate/core';
import { GetWeatherDataLastDayResponse } from '../../models/get-weather-last-day-response';

@Component({
  selector: 'app-last-day-hourly-data',
  standalone: true,
  imports: [CommonModule, DailyChartComponent, MaterialModule, TranslateModule],
  templateUrl: './last-day-hourly-data.component.html',
})
export class LastDayHourlyDataComponent {
  data = input.required<GetWeatherDataLastDayResponse>();

  chartInput: Signal<number[][]> = computed(() => {
    const data = this.data();
    return data?.hourlyData
      ? data.hourlyData.map((x) => [
          x.hourlyTimeStamp ? new Date(x.hourlyTimeStamp).getTime() : 0,
          x.avgTemperature ?? 0,
        ])
      : [];
  });

  startDate = computed(() => {
    const currentDate = this.data().currentDate;
    return currentDate instanceof Date
      ? new Date(currentDate.getTime() - 23 * 60 * 60 * 1000)
      : new Date();
  });

  endDate = computed(() => {
    const currentDate = this.data().currentDate;
    return currentDate instanceof Date ? new Date(currentDate) : new Date();
  });
}
