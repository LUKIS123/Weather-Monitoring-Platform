import { Component, EventEmitter, inject, input, Output } from '@angular/core';
import { PendingPermissionWithAddress } from '../../models/pending-permission-with-address';
import { MaterialModule } from '../../../../shared/material.module';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FormatDateService } from '../../../../shared/services/format-date.service';
import { MatDialog } from '@angular/material/dialog';
import { PermissionDecisionComponent } from '../permission-decision/permission-decision.component';

@Component({
  selector: 'app-permission-request-list-element',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './permission-request-list-element.component.html',
})
export class PermissionRequestListElementComponent {
  private readonly translateService = inject(TranslateService);
  private readonly formatDateService = inject(FormatDateService);
  #dialog = inject(MatDialog);
  @Output() detectChange = new EventEmitter<boolean>();
  permissionRequest = input.required<PendingPermissionWithAddress>();

  openPermissionRequestPopup() {
    const request = this.permissionRequest();
    const dialogRef = this.#dialog.open<
      PermissionDecisionComponent,
      unknown,
      boolean
    >(PermissionDecisionComponent, {
      data: { permissionRequest: request },
      panelClass: 'popup',
      maxWidth: '100dvw',
    });

    dialogRef.afterClosed().subscribe((result?: boolean) => {
      if (result === true) {
        this.detectChange.emit(true);
      }
    });
  }

  public getDateFormatted(): string {
    return this.formatDateService.formatDate(
      this.permissionRequest()?.permission.requestedAt ?? ''
    );
  }

  public getPermissionStatus(): string {
    const status = this.permissionRequest()?.permission.status;
    switch (status) {
      case 1:
        return this.translateService.instant('PermissionStatus.NotRequested');
      case 2:
        return this.translateService.instant('PermissionStatus.Pending');
      case 3:
        return this.translateService.instant('PermissionStatus.Granted');
      case 4:
        return this.translateService.instant('PermissionStatus.Denied');
      default:
        return this.translateService.instant('PermissionStatus.Unknown');
    }
  }
}
