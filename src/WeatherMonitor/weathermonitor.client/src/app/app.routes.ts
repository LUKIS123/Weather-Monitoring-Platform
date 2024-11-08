import { Routes } from '@angular/router';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { WelcomeComponent } from './features/welcome/welcome.component';
import { authGuard } from './guards/auth.guard';
import { isAdminAuthGuard } from './guards/isAdminAuth.guard';

export const routes: Routes = [
  {
    path: '',
    component: WelcomeComponent,
  },
  {
    path: 'Home',
    canActivate: [],
    loadChildren: () =>
      import('./features/home-page/routes').then((mod) => mod.FeedRoutes),
  },
  {
    path: 'UserSettings',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/user-settings/routes').then(
        (mod) => mod.UserSettingsRoutes
      ),
  },
  {
    path: 'DeviceManagement',
    canActivate: [isAdminAuthGuard],
    loadChildren: () =>
      import('./features/device-management/routes').then(
        (mod) => mod.DeviceManagementRoutes
      ),
  },
  {
    path: '**',
    component: NotFoundComponent,
  },
];
