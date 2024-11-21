import { HttpClient, HttpResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SendPermissionRequestService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/permissions/sendRequest';

  public sendRequest(stationId: number): Observable<HttpResponse<void>> {
    return this.httpClient.post<void>(
      `${this.baseApiUrl}?stationId=${stationId}`,
      null,
      { observe: 'response' }
    );
  }
}
