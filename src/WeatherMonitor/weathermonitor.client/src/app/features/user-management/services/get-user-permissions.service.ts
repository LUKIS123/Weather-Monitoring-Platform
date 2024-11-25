import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PageResult } from '../../../shared/models/page-result';
import { UserPermissionRequest } from '../models/user-permission-request';

@Injectable({
  providedIn: 'root',
})
export class GetUserPermissionsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/userManagement/userPermissions';

  public getPermissions(
    pageNumber: number,
    userId: string
  ): Observable<PageResult<UserPermissionRequest>> {
    const page = pageNumber == 0 ? 1 : pageNumber;
    return this.httpClient.get<PageResult<UserPermissionRequest>>(
      `${this.baseApiUrl}?pageNumber=${page}&userId=${userId}`
    );
  }
}
