export interface CreateDeviceResponse {
  id: number;
  googleMapsPlusCode: string;
  deviceExtraInfo: string | null;
  isActivate: boolean;
  username: string;
  password: string;
  clientId: string;
  topic: string;
}
