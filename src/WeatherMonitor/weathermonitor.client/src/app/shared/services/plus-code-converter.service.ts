import { inject, Injectable } from '@angular/core';
import { MapGeocoder } from '@angular/google-maps';
import { catchError, map, Observable, of } from 'rxjs';
import { ToastService } from './toast.service';
import { TranslateService } from '@ngx-translate/core';
import { StationAddress } from '../models/station-address';

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

  getAddressByPlusCode(plusCode: string): Observable<StationAddress> {
    return this.geoService
      .geocode({
        address: plusCode,
      })
      .pipe(
        map((response) => {
          const addressComponents = response.results[0].address_components;

          const streetNumber = addressComponents.find((component) =>
            component.types.includes('street_number')
          )?.long_name;
          const street = addressComponents.find((component) =>
            component.types.includes('route')
          )?.long_name;
          const city = addressComponents.find((component) =>
            component.types.includes('locality')
          )?.long_name;
          const voivodeship = addressComponents.find((component) =>
            component.types.includes('administrative_area_level_1')
          )?.long_name;

          return {
            street: street,
            streetNumber: streetNumber,
            city: city || '',
            voivodship: voivodeship || '',
          };
        }),
        catchError(() => {
          this.toastService.openError(
            this.translateService.instant('AvailableStations.AddressFetchError')
          );
          return of({
            street: undefined,
            streetNumber: undefined,
            city: '',
            voivodship: '',
          });
        })
      );
  }
}
