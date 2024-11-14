import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetWeatherDataLastMonthResponse } from '../models/get-weather-last-month-response';

@Injectable({
  providedIn: 'root',
})
export class GetLastMonthDataService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/dataView/history/month';

  public getLastMonthkData(
    deviceId: number
  ): Observable<GetWeatherDataLastMonthResponse> {
    return this.httpClient.get<GetWeatherDataLastMonthResponse>(
      `${this.baseApiUrl}?deviceId=${deviceId}`
    );
  }
}
