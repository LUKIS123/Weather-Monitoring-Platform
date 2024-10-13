import { UserRole } from './user-role';

export interface UserSettings {
  userName: string;
  photoUrl: string | null;
  email: string | null;
  role: UserRole;
}
