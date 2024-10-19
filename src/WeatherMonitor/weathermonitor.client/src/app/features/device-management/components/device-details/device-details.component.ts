import { CommonModule } from '@angular/common';
import { Component, inject, Inject, signal } from '@angular/core';
import { GetDeviceResponse } from '../../models/get-device-response';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MaterialModule } from '../../../../shared/material.module';
import { DeviceMqttCredentialsDataService } from '../../services/device-mqtt-credentials-data.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { DeviceMqttCredentialsResponse } from '../../models/device-mqtt-credentials-response';
import { finalize } from 'rxjs';
import { DeviceMqttCredentialsFormControl } from '../../models/device-mqtt-credentials-form-control';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-device-details',
  standalone: true,
  imports: [CommonModule, TranslateModule, MaterialModule, ReactiveFormsModule],
  templateUrl: './device-details.component.html',
})
export class DeviceDetailsComponent {
  private readonly translateService = inject(TranslateService);
  private readonly deviceMqttService = inject(DeviceMqttCredentialsDataService);
  private readonly toastService = inject(ToastService);

  #details = signal<GetDeviceResponse | null>(null);
  public readonly details = this.#details.asReadonly();
  #credentials = signal<DeviceMqttCredentialsResponse | null>(null);
  public readonly credentials = this.#credentials.asReadonly();
  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #hide = signal<boolean>(true);
  public readonly hide = this.#hide.asReadonly();
  public formGroup: FormGroup<DeviceMqttCredentialsFormControl>;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: { getDeviceResponse: GetDeviceResponse }
  ) {
    this.#details.set(data.getDeviceResponse);
    const deviceId = this.#details()?.id;
    if (deviceId !== undefined) {
      this.loadCredentials(deviceId);
    } else {
      this.#isLoading.set(false);
    }

    this.formGroup = new FormGroup<DeviceMqttCredentialsFormControl>({
      id: new FormControl<number>(this.#details()?.id ?? 0, []),
      username: new FormControl<string>(
        this.#details()?.mqttUsername ?? '',
        []
      ),
      password: new FormControl<string>(
        this.#credentials()?.password ?? '',
        []
      ),
      clientId: new FormControl<string>(
        this.#details()?.mqttBrokerClientId ?? '',
        []
      ),
      topic: new FormControl<string>(this.#credentials()?.topic ?? '', []),
    });
  }

  private loadCredentials(deviceId: number) {
    this.deviceMqttService
      .getMqttCredentials(deviceId)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: (credentials) => {
          this.#credentials.set(credentials);
          this.createForm();
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant('DeviceManagement.Details.Error')
          );
        },
      });
  }

  private createForm() {
    this.formGroup = new FormGroup<DeviceMqttCredentialsFormControl>({
      id: new FormControl<number>(this.#credentials()?.id ?? 0, []),
      username: new FormControl<string>(
        this.#credentials()?.username ?? '',
        []
      ),
      password: new FormControl<string>(
        this.#credentials()?.password ?? '',
        []
      ),
      clientId: new FormControl<string>(
        this.#credentials()?.clientId ?? '',
        []
      ),
      topic: new FormControl<string>(this.#credentials()?.topic ?? '', []),
    });
  }

  public siwtchPasswordVisibility(): void {
    this.#hide.set(!this.#hide());
  }
}
