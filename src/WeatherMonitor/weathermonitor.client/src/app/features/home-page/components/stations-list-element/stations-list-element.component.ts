import { CommonModule } from '@angular/common';
import { Component, computed, inject, input } from '@angular/core';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule } from '@ngx-translate/core';
import { GetStationResponse } from '../../models/get-stations-response';
import { Router } from '@angular/router';
import { FormatDateService } from '../../../../shared/services/format-date.service';
import { WeatherDataPreviewComponent } from '../../../../shared/components/weather-data-preview/weather-data-preview.component';

@Component({
  selector: 'app-stations-list-element',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    WeatherDataPreviewComponent,
  ],
  templateUrl: './stations-list-element.component.html',
})
export class StationsListElementComponent {
  private readonly router = inject(Router);
  private readonly formatDateService = inject(FormatDateService);

  weatherStation = input.required<GetStationResponse>();
  formattedDate = computed(() => {
    return this.formatDateService.formatDate(
      this.weatherStation().receivedAt ?? undefined
    );
  });
  public panelOpenState = false;

  navigate() {
    this.router.navigate([
      `/DataVisualization/Station/${this.weatherStation().deviceId}`,
    ]);
  }
}
