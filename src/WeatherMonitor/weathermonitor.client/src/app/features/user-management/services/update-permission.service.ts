import { inject, Injectable } from '@angular/core';
import { UpdatePermissionRequest } from '../models/update-permission-request ';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class UpdatePermissionService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/userManagement/updatePermission';

  public updatePermission(
    request: UpdatePermissionRequest
  ): Observable<UpdatePermissionRequest> {
    return this.httpClient.post<UpdatePermissionRequest>(
      this.baseApiUrl,
      request
    );
  }
}
