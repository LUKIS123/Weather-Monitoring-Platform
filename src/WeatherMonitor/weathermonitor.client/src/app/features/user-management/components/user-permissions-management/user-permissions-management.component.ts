import { CommonModule } from '@angular/common';
import { Component, Inject, inject, OnInit, signal } from '@angular/core';
import {
  MatPaginatorModule,
  MatPaginatorIntl,
  PageEvent,
} from '@angular/material/paginator';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatPaginatorIntlPl } from '../../../../shared/components/paginator/MatPaginatorIntlPl';
import { MaterialModule } from '../../../../shared/material.module';
import { PageResult } from '../../../../shared/models/page-result';
import { User } from '../../models/user';
import { GetUserPermissionsService } from '../../services/get-user-permissions.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { finalize } from 'rxjs';
import { UserPermissionRequest } from '../../models/user-permission-request';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { PermissionDecisionComponent } from '../permission-decision/permission-decision.component';
import { UserPermissionManagementElementComponent } from '../user-permission-management-element/user-permission-management-element.component';

@Component({
  selector: 'app-user-permissions-management',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    MatPaginatorModule,
    UserPermissionManagementElementComponent,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: MatPaginatorIntlPl }],
  templateUrl: './user-permissions-management.component.html',
})
export class UserPermissionsManagementComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly getUserPermissionsService = inject(
    GetUserPermissionsService
  );

  #user = signal<User>({} as User);
  public user = this.#user.asReadonly();

  #permissionsPageResult = signal<PageResult<UserPermissionRequest>>({
    items: [],
    totalPages: 0,
    itemsFrom: 0,
    itemsTo: 0,
    totalItemsCount: 0,
    pageSize: 0,
  });
  public permissionsPageResult = this.#permissionsPageResult.asReadonly();

  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #currentPage = signal<number>(0);
  public readonly currentPage = this.#currentPage.asReadonly();

  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: { userData: User },
    private dialogRef: MatDialogRef<PermissionDecisionComponent>
  ) {
    this.#user.set(data.userData);
  }

  ngOnInit(): void {
    this.#isLoading.set(true);
    this.loadPermissions(this.#currentPage(), this.user().id);
  }

  pageEvent!: PageEvent;
  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.#currentPage.set(e.pageIndex);
    this.loadPermissions(this.#currentPage(), this.user().id);
  }

  public refresh(): void {
    this.#isLoading.set(true);
    this.loadPermissions(this.#currentPage(), this.user().id);
  }

  private loadPermissions(pageNumber: number, userId: string): void {
    this.getUserPermissionsService
      .getPermissions(pageNumber + 1, userId)
      .pipe(
        finalize(() => {
          this.#isLoading.set(false);
        })
      )
      .subscribe({
        next: (result) => {
          this.#permissionsPageResult.set(result);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant(
              'UserManagement.Users.Permissions.Error'
            )
          );
        },
      });
  }

  getUserRole(): string {
    const role = this.user()?.role;
    switch (role) {
      case 1:
        return this.translateService.instant('UserAccount.User');
      case 2:
        return this.translateService.instant('UserAccount.Admin');
      default:
        return this.translateService.instant('UserAccount.Unknown');
    }
  }

  onChangeDetected(changeDetected: boolean, index: number) {
    if (changeDetected) {
      this.refresh();
    }
  }
}
