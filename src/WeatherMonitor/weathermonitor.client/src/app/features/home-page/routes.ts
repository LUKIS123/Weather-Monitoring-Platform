import { Route } from '@angular/router';
import { HomePageComponent } from './home-page.component';

export const FeedRoutes: Route[] = [
  {
    path: '',
    component: HomePageComponent,
  },
];
