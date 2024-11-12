import { Component, inject, input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ChartsConfigService } from '../../../../shared/services/charts-config.service';
import { GetWeatherDataLastWeekResponse } from '../../models/get-weather-last-week-response';

@Component({
  selector: 'app-last-week-hourly-data',
  standalone: true,
  imports: [],
  templateUrl: './last-week-hourly-data.component.html',
})
export class LastWeekHourlyDataComponent {
  private readonly translateService = inject(TranslateService);
  private readonly chartsConfigService = inject(ChartsConfigService);
  data = input.required<GetWeatherDataLastWeekResponse>();
  dataType = input.required<'weather' | 'pollution'>();
}
