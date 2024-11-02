import { Component, inject, OnInit, signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ToastService } from '../../../../shared/services/toast.service';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { GetStationsListService } from '../../services/get-stations-list.service';
import { PageResult } from '../../../../shared/models/page-result';
import { GetStationResponse } from '../../models/get-stations-response';
import {
  MatPaginatorIntl,
  MatPaginatorModule,
  PageEvent,
} from '@angular/material/paginator';
import { finalize } from 'rxjs';
import { MatPaginatorIntlPl } from '../../../../shared/components/paginator/MatPaginatorIntlPl';
import { StationsListElementComponent } from '../stations-list-element/stations-list-element.component';

@Component({
  selector: 'app-stations-list',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    MatPaginatorModule,
    StationsListElementComponent,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: MatPaginatorIntlPl }],
  templateUrl: './stations-list.component.html',
})
export class StationsListComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly getStationsListService = inject(GetStationsListService);

  #stationsPageResult = signal<PageResult<GetStationResponse>>({
    items: [],
    totalPages: 0,
    itemsFrom: 0,
    itemsTo: 0,
    totalItemsCount: 0,
    pageSize: 0,
  });
  public stationsPageResult = this.#stationsPageResult.asReadonly();
  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #currentPage = signal<number>(0);
  public readonly currentPage = this.#currentPage.asReadonly();

  ngOnInit(): void {
    this.#isLoading.set(true);
    this.loadWeatherStations(this.#currentPage());
  }

  pageEvent!: PageEvent;
  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.#currentPage.set(e.pageIndex);
    this.loadWeatherStations(this.#currentPage());
  }

  private loadWeatherStations(pageNumber: number): void {
    this.getStationsListService
      .getStationsList(pageNumber + 1)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: (stations) => {
          this.#stationsPageResult.set(stations);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant('StationsList.Lsit.Error')
          );
        },
      });
  }
}
