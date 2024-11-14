import { Route } from '@angular/router';
import { DataViewComponent } from './data-view.component';

export const DataViewRoutes: Route[] = [
  {
    path: 'Station/:deviceId',
    component: DataViewComponent,
  },
];
