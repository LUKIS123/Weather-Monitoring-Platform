import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  inject,
  Inject,
  OnInit,
  signal,
} from '@angular/core';
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
import { finalize } from 'rxjs';

@Component({
  selector: 'app-device-registration',
  standalone: true,
  imports: [CommonModule, TranslateModule, MaterialModule, ReactiveFormsModule],
  templateUrl: './device-registration.component.html',
})
export class DeviceRegistrationComponent implements OnInit {
  private readonly deviceRegistrationService = inject(
    DeviceRegistrationService
  );
  private readonly toastService = inject(ToastService);
  private readonly translateService = inject(TranslateService);

  #isLoading = signal(false);
  public readonly isLoading = this.#isLoading.asReadonly();

  buttonDisable = signal(false);
  validForm = signal(false);
  shouldDisable = computed(() => {
    return this.buttonDisable() || !this.validForm() || !this.isFormValid;
  });

  #registrationSuccess = signal<boolean>(false);
  public readonly registrationSuccess = this.#registrationSuccess.asReadonly();

  #createResponse = signal<CreateDeviceResponse | null>(null);

  constructor(
    @Inject(MAT_DIALOG_DATA) data: unknown,
    private dialogRef: MatDialogRef<DeviceRegistrationComponent>
  ) {}

  ngOnInit(): void {
    this.formGroup.statusChanges.subscribe((status) => {
      this.validForm.set(status === 'VALID');
    });
  }

  public formGroup = new FormGroup<RegisterDeviceFormControl>({
    deviceUsername: new FormControl<string | null>(null, [
      Validators.required,
      Validators.minLength(3),
      Validators.maxLength(100),
    ]),
    googleMapsPlusCode: new FormControl<string | null>(null, [
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

    this.buttonDisable.set(true);
    this.#isLoading.set(true);

    this.deviceRegistrationService
      .registerNewDevice(deviceUsername, googleMapsPlusCode, deviceExtraInfo)
      .pipe(
        finalize(() => {
          this.#isLoading.set(false);
        })
      )
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
          }, 2000);
        },
        error: () => {
          this.buttonDisable.set(false);
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
