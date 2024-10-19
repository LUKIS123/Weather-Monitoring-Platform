import { Component, inject, input, output } from '@angular/core';
import { GetDeviceResponse } from '../../models/get-device-response';
import { MatDialog } from '@angular/material/dialog';
import { DeviceDetailsComponent } from '../device-details/device-details.component';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule } from '@ngx-translate/core';
import { RemoveDeviceComponent } from '../remove-device/remove-device.component';

@Component({
  selector: 'app-device-list-item',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './device-list-item.component.html',
})
export class DeviceListItemComponent {
  deviceInfo = input.required<GetDeviceResponse>();
  itemDelete = output<boolean>();

  public panelOpenState = false;
  #dialog = inject(MatDialog);

  public openDetails(): void {
    this.#dialog.open(DeviceDetailsComponent, {
      data: {
        getDeviceResponse: this.deviceInfo(),
      },
      panelClass: 'popup',
      maxWidth: '100dvw',
    });
  }

  public OpenDelete(): void {
    const dialogRef = this.#dialog.open(RemoveDeviceComponent, {
      data: {
        getDeviceResponse: this.deviceInfo(),
      },
      panelClass: 'popup',
      maxWidth: '100dvw',
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result === true) {
        this.emitDelete();
      }
    });
  }

  private emitDelete(): void {
    this.itemDelete.emit(true);
  }
}
