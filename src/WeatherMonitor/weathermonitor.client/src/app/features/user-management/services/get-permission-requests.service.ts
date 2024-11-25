import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PageResult } from '../../../shared/models/page-result';
import { PendingPermission } from '../models/pending-permission';

@Injectable({
  providedIn: 'root',
})
export class GetPermissionRequestsService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/userManagement/pending';

  public getPendingRequests(
    pageNumber: number
  ): Observable<PageResult<PendingPermission>> {
    const page = pageNumber == 0 ? 1 : pageNumber;
    return this.httpClient.get<PageResult<PendingPermission>>(
      `${this.baseApiUrl}?pageNumber=${page}`
    );
  }
}
