import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { GetStationResponse } from '../models/get-stations-response';
import { PageResult } from '../../../shared/models/page-result';

@Injectable({
  providedIn: 'root',
})
export class GetAllStationsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/dataView/stations/map';

  public getStations(): Observable<GetStationResponse[]> {
    return this.httpClient
      .get<PageResult<GetStationResponse>>(this.baseApiUrl)
      .pipe(map((response) => response.items));
  }
}
