import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialModule } from '../../shared/material.module';
import { AvailableStationsListComponent } from './components/available-stations-list/available-stations-list.component';
import { PermissionsListComponent } from './components/permissions-list/permissions-list.component';
import { GoogleSigninButtonModule } from '@abacritt/angularx-social-login';
import { AuthorizationService } from '../authorization/services/authorization-service';

type ViewSwitch = 'stationsAvailable' | 'permissions';

@Component({
  selector: 'app-stations-menu',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    MatButtonToggleModule,
    TranslateModule,
    AvailableStationsListComponent,
    PermissionsListComponent,
    GoogleSigninButtonModule,
  ],
  templateUrl: './stations-menu.component.html',
})
export class StationsMenuComponent {
  viewMode = signal<ViewSwitch>('stationsAvailable');

  private authService = inject(AuthorizationService);
  isLoggedIn = this.authService.isAuthorized;
}
