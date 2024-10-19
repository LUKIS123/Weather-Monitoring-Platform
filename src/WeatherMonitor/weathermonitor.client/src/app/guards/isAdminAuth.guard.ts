import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthorizationService } from '../features/authorization/services/authorization-service';

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export const isAdminAuthGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthorizationService);

  if (authService.isAdminRole()) {
    return true;
  }
  return false;
};
