import { Component, signal } from '@angular/core';
import { PermissionRequestListComponent } from './components/permission-request-list/permission-request-list.component';
import { CommonModule } from '@angular/common';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialModule } from '../../shared/material.module';

type ViewSelection = 'permissionRequests' | 'users';

@Component({
  selector: 'app-permission-requests-management',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MaterialModule,
    MatButtonToggleModule,
    TranslateModule,
    PermissionRequestListComponent,
  ],
  templateUrl: './permission-requests-management.component.html',
})
export class PermissionRequestsManagementComponent {
  viewSelection = signal<ViewSelection>('permissionRequests');
}
