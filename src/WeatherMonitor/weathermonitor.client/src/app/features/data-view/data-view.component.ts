import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ToastService } from '../../shared/services/toast.service';
import { CommonModule } from '@angular/common';
import { GetLastDayDataService } from './services/get-last-day-data.service';
import { finalize } from 'rxjs';
import { GetWeatherDataLastDayResponse } from './models/get-weather-last-day-response';
import { MaterialModule } from '../../shared/material.module';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { LastDayHourlyWeatherDataComponent } from './components/last-day-hourly-weather-data/last-day-hourly-weather-data.component';

@Component({
  selector: 'app-data-view',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    MaterialModule,
    MatButtonToggleModule,
    LastDayHourlyWeatherDataComponent,
  ],
  templateUrl: './data-view.component.html',
})
export class DataViewComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly toastService = inject(ToastService);
  private readonly getLastDayDataService = inject(GetLastDayDataService);

  timeFrame = signal<'24h' | '7d' | '30d'>('24h');
  dataType = signal<'weather' | 'pollution'>('weather');

  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();

  #deviceId = signal<number>(0);
  public deviceId = this.#deviceId.asReadonly();

  #data = signal<GetWeatherDataLastDayResponse>(
    {} as GetWeatherDataLastDayResponse
  );
  public readonly data = this.#data.asReadonly();

  constructor() {
    this.activatedRoute.params.subscribe((params) => {
      const deviceId = params['deviceId'];
      this.#deviceId.set(deviceId);
    });
  }

  ngOnInit(): void {
    this.loadStationsDataLast24h(this.deviceId());
  }

  private loadStationsDataLast24h(deviceId: number): void {
    this.getLastDayDataService
      .getLastDayData(deviceId)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.#data.set(data);
        },
        error: () =>
          this.toastService.openError(
            this.translateService.instant('DataVisualisation.Error')
          ),
      });
  }
}
