import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateDeviceResponse } from '../models/create-device-response';

@Injectable({
  providedIn: 'root',
})
export class DeviceRegistrationService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/deviceManagement/register';

  public registerNewDevice(
    mqttUsername: string,
    googleMapsPlusCode: string,
    deviceExtraInfo: string | null
  ): Observable<CreateDeviceResponse> {
    return this.httpClient.post<CreateDeviceResponse>(this.baseApiUrl, {
      mqttUsername,
      googleMapsPlusCode,
      deviceExtraInfo,
    });
  }
}
