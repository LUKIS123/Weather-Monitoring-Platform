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
    path: 'AvailableStations',
    canActivate: [],
    loadChildren: () =>
      import('./features/stations-menu/routes').then(
        (mod) => mod.AvailableStationsRoutes
      ),
  },
  {
    path: 'Home',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/home-page/routes').then((mod) => mod.FeedRoutes),
  },
  {
    path: 'DataVisualization',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/data-view/routes').then((mod) => mod.DataViewRoutes),
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
    path: 'UserManagement',
    canActivate: [isAdminAuthGuard],
    loadChildren: () =>
      import('./features/user-management/routes').then(
        (mod) => mod.UserManagementRoutes
      ),
  },
  {
    path: '**',
    component: NotFoundComponent,
  },
];
