import { CommonModule } from '@angular/common';
import { Component, inject, ViewChild } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { DeviceListComponent } from './components/device-list/device-list.component';
import { MatDialog } from '@angular/material/dialog';
import { DeviceRegistrationComponent } from './components/device-registration/device-registration.component';
import { MaterialModule } from '../../shared/material.module';

@Component({
  selector: 'app-device-management',
  standalone: true,
  imports: [CommonModule, TranslateModule, MaterialModule, DeviceListComponent],
  templateUrl: './device-management.component.html',
})
export class DeviceManagementComponent {
  #dialog = inject(MatDialog);
  @ViewChild(DeviceListComponent) deviceListComponent!: DeviceListComponent;

  public openDeviceRegistration(): void {
    const dialogRef = this.#dialog.open(DeviceRegistrationComponent, {
      data: {},
      panelClass: 'popup',
      maxWidth: '100dvw',
    });

    dialogRef.afterClosed().subscribe(() => {
      if (this.deviceListComponent) {
        this.deviceListComponent.refresh();
      }
    });
  }
}
