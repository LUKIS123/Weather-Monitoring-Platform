import { Component, inject, input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ChartsConfigService } from '../../../../shared/services/charts-config.service';
import { GetWeatherDataLastMonthResponse } from '../../models/get-weather-last-month-response';

@Component({
  selector: 'app-last-month-daily-data',
  standalone: true,
  imports: [],
  templateUrl: './last-month-daily-data.component.html',
})
export class LastMonthDailyDataComponent {
  private readonly translateService = inject(TranslateService);
  private readonly chartsConfigService = inject(ChartsConfigService);
  data = input.required<GetWeatherDataLastMonthResponse>();
  dataType = input.required<'weather' | 'pollution'>();
}
