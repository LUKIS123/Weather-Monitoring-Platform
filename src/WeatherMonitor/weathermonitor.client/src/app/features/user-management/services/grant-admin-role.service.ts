import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SetUserRoleRequest } from '../models/set-user-role-request';
import { UserRole } from '../../../shared/models/user-role';

@Injectable({
  providedIn: 'root',
})
export class GrantAdminRoleService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/userManagement/grantAdminRole';

  public grantAdmin(userId: string): Observable<void> {
    const request: SetUserRoleRequest = {
      userId: userId,
      role: UserRole.Admin,
    };
    return this.httpClient.post<void>(this.baseApiUrl, request);
  }
}
