import { Component, inject, OnInit, signal } from '@angular/core';
import { ToastService } from '../../../../shared/services/toast.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import {
  MatPaginatorModule,
  MatPaginatorIntl,
  PageEvent,
} from '@angular/material/paginator';
import { MatPaginatorIntlPl } from '../../../../shared/components/paginator/MatPaginatorIntlPl';
import { PageResult } from '../../../../shared/models/page-result';
import { GetPermissionRequestsService } from '../../services/get-permission-requests.service';
import { finalize, map, switchMap, zip } from 'rxjs';
import { PlusCodeConverterService } from '../../../../shared/services/plus-code-converter.service';
import { PendingPermissionWithAddress } from '../../models/pending-permission-with-address';
import { PermissionRequestListElementComponent } from '../permission-request-list-element/permission-request-list-element.component';

@Component({
  selector: 'app-permission-request-list',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    MatPaginatorModule,
    PermissionRequestListElementComponent,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: MatPaginatorIntlPl }],
  templateUrl: './permission-request-list.component.html',
})
export class PermissionRequestListComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly getPendingPermissionsService = inject(
    GetPermissionRequestsService
  );
  private readonly plusCodeConverterService = inject(PlusCodeConverterService);

  #requestsPageResult = signal<PageResult<PendingPermissionWithAddress>>({
    items: [],
    totalPages: 0,
    itemsFrom: 0,
    itemsTo: 0,
    totalItemsCount: 0,
    pageSize: 0,
  });
  public requestsPageResult = this.#requestsPageResult.asReadonly();
  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #currentPage = signal<number>(0);
  public readonly currentPage = this.#currentPage.asReadonly();

  ngOnInit(): void {
    this.#isLoading.set(true);
    this.loadPendingPermissions(this.#currentPage());
  }

  pageEvent!: PageEvent;
  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.#currentPage.set(e.pageIndex);
    this.loadPendingPermissions(this.#currentPage());
  }

  public refresh(): void {
    this.#isLoading.set(true);
    this.loadPendingPermissions(this.#currentPage());
  }

  private loadPendingPermissions(pageNumber: number): void {
    this.getPendingPermissionsService
      .getPendingRequests(pageNumber + 1)
      .pipe(
        switchMap((pageResult) => {
          const stationObservables = pageResult.items.map((permission) =>
            this.plusCodeConverterService
              .getAddressByPlusCode(permission.googleMapsPlusCode)
              .pipe(
                map(
                  (address) =>
                    ({
                      permission: permission,
                      address: address,
                    } as PendingPermissionWithAddress)
                )
              )
          );
          return zip(stationObservables).pipe(
            map(
              (permissionsWithAddresses) =>
                ({
                  ...pageResult,
                  items: permissionsWithAddresses,
                } as PageResult<PendingPermissionWithAddress>)
            )
          );
        }),
        finalize(() => {
          this.#isLoading.set(false);
        })
      )
      .subscribe({
        next: (pageResultWithAddresses) => {
          this.#requestsPageResult.set(pageResultWithAddresses);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant(
              'UserManagement.PendingPermissions.Error'
            )
          );
        },
      });
  }
}
