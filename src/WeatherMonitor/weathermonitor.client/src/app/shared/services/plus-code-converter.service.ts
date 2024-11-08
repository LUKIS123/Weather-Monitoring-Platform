import { inject, Injectable } from '@angular/core';
import { MapGeocoder } from '@angular/google-maps';
import { catchError, map, Observable, of } from 'rxjs';
import { ToastService } from './toast.service';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root',
})
export class PlusCodeConverterService {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly geoService = inject(MapGeocoder);

  getLocationByPlusCode(
    plusCode: string
  ): Observable<google.maps.LatLngLiteral> {
    return this.geoService
      .geocode({
        address: plusCode,
      })
      .pipe(
        map((response) => {
          const lat = response.results[0].geometry.location.lat();
          const lng = response.results[0].geometry.location.lng();
          return { lat, lng };
        }),
        catchError(() => {
          this.toastService.openError(
            this.translateService.instant('StationsMap.GeoCodeError')
          );
          return of({ lat: 0, lng: 0 });
        })
      );
  }
}
