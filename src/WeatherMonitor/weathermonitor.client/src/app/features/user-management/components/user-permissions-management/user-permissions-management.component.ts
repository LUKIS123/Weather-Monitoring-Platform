import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  Inject,
  inject,
  OnInit,
  signal,
} from '@angular/core';
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
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef,
} from '@angular/material/dialog';
import { PermissionDecisionComponent } from '../permission-decision/permission-decision.component';
import { UserPermissionManagementElementComponent } from '../user-permission-management-element/user-permission-management-element.component';
import { GrantAdminRoleService } from '../../services/grant-admin-role.service';
import { UserRole } from '../../../../shared/models/user-role';
import { GrantAdminRoleDialogComponent } from '../grant-admin-role-dialog/grant-admin-role-dialog.component';

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
  private readonly grantAdminService = inject(GrantAdminRoleService);

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

  adminPrivilegesChangeDetected = signal(false);
  buttonDisable = signal(false);
  grantAdminButtonDisabled = computed(() => {
    return this.buttonDisable() || this.user().role === 2;
  });

  #dialog = inject(MatDialog);

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

  openConfirmationDialog() {
    const user = this.user();
    const dialogRef = this.#dialog.open<
      GrantAdminRoleDialogComponent,
      unknown,
      boolean
    >(GrantAdminRoleDialogComponent, {
      data: { userData: user },
      panelClass: 'popup',
      maxWidth: '75dvw',
      maxHeight: '50dvh',
    });

    dialogRef.afterClosed().subscribe((result?: boolean) => {
      if (result !== undefined && result === true) {
        this.grantAdmin();
      }
    });
  }

  private grantAdmin() {
    this.#isLoading.set(true);
    this.buttonDisable.set(true);

    const userId = this.user().id;
    this.grantAdminService
      .grantAdmin(userId)
      .pipe(
        finalize(() => {
          this.#isLoading.set(false);
          this.buttonDisable.set(false);
        })
      )
      .subscribe({
        next: (result) => {
          this.adminPrivilegesChangeDetected.set(true);
          this.toastService.openSuccess(
            this.translateService.instant(
              'UserManagement.Users.Permissions.GrantAdmin.Success'
            )
          );
          this.user().role = UserRole.Admin;
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant(
              'UserManagement.Users.Permissions.GrantAdmin.Error'
            )
          );
        },
      });
  }
}
