import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PageResult } from '../../../shared/models/page-result';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class GetUsersService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseApiUrl = '/api/userManagement/users';

  public getUsers(
    pageNumber: number,
    nicknameSearch: string | null = null
  ): Observable<PageResult<User>> {
    const page = pageNumber == 0 ? 1 : pageNumber;
    const search = nicknameSearch ? `&nicknameSearch=${nicknameSearch}` : '';

    return this.httpClient.get<PageResult<User>>(
      `${this.baseApiUrl}?pageNumber=${page}${search}`
    );
  }
}
