import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { UserSettings } from './models/user-settings';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserSettingService {
  private readonly httpClient = inject(HttpClient);

  private readonly baseApiUrl = 'api/user/user-settings';

  public getUserSettings(): Observable<UserSettings> {
    return this.httpClient.get<UserSettings>(this.baseApiUrl);
  }
}
