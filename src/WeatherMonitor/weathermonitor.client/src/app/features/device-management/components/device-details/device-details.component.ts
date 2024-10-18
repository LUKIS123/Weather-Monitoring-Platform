import { CommonModule } from '@angular/common';
import { Component, Inject, signal } from '@angular/core';
import { GetDeviceResponse } from '../../models/get-device-response';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-device-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './device-details.component.html',
})
export class DeviceDetailsComponent {
  #details = signal<GetDeviceResponse | null>(null);
  public readonly details = this.#details.asReadonly();

  // TODO: tutaj bedzie trzea wyswietlac nowy request z credentialsami do urzadzenia, ale to moze pozniej
  constructor(
    @Inject(MAT_DIALOG_DATA) data: { getDeviceResponse: GetDeviceResponse }
  ) {
    this.#details.set(data.getDeviceResponse);
  }
}
