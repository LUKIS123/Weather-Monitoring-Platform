import { CommonModule } from '@angular/common';
import { Component, inject, Inject, signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MaterialModule } from '../../../../shared/material.module';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { GetDeviceResponse } from '../../models/get-device-response';
import { RemoveDeviceService } from '../../services/remove-device.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-remove-device',
  standalone: true,
  imports: [CommonModule, TranslateModule, MaterialModule],
  templateUrl: './remove-device.component.html',
})
export class RemoveDeviceComponent {
  private readonly removeDeviceService = inject(RemoveDeviceService);
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);

  #itemRemoved = signal<boolean>(false);
  public readonly itemRemoved = this.#itemRemoved.asReadonly();
  #details = signal<GetDeviceResponse | null>(null);
  public readonly details = this.#details.asReadonly();
  #isLoading = signal<boolean>(false);
  public readonly isLoading = this.#isLoading.asReadonly();
  #disable = signal<boolean>(false);
  public readonly disable = this.#disable.asReadonly();

  constructor(
    @Inject(MAT_DIALOG_DATA) data: { getDeviceResponse: GetDeviceResponse },
    private dialogRef: MatDialogRef<RemoveDeviceComponent>
  ) {
    this.#details.set(data.getDeviceResponse);
  }

  removeDevice() {
    this.#isLoading.set(true);
    this.#disable.set(true);

    this.removeDeviceService
      .removeDevice(this.#details()!.id)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: () => {
          this.#itemRemoved.set(true);
          this.toastService.openSuccess(
            this.translateService.instant(
              'DeviceManagement.Details.Remove.Success'
            )
          );
          this.dialogRef.close(this.#itemRemoved());
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant(
              'DeviceManagement.Details.Remove.Error'
            )
          );
        },
      });
  }

  cancelRemove() {
    this.dialogRef.close(this.#itemRemoved());
  }
}
