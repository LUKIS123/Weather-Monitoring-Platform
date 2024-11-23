import { StationAddress } from '../../../shared/models/station-address';
import { PendingPermission } from './pending-permission';

export interface PendingPermissionWithAddress {
  address: StationAddress;
  permission: PendingPermission;
}
