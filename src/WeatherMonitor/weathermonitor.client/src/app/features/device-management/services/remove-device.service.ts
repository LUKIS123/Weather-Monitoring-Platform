import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class RemoveDeviceService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/deviceManagement/remove';

  public removeDevice(deviceId: number): Observable<void> {
    return this.httpClient.delete<void>(
      `${this.baseApiUrl}?deviceId=${deviceId}`
    );
  }
}
