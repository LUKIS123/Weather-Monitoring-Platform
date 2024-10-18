import { CommonModule } from '@angular/common';
import { Component, Inject, signal } from '@angular/core';
import { CreateDeviceResponse } from '../../models/create-device-response';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-created-device-credentials',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './created-device-credentials.component.html',
})
export class CreatedDeviceCredentialsComponent {
  #createResult = signal<CreateDeviceResponse | null>(null);
  public readonly createResult = this.#createResult.asReadonly();

  constructor(@Inject(MAT_DIALOG_DATA) data: { device: CreateDeviceResponse }) {
    this.#createResult.set(data.device);
  }
}
