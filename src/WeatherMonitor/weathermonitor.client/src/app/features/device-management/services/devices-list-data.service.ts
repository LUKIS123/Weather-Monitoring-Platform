import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PageResult } from '../../../shared/models/page-result';
import { GetDeviceResponse } from '../models/get-device-response';

@Injectable({
  providedIn: 'root',
})
export class DevicesListDataService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/deviceManagement/list';

  public getDevices(
    pageNumber: number
  ): Observable<PageResult<GetDeviceResponse>> {
    return this.httpClient.get<PageResult<GetDeviceResponse>>(
      `${this.baseApiUrl}?pageNumber=${pageNumber}`
    );
  }
}
