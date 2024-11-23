import { Component, input } from '@angular/core';
import { PendingPermissionWithAddress } from '../../models/pending-permission-with-address';

@Component({
  selector: 'app-permission-request-list-element',
  standalone: true,
  imports: [],
  templateUrl: './permission-request-list-element.component.html',
})
export class PermissionRequestListElementComponent {
  permissionRequest = input.required<PendingPermissionWithAddress>();
}
