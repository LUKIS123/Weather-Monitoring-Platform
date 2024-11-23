import { Component, signal } from '@angular/core';
import { MaterialModule } from '../../shared/material.module';
import { CommonModule } from '@angular/common';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { UserListComponent } from './components/user-list/user-list.component';

type ViewSelection = 'permissionRequests' | 'users';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule,
    MatButtonToggleModule,
    TranslateModule,
    UserListComponent,
  ],
  templateUrl: './user-management.component.html',
})
export class UserManagementComponent {
  viewSelection = signal<ViewSelection>('users');
}
