import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PageResult } from '../../../shared/models/page-result';
import { UsersPermissionRequest } from '../models/users-permission';

@Injectable({
  providedIn: 'root',
})
export class GetUsersPermissionsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/permissions/permissionRequests';

  public getStationPermissions(
    pageNumber: number
  ): Observable<PageResult<UsersPermissionRequest>> {
    return this.httpClient.get<PageResult<UsersPermissionRequest>>(
      `${this.baseApiUrl}?pageNumber=${pageNumber}`
    );
  }
}
