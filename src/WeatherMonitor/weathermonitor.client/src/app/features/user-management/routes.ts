import { Route } from '@angular/router';
import { UserManagementComponent } from './user-management.component';
import { PermissionRequestsManagementComponent } from './permission-requests-management.component';

export const UserManagementRoutes: Route[] = [
  {
    path: 'PermissionRequests',
    component: PermissionRequestsManagementComponent,
  },
  {
    path: 'Users',
    component: UserManagementComponent,
  },
];
