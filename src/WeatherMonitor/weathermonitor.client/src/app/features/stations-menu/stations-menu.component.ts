import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialModule } from '../../shared/material.module';
import { AvailableStationsListComponent } from "./components/available-stations-list/available-stations-list.component";
import { PermissionsListComponent } from "./components/permissions-list/permissions-list.component";

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
    PermissionsListComponent
],
  templateUrl: './stations-menu.component.html',
})
export class StationsMenuComponent {
  viewMode = signal<ViewSwitch>('stationsAvailable');
}
