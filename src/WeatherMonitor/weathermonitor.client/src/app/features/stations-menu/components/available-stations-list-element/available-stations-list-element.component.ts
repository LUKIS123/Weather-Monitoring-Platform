import { Component, inject, input } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { StationWithAddress } from '../../models/station-with-address';
import { AuthorizationService } from '../../../authorization/services/authorization-service';
import { MatDialog } from '@angular/material/dialog';
import { PermissionRequestDialogComponent } from '../permission-request-dialog/permission-request-dialog.component';

@Component({
  selector: 'app-available-stations-list-element',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './available-stations-list-element.component.html',
})
export class AvailableStationsListElementComponent {
  #dialog = inject(MatDialog);

  private authService = inject(AuthorizationService);
  isLoggedIn = this.authService.isAuthorized;
  public panelOpenState = false;

  station = input.required<StationWithAddress>();

  public openPermissionRequestPopup(): void {
    const station = this.station();
    this.#dialog.open<PermissionRequestDialogComponent, unknown, unknown>(
      PermissionRequestDialogComponent,
      {
        data: { station },
        panelClass: 'popup',
        maxWidth: '100dvw',
      }
    );
  }
}
