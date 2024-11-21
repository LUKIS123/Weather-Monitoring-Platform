import { Route } from '@angular/router';
import { DataViewComponent } from './data-view.component';
import { AggregateDataViewComponent } from './aggregate-data-view.component';

export const DataViewRoutes: Route[] = [
  {
    path: '',
    component: AggregateDataViewComponent,
  },
  {
    path: 'Station/:deviceId',
    component: DataViewComponent,
  },
];
