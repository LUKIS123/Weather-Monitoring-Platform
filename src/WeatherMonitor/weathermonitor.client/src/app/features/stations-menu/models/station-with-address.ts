import { StationAddress } from '../../../shared/models/station-address';
import { AvailableStation } from './available-station';

export interface StationWithAddress {
  address: StationAddress;
  station: AvailableStation;
}
