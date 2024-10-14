import { UserRole } from '../../../shared/models/user-role';

export interface UserData {
  isAuthorized: boolean;
  userId: string | null;
  userName: string | null;
  userPhotoUrl: string | null;
  email: string | null;
  role: UserRole | null;
}
