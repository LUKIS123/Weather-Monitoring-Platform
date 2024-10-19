import { CommonModule } from '@angular/common';
import { Component, inject, Inject, signal } from '@angular/core';
import { CreateDeviceResponse } from '../../models/create-device-response';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CreateDeviceDeviceFormControl } from '../../models/create-device-form-control';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MaterialModule } from '../../../../shared/material.module';

@Component({
  selector: 'app-created-device-credentials',
  standalone: true,
  imports: [CommonModule, TranslateModule, MaterialModule, ReactiveFormsModule],
  templateUrl: './created-device-credentials.component.html',
})
export class CreatedDeviceCredentialsComponent {
  private readonly translateService = inject(TranslateService);

  #createResult = signal<CreateDeviceResponse | null>(null);
  public readonly createResult = this.#createResult.asReadonly();
  #hide = signal<boolean>(true);
  public readonly hide = this.#hide.asReadonly();
  public formGroup: FormGroup<CreateDeviceDeviceFormControl>;

  constructor(@Inject(MAT_DIALOG_DATA) data: { device: CreateDeviceResponse }) {
    this.#createResult.set(data.device);

    this.formGroup = new FormGroup<CreateDeviceDeviceFormControl>({
      id: new FormControl<number>(this.#createResult()?.id ?? 0, []),
      googleMapsPlusCode: new FormControl<string>(
        this.#createResult()?.googleMapsPlusCode ?? '',
        []
      ),
      isActivate: new FormControl<string>(
        this.#createResult()?.isActivate == true
          ? this.translateService.instant('DeviceManagement.Credentials.Active')
          : this.translateService.instant(
              'DeviceManagement.Credentials.InActive'
            ),

        []
      ),
      username: new FormControl<string>(
        this.#createResult()?.username ?? '',
        []
      ),
      password: new FormControl<string>(
        this.#createResult()?.password ?? '',
        []
      ),
      clientId: new FormControl<string>(
        this.#createResult()?.clientId ?? '',
        []
      ),
      topic: new FormControl<string>(this.#createResult()?.topic ?? '', []),
    });
  }

  public siwtchPasswordVisibility(): void {
    this.#hide.set(!this.#hide());
  }
}
