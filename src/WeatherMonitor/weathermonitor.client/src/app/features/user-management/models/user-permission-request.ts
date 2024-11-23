import { PermissionStatusEnum } from '../../../shared/models/permission-status';

export interface UserPermissionRequest {
  id: number;
  userId: string;
  deviceId: number;
  permissionStatus: PermissionStatusEnum;
  dateTime: Date;
}
