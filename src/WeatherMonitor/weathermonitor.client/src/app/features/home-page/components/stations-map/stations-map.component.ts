import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, Renderer2, signal } from '@angular/core';
import { ToastService } from '../../../../shared/services/toast.service';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { GetAllStationsService } from '../../services/get-all-stations.service';
import { GoogleMap, MapAdvancedMarker } from '@angular/google-maps';
import { finalize, map, switchMap, zip } from 'rxjs';
import { PlusCodeConverterService } from '../../../../shared/services/plus-code-converter.service';
import { StationLocation } from '../../models/station-location';
import { StationMapMarkerContentService } from '../../../../shared/services/station-map-marker-content.service';

@Component({
  selector: 'app-stations-map',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    GoogleMap,
    MapAdvancedMarker,
  ],
  templateUrl: './stations-map.component.html',
})
export class StationsMapComponent implements OnInit {
  center: google.maps.LatLngLiteral = { lat: 51.110883, lng: 17.048538 };
  zoom = 15;
  advancedMarkerOptions: google.maps.marker.AdvancedMarkerElementOptions = {
    gmpDraggable: false,
    gmpClickable: true,
  };
  mapOptions: google.maps.MapOptions = {
    mapTypeControl: false,
    fullscreenControl: true,
    streetViewControl: false,
  };

  private readonly toastService = inject(ToastService);
  private readonly translateService = inject(TranslateService);
  private readonly getAllStationsService = inject(GetAllStationsService);
  private readonly plusCodeConverterService = inject(PlusCodeConverterService);
  private readonly stationMapMarkerContentService = inject(
    StationMapMarkerContentService
  );
  private readonly renderer = inject(Renderer2);

  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #weatherStations = signal<StationLocation[]>([]);
  public weatherStations = this.#weatherStations.asReadonly();

  ngOnInit(): void {
    this.#isLoading.set(true);
    this.loadWeatherStations();
  }

  handleMarkerClick($event: google.maps.MapMouseEvent) {
    $event.stop();
  }

  private loadWeatherStations(): void {
    this.getAllStationsService
      .getStations()
      .pipe(
        map((stations) => {
          return stations.map((station) => {
            return this.plusCodeConverterService
              .getLocationByPlusCode(station.googleMapsPlusCode)
              .pipe(
                map((position) => {
                  return {
                    station: station,
                    location: position,
                  } as StationLocation;
                })
              );
          });
        }),
        switchMap((stations) => {
          return zip(stations);
        }),
        finalize(() => {
          this.#isLoading.set(false);
        })
      )
      .subscribe({
        next: (stations) => {
          this.#weatherStations.set(stations);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant('StationsMap.NoStations')
          );
        },
      });
  }

  buildContent(stationlocation: StationLocation): HTMLElement {
    return this.stationMapMarkerContentService.buildMarkerContent(
      this.renderer,
      stationlocation
    );
  }
}
