import { UserPermission } from './user-permission';
import { UserPermissionRequest } from './user-permission-request';

export interface UpdatePermissionResponse {
  userPermissionRequestDto: UserPermissionRequest;
  userPermissionDto?: UserPermission | null;
}
