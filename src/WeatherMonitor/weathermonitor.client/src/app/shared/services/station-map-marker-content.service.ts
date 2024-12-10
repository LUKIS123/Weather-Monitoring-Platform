import { inject, Injectable, Renderer2 } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { StationLocation } from '../../features/home-page/models/station-location';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class StationMapMarkerContentService {
  private readonly translateService = inject(TranslateService);
  private readonly router = inject(Router);
  private readonly imgPath = '/assets/images/weather-station-icon.png';
  private readonly dataViewBasePath = '/DataVisualization/Station';

  buildMarkerContent(
    renderer: Renderer2,
    stationLocation: StationLocation
  ): HTMLElement {
    const content = renderer.createElement('div');
    renderer.addClass(content, 'stationMarkerProperty');

    renderer.listen(content, 'click', (event) => {
      event.stopPropagation();
      if (content.classList.contains('highlight')) {
        renderer.removeClass(content, 'highlight');
        renderer.setStyle(content, 'zIndex', '0');
      } else {
        renderer.addClass(content, 'highlight');
        renderer.setStyle(content, 'zIndex', '3');
      }
    });

    const icon = renderer.createElement('div');
    renderer.addClass(icon, 'icon');

    const imgWrapper = renderer.createElement('i');
    renderer.addClass(imgWrapper, 'stationMarkerColor');
    renderer.setAttribute(
      imgWrapper,
      'title',
      `${this.translateService.instant('Shared.WeatherStation')}: ${
        stationLocation.station.username
      }`
    );
    renderer.setAttribute(imgWrapper, 'aria-hidden', 'true');

    const img = renderer.createElement('img');
    renderer.setAttribute(img, 'src', this.imgPath);
    renderer.setAttribute(img, 'alt', 'Weather Station');
    renderer.appendChild(imgWrapper, img);
    renderer.appendChild(icon, imgWrapper);

    const details = renderer.createElement('div');
    renderer.addClass(details, 'details');

    const button = renderer.createElement('button');
    renderer.setAttribute(
      button,
      'style',
      `position: absolute; top: 0; left: 0; margin: 4px; padding: 2px 2px; background-color: #007bff; color: white;
      border: none; border-radius: 5px; cursor: pointer; font-size: 12px;`
    );
    const buttonText = renderer.createText(
      this.translateService.instant('Shared.DataVisualization.Short')
    );
    renderer.appendChild(button, buttonText);

    renderer.listen(button, 'click', (event) => {
      event.stopPropagation();
      this.router.navigate([
        `${this.dataViewBasePath}/${stationLocation.station.deviceId}`,
      ]);
    });

    const temperature = this.createDetailRow(
      renderer,
      this.translateService.instant('Shared.Temperature'),
      `${stationLocation.station.temperature.toFixed(
        1
      )} ${this.translateService.instant('Shared.TemperatureUnit')}`
    );
    const humidity = this.createDetailRow(
      renderer,
      this.translateService.instant('Shared.Humidity'),
      `${stationLocation.station.humidity.toFixed(
        2
      )} ${this.translateService.instant('Shared.HumidityUnit')}`
    );
    const pressure = this.createDetailRow(
      renderer,
      this.translateService.instant('Shared.Pressure'),
      `${(stationLocation.station.airPressure / 100).toFixed(
        2
      )} ${this.translateService.instant('Shared.PressureUnit')}`
    );

    const features = renderer.createElement('div');
    renderer.addClass(features, 'features');
    this.appendFeature(
      renderer,
      features,
      'PM1_0',
      stationLocation.station.pM1_0
    );
    this.appendFeature(
      renderer,
      features,
      'PM2_5',
      stationLocation.station.pM2_5
    );
    this.appendFeature(
      renderer,
      features,
      'PM10',
      stationLocation.station.pM10
    );

    renderer.appendChild(details, button);
    renderer.appendChild(details, temperature);
    renderer.appendChild(details, humidity);
    renderer.appendChild(details, pressure);
    renderer.appendChild(details, features);

    renderer.appendChild(content, icon);
    renderer.appendChild(content, details);

    return content;
  }

  createDetailRow(
    renderer: Renderer2,
    label: string,
    value: string
  ): HTMLElement {
    const row = renderer.createElement('div');
    renderer.setAttribute(
      row,
      'style',
      'display: flex; justify-content: space-between; padding-left: 1rem; padding-right: 0.75rem;'
    );

    const labelSpan = renderer.createElement('span');
    const labelText = renderer.createText(label);
    renderer.appendChild(labelSpan, labelText);

    const valueSpan = renderer.createElement('span');
    const valueText = renderer.createText(value);
    renderer.appendChild(valueSpan, valueText);

    renderer.appendChild(row, labelSpan);
    renderer.appendChild(row, valueSpan);

    return row;
  }

  appendFeature(
    renderer: Renderer2,
    parent: HTMLElement,
    featureName: string,
    value: number | null | undefined
  ): void {
    const featureDiv = renderer.createElement('div');

    const icon = renderer.createElement('i');
    renderer.setAttribute(icon, 'aria-hidden', 'true');
    renderer.setAttribute(icon, 'title', featureName);

    const label = renderer.createElement('span');
    const labelText = renderer.createText(
      `${this.translateService.instant(`Shared.${featureName}`)}:`
    );
    renderer.appendChild(label, labelText);

    const valueElement = this.getnerateFeatureElementWithRenderer(
      renderer,
      value
    );
    renderer.appendChild(featureDiv, icon);
    renderer.appendChild(featureDiv, label);
    renderer.appendChild(featureDiv, valueElement);

    renderer.appendChild(parent, featureDiv);
  }

  getnerateFeatureElementWithRenderer(
    renderer: Renderer2,
    data: number | null | undefined
  ): HTMLElement {
    const span = renderer.createElement('span');
    if (data === null || data === undefined) {
      const text = renderer.createText(
        this.translateService.instant('Shared.MeasurementNotAvailableShort')
      );
      renderer.appendChild(span, text);
    } else {
      const text = renderer.createText(
        `${data} ${this.translateService.instant('Shared.PMUnit')}`
      );
      renderer.appendChild(span, text);
    }
    return span;
  }
}
