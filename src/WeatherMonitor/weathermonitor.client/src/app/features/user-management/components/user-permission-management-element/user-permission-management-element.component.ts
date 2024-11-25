import {
  Component,
  computed,
  EventEmitter,
  inject,
  input,
  Output,
  signal,
} from '@angular/core';
import { User } from '../../models/user';
import { UserPermissionRequest } from '../../models/user-permission-request';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MaterialModule } from '../../../../shared/material.module';
import { ToastService } from '../../../../shared/services/toast.service';
import { FormatDateService } from '../../../../shared/services/format-date.service';
import { UpdatePermissionService } from '../../services/update-permission.service';
import { PermissionStatusEnum } from '../../../../shared/models/permission-status';
import { UpdatePermissionRequest } from '../../models/update-permission-request ';

@Component({
  selector: 'app-user-permission-management-element',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './user-permission-management-element.component.html',
})
export class UserPermissionManagementElementComponent {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly formatDateService = inject(FormatDateService);
  private readonly updatePermissionService = inject(UpdatePermissionService);

  @Output() detectChange = new EventEmitter<boolean>();
  user = input.required<User>();
  permission = input.required<UserPermissionRequest>();

  changeSuccess = signal(false);
  enableGrant = computed(
    () =>
      !this.buttonDisable() &&
      (this.permission().permissionStatus === 2 ||
        this.permission().permissionStatus === 4)
  );
  enableRevoke = computed(
    () =>
      !this.buttonDisable() &&
      (this.permission().permissionStatus === 2 ||
        this.permission().permissionStatus === 3)
  );
  buttonDisable = signal(false);

  public getPermissionStatus(): string {
    const status = this.permission().permissionStatus;
    switch (status) {
      case 1:
        return this.translateService.instant('PermissionStatus.NotRequested');
      case 2:
        return this.translateService.instant('PermissionStatus.Pending');
      case 3:
        return this.translateService.instant('PermissionStatus.Granted');
      case 4:
        return this.translateService.instant('PermissionStatus.Denied');
      default:
        return this.translateService.instant('PermissionStatus.Unknown');
    }
  }

  public getDateFormatted(): string {
    return this.formatDateService.formatDate(this.permission().dateTime ?? '');
  }

  updatePermissionSetGranted() {
    this.sendPermissionUpdate(PermissionStatusEnum.Granted);
  }

  updatePermissionSetDenied() {
    this.sendPermissionUpdate(PermissionStatusEnum.Denied);
  }

  private sendPermissionUpdate(status: number) {
    if (!(status in PermissionStatusEnum)) {
      return;
    }
    const permissionStatus: PermissionStatusEnum =
      status as PermissionStatusEnum;

    const updatePermissionRequest: UpdatePermissionRequest = {
      userId: this.permission().userId,
      deviceId: this.permission().deviceId,
      status: permissionStatus,
    };

    this.buttonDisable.set(true);
    this.updatePermissionService
      .updatePermission(updatePermissionRequest)
      .subscribe({
        next: () => {
          this.toastService.openSuccess(
            this.translateService.instant(
              'UserManagement.Users.Permissions.Update'
            )
          );
          this.buttonDisable.set(false);

          this.changeSuccess.set(true);
          this.emitChange();
        },
        error: () => {
          this.buttonDisable.set(false);

          this.changeSuccess.set(false);
          this.toastService.openError(
            this.translateService.instant(
              'UserManagement.Users.Permissions.UpdateError'
            )
          );
        },
      });
  }

  private emitChange(): void {
    this.detectChange.emit(this.changeSuccess());
  }
}
