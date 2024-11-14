import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetWeatherDataLastDayResponse } from '../models/get-weather-last-day-response';

@Injectable({
  providedIn: 'root',
})
export class GetLastDayDataService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/dataView/history/day';

  public getLastDayData(
    deviceId: number
  ): Observable<GetWeatherDataLastDayResponse> {
    return this.httpClient.get<GetWeatherDataLastDayResponse>(
      `${this.baseApiUrl}?deviceId=${deviceId}`
    );
  }
}
