import { Component, inject, OnInit, signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ToastService } from '../../../../shared/services/toast.service';
import { DevicesListDataService } from '../../services/devices-list-data.service';
import { PageResult } from '../../../../shared/models/page-result';
import { GetDeviceResponse } from '../../models/get-device-response';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { DeviceListItemComponent } from '../device-list-item/device-list-item.component';
import {
  PageEvent,
  MatPaginatorModule,
  MatPaginatorIntl,
} from '@angular/material/paginator';
import { MatPaginatorIntlPl } from '../../../../shared/components/paginator/MatPaginatorIntlPl';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    DeviceListItemComponent,
    MatPaginatorModule,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: MatPaginatorIntlPl }],
  templateUrl: './device-list.component.html',
})
export class DeviceListComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly devicesListDataService = inject(DevicesListDataService);

  #devicesPageResult = signal<PageResult<GetDeviceResponse>>({
    items: [],
    totalPages: 0,
    itemsFrom: 0,
    itemsTo: 0,
    totalItemsCount: 0,
    pageSize: 0,
  });
  public devicesPageResult = this.#devicesPageResult.asReadonly();
  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #currentPage = signal<number>(0);
  public readonly currentPage = this.#currentPage.asReadonly();

  ngOnInit(): void {
    this.#isLoading.set(true);
    this.loadDevices(this.#currentPage());
  }

  pageEvent!: PageEvent;
  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.#currentPage.set(e.pageIndex);
    this.loadDevices(this.#currentPage());
  }

  public refresh(): void {
    this.#isLoading.set(true);
    this.loadDevices(this.#currentPage());
  }

  private loadDevices(pageNumber: number): void {
    this.devicesListDataService
      .getDevices(pageNumber)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: (devices) => {
          this.#devicesPageResult.set(devices);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant('DeviceManagement.List.Error')
          );
        },
      });
  }
}
