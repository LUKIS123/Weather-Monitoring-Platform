import { PermissionStatusEnum } from '../../../shared/models/permission-status';
import { UserRole } from '../../../shared/models/user-role';

export interface StationPermissionStatusResponse {
  stationUserPermission: StationUserPermissionDto;
  hasPermission: boolean;
  canRequestPermission: boolean;
}

export interface StationUserPermissionDto {
  stationId: number;
  stationName: string;
  userId: string;
  userRole: UserRole;
  permissionStatus?: PermissionStatusEnum | null;
  changeDate?: Date | null;
  permissionRecordExists: boolean;
}
