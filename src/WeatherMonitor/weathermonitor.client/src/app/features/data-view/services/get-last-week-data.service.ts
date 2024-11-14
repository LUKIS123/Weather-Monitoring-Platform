import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { GetWeatherDataLastWeekResponse } from '../models/get-weather-last-week-response';

@Injectable({
  providedIn: 'root',
})
export class GetLastWeekDataService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/dataView/history/week';

  public getLastWeekData(
    deviceId: number
  ): Observable<GetWeatherDataLastWeekResponse> {
    return this.httpClient.get<GetWeatherDataLastWeekResponse>(
      `${this.baseApiUrl}?deviceId=${deviceId}`
    );
  }
}
