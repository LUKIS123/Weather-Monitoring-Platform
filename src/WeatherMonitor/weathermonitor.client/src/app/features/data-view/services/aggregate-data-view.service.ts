import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetWeatherDataLastDayResponse } from '../models/get-weather-last-day-response';
import { GetWeatherDataLastMonthResponse } from '../models/get-weather-last-month-response';
import { GetWeatherDataLastWeekResponse } from '../models/get-weather-last-week-response';

@Injectable({
  providedIn: 'root',
})
export class AggregateDataViewService {
  private readonly httpClient = inject(HttpClient);
  private readonly getLastDayDataBaseApiUrl = '/api/dataView/history/day';
  private readonly getLastMonthDataBaseApiUrl = '/api/dataView/history/month';
  private readonly getLastWeekDataBaseApiUrl = '/api/dataView/history/week';

  public getLastDayData(
    searchPhrase: string
  ): Observable<GetWeatherDataLastDayResponse> {
    return this.httpClient.get<GetWeatherDataLastDayResponse>(
      `${this.getLastDayDataBaseApiUrl}?plusCodeSearch=${searchPhrase}`
    );
  }

  public getLastWeekData(
    searchPhrase: string
  ): Observable<GetWeatherDataLastWeekResponse> {
    return this.httpClient.get<GetWeatherDataLastWeekResponse>(
      `${this.getLastWeekDataBaseApiUrl}?plusCodeSearch=${searchPhrase}`
    );
  }

  public getLastMonthkData(
    searchPhrase: string
  ): Observable<GetWeatherDataLastMonthResponse> {
    return this.httpClient.get<GetWeatherDataLastMonthResponse>(
      `${this.getLastMonthDataBaseApiUrl}?plusCodeSearch=${searchPhrase}`
    );
  }
}
