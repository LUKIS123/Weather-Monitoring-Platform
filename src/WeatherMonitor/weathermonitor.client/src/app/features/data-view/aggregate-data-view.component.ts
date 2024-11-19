import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { GetWeatherDataLastDayResponse } from './models/get-weather-last-day-response';
import { GetWeatherDataLastWeekResponse } from './models/get-weather-last-week-response';
import { GetWeatherDataLastMonthResponse } from './models/get-weather-last-month-response';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ToastService } from '../../shared/services/toast.service';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../shared/material.module';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { LastDayHourlyDataComponent } from './components/last-day-hourly-data/last-day-hourly-data.component';
import { LastWeekHourlyDataComponent } from './components/last-week-hourly-data/last-week-hourly-data.component';
import { LastMonthDailyDataComponent } from './components/last-month-daily-data/last-month-daily-data.component';
import { AggregateDataViewService } from './services/aggregate-data-view.service';
import {
  FormControl,
  FormGroup,
  FormGroupDirective,
  FormsModule,
  NgForm,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ErrorStateMatcher } from '@angular/material/core';

export class SearchInputErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(
    control: FormControl | null,
    form: FormGroupDirective | NgForm | null
  ): boolean {
    const isSubmitted = form && form.submitted;
    return !!(
      control &&
      control.invalid &&
      (control.dirty || control.touched || isSubmitted)
    );
  }
}

export interface PlusCodeSeachFormControl {
  plusCodeSearchPhrase: FormControl<string | null>;
}

@Component({
  selector: 'app-aggregate-data-view',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    MaterialModule,
    MatButtonToggleModule,
    LastDayHourlyDataComponent,
    LastWeekHourlyDataComponent,
    LastMonthDailyDataComponent,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
  ],
  templateUrl: './aggregate-data-view.component.html',
})
export class AggregateDataViewComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly aggregateDataService = inject(AggregateDataViewService);

  timeFrame = signal<'24h' | '7d' | '30d'>('24h');
  dataType = signal<'weather' | 'pollution'>('weather');

  matcher = new SearchInputErrorStateMatcher();
  public formGroup = new FormGroup<PlusCodeSeachFormControl>({
    plusCodeSearchPhrase: new FormControl('Wrocław', [
      Validators.required,
      Validators.minLength(1),
    ]),
  });

  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();

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

  ngOnInit(): void {
    const googleMapsPlusCode =
      this.formGroup.get('plusCodeSearchPhrase')?.value ?? 'Wrocław';
    this.loadStationsDataLast24h(googleMapsPlusCode);
  }

  public onTimeFrameChange(timeFrame: '24h' | '7d' | '30d'): void {
    this.timeFrame.set(timeFrame);
    this.#isLoading.set(true);
    const googleMapsPlusCode =
      this.formGroup.get('plusCodeSearchPhrase')?.value ?? 'Wrocław';
    switch (timeFrame) {
      case '24h':
        this.loadStationsDataLast24h(googleMapsPlusCode);
        break;
      case '7d':
        this.loadStationsDataLast7d(googleMapsPlusCode);
        break;
      case '30d':
        this.loadStationsDataLast30d(googleMapsPlusCode);
        break;
    }
  }

  public get isFormValid() {
    return this.formGroup.valid;
  }

  submit() {
    this.#isLoading.set(true);
    const googleMapsPlusCode =
      this.formGroup.get('plusCodeSearchPhrase')?.value ?? 'Wrocław';
    switch (this.timeFrame()) {
      case '24h':
        this.loadStationsDataLast24h(googleMapsPlusCode);
        break;
      case '7d':
        this.loadStationsDataLast7d(googleMapsPlusCode);
        break;
      case '30d':
        this.loadStationsDataLast30d(googleMapsPlusCode);
        break;
    }
  }

  private loadStationsDataLast24h(searchPhrase: string): void {
    this.aggregateDataService
      .getLastDayData(searchPhrase)
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

  private loadStationsDataLast7d(searchPhrase: string) {
    this.aggregateDataService
      .getLastWeekData(searchPhrase)
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

  private loadStationsDataLast30d(searchPhrase: string) {
    this.aggregateDataService
      .getLastMonthkData(searchPhrase)
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
}
