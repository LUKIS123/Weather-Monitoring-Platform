import { CommonModule } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { MaterialModule } from '../../../../shared/material.module';
import { UsersPermissionRequest } from '../../models/users-permission';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FormatDateService } from '../../../../shared/services/format-date.service';

@Component({
  selector: 'app-permissions-list-item',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './permissions-list-item.component.html',
})
export class PermissionsListItemComponent {
  private readonly translateService = inject(TranslateService);
  private readonly formatDateService = inject(FormatDateService);

  permissionRequest = input.required<UsersPermissionRequest>();

  public getDateFormatted(): string {
    return this.formatDateService.formatDate(
      this.permissionRequest()?.changeDate ?? ''
    );
  }

  public getPermissionStatus(): string {
    const status = this.permissionRequest()?.permissionStatus;
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
