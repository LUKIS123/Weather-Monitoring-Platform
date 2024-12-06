import { inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { StationLocation } from '../../features/home-page/models/station-location';

@Injectable({
  providedIn: 'root',
})
export class StationMapMarkerContentServiceService {
  private readonly translateService = inject(TranslateService);
  private readonly imgPath = '/assets/images/weather-station-icon.png';
  private readonly dataViewBasePath = '/DataVisualization/Station';

  buildMarkerContent(stationlocation: StationLocation): HTMLElement {
    const content = document.createElement('div');
    content.classList.add('stationMarkerProperty');

    content.addEventListener('click', (event) => {
      event.stopPropagation();
      if (content.classList.contains('highlight')) {
        content.classList.remove('highlight');
        content.style.zIndex = '0';
      } else {
        content.classList.add('highlight');
        content.style.zIndex = '3';
      }
    });

    content.innerHTML = `
      <div class="icon">
        <i aria-hidden="true" class="stationMarkerColor" title="${this.translateService.instant(
          'Shared.WeatherStation'
        )}: ${stationlocation.station.username}">          
          <img src=${this.imgPath} alt="Weather Station"/>
        </i>
      </div>
      <div class="details">
        <a
          href='${this.dataViewBasePath}/${stationlocation.station.deviceId}'"
          target="_self"
          style="position: absolute; top: 0; left: 0; margin: 4px; padding: 2px 2px; background-color: #007bff; color: white; 
            border: none; border-radius: 5px; cursor: pointer; font-size: 12px;"
        >
          ${this.translateService.instant('Shared.DataVisualization.Short')}
        </a>
        <div style="display: flex; justify-content: space-between; padding-left: 1rem; padding-right: 0.75rem;">
            <span>${this.translateService.instant(
              'Shared.Temperature'
            )}: </span>
            <span>${stationlocation.station.temperature.toFixed(
              1
            )} ${this.translateService.instant('Shared.TemperatureUnit')}</span>
          </div>
          <div style="display: flex; justify-content: space-between; padding-left: 1rem; padding-right: 0.75rem;">
            <span>${this.translateService.instant('Shared.Humidity')}: </span>
            <span>${stationlocation.station.humidity.toFixed(
              2
            )} ${this.translateService.instant('Shared.HumidityUnit')}</span>
          </div>
          <div style="display: flex; justify-content: space-between; padding-left: 1rem; padding-right: 0.75rem;">
            <span>${this.translateService.instant('Shared.Pressure')}: </span>
            <span>${(stationlocation.station.airPressure / 100).toFixed(
              2
            )} ${this.translateService.instant('Shared.PressureUnit')}</span>
          </div>
          <div class="features">
          <div>
              <i aria-hidden="true" title="PM1.0"></i>
              <span>${this.translateService.instant('Shared.PM1_0')}:</span>
              ${this.getnerateFeatureElement(stationlocation.station.pM1_0)}
          </div>
          <div>
              <i aria-hidden="true" title="PM2.5"></i>
              <span>${this.translateService.instant('Shared.PM2_5')}:</span>
              ${this.getnerateFeatureElement(stationlocation.station.pM2_5)}
          </div>
          <div>
              <i aria-hidden="true" title="PM10"></i>
              <span>${this.translateService.instant('Shared.PM10')}:</span>
              ${this.getnerateFeatureElement(stationlocation.station.pM10)}
          </div>
          </div>
      </div>
    `;
    return content;
  }

  private getnerateFeatureElement(data: number | null | undefined): string {
    if (data === null || data === undefined) {
      return `<span>${this.translateService.instant(
        'Shared.MeasurementNotAvailableShort'
      )}</span>`;
    }

    return `<span>${data} ${this.translateService.instant(
      'Shared.PMUnit'
    )}</span>`;
  }
}
