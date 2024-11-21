import { PermissionStatusEnum } from '../../../shared/models/permission-status';

export interface UsersPermissionRequest {
  id: number;
  userId: string;
  deviceId: number;
  deviceName: string;
  permissionStatus?: PermissionStatusEnum | null;
  changeDate?: Date | null;
}
