import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { DeviceMqttCredentialsResponse } from '../models/device-mqtt-credentials-response';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DeviceMqttCredentialsDataService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/deviceManagement/credentials';

  public getMqttCredentials(
    deviceId: number
  ): Observable<DeviceMqttCredentialsResponse> {
    return this.httpClient.get<DeviceMqttCredentialsResponse>(
      `${this.baseApiUrl}?deviceId=${deviceId}`
    );
  }
}
