import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { GetStationResponse } from '../models/get-stations-response';
import { PageResult } from '../../../shared/models/page-result';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class GetStationsListService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/dataView/stations/list';

  public getStationsList(
    pageNumber: number
  ): Observable<PageResult<GetStationResponse>> {
    const page = pageNumber == 0 ? 1 : pageNumber;
    return this.httpClient.get<PageResult<GetStationResponse>>(
      `${this.baseApiUrl}?pageNumber=${page}`
    );
  }
}
