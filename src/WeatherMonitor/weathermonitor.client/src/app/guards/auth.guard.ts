import { CanActivateFn } from '@angular/router';
import { AuthorizationService } from '../features/authorization/services/authorization-service';
import { inject } from '@angular/core';

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthorizationService);

  if (authService.isAuthorized()) {
    return true;
  }
  return false;
};
