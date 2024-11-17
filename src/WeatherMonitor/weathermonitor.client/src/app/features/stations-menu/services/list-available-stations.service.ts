import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PageResult } from '../../../shared/models/page-result';
import { AvailableStation } from '../models/available-station';

@Injectable({
  providedIn: 'root',
})
export class ListAvailableStationsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/permissions/availableStations';

  public getStationsList(
    pageNumber: number
  ): Observable<PageResult<AvailableStation>> {
    const page = pageNumber == 0 ? 1 : pageNumber;
    return this.httpClient.get<PageResult<AvailableStation>>(
      `${this.baseApiUrl}?pageNumber=${page}`
    );
  }
}
