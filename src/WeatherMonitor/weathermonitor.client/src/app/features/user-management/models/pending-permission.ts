import { PermissionStatusEnum } from '../../../shared/models/permission-status';

export interface PendingPermission {
  id: number;
  userId: string;
  deviceId: number;
  deviceName: string;
  googleMapsPlusCode: string;
  status: PermissionStatusEnum;
  requestedAt: Date;
  email: string;
  nickname: string;
}
