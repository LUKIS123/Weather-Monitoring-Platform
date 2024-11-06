import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  OnInit,
  signal,
  viewChild,
  ViewChild,
} from '@angular/core';
import { Loader } from '@googlemaps/js-api-loader';
import { AppSecrets } from '../../../../app.secrets';
import { ToastService } from '../../../../shared/services/toast.service';
import { MaterialModule } from '../../../../shared/material.module';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { GetAllStationsService } from '../../services/get-all-stations.service';
import { GetStationResponse } from '../../models/get-stations-response';
import {
  GoogleMap,
  MapAdvancedMarker,
  MapInfoWindow,
} from '@angular/google-maps';
import { SafeHtmlPipe } from '../../../../shared/pipes/safe-html.pipe';

@Component({
  selector: 'app-stations-map',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    GoogleMap,
    MapAdvancedMarker,
    SafeHtmlPipe,
    MapInfoWindow,
  ],
  templateUrl: './stations-map.component.html',
})
export class StationsMapComponent implements OnInit, AfterViewInit {
  infoWin = viewChild(MapInfoWindow);

  center: google.maps.LatLngLiteral = { lat: 24, lng: 12 };
  zoom = 4;
  advancedMarkerOptions: google.maps.marker.AdvancedMarkerElementOptions = {
    gmpDraggable: false,
  };
  advancedMarkerPositions: google.maps.LatLngLiteral[] = [];

  addAdvancedMarker(event: google.maps.MapMouseEvent) {
    this.advancedMarkerPositions.push(event.latLng!.toJSON());
  }

  handleMarkerClick(event: MapAdvancedMarker, window: MapInfoWindow) {
    // this.infoWin()?.open(event);
    window.open(event);

    // const elem = event.domEvent.target as HTMLElement;
    // elem.innerHTML = document.createElement('div');
  }

  mapElementRef = viewChild('stationsMap');

  private readonly toastService = inject(ToastService);
  private readonly translateService = inject(TranslateService);
  private readonly getAllStationsService = inject(GetAllStationsService);

  #weatherStations = signal<GetStationResponse[]>([]);
  public stationsPageResult = this.#weatherStations.asReadonly();

  #isMapLoading = signal<boolean>(true);
  public isMapLoading = this.#isMapLoading.asReadonly();

  #mapOptions: google.maps.MapOptions = {
    center: { lat: 51.107883, lng: 17.038538 },
    zoom: 14,
    styles: [
      {
        featureType: 'poi',
        elementType: 'labels',
        stylers: [{ visibility: 'off' }],
      },
    ],
    mapTypeControl: false,
    fullscreenControl: false,
    streetViewControl: false,
  };

  ngOnInit(): void {
    this.loadWeatherStations();
  }

  ngAfterViewInit(): void {
    // const loader = new Loader({
    //   apiKey: AppSecrets.GOOGLE_API_KEY,
    //   version: 'weekly',
    // });
    // const mapElement = this.mapElementRef.nativeElement;
    // if (mapElement) {
    //   loader
    //     .importLibrary('maps')
    //     .then(({ Map }) => {
    //       new Map(mapElement, this.#mapOptions);
    //       this.#isMapLoading.set(false);
    //     })
    //     .catch(() => {
    //       this.toastService.openError(
    //         this.translateService.instant('StationsMap.Map.Error')
    //       );
    //       this.#isMapLoading.set(false);
    //     });
    // } else {
    //   this.toastService.openError(
    //     this.translateService.instant('StationsMap.Error')
    //   );
    // }
    // this.#isMapLoading.set(false);
  }

  private loadWeatherStations(): void {
    this.getAllStationsService.getStations().subscribe({
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

  // async initMap(): Promise<void> {
  //   const { Map } = (await google.maps.importLibrary(
  //     'maps'
  //   )) as google.maps.MapsLibrary;
  //   const { AdvancedMarkerElement } = (await google.maps.importLibrary(
  //     'marker'
  //   )) as google.maps.MarkerLibrary;

  //   const center = { lat: 37.43238031167444, lng: -122.16795397128632 };
  //   const map = new Map(document.getElementById('map') as HTMLElement, {
  //     zoom: 11,
  //     center,
  //     mapId: '4504f8b37365c3d0',
  //   });

  //   for (const property of this.properties) {
  //     const advancedMarkerElement = new AdvancedMarkerElement({
  //       map,
  //       content: this.buildContent(property),
  //       position: property.position,
  //       title: property.description,
  //     });

  //     advancedMarkerElement.addListener('click', () => {
  //       toggleHighlight(advancedMarkerElement, property);
  //     });
  //   }
  // }

  // private toggleHighlight(markerView: any, property: any): void {
  //   if (markerView.content.classList.contains('highlight')) {
  //     markerView.content.classList.remove('highlight');
  //     markerView.zIndex = null;
  //   } else {
  //     markerView.content.classList.add('highlight');
  //     markerView.zIndex = 1;
  //   }
  // }

  buildContent(property: any): HTMLElement {
    const content = document.createElement('div');
    content.classList.add('property');
    content.innerHTML = `
      <div class="icon">
          <i aria-hidden="true" class="fa fa-icon fa-${property.type}" title="${property.type}"></i>
          <span class="fa-sr-only">${property.type}</span>
      </div>
      <div class="details">
          <div class="price">${property.price}</div>
          <div class="address">${property.address}</div>
          <div class="features">
          <div>
              <i aria-hidden="true" class="fa fa-bed fa-lg bed" title="bedroom"></i>
              <span class="fa-sr-only">bedroom</span>
              <span>${property.bed}</span>
          </div>
          <div>
              <i aria-hidden="true" class="fa fa-bath fa-lg bath" title="bathroom"></i>
              <span class="fa-sr-only">bathroom</span>
              <span>${property.bath}</span>
          </div>
          <div>
              <i aria-hidden="true" class="fa fa-ruler fa-lg size" title="size"></i>
              <span class="fa-sr-only">size</span>
              <span>${property.size} ft<sup>2</sup></span>
          </div>
          </div>
      </div>
      `;
    return content;
  }

  // properties = [
  //   {
  //     address: '215 Emily St, MountainView, CA',
  //     description: 'Single family house with modern design',
  //     price: '$ 3,889,000',
  //     type: 'home',
  //     bed: 5,
  //     bath: 4.5,
  //     size: 300,
  //     position: {
  //       lat: 37.50024109655184,
  //       lng: -122.28528451834352,
  //     },
  //   },
  //   {
  //     address: '108 Squirrel Ln &#128063;, Menlo Park, CA',
  //     description: 'Townhouse with friendly neighbors',
  //     price: '$ 3,050,000',
  //     type: 'building',
  //     bed: 4,
  //     bath: 3,
  //     size: 200,
  //     position: {
  //       lat: 37.44440882321596,
  //       lng: -122.2160620727,
  //     },
  //   },
  // ];
}
