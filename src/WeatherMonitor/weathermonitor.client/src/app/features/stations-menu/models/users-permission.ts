export interface UsersPermissionRequest {
  id: number;
  userId: string;
  deviceId: number;
  deviceName: string;
  permissionStatus?: PermissionStatus | null;
  changeDate?: Date | null;
}
