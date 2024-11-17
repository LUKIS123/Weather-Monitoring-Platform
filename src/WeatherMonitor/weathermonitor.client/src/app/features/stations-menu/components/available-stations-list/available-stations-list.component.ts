import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  MatPaginatorIntl,
  MatPaginatorModule,
  PageEvent,
} from '@angular/material/paginator';
import { MatPaginatorIntlPl } from '../../../../shared/components/paginator/MatPaginatorIntlPl';
import { ListAvailableStationsService } from '../../services/list-available-stations.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { PageResult } from '../../../../shared/models/page-result';
import { AvailableStation } from '../../models/available-station';
import { finalize, map, switchMap, zip } from 'rxjs';
import { AvailableStationsListElementComponent } from '../available-stations-list-element/available-stations-list-element.component';
import { PlusCodeConverterService } from '../../../../shared/services/plus-code-converter.service';
import { StationAddress } from '../../../../shared/models/station-address';
import { StationWithAddress } from '../../models/station-with-address';

@Component({
  selector: 'app-available-stations-list',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    MatPaginatorModule,
    AvailableStationsListElementComponent,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: MatPaginatorIntlPl }],
  templateUrl: './available-stations-list.component.html',
})
export class AvailableStationsListComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly listAvailableStationsService = inject(
    ListAvailableStationsService
  );
  private readonly plusCodeConverterService = inject(PlusCodeConverterService);

  #stationsPageResult = signal<PageResult<StationWithAddress>>({
    items: [],
    totalPages: 0,
    itemsFrom: 0,
    itemsTo: 0,
    totalItemsCount: 0,
    pageSize: 0,
  });
  public stationsPageResult = this.#stationsPageResult.asReadonly();
  #stationsWithAddresses = signal<StationAddress[]>([]);
  public stationsWithAddresses = this.#stationsWithAddresses.asReadonly();

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

  // private loadWeatherStations(pageNumber: number): void {
  //   this.listAvailableStationsService
  //     .getStationsList(pageNumber + 1)
  //     .pipe(finalize(() => this.#isLoading.set(false)))
  //     .subscribe({
  //       next: (stations) => {
  //         this.#stationsPageResult.set(stations);
  //         this.plusCodeConverterService.getAddressByPlusCode(
  //           stations.items[0].googleMapsPlusCode
  //         );
  //       },
  //       error: () => {
  //         this.toastService.openError(
  //           this.translateService.instant('AvailableStations.Error')
  //         );
  //       },
  //     });
  // }

  private loadWeatherStations(pageNumber: number): void {
    this.listAvailableStationsService
      .getStationsList(pageNumber + 1)
      .pipe(
        switchMap((pageResult) => {
          const stationObservables = pageResult.items.map((station) =>
            this.plusCodeConverterService
              .getAddressByPlusCode(station.googleMapsPlusCode)
              .pipe(
                map(
                  (address) =>
                    ({
                      station,
                      address,
                    } as StationWithAddress)
                )
              )
          );

          return zip(stationObservables).pipe(
            map(
              (stationsWithAddresses) =>
                ({
                  ...pageResult,
                  items: stationsWithAddresses,
                } as PageResult<StationWithAddress>)
            )
          );
        }),
        finalize(() => {
          this.#isLoading.set(false);
        })
      )
      .subscribe({
        next: (pageResultWithAddresses) => {
          this.#stationsPageResult.set(pageResultWithAddresses);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant('AvailableStations.Error')
          );
        },
      });
  }
}
