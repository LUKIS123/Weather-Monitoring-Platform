import { Routes } from '@angular/router';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { WelcomeComponent } from './features/welcome/welcome.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: WelcomeComponent,
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
    path: '**',
    component: NotFoundComponent,
  },
];
