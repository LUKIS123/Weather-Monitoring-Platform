import { Component, inject, OnInit, signal } from '@angular/core';
import { PageResult } from '../../../../shared/models/page-result';
import { UsersPermissionRequest } from '../../models/users-permission';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ToastService } from '../../../../shared/services/toast.service';
import { GetUsersPermissionsService } from '../../services/get-users-permissions.service';
import {
  MatPaginatorIntl,
  MatPaginatorModule,
  PageEvent,
} from '@angular/material/paginator';
import { finalize } from 'rxjs';
import { MaterialModule } from '../../../../shared/material.module';
import { CommonModule } from '@angular/common';
import { MatPaginatorIntlPl } from '../../../../shared/components/paginator/MatPaginatorIntlPl';
import { PermissionsListItemComponent } from '../permissions-list-item/permissions-list-item.component';

@Component({
  selector: 'app-permissions-list',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    MatPaginatorModule,
    PermissionsListItemComponent,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: MatPaginatorIntlPl }],
  templateUrl: './permissions-list.component.html',
})
export class PermissionsListComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly getUsersPermissionsService = inject(
    GetUsersPermissionsService
  );

  #perrmissionsPageResult = signal<PageResult<UsersPermissionRequest>>({
    items: [],
    totalPages: 0,
    itemsFrom: 0,
    itemsTo: 0,
    totalItemsCount: 0,
    pageSize: 0,
  });
  public perrmissionsPageResult = this.#perrmissionsPageResult.asReadonly();

  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #currentPage = signal<number>(0);
  public readonly currentPage = this.#currentPage.asReadonly();

  ngOnInit(): void {
    this.#isLoading.set(true);
    this.loadPermissions(this.#currentPage());
  }

  pageEvent!: PageEvent;
  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.#currentPage.set(e.pageIndex);
    this.loadPermissions(this.#currentPage());
  }

  private loadPermissions(pageNumber: number): void {
    this.getUsersPermissionsService
      .getStationPermissions(pageNumber + 1)
      .pipe(
        finalize(() => {
          this.#isLoading.set(false);
        })
      )
      .subscribe({
        next: (result) => {
          this.#perrmissionsPageResult.set(result);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant('AvailableStations.Permissions.Error')
          );
        },
      });
  }
}
