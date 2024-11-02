import { CommonModule } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule } from '@ngx-translate/core';
import { GetStationResponse } from '../../models/get-stations-response';
import { Router } from '@angular/router';

@Component({
  selector: 'app-stations-list-element',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './stations-list-element.component.html',
})
export class StationsListElementComponent {
  private readonly router = inject(Router);

  weatherStation = input.required<GetStationResponse>();
  public panelOpenState = false;

  navigate() {
    this.router.navigate(['/']);
  }
}
