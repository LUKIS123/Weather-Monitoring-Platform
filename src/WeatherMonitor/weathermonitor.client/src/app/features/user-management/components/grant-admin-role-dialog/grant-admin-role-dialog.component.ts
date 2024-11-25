import { Component, Inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { User } from '../../models/user';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialModule } from '../../../../shared/material.module';

@Component({
  selector: 'app-grant-admin-role-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './grant-admin-role-dialog.component.html',
})
export class GrantAdminRoleDialogComponent {
  #user = signal<User>({} as User);
  public user = this.#user.asReadonly();

  confirmGrandAdminRole = signal(false);

  constructor(
    @Inject(MAT_DIALOG_DATA) data: { userData: User },
    private dialogRef: MatDialogRef<User>
  ) {
    this.#user.set(data.userData);
  }

  grantAdminRole() {
    this.confirmGrandAdminRole.set(true);
    this.dialogRef.close(this.confirmGrandAdminRole());
  }

  cancel() {
    this.confirmGrandAdminRole.set(false);
    this.dialogRef.close(this.confirmGrandAdminRole());
  }
}
