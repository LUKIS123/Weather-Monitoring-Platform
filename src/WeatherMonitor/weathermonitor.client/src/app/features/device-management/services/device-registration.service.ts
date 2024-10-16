import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DeviceRegistrationService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/deviceManagement/register';

  public registerNewDevice(
    deviceUsername: string,
    googleMapsPlusCode: string,
    deviceExtraInfo: string
  ): Observable<void> {
    return this.httpClient.post<void>(this.baseApiUrl, {
      deviceUsername,
      googleMapsPlusCode,
      deviceExtraInfo,
    });
  }
}
