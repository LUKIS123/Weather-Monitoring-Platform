import { UserRole } from '../../../shared/models/user-role';

export interface User {
  id: string;
  role: UserRole;
  email: string;
  nickname: string;
}
