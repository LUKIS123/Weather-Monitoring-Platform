import { Component, inject, Inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { PendingPermissionWithAddress } from '../../models/pending-permission-with-address';
import { ToastService } from '../../../../shared/services/toast.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FormatDateService } from '../../../../shared/services/format-date.service';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { PermissionStatusEnum } from '../../../../shared/models/permission-status';
import { UpdatePermissionService } from '../../services/update-permission.service';
import { finalize } from 'rxjs';
import { UpdatePermissionRequest } from '../../models/update-permission-request ';

@Component({
  selector: 'app-permission-decision',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './permission-decision.component.html',
})
export class PermissionDecisionComponent {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly formatDateService = inject(FormatDateService);
  private readonly updatePermissionService = inject(UpdatePermissionService);

  #permissionRequest = signal<PendingPermissionWithAddress>(
    {} as PendingPermissionWithAddress
  );
  public readonly permissionRequest = this.#permissionRequest.asReadonly();

  #isLoading = signal(false);
  public readonly isLoading = this.#isLoading.asReadonly();
  buttonDisable = signal(false);
  updateStatusSuccess = signal<boolean>(false);

  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: { permissionRequest: PendingPermissionWithAddress },
    private dialogRef: MatDialogRef<PermissionDecisionComponent>
  ) {
    this.#permissionRequest.set(data.permissionRequest);
  }

  public getDateFormatted(): string {
    return this.formatDateService.formatDate(
      this.permissionRequest()?.permission.requestedAt ?? ''
    );
  }

  public getPermissionStatus(): string {
    const status = this.permissionRequest()?.permission.status;
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

  cancel() {
    this.dialogRef.close(this.updateStatusSuccess());
  }

  sendPermissionDecision(status: number) {
    if (!(status in PermissionStatusEnum)) {
      return;
    }
    const permissionStatus: PermissionStatusEnum =
      status as PermissionStatusEnum;

    const updatePermissionRequest: UpdatePermissionRequest = {
      userId: this.permissionRequest().permission.userId,
      deviceId: this.permissionRequest().permission.deviceId,
      status: permissionStatus,
    };

    this.#isLoading.set(true);
    this.buttonDisable.set(true);

    this.updatePermissionService
      .updatePermission(updatePermissionRequest)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: () => {
          this.updateStatusSuccess.set(true);
          this.toastService.openSuccess(
            this.translateService.instant(
              'UserManagement.Permissions.Action.Success'
            )
          );
          setTimeout(() => {
            this.dialogRef.close(this.updateStatusSuccess());
          }, 2000);
        },
        error: () => {
          this.buttonDisable.set(false);
          this.updateStatusSuccess.set(false);
          this.toastService.openError(
            this.translateService.instant(
              'UserManagement.Permissions.Action.Error'
            )
          );
        },
      });
  }
}
