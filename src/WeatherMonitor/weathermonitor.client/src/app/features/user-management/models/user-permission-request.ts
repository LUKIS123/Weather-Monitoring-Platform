import { PermissionStatusEnum } from '../../../shared/models/permission-status';

export interface UserPermissionRequest {
  id: number;
  userId: string;
  deviceId: number;
  deviceName: string;
  googleMapsPlusCode: string;
  permissionStatus: PermissionStatusEnum;
  dateTime: Date;
}
