import { CommonModule } from '@angular/common';
import { Component, computed, input } from '@angular/core';
import { MaterialModule } from '../../material.module';
import { TranslateModule } from '@ngx-translate/core';
import { WeatherSensorData } from '../../models/weather-data';
import { GetStationResponse } from '../../../features/home-page/models/get-stations-response';

@Component({
  selector: 'app-weather-data-preview',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './weather-data-preview.component.html',
})
export class WeatherDataPreviewComponent {
  weatherResponse = input.required<GetStationResponse | null>();
  sensorData = computed<WeatherSensorData | undefined>(() => {
    const response = this.weatherResponse();
    return response !== null && response?.temperature !== null
      ? this.parseWeatherData(response)
      : undefined;
  });

  private parseWeatherData(weatherData: GetStationResponse): WeatherSensorData {
    return {
      temperature: parseFloat(weatherData.temperature.toFixed(2)),
      humidity: parseFloat(weatherData.humidity.toFixed(2)),
      airPressure: parseFloat((weatherData.airPressure / 100).toFixed(2)),
      altitude: parseFloat(weatherData.altitude.toFixed(2)),
      pM10: weatherData.pM10,
      pM1_0: weatherData.pM1_0,
      pM2_5: weatherData.pM2_5,
    };
  }
}
