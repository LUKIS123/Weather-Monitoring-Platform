import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { MaterialModule } from '../../shared/material.module';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { TranslateModule } from '@ngx-translate/core';
import { StationsListComponent } from './components/stations-list/stations-list.component';
import { StationsMapComponent } from './components/stations-map/stations-map.component';

type ViewMode = 'list' | 'map';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    MatButtonToggleModule,
    TranslateModule,
    StationsListComponent,
    StationsMapComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './home-page.component.html',
})
export class HomePageComponent {
  viewMode = signal<ViewMode>('list');
}
