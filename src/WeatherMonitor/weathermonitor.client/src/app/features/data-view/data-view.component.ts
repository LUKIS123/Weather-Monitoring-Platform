import {
  Component,
  computed,
  inject,
  OnDestroy,
  OnInit,
  signal,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ToastService } from '../../shared/services/toast.service';
import { CommonModule } from '@angular/common';
import { GetLastDayDataService } from './services/get-last-day-data.service';
import {
  debounceTime,
  distinctUntilChanged,
  finalize,
  Subject,
  takeUntil,
} from 'rxjs';
import { GetWeatherDataLastDayResponse } from './models/get-weather-last-day-response';
import { MaterialModule } from '../../shared/material.module';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { LastDayHourlyDataComponent } from './components/last-day-hourly-data/last-day-hourly-data.component';
import { LastWeekHourlyDataComponent } from './components/last-week-hourly-data/last-week-hourly-data.component';
import { LastMonthDailyDataComponent } from './components/last-month-daily-data/last-month-daily-data.component';
import { GetLastWeekDataService } from './services/get-last-week-data.service';
import { GetLastMonthDataService } from './services/get-last-month-data.service';
import { GetWeatherDataLastWeekResponse } from './models/get-weather-last-week-response';
import { GetWeatherDataLastMonthResponse } from './models/get-weather-last-month-response';

@Component({
  selector: 'app-data-view',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    MaterialModule,
    MatButtonToggleModule,
    LastDayHourlyDataComponent,
    LastWeekHourlyDataComponent,
    LastMonthDailyDataComponent,
  ],
  templateUrl: './data-view.component.html',
})
export class DataViewComponent implements OnInit, OnDestroy {
  private readonly translateService = inject(TranslateService);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly toastService = inject(ToastService);
  private readonly getLastDayDataService = inject(GetLastDayDataService);
  private readonly getLastWeekDataService = inject(GetLastWeekDataService);
  private readonly getLastMonthDataService = inject(GetLastMonthDataService);

  timeFrame = signal<'24h' | '7d' | '30d'>('24h');
  dataType = signal<'weather' | 'pollution'>('weather');
  private timeFrameSubject = new Subject<'24h' | '7d' | '30d'>();
  private destroy$ = new Subject<void>();

  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #deviceId = signal<number>(0);
  public deviceId = this.#deviceId.asReadonly();

  #last24hData = signal<GetWeatherDataLastDayResponse>(
    {} as GetWeatherDataLastDayResponse
  );
  public readonly last24hData = this.#last24hData.asReadonly();
  #last7dData = signal<GetWeatherDataLastWeekResponse>(
    {} as GetWeatherDataLastWeekResponse
  );
  public readonly last7dData = this.#last7dData.asReadonly();
  #last30dData = signal<GetWeatherDataLastMonthResponse>(
    {} as GetWeatherDataLastMonthResponse
  );
  public readonly last30dData = this.#last30dData.asReadonly();

  isDataLoaded = computed(() => {
    switch (this.timeFrame()) {
      case '24h':
        return (
          this.last24hData().currentDate !== null &&
          this.last24hData().currentDate !== undefined
        );
      case '7d':
        return (
          this.last7dData().startDateTime !== null &&
          this.last7dData().startDateTime !== undefined
        );
      case '30d':
        return (
          this.last30dData().startDate !== null &&
          this.last30dData().startDate !== undefined
        );
    }
  });

  constructor() {
    this.activatedRoute.params.subscribe((params) => {
      const deviceId = params['deviceId'];
      this.#deviceId.set(deviceId);
    });
  }

  ngOnInit(): void {
    this.timeFrameSubject
      .pipe(debounceTime(50), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((timeFrame) => this.handleTimeFrameChange(timeFrame));
  }

  public onTimeFrameChange(timeFrame: '24h' | '7d' | '30d'): void {
    this.timeFrameSubject.next(timeFrame);
  }

  private handleTimeFrameChange(timeFrame: '24h' | '7d' | '30d'): void {
    this.timeFrame.set(timeFrame);
    this.#isLoading.set(true);
    switch (timeFrame) {
      case '24h':
        this.loadStationsDataLast24h(this.deviceId());
        break;
      case '7d':
        this.loadStationsDataLast7d(this.deviceId());
        break;
      case '30d':
        this.loadStationsDataLast30d(this.deviceId());
        break;
    }
  }

  private loadStationsDataLast24h(deviceId: number): void {
    this.getLastDayDataService
      .getLastDayData(deviceId)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.#last24hData.set(data);
        },
        error: () =>
          this.toastService.openError(
            this.translateService.instant('DataVisualisation.Error')
          ),
      });
  }

  private loadStationsDataLast7d(deviceId: number) {
    this.getLastWeekDataService
      .getLastWeekData(deviceId)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.#last7dData.set(data);
        },
        error: () =>
          this.toastService.openError(
            this.translateService.instant('DataVisualisation.Error')
          ),
      });
  }

  private loadStationsDataLast30d(deviceId: number) {
    this.getLastMonthDataService
      .getLastMonthkData(deviceId)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.#last30dData.set(data);
        },
        error: () =>
          this.toastService.openError(
            this.translateService.instant('DataVisualisation.Error')
          ),
      });
  }

  ngOnDestroy(): void {
    this.timeFrameSubject.complete();
    this.destroy$.next();
    this.destroy$.complete();
  }
}
