import { PermissionStatusEnum } from '../../../shared/models/permission-status';

export interface UpdatePermissionRequest {
  userId: string;
  deviceId: number;
  status: PermissionStatusEnum;
}
