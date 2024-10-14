import { UserRole } from '../../../shared/models/user-role';

export interface UserSettings {
  userName: string;
  photoUrl: string | null;
  email: string | null;
  role: UserRole;
}
