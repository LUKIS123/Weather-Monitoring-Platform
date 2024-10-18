import { CommonModule } from '@angular/common';
import { Component, inject, Inject, signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MaterialModule } from '../../../../shared/material.module';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RegisterDeviceFormControl } from '../../models/register-device-form-control';
import { DeviceRegistrationService } from '../../services/device-registration.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { CreateDeviceResponse } from '../../models/create-device-response';
import { DeviceRegistrationResult } from '../../models/device-registration-result';

@Component({
  selector: 'app-device-registration',
  standalone: true,
  imports: [CommonModule, TranslateModule, MaterialModule, ReactiveFormsModule],
  templateUrl: './device-registration.component.html',
})
export class DeviceRegistrationComponent {
  private readonly deviceRegistrationService = inject(
    DeviceRegistrationService
  );
  private readonly toastService = inject(ToastService);
  private readonly translateService = inject(TranslateService);

  #registrationSuccess = signal<boolean>(false);
  public readonly registrationSuccess = this.#registrationSuccess.asReadonly();

  #createResponse = signal<CreateDeviceResponse | null>(null);

  constructor(
    @Inject(MAT_DIALOG_DATA) data: unknown,
    private dialogRef: MatDialogRef<DeviceRegistrationComponent>
  ) {}

  public formGroup = new FormGroup<RegisterDeviceFormControl>({
    deviceUsername: new FormControl<string | null>(null, [
      Validators.required,
      Validators.minLength(3),
      Validators.maxLength(100),
    ]),
    googleMapsPlusCode: new FormControl<string | null>(null, [
      Validators.required,
      Validators.maxLength(50),
    ]),
    deviceExtraInfo: new FormControl<string | null>(null, [
      Validators.maxLength(255),
    ]),
  });

  public get isFormValid() {
    return this.formGroup.valid;
  }

  submit() {
    const deviceUsername = this.formGroup.get('deviceUsername')?.value ?? '';
    const googleMapsPlusCode =
      this.formGroup.get('googleMapsPlusCode')?.value ?? '';
    const deviceExtraInfo =
      this.formGroup.get('deviceExtraInfo')?.value ?? null;

    this.deviceRegistrationService
      .registerNewDevice(deviceUsername, googleMapsPlusCode, deviceExtraInfo)
      .subscribe({
        next: (response) => {
          this.#createResponse.set(response);
          this.formGroup.reset();
          this.#registrationSuccess.set(true);
          this.toastService.openSuccess(
            this.translateService.instant('DeviceManagement.Register.Success')
          );
          setTimeout(() => {
            this.dialogRef.close({
              registered: this.#registrationSuccess(),
              createdResponse: this.#createResponse(),
            } as DeviceRegistrationResult);
          }, 2500);
        },
        error: () => {
          this.#registrationSuccess.set(false);
          this.toastService.openError(
            this.translateService.instant('DeviceManagement.Register.Error')
          );
        },
      });
  }

  cancelForm() {
    this.formGroup.reset();
    this.dialogRef.close({
      registered: this.#registrationSuccess(),
      createdResponse: this.#createResponse(),
    } as DeviceRegistrationResult);
  }
}
