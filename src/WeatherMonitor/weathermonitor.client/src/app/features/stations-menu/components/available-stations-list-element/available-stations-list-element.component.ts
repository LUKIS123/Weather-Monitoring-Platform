import { Component, inject, input } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import {
  MatPaginatorIntl,
  MatPaginatorModule,
} from '@angular/material/paginator';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { MatPaginatorIntlPl } from '../../../../shared/components/paginator/MatPaginatorIntlPl';
import { StationWithAddress } from '../../models/station-with-address';
import { AuthorizationService } from '../../../authorization/services/authorization-service';

@Component({
  selector: 'app-available-stations-list-element',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule, MatPaginatorModule],
  providers: [{ provide: MatPaginatorIntl, useClass: MatPaginatorIntlPl }],
  templateUrl: './available-stations-list-element.component.html',
})
export class AvailableStationsListElementComponent {
  private authService = inject(AuthorizationService);
  isLoggedIn = this.authService.isAuthorized;

  station = input.required<StationWithAddress>();

  public panelOpenState = false;
}
