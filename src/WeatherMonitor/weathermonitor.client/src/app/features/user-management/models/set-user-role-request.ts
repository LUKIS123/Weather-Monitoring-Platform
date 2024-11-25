import { UserRole } from '../../../shared/models/user-role';

export interface SetUserRoleRequest {
  userId: string;
  role: UserRole;
}
