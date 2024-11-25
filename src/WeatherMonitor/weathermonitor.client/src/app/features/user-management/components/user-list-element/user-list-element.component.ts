import { Component, EventEmitter, inject, input, Output } from '@angular/core';
import { User } from '../../models/user';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FormatDateService } from '../../../../shared/services/format-date.service';
import { PermissionDecisionComponent } from '../permission-decision/permission-decision.component';
import { UserPermissionsManagementComponent } from '../user-permissions-management/user-permissions-management.component';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { AuthorizationService } from '../../../authorization/services/authorization-service';

@Component({
  selector: 'app-user-list-element',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './user-list-element.component.html',
})
export class UserListElementComponent {
  private readonly translateService = inject(TranslateService);
  private readonly authService = inject(AuthorizationService);

  #dialog = inject(MatDialog);
  @Output() detectChange = new EventEmitter<boolean>();
  user = input.required<User>();

  openUserManagementPopup() {
    const user = this.user();
    const dialogRef = this.#dialog.open<
      UserPermissionsManagementComponent,
      unknown,
      boolean
    >(UserPermissionsManagementComponent, {
      data: { userData: user },
      panelClass: 'popup',
      maxWidth: '100dvw',
    });

    dialogRef.afterClosed().subscribe((result?: boolean) => {
      if (result === undefined || result === true) {
        this.detectChange.emit(true);
      }
    });
  }

  getUserRole(): string {
    const role = this.user()?.role;
    switch (role) {
      case 1:
        return this.translateService.instant('UserAccount.User');
      case 2:
        return this.translateService.instant('UserAccount.Admin');
      default:
        return this.translateService.instant('UserAccount.Unknown');
    }
  }

  currentUserDisable(): boolean {
    if (this.user()?.id === this.authService.userId()) {
      return true;
    }
    return false;
  }
}
