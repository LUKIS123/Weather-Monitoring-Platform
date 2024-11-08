import { GetStationResponse } from './get-stations-response';

export interface StationLocation {
  location: google.maps.LatLngLiteral;
  station: GetStationResponse;
}
