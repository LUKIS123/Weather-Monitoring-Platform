import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StationPermissionStatusResponse } from '../models/station-permission-status';

@Injectable({
  providedIn: 'root',
})
export class GetStationPermissionStatusService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/permissions/stationPermissionStatus';

  public getStationPermissions(
    stationId: number
  ): Observable<StationPermissionStatusResponse> {
    return this.httpClient.get<StationPermissionStatusResponse>(
      `${this.baseApiUrl}?stationId=${stationId}`
    );
  }
}
